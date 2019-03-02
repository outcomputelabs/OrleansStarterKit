namespace Silo.Options
{
    public class SiloHostedServiceOptions
    {
        public PortRange SiloPortRange { get; } = new PortRange();
        public PortRange GatewayPortRange { get; } = new PortRange();
        public PortRange DashboardPortRange { get; } = new PortRange();
        public string AdoNetConnectionString { get; set; }
        public string AdoNetInvariant { get; set; }
        public string ClusterId { get; set; }
        public string ServiceId { get; set; }
    }
}