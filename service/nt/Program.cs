using System.Configuration.Install;
using System.ServiceProcess;
using Microsoft.Win32;

namespace Service
{
    public class Program : ServiceBase
    {
        public static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                if (TryInstall(args[0]))
                {
                    return;
                }
            }

            Run(new Program());
            return;
        }

        public static bool TryInstall(string option)
        {
            string mode = option.ToLowerInvariant();
            string path = typeof(Program).Assembly.Location;

            if (mode == "/i")
            {
                ManagedInstallerClass.InstallHelper([path]);
                return true;
            }
            else if (mode == "/u")
            {
                ManagedInstallerClass.InstallHelper(["/u", path]);
                return true;
            }

            return false;
        }

        public Program()
            : base()
        {
            this.AutoLog = true;
            this.CanShutdown = true;
            this.CanStop = true;
            this.ServiceName = "SampleService";

            this.EventLog.WriteEntry("インスタンスが作成されました。");
        }

        protected override void OnStart(string[] args)
        {
            int timeout = this.GetDword("BeforeStartupTime", 10);
            this.EventLog.WriteEntry($"{timeout}秒後にサービスを開始します。");

            this.RequestAdditionalTime(timeout * 1000);
            Task.Delay(TimeSpan.FromSeconds(timeout)).Wait();

            this.EventLog.WriteEntry("サービスを開始しました。");
        }

        protected override void OnStop()
        {
            this.EventLog.WriteEntry("サービスを終了します。");

            this.RequestAdditionalTime(10 * 1000);

            this.EventLog.WriteEntry("サービスを終了しました。");
        }

        private int GetDword(string value, int alternative)
        {
            try
            {
                var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\SAMPLESERVICE");
                return (int)key.GetValue(value, alternative, RegistryValueOptions.None);

            }
            catch { }

            return alternative;
        }
    }
}
