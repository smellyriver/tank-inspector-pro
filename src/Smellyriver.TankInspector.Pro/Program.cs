using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Threading;
using log4net;
using Smellyriver.TankInspector.Common;
using Smellyriver.TankInspector.Common.Utilities;

namespace Smellyriver.TankInspector.Pro
{
    static class Program
    {
        private static readonly ILog log = SafeLog.GetLogger("Program");

        static Type[] s_ignoredFirstChanceExceptions = 
        {
            typeof( AmbiguousMatchException ),
        };

        private static readonly Queue<Exception> s_recentFirstChanceExceptions
            = new Queue<Exception>();

        private const int c_recentFirstChanceExceptionsToKeep = 10;

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        public static void InitializeUnhandledExceptionHandler()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;

            currentDomain.FirstChanceException += Program.FirstChanceExceptionHandler;

            if (Debugger.IsAttached)
            {
                return; //do nothing when debugger attached
            }

            currentDomain.UnhandledException += Program.OnUnhandledExceptionThrown;

            Application.Current.DispatcherUnhandledException += Program.OnDispatcherUnhandledExceptionThrown;
        }


        private static void FirstChanceExceptionHandler(object sender, FirstChanceExceptionEventArgs e)
        {
            var isInPotentialExceptionRegion = Diagnostics.IsInPotentialExceptionRegion;
            using (Diagnostics.PotentialExceptionRegion)
            {

                try
                {
                    if (!isInPotentialExceptionRegion
                        && !s_ignoredFirstChanceExceptions.Any(type => e.Exception.GetType() == type))
                    {
                        s_recentFirstChanceExceptions.Enqueue(e.Exception);
                        if (s_recentFirstChanceExceptions.Count > c_recentFirstChanceExceptionsToKeep)
                            s_recentFirstChanceExceptions.Dequeue();
                    }
                }
                catch (Exception)
                {
                    //don't do anything, let'em go.
                }
            }
        }

        public static void LogLastFirstChanceException()
        {
            log.Fatal("===========================================\n10 most recent first-chance exceptions:");
            for (var i = 0; i < s_recentFirstChanceExceptions.Count; ++i )
            {
                var ex = s_recentFirstChanceExceptions.Dequeue();
                log.Fatal(i, ex);
            }
        }

        private static void OnDispatcherUnhandledExceptionThrown(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            log.Fatal("an unhandled exception occurre in dispatcher: ", e.Exception);
            Program.LogLastFirstChanceException();

            var trace = new StackTrace(e.Exception);
            MessageBox.Show(string.Format(
                "An unhandled exception occurred in the dispatcher, application will be terminated.\nException information :\n {0}\nStackTrace :\n {1}",
                e.Exception.ToInformationString(),
                trace.ToString()));
            Environment.Exit(1);
        }

        private static void OnUnhandledExceptionThrown(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception)
            {
                log.Fatal("an unhandled exception occurre :", (Exception)e.ExceptionObject);
            }

            Program.LogLastFirstChanceException();

            MessageBox.Show("An unhandled exception occurred, application will be terminated.");
            Environment.Exit(1);
        }



        private static string[] hardwares = 
        {
          "Win32_OperatingSystem",
          "Win32_Processor",
          "Win32_PhysicalMemory",
          "Win32_VideoController",     
        };



        public static void LogHardwareInfo()
        {
            try
            {
                foreach (var hardware in hardwares)
                {
                    var searcher = new ManagementObjectSearcher("SELECT * FROM " + hardware);

                    var hardwareLog = SafeLog.GetLogger(hardware);

                    foreach (ManagementObject managementObject in searcher.Get())
                    {
                        var searcherProperties = managementObject.Properties;
                        foreach (var property in searcherProperties)
                        {
                            hardwareLog.Info(property.Name + ' ' + property.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("logging hardware info", ex);
            }
        }
    }
}
