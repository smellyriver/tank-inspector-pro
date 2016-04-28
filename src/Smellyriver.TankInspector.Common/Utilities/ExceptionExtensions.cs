using System;
using System.Collections.Generic;
using System.Text;

namespace Smellyriver.TankInspector.Common.Utilities
{
    public static class ExceptionExtensions
    {
        public static string ToInformationString(this Exception ex)
        {
            StringBuilder infoString = new StringBuilder();
            infoString.AppendFormat("{0}:\n", ex.GetType().FullName);
            infoString.AppendFormat("    {0}\n", ex.Message);
            var inner = ex.InnerException;
            while (inner != null)
            {
                infoString.AppendFormat("Inner Exception: {0}:\n", inner.GetType().FullName);
                infoString.AppendFormat("    {0}\n", inner.Message);
                inner = inner.InnerException;
            }

            return infoString.ToString();
        }

        public static IEnumerable<Exception> ExpandException(this Exception ex)
        {
            var inner = ex.InnerException;
            while (inner != null)
            {
                yield return inner;
                inner = inner.InnerException;
            }
        }
    }
}
