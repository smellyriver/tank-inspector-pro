using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;

namespace Smellyriver.TankInspector.Pro.Sandbox
{
    class Program
    {
        private static readonly ILog log = SafeLog.GetLogger("Sandbox");

        private static Type[] s_ignoredFirstChanceExceptions = 
        {
            typeof( AmbiguousMatchException ),
        };

        static void Main(string[] args)
        {
            var permissions = new PermissionSet(PermissionState.Unrestricted);
            // ps.AddPermission(new System.Security.Permissions.*); // Add Whatever Permissions you want to grant here

            var setup = new AppDomainSetup();
            setup.ApplicationBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var evidence = new Evidence();

            var sandbox = AppDomain.CreateDomain("Tank Inspector PRO Sandbox",
                evidence,
                setup,
                permissions);

            sandbox.FirstChanceException += sandbox_FirstChanceException;
            sandbox.UnhandledException += sandbox_UnhandledException;

            sandbox.ExecuteAssembly("stipro.exe");
        }

        static void sandbox_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            if(exception!=null)
                log.Error("an unhandled chance exception occurred: ", exception);
        }

        static void sandbox_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            var isInPotentialExceptionRegion = Diagnostics.IsInPotentialExceptionRegion;
            using (Diagnostics.PotentialExceptionRegion)
            {

                try
                {
                    if (!isInPotentialExceptionRegion
                        && !s_ignoredFirstChanceExceptions.Any(type => e.Exception.GetType() == type))
                    {
                        log.Error("an first chance exception occurred: ", e.Exception);
                    }
                }
                catch (Exception)
                {
                    //don't do anything, let'em go.
                }
            }
        }
    }
}
