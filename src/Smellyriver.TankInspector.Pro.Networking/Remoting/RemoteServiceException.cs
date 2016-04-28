using System;

namespace Smellyriver.TankInspector.Pro.Networking.Remoting
{
    public class RemoteServiceException: ApplicationException
    {
        public RemoteServiceException(string message, Exception inner)
            : base(message, inner)
        {

        }

        public RemoteServiceException(Exception inner)
            : base("", inner)
        {

        }
    }
}
