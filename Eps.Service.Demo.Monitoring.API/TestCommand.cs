using System;
using System.Collections.Generic;
using System.Text;

namespace Eps.Service.Demo.Monitoring.API
{
    public class TestCommand : Command
    {
        public bool Valid { get; set; }
        public bool ThrowHandledException { get; set; }
        public bool ThrowUnhandledException { get; set; }
        public bool ThrowMiddleWareException { get; set; }
        public bool IncrementCounter { get; set; }
        public string ExampleParameter { get; set; }
    }
}
