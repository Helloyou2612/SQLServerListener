using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;

namespace SQLServerListener
{
    internal class Program
    {
        private static string _con = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=CU10;Data Source=.;";

        private static void Main(string[] args)
        {
            var mapper = new ModelToTableMapper<Customer>();
            mapper.AddMapping(c => c.Id, "Id");
            mapper.AddMapping(c => c.FirstName, "FirstName");

            using (var dep = new SqlTableDependency<Customer>(_con, tableName: "Emp", mapper: mapper, notifyOn: DmlTriggerType.All))
            {
                dep.OnChanged += TableDependency_Changed;
                dep.OnError += TableDependency_OnError;
                dep.OnStatusChanged += TableDependency_OnStatusChanged;

                dep.TraceLevel = TraceLevel.Verbose;
                dep.TraceListener = new TextWriterTraceListener(Console.Out); //log console
                //dep.TraceListener = new TextWriterTraceListener(File.Create("c:\\temp\\output.txt")); //log file
                dep.Start();

                Console.ReadKey();

                dep.Stop();
            }
        }

        private static void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        {
            throw e.Error;
        }

        private static void TableDependency_Changed(object sender, RecordChangedEventArgs<Customer> e)
        {
            if (e.ChangeType != ChangeType.None)
            {
                Console.WriteLine("\n================Events=============================\n");
                var changedEntity = e.Entity;
                Console.WriteLine("DML operation: " + e.ChangeType);
                Console.WriteLine("ID: " + changedEntity.Id);
                Console.WriteLine("Name: " + changedEntity.FirstName);
                Console.WriteLine("\n===================Result==========================\n");

                List<Customer> lst = GetAllStocks();
                foreach (var item in lst)
                {
                    Console.WriteLine("----------------------------------------------\n");
                    Console.WriteLine("Id: " + item.Id);
                    Console.WriteLine("First Name: " + item.FirstName);
                    Console.WriteLine("Last Name: " + item.LastName);
                }

                Console.WriteLine("\n Press a key to exit");
            }
        }

        private static void TableDependency_OnStatusChanged(object sender, StatusChangedEventArgs e)
        {
            Console.WriteLine(@"Status: " + e.Status);
        }

        public static List<Customer> GetAllStocks()
        {
            List<Customer> lstCustomer = new List<Customer>();
            using (var sqlConnection = new SqlConnection(_con))
            {
                sqlConnection.Open();
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "SELECT * FROM Emp";

                    using (var sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            var Id = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("Id"));
                            var name = sqlDataReader.GetString(sqlDataReader.GetOrdinal("FirstName"));
                            var Surname = sqlDataReader.GetString(sqlDataReader.GetOrdinal("LastName"));

                            lstCustomer.Add(new Customer { Id = Id, FirstName = name, LastName = Surname });
                        }
                    }
                }
            }

            return lstCustomer;
        }
    }
}