namespace Silo.Options
{
    public class SiloHostedServiceOptions
    {
        public PortRange SiloPortRange { get; } = new PortRange();
        public PortRange GatewayPortRange { get; } = new PortRange();
        public PortRange DashboardPortRange { get; } = new PortRange();

        public string AdoNetClusteringConnectionString { get; set; }
        public string AdoNetClusteringInvariant { get; set; }

        public string AdoNetDefaultStorageConnectionString { get; set; }
        public string AdoNetDefaultStorageInvariant { get; set; }

        public string AdoNetPubSubStorageConnectionString { get; set; }
        public string AdoNetPubSubStorageInvariant { get; set; }

        public string ClusterId { get; set; }
        public string ServiceId { get; set; }
        public SiloHostedServiceClusteringProvider ClusteringProvider { get; set; } = SiloHostedServiceClusteringProvider.Localhost;
        public SiloHostedServiceReminderProvider ReminderProvider { get; set; } = SiloHostedServiceReminderProvider.InMemory;
        public SiloHostedServiceStorageProvider DefaultStorageProvider { get; set; } = SiloHostedServiceStorageProvider.InMemory;
        public SiloHostedServiceStorageProvider PubSubStorageProvider { get; set; } = SiloHostedServiceStorageProvider.InMemory;
    }
}