using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using Smellyriver.TankInspector.IO;
using Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups;
using Smellyriver.TankInspector.Common.Utilities;

namespace Smellyriver.TankInspector.Pro.IO
{
    class ExtractPackageContentAction : IDisposable
    {
        public string PackagePath { get; }
        public string LocalPath { get; }
        public string Destination { get; }
        public OverwriteStrategy OverwriteStrategy { get; }

        public bool UseRelativePath { get; }

        private IProgressDialogController _progressController;

        private Task _task;

        private bool _isDisposed;

        public string[] FileList { get; }

        public ExtractPackageContentAction(string packagePath,
                                           string localPath,
                                           string destination,
                                           OverwriteStrategy overwriteStrategy = OverwriteStrategy.Ask,
                                           bool useRelativePath = true)
        {
            if (!File.Exists(packagePath))
                throw new ArgumentException("packagePath", "invalid package path");

            this.PackagePath = packagePath;
            this.LocalPath = localPath.Replace('\\', '/');
            this.Destination = destination;
            this.OverwriteStrategy = overwriteStrategy;
            this.UseRelativePath = useRelativePath;
        }

        public ExtractPackageContentAction(string[] fileList,
                                           string destination,
                                           OverwriteStrategy overwriteStrategy = OverwriteStrategy.Ask)
        {
            this.FileList = fileList;
            this.Destination = destination;
            this.OverwriteStrategy = overwriteStrategy;
            this.UseRelativePath = false;
        }

        public void BeginAsync()
        {
            var task = new Task(() => this.ShowProgressDialog(this.L("package_support", "extract_state_preparing"), true, false, true));
            _task = task.ContinueWith(t => this.BuildExtractFileList());
            task.Start();
        }

        private void ShowProgressDialog(string title, bool animateShow, bool animateHide, bool cancellable)
        {
            var settings = new DialogSettings
            {
                UseAnimationOnShow = animateShow,
                UseAnimationOnHide = animateHide
            };

            var task = DialogManager.Instance.ShowProgressAsync(title, string.Empty, cancellable, settings);
            task.Wait();
            _progressController = task.Result;
        }

        private void CheckForExistedFiles(List<ExtractFileInfo> fileList)
        {
            var existedFiles = fileList.Where(f =>
                {
                    if (File.Exists(f.DestinationPath))
                        f.IsExisted = true;
                    return f.IsExisted;
                }).ToArray();

            if (existedFiles.Length == 0 || this.OverwriteStrategy != OverwriteStrategy.Ask)
            {
                _task = _task.ContinueWith(task => this.ApplyOverwriteStrategy(this.OverwriteStrategy, existedFiles))
                             .ContinueWith(task => this.ExtractFiles(fileList));
            }
            else
            {
                _task = _task.ContinueWith(task => _progressController.CloseAsync().Wait())
                             .ContinueWith(task => this.QueryOverwriteStrategy(fileList, existedFiles));
            }
        }

        private void ApplyOverwriteStrategy(OverwriteStrategy strategy, ExtractFileInfo[] existedFiles)
        {
            foreach (var fileInfo in existedFiles)
                fileInfo.ShouldOverwrite = strategy == OverwriteStrategy.Overwrite;
        }

        private void QueryOverwriteStrategy(List<ExtractFileInfo> fileList, ExtractFileInfo[] existedFiles)
        {
            var overwriteStrategy = this.OverwriteStrategy;

            if (overwriteStrategy == OverwriteStrategy.Ask)
            {
                var firstFileName = Path.GetFileName(existedFiles[0].DestinationPath);
                var firstFileDirectory = Path.GetDirectoryName(existedFiles[0].DestinationPath);

                string message, affirmativeText, negativeText;
                if (existedFiles.Length == 1)
                {
                    message = this.L("package_support",
                                     "overwrite_single_file_prompt_message",
                                     firstFileName,
                                     firstFileDirectory);
                    affirmativeText = this.L("common", "yes");
                    negativeText = this.L("common", "no");
                }
                else
                {
                    if (existedFiles.Length == 2)
                        message = this.L("package_support",
                                         "overwrite_two_files_prompt_message",
                                         firstFileName,
                                         firstFileDirectory);
                    else
                        message = this.L("package_support",
                                         "overwrite_multiple_file_prompt_message",
                                         firstFileName,
                                         firstFileDirectory);
                    affirmativeText = this.L("common", "yes_to_all");
                    negativeText = this.L("common", "no_to_all");
                }

                var settings = new DialogSettings()
                    {
                        AffirmativeButtonText = affirmativeText,
                        NegativeButtonText = negativeText,
                        FirstAuxiliaryButtonText = this.L("common", "cancel"),
                        UseAnimationOnShow = false,
                        UseAnimationOnHide = false
                    };

                var task = DialogManager.Instance.ShowYesNoCancelMessageAsync(this.L("package_support", "overwrite_file_prompt_title"),
                                                                              message,
                                                                              settings);

                task.Wait();

                if (task.Result == MessageDialogResult.FirstAuxiliary)
                {
                    _task = _task.ContinueWith(t => this.EndAndDispose(this.L("package_support", "extract_state_cancelled")));
                    return;
                }

                overwriteStrategy = task.Result == MessageDialogResult.Affirmative
                                  ? OverwriteStrategy.Overwrite
                                  : OverwriteStrategy.Skip;
            }

            _task = _task.ContinueWith(task => this.ApplyOverwriteStrategy(overwriteStrategy, existedFiles))
                         .ContinueWith(task => this.ShowProgressDialog(this.L("package_support", "extract_state_extracting"), false, false, true))
                         .ContinueWith(task => this.ExtractFiles(fileList));

        }

        private void EndAndDispose(string title)
        {
            this.ShowProgressDialog(title, false, true, false);
            Thread.Sleep(1000);
            _progressController.CloseAsync();
            this.Dispose();
        }

        private void ExtractFiles(List<ExtractFileInfo> fileList)
        {
            _progressController.SetTitle(this.L("package_support", "extract_state_extracting"));

            for (var i = 0; i < fileList.Count; ++i)
            {
                if (_progressController.IsCanceled)
                {
                    _task = _task.ContinueWith(t => _progressController.CloseAsync().Wait())
                                 .ContinueWith(t => this.EndAndDispose(this.L("package_support", "extract_state_cancelled")));
                    return;
                }

                var file = fileList[i];

                _progressController.SetProgress((double)i / fileList.Count);
                _progressController.SetMessage(file.RelativePath);


                string directory;

                if (file.Entry.IsDirectory)
                    directory = file.DestinationPath;
                else
                    directory = Path.GetDirectoryName(file.DestinationPath);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                if (file.Entry.IsDirectory)
                    continue;

                using (var zipStream = file.ZipFile.GetInputStream(file.Entry))
                {
                    using (var fileStream = File.Create(file.DestinationPath))
                    {
                        zipStream.CopyTo(fileStream);
                    }
                }
            }

            foreach (var zipFile in fileList.Select(f => f.ZipFile).Distinct())
                zipFile.Close();

            _task = _task.ContinueWith(t => _progressController.CloseAsync().Wait())
                         .ContinueWith(t => this.EndAndDispose(this.L("package_support", "extract_state_finished")));
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _progressController = null;
            _task = null;
            _isDisposed = true;
        }

        private void BuildExtractFileList()
        {
            List<ExtractFileInfo> extractFileList;
            if (this.FileList != null)
                extractFileList = this.BuildExtractFileListFromSpecifiedFileList();
            else
                extractFileList = this.BuildExtractFileListFromPaths();

            _task = _task.ContinueWith(task => this.CheckForExistedFiles(extractFileList));
        }

        private List<ExtractFileInfo> BuildExtractFileListFromSpecifiedFileList()
        {
            _progressController.SetIndeterminate();
            var distinctFileList = new Dictionary<string, ExtractFileInfo>();
            foreach (var file in this.FileList)
            {
                var fileInfo = distinctFileList.GetOrCreate(file, () =>
                    {
                        var info = new ExtractFileInfo();
                        string packagePath, localPath;
                        UnifiedPath.ParsePath(file, out packagePath, out localPath);
                        info.PackagePath = packagePath;
                        info.DestinationRoot = this.GetDestinationRoot(localPath);
                        info.RelativePath = localPath;
                        return info;
                    });
            }

            var packageGroups = distinctFileList.Values.Aggregate(new Dictionary<string, List<ExtractFileInfo>>(),
                                                                  (dict, item) =>
                                                                  {
                                                                      var list = dict.GetOrCreate(item.PackagePath.ToLower(),
                                                                                                  () => new List<ExtractFileInfo>());
                                                                      list.Add(item);
                                                                      return dict;
                                                                  });

            var nonExistedFiles = new List<ExtractFileInfo>();

            foreach (var item in packageGroups)
            {
                var zipFile = new ZipFile(File.OpenRead(item.Key));

                foreach (var info in item.Value)
                {
                    var entryIndex = zipFile.FindEntry(info.RelativePath, true);
                    if (entryIndex == -1)
                        nonExistedFiles.Add(info);
                    else
                    {
                        info.Entry = zipFile[entryIndex];
                        info.ZipFile = zipFile;
                    }
                }
            }

            // todo: handle non exisiting files

            return packageGroups.Values.SelectMany(i => i).ToList();
        }

        private string GetDestinationRoot(string localPath)
        {
            if (this.UseRelativePath && localPath[localPath.Length - 1] != '/')
            {
                var lastSlashIndex = localPath.LastIndexOf('/');
                if (lastSlashIndex == -1)
                    return Path.Combine(this.Destination, localPath);
                else
                    return Path.Combine(this.Destination, localPath.Substring(0, lastSlashIndex));
            }
            else
                return this.Destination;
        }

        private List<ExtractFileInfo> BuildExtractFileListFromPaths()
        {

            _progressController.SetIndeterminate();

            var destinationRoot = this.GetDestinationRoot(this.LocalPath); // the root directory of output

            var zipFile = new ZipFile(File.OpenRead(this.PackagePath));
            var entries = zipFile.Cast<ZipEntry>().Where(entry => entry.Name.StartsWith(this.LocalPath)).ToArray();

            var fileList = new List<ExtractFileInfo>();

            foreach (var entry in entries)
            {
                if (_progressController.IsCanceled)
                {
                    _task = _task.ContinueWith(t => _progressController.CloseAsync().Wait())
                                 .ContinueWith(t => this.EndAndDispose(this.L("package_support", "extract_state_cancelled")));
                    return null;
                }

                string relativePath;
                if (this.UseRelativePath)
                {
                    relativePath = PathEx.Relativize(entry.Name, this.LocalPath);

                    if (relativePath[0] == '/')
                        relativePath = relativePath.Substring(1);
                }
                else
                {
                    relativePath = entry.Name;
                }

                var fileInfo = new ExtractFileInfo();
                fileInfo.Entry = entry;
                fileInfo.ZipFile = zipFile;
                fileInfo.DestinationRoot = destinationRoot;
                fileInfo.RelativePath = relativePath;
                fileInfo.PackagePath = this.PackagePath;

                fileList.Add(fileInfo);

            }

            return fileList;
        }
    }
}
