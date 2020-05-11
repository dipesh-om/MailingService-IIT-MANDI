using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Security.Permissions;

namespace MailingService
{

    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
    public partial class MainService : ServiceBase
    {
        public MainService()
        {
            InitializeComponent();
        }
        ITaskScheduler scheduler;
        public void OnDebug()
        {
            OnStart(null);
        }
        protected override void OnStart(string[] args)
        {
            System.Diagnostics.Debugger.Launch();
            //System.IO.File.Create(AppDomain.CurrentDomain.BaseDirectory + "OnStart.txt");
            System.IO.File.Create(AppDomain.CurrentDomain.BaseDirectory + "ServiceLog.txt");
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);

            scheduler = new TaskScheduler();
            scheduler.Run();
        }
        protected override void OnStop()
        {
            if (scheduler != null)
            {
                scheduler.Stop();
            }
        }
        void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            WriteToFile("Simple Service Error on: {0} " + e.Message + e.StackTrace);
        }
        private void WriteToFile(string text)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "ServiceLog.txt";
            try
            {
                StreamWriter writer = new StreamWriter(path, true);
                writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                writer.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
