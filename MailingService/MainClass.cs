using System;
using System.Threading.Tasks;
using Quartz;
using System.Threading;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Collections.Generic;

namespace MailingService
{
    internal class MainClass : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine(string.Format("Execute..."+ DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = ConfigurationManager.AppSettings["DataSource"].ToString(),   // update me
                UserID = ConfigurationManager.AppSettings["UserID"].ToString(),              // update me
                Password = ConfigurationManager.AppSettings["PasswordDatabase"].ToString(),      // update me
                InitialCatalog = ConfigurationManager.AppSettings["InitialCatalog"].ToString()
            };

            try
            {
                //Create SqlConnection
                ProcessRawData prd = new ProcessRawData();
                DataTable dt = prd.GetMachineAndEmailInfo(builder);
                
                Console.WriteLine(string.Format(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));

                prd.dayNo = -1;

                //List<String> ls = new List<string>()
                //{
                //    "B17043",
                //    "dipesh",
                //    "7004617522",
                //    "B-9",
                //    "1106",
                //    "S",
                //    "B17043@students.iitmandi.ac.in",
                //};
                //Email mail = new Email();
                //List<List<String>> lls = new List<List<String>>() { ls };
                //mail.SendEmailTOAbsenties(lls, prd.dayNo);

                prd.ProcessDataForAbsents(builder, dt);

                Console.WriteLine("Email sent to students." + string.Format(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));

                Thread.Sleep(TimeSpan.FromMinutes(30));

                Console.WriteLine(string.Format(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));

                prd.dayNo = -1;
                prd.ProcessDataForEmailing(builder, dt);

                prd.TakeBackUpFromMaster(builder);
                prd.DeleteMasterData(builder);
                Console.WriteLine("End .. ");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return null;
        }
    }
}
