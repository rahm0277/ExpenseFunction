using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ExpenseFunction
{
    public static class Function2
    {
        [FunctionName("Function2")]
        public static void Run([TimerTrigger("0 0 10,22 * * *")]TimerInfo myTimer, TraceWriter log)
        //public static void Run([TimerTrigger("0 0 10 * * *")]TimerInfo myTimer, TraceWriter log)
        //public static void Run([TimerTrigger("0 * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

            try
            {
                int recordCount = 0;
                var str = Environment.GetEnvironmentVariable("sqldb_connection");
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();

                    string getFileImports = "select * from FileImports where status = 0";

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = getFileImports;
                        cmd.Connection = conn;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            
                            while (reader.Read())
                            {
                                recordCount++;
                                int fileImportID = int.Parse(reader["FileImportID"].ToString());
                                Runner runner = new Runner(reader["FileNameAndPath"].ToString(), Guid.Parse(reader["UserID"].ToString()), fileImportID, log);
                                runner.Run();
                            }
                        }

                    }

                }


                string msg = "Imports completed: " + recordCount.ToString() + System.Environment.NewLine;
                log.Info(msg);
                
            }
            catch(Exception ex)
            {
                string errMsg = "---------------------------------" + System.Environment.NewLine;
                errMsg += "ERROR" + System.Environment.NewLine;
                errMsg += ex.Message + System.Environment.NewLine;
                errMsg += ex.StackTrace + System.Environment.NewLine;
                errMsg += "---------------------------------";
                log.Info(errMsg);
            }

        }

        
    }
}
