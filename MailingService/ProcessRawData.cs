using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MailingService
{
    class ProcessRawData
    {
        public List<List<String>> present = new List<List<String>>();
        public List<List<String>> absent = new List<List<String>>();

        public int dayNo = 0;
        public DataTable GetMachineAndEmailInfo(SqlConnectionStringBuilder builder)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                String cmd = "SELECT [Building Name],[Hostel Name],[Machine No],[Machine I.P],[Warden],[Asst. Warden],[Warden Email I.D],[Asst. Warden Email I.D],[Caretaker],[Caretaker Email I.D],[Status],[Resident Warden],[Resident Warden Email I.D] FROM [MachineData].[dbo].[MachineCodeEmails]";
                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    connection.Open();
                    using (SqlDataReader dr = command.ExecuteReader())
                    {
                        dt.Load(dr);
                    }
                }
                connection.Close();
            }
            return dt;
        }
        public DataTable GetStudentData(SqlConnectionStringBuilder builder, string buildingName)
        {
            DataTable stuAtHosteldt = new DataTable();
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                String cmd = "SELECT [Roll No], [Name], [Mobile Number], [Building Name], [Room No], [Floor],[email id] FROM[StudentsDATA].[dbo].[HostelRoomData] WHERE[StudentsDATA].[dbo].[HostelRoomData].[Building Name] = @bName";
                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@bName", buildingName);
                    connection.Open();
                    using (SqlDataReader stuAtHosteldr = command.ExecuteReader())
                    {
                        stuAtHosteldt.Load(stuAtHosteldr);
                    }
                }
                connection.Close();
            }
            return stuAtHosteldt;
        }
        public DataTable GetProCommData(SqlConnectionStringBuilder builder, String rawStr)
        {
            DataTable dailyHostelAttddt = new DataTable();
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                String cmd = "SELECT [CommandData] FROM [ProComm].[dbo].[tblTAttendanceJob] WHERE [ProComm].[dbo].[tblTAttendanceJob].[CommandData] LIKE @rawData;";
                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@rawData", rawStr);
                    connection.Open();
                    
                    using (SqlDataReader dailyHostelAttddr = command.ExecuteReader())
                    {
                        dailyHostelAttddt.Load(dailyHostelAttddr);
                    }
                    connection.Close();
                }
            }
            return dailyHostelAttddt;
        }
        public void ProcessProCommData(SqlConnectionStringBuilder builder, DataTable stuAtHosteldt, String buildingNo)
        {
            foreach (DataRow stuRowdr in stuAtHosteldt.Rows)
            {
                List<String> datalist = new List<String> { stuRowdr.Field<String>(0).Trim(), stuRowdr.Field<String>(1).Trim() };

                String todayDate = DateTime.Now.AddDays(this.dayNo).ToString("MMddyyyy");
                UtilityClass utility = new UtilityClass();
                String rollCode = utility.GetCode(stuRowdr.Field<String>(0).Trim());
                String rawStr = "AA" + buildingNo + todayDate + "____" + rollCode + "__________";

                DataTable dailyHostelAttddt = this.GetProCommData(builder, rawStr);

                int cnt = 0;
                foreach (DataRow dhdr in dailyHostelAttddt.Rows)
                {
                    UtilityClass utime = new UtilityClass();
                    if (utime.ValidateEntryTime(dhdr.Field<String>(0), this.dayNo))
                    {
                        datalist.Add(utime.Gettime(dhdr.Field<String>(0)).ToString());
                        ++cnt;
                    }
                }
                if (cnt > 0)
                {
                    this.present.Add(datalist);
                }
                else
                {
                    datalist.Add(stuRowdr.Field<String>(2).Trim());
                    datalist.Add(stuRowdr.Field<String>(3).Trim());
                    datalist.Add(stuRowdr.Field<String>(4).Trim());
                    datalist.Add(stuRowdr.Field<String>(5).Trim());
                    datalist.Add(stuRowdr.Field<String>(6).Trim());
                    this.absent.Add(datalist);
                }

            }
        }
        public void ProcessDataForAbsents(SqlConnectionStringBuilder builder, DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                this.absent = new System.Collections.Generic.List<System.Collections.Generic.List<string>>();
                this.present = new System.Collections.Generic.List<System.Collections.Generic.List<string>>();
                if (dr.Field<String>(10).ToString().Trim() == "NO")
                {
                    continue;
                }

                String buildingName = dr.Field<String>(0);
                String buildingNo = dr.Field<String>(2).Trim();

                DataTable stuAtHosteldt = this.GetStudentData(builder, buildingName);
                this.ProcessProCommData(builder, stuAtHosteldt, buildingNo);

                Email mail = new Email();
                mail.SendEmailTOAbsenties(this.absent, this.dayNo);
            }
        }
        public void ProcessDataForEmailing(SqlConnectionStringBuilder builder, DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                this.absent = new System.Collections.Generic.List<System.Collections.Generic.List<string>>();
                this.present = new System.Collections.Generic.List<System.Collections.Generic.List<string>>();
                if (dr.Field<String>(10).ToString().Trim() == "NO")
                {
                    continue;
                }
                String buildingName = dr.Field<String>(0);
                String buildingNo = dr.Field<String>(2).Trim();

                DataTable stuAtHosteldt = this.GetStudentData(builder, buildingName);
                this.ProcessProCommData(builder, stuAtHosteldt, buildingNo);

                CreatePDF makepdf = new CreatePDF
                {
                    dayNo = this.dayNo
                };

                String pathPresent = makepdf.MakePDFPresent(this.present, buildingName);
                String pathAbsent = makepdf.MakePDFAbsent(this.absent, buildingName);
                Console.Write(" Pdf-Generated \t");

                KeyValuePair<String, String> toMail = new KeyValuePair<String, String>(dr.Field<String>(8).ToString().Trim(), dr.Field<String>(9).ToString().Trim());
                UtilityClass utc = new UtilityClass();
                List<KeyValuePair<String, String>> cc = utc.AddCCs(dr);


                List<String> attachments = new List<string>
                    {
                        pathAbsent,
                        pathPresent
                    };

                Email mail = new Email();
                mail.SendEmailTOCaretakerWardens(toMail, cc, attachments, absent, buildingName, this.dayNo);

                // Take Rest
            }
        }
        public void TakeBackUpFromMaster(SqlConnectionStringBuilder builder)
        {
            String name = "BackUp " + string.Format(DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt"));
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                String cmd = "SELECT* INTO[BackUpProCommData].[dbo].[" + name + "]" +
                    " FROM[ProComm].[dbo].[tblTAttendanceJob]" ;
                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public void DeleteMasterData(SqlConnectionStringBuilder builder)
        {
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                String cmd = "DELETE FROM[ProComm].[dbo].[tblTAttendanceJob]";
                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

    }
}
