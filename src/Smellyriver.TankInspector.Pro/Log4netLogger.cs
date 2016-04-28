using log4net;
using Microsoft.Practices.Prism.Logging;

namespace Smellyriver.TankInspector.Pro
{
    class Log4NetLogger : ILoggerFacade
    {

        private readonly ILog m_Logger = SafeLog.GetLogger("Prism");

        public void Log(string message, Category category, Priority priority)
        {
            switch (category)
            {
                case Category.Debug:
                    m_Logger.Debug(message);
                    break;
                case Category.Warn:
                    m_Logger.Warn(message);
                    break;
                case Category.Exception:
                    m_Logger.Error(message);
                    break;
                case Category.Info:
                    m_Logger.Info(message);
                    break;
            }
        }

    }
}
