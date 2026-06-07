using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Service
{
    [RunInstaller(true)]
    public class ProgramInstaller : Installer
    {
        public ProgramInstaller()
            : base()
        {
            ServiceProcessInstaller process = new()
            {
                Account = ServiceAccount.LocalSystem,
                Username = Environment.UserName,
            };

            ServiceInstaller service = new()
            {
                ServiceName = "SampleService",
                DisplayName = "Sample .Net Service",
                Description = "Sample .Net Service Description.",
                StartType = ServiceStartMode.Automatic,
            };

            _ = this.Installers.Add(process);
            _ = this.Installers.Add(service);
        }
    }
}
