namespace Silo.Options
{
    public class SupportApiOptions
    {
        public string Title { get; set; }
        public PortRange PortRange { get; } = new PortRange();
    }
}
