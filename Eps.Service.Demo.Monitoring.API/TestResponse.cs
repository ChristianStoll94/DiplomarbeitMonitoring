using System;
using System.Collections.Generic;
using System.Text;

namespace Eps.Service.Demo.Monitoring.API
{
    public class TestResponse : Response
    {
        public enum TestErrorCodes : int
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

        public TestResponse(int uniqueId, TestErrorCodes errorCode, string errorText)
            : base(uniqueId)
        {
            ErrorCode = errorCode;
            ErrorText = errorText;
        }

        public TestErrorCodes ErrorCode { get; set; }

        public string ErrorText { get; set; }

    }
}
