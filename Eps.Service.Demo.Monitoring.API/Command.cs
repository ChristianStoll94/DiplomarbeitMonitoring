using Destructurama.Attributed;
using Eps.Framework.Logging.Extensions.Pattern;
using Eps.Framework.Randomizer;

namespace Eps.Service.Demo.Monitoring.API
{
    public abstract class Command : LoggingObject
    {
        private static readonly UniqueIdRandomizer UniqueIdRandomizer;

        static Command()
        {
            UniqueIdRandomizer = new UniqueIdRandomizer();
        }

        public Command()
        {
            UniqueId = UniqueIdRandomizer.Next();
            PropertyToExcludeFromLogging = "SomeString";
            PropertyWithCustomLoggingName = "TestString";
        }

        public Command(string identification)
            : this()
        {
            Identification = identification;
        }

        public Command(int uniqueId, string identification)
            : this()
        {
            UniqueId = uniqueId;
            Identification = identification;
        }

        public int UniqueId { get; set; }

        public string Identification { get; set; }

        [NotLogged]
        public string PropertyToExcludeFromLogging { get; set; }

        [LogWithName("ExampleName")]
        public string PropertyWithCustomLoggingName { get; set; }
    }
}