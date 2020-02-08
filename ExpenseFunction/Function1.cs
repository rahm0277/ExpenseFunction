using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using CsvHelper;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace ExpenseFunction
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([BlobTrigger("transactions/{name}")]Stream myBlob, string name, TraceWriter log)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            using (StreamReader sr = new StreamReader(myBlob))
            {
                using (var csv = new CsvReader(sr,  CultureInfo.InvariantCulture))
                {
                    var records = new List<Expense>();
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        var record = new Expense
                        {
                            Date = csv.GetField<DateTime>("Date"),
                            Reference = csv.GetField("Reference"),
                            Description = csv.GetField("Description"),
                            //CardMember = csv.GetField("Card Member"),
                            //CardNumber = csv.GetField("Card Number"),
                            Amount = csv.GetField<Double>("Amount"),
                            Category = csv.GetField("Category"),
                            ChargeType = csv.GetField("Type")
                        };
                        records.Add(record);
                    }


                    var str = Environment.GetEnvironmentVariable("sqldb_connection");
                    using (SqlConnection conn = new SqlConnection(str))
                    {
                        conn.Open();

                        foreach (Expense e in records)
                        {
                            //log.Info(e.Description + " " + e.Amount.ToString());
                            //log.Info(System.Environment.NewLine);

                            string sql = "insert Expenses(Amount, Description, Title, Type, UserID, Category_CategoryID, Source_SourceID, User_id, created)";
                            sql += "values(@amount, @description, @title, @type, @userID, @category, @source, @userID1, getdate())";
                            //sql += "values(10, 'testing', 'asdasd', 1, 'dcb8d2ea-a71b-49f5-9e1d-b7ae8e58b53e', 14, 2, 'dc6d042d-06f0-4aa6-9070-941b89fdc856', getdate())";


                            using (SqlCommand cmd = new SqlCommand())
                            {
                                cmd.Connection = conn;
                                cmd.Parameters.Add("@amount", SqlDbType.Float).Value = e.Amount;
                                cmd.Parameters.Add("@description", SqlDbType.NVarChar, 500).Value = e.Description;
                                cmd.Parameters.Add("@title", SqlDbType.NVarChar, 500).Value = e.Description;
                                cmd.Parameters.Add("@type", SqlDbType.Int).Value = 1;
                                cmd.Parameters.Add("@userID", SqlDbType.UniqueIdentifier).Value = Guid.Parse("dc6d042d-06f0-4aa6-9070-941b89fdc856");
                                cmd.Parameters.Add("@category", SqlDbType.Int).Value = 14;
                                cmd.Parameters.Add("@source", SqlDbType.Int).Value = 1;
                                cmd.Parameters.Add("@userID1", SqlDbType.NVarChar, 128).Value = "dc6d042d-06f0-4aa6-9070-941b89fdc856";

                                cmd.CommandType = CommandType.Text;
                                cmd.ExecuteNonQuery();
                                log.Info(System.Environment.NewLine);
                            }
                        }

                        

                        
                    }

                    
                }
            }
        }
    }

    
}
