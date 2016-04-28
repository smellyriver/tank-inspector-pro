using System;
using System.Collections.Generic;
using log4net;
using Smellyriver.TankInspector.Common.Utilities;

namespace Smellyriver.TankInspector.Pro
{
    public static class LogExtensions
    {
        private readonly static Dictionary<Type, ILog> _loggers
            = new Dictionary<Type, ILog>();

        public static ILog GetLogger(this Type type)
        {
            return _loggers.GetOrCreate(type, t => SafeLog.GetLogger(t.Name));
        }

        public static ILog GetLogger(this object @this)
        {
            return LogExtensions.GetLogger(@this.GetType());
        }

        public static void LogError(this object @this, string message, Exception ex)
        {
            LogExtensions.GetLogger(@this).Error(message, ex);
        }


        public static void LogError(this object @this, string message)
        {
            LogExtensions.GetLogger(@this).Error(message);
        }

        public static void LogError(this object @this, string message, object arg)
        {
            LogExtensions.GetLogger(@this).ErrorFormat(message, arg);
        }

        public static void LogError(this object @this, string message, object arg0, object arg1)
        {
            LogExtensions.GetLogger(@this).ErrorFormat(message, arg0, arg1);
        }

        public static void LogError(this object @this, string message, object arg0, object arg1, object arg2)
        {
            LogExtensions.GetLogger(@this).ErrorFormat(message, arg0, arg1, arg2);
        }

        public static void LogError(this object @this, string message, params object[] args)
        {
            LogExtensions.GetLogger(@this).ErrorFormat(message, args);
        }

        public static void LogWarning(this object @this, string message)
        {
            LogExtensions.GetLogger(@this).Warn(message);
        }

        public static void LogWarning(this object @this, string message, object arg)
        {
            LogExtensions.GetLogger(@this).WarnFormat(message, arg);
        }

        public static void LogWarning(this object @this, string message, object arg0, object arg1)
        {
            LogExtensions.GetLogger(@this).WarnFormat(message, arg0, arg1);
        }

        public static void LogWarning(this object @this, string message, object arg0, object arg1, object arg2)
        {
            LogExtensions.GetLogger(@this).WarnFormat(message, arg0, arg1, arg2);
        }

        public static void LogWarning(this object @this, string message, params object[] args)
        {
            LogExtensions.GetLogger(@this).WarnFormat(message, args);
        }

        public static void LogInfo(this object @this, string message)
        {
            LogExtensions.GetLogger(@this).Info(message);
        }

        public static void LogInfo(this object @this, string message, object arg)
        {
            LogExtensions.GetLogger(@this).InfoFormat(message, arg);
        }

        public static void LogInfo(this object @this, string message, object arg0, object arg1)
        {
            LogExtensions.GetLogger(@this).InfoFormat(message, arg0, arg1);
        }

        public static void LogInfo(this object @this, string message, object arg0, object arg1, object arg2)
        {
            LogExtensions.GetLogger(@this).InfoFormat(message, arg0, arg1, arg2);
        }

        public static void LogInfo(this object @this, string message, params object[] args)
        {
            LogExtensions.GetLogger(@this).InfoFormat(message, args);
        }
    }
}
