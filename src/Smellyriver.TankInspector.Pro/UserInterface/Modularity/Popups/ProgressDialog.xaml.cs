using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using ProgressBarClass = System.Windows.Controls.ProgressBar;

namespace Smellyriver.TankInspector.Pro.UserInterface.Modularity.Popups
{
    public partial class ProgressDialog : Window
    {
        private const int GWL_HWNDPARENT = -8; // Owner --> not the parent

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, IntPtr newStyle);


        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(double), typeof(ProgressDialog), new PropertyMetadata(0.0, ProgressDialog.OnProgressChanged));

        private static void OnProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ProgressDialog)d).OnProgressChanged((double)e.NewValue);
        }
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(ProgressDialog), new PropertyMetadata(string.Empty));

        public bool IsProgressIndeterminate
        {
            get { return (bool)GetValue(IsProgressIndeterminateProperty); }
            set { SetValue(IsProgressIndeterminateProperty, value); }
        }

        public static readonly DependencyProperty IsProgressIndeterminateProperty =
            DependencyProperty.Register("IsProgressIndeterminate", typeof(bool), typeof(ProgressDialog), new PropertyMetadata(false));

        public bool IsCancellable
        {
            get { return (bool)GetValue(IsCancellableProperty); }
            set { SetValue(IsCancellableProperty, value); }
        }

        public static readonly DependencyProperty IsCancellableProperty =
            DependencyProperty.Register("IsCancellable", typeof(bool), typeof(ProgressDialog), new PropertyMetadata(true));

        public bool IsCanceled { get; private set; }
        public bool IsOpen { get; private set; }

        public IProgressDialogController ProgressDialogController { get; private set; }

        private readonly IntPtr _ownerHandle;

        public ProgressDialog(IntPtr ownerHandle)
        {
            _ownerHandle = ownerHandle;
            InitializeComponent();
            this.ProgressDialogController = new ProgressDialogController(this);
            this.Loaded += ProgressDialog_Loaded;
            this.Unloaded += ProgressDialog_Unloaded;
        }

        private void OnProgressChanged(double progress)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                var duration = new Duration(TimeSpan.FromSeconds(Math.Abs((progress - this.ProgressBar.Value)) / 0.5));
                var progressBarAnimation = new DoubleAnimation(progress, duration);
                this.ProgressBar.BeginAnimation(RangeBase.ValueProperty, progressBarAnimation);
            }));
        }

        void ProgressDialog_Unloaded(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
        }

        void ProgressDialog_Loaded(object sender, RoutedEventArgs e)
        {
            this.IsOpen = true;
            ProgressDialog.SetWindowLong(new WindowInteropHelper(this).Handle, GWL_HWNDPARENT, _ownerHandle);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.IsCancellable)
                return;

            this.IsCanceled = true;
            this.AnimateAndClose();
        }

        public void AnimateAndShow()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.Show();
                var duration = new Duration(TimeSpan.FromSeconds(0.5));
                var opacityAnimation = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromSeconds(0.5)));
                this.BeginAnimation(OpacityProperty, opacityAnimation);
            }));
        }

        public Task AnimateAndClose()
        {
            return (Task)this.Dispatcher.Invoke(new Func<Task>(() =>
            {
                var isClosed = false;
                var duration = new Duration(TimeSpan.FromSeconds(0.5));
                var opacityAnimation = new DoubleAnimation(1.0, 0.0, new Duration(TimeSpan.FromSeconds(0.5)));
                opacityAnimation.Completed +=
                    (o, e) =>
                    {
                        isClosed = true;
                        this.Close();
                    };
                this.BeginAnimation(OpacityProperty, opacityAnimation);

                return Task.Factory.StartNew(
                    () =>
                    {
                        while (!isClosed)
                            Thread.Sleep(10);
                    });
            }));
        }
    }
}
