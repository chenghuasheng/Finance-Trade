using System;
using SmartQuant.Providers;

namespace Strategy.Assistants
{
    public class ReconnectProviderJob : Job
    {
        private IProvider provider;
        public ReconnectProviderJob(string name, IProvider provider, Job[] needJobs) : base(name, needJobs)
        {
            this.provider = provider;
        }
        public ReconnectProviderJob(string name, IProvider provider) : base(name)
        {
            this.provider = provider;
        }
        protected override bool doJob()
        {
            this.provider.Disconnect();
            this.provider.Connect(5000);
            return this.provider.IsConnected;
        }
    }
}
