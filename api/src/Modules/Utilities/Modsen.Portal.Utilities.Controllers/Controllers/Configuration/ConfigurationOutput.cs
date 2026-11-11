namespace Modsen.Portal.Utilities.Controllers.Configuration
{
    public class ConfigurationOutput
    {
        public PersistentValues PersistentValues { get; set; } = new();
        public ConfigurableValues ConfigurableValues { get; set; } = new();
    }
}
