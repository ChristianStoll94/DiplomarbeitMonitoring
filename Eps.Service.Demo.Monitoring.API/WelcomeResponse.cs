using System.Reflection;
using Eps.Framework.Reflection;

namespace Eps.Service.Demo.Monitoring.API
{
    public class WelcomeResponse : Response
    {
        public enum WelcomeErrorCodes : int
        {
            Invalid = -1,
            Undefined = 0,

            NoError = 1,
            /// <summary>
            /// Marked when an unexpected error has occurred
            /// </summary>
            UnexpectedException = 2,
            /// <summary>
            /// Marked when an unspecified error has occurred
            /// </summary>
            ExecutionError = 3,

            /// <summary>
            /// Marked when the command is unknown
            /// </summary>
            UnknownCommand = 4,

            InvalidParameter = 10,
        }

        public WelcomeResponse()
            : base()
        {

        }

        public WelcomeResponse(int uniqueId, WelcomeErrorCodes errorCode, string errorText)
            : base(uniqueId)
        {
            ErrorCode = errorCode;
            ErrorText = errorText;
        }

        public WelcomeResponse(int uniqueId, WelcomeErrorCodes errorCode, string errorText, string version)
            : base(uniqueId)
        {
            ErrorCode = errorCode;
            ErrorText = errorText;
            Version = version;
            ApiVersion = new AssemblyReader(Assembly.GetExecutingAssembly()).Version.ToString();
        }

        public WelcomeErrorCodes ErrorCode { get; set; }

        public string ErrorText { get; set; }

        public string Version { get; set; }

        public string ApiVersion { get; set; }
    }
}