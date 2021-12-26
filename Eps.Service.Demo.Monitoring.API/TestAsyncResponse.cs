using System;
using System.Collections.Generic;
using System.Text;

namespace Eps.Service.Demo.Monitoring.API
{
    public class TestAsyncResponse : Response
    {
        public enum TestAsyncErrorCodes : int
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

        public TestAsyncResponse()
            : base()
        {

        }

        public TestAsyncResponse(int uniqueId, TestAsyncErrorCodes errorCode, string errorText)
            : base(uniqueId)
        {
            ErrorCode = errorCode;
            ErrorText = errorText;
        }

        public TestAsyncErrorCodes ErrorCode { get; set; }

        public string ErrorText { get; set; }

        public string ResponseString { get; set; }
    }
}
