using Eps.Framework.Logging.Extensions.Pattern;

namespace Eps.Service.Demo.Monitoring.API
{
    public abstract class Response : LoggingObject
    {
        protected Response()
        {

        }

        protected Response(int uniqueId)
        {
            UniqueId = uniqueId;
        }

        public int UniqueId { get; set; }
    }
}