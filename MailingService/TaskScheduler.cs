using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using System.Configuration;
using System.Threading;
using System.IO;

namespace MailingService
{
    class TaskScheduler : ITaskScheduler
    {
        private IScheduler _scheduler;
        
        public string Name
        {
            get { return GetType().Name; }
        }
        public void Run()
        {

            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            _scheduler = schedulerFactory.GetScheduler().Result;

            IJobDetail testJob = JobBuilder.Create<MainClass>()
                    .WithIdentity("startJob")
                    .Build();

            ITrigger testTrigger = TriggerBuilder.Create()
                    .WithIdentity("startTriggr")
                    //.WithCronSchedule(ConfigurationManager.AppSettings["Interval"])
                    //.Build();
            
                    .StartNow()
                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(150000).RepeatForever())
                    .Build();



            _scheduler.ScheduleJob(testJob, testTrigger);
            _scheduler.Start();
        }
        public void Stop()
        {
            _scheduler.Shutdown();
        }
        private void WriteToFile(string text, string file)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + file;
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
