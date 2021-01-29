using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;

namespace SQLServerListener
{
    internal class Program
    {
        private static string _con = "Data Source=.;Initial Catalog=SQLServerListener;User ID=sa;Password=123456;persist security info=True";

        private static SqlTableDependency<Customer> _dependency = new SqlTableDependency<Customer>(_con,
            schemaName: "dbo",
            tableName: "Emp",
            mapper: ModelToTableMapper.CustomerMapper(),
            includeOldValues: true,
            notifyOn: DmlTriggerType.All);

        private static void Main(string[] args)
        {
            try
            {
                _dependency.OnChanged += TableDependency_Changed;
                _dependency.OnError += TableDependency_OnError;
                _dependency.OnStatusChanged += TableDependency_OnStatusChanged;

                _dependency.TraceLevel = TraceLevel.Verbose;
                _dependency.TraceListener = new TextWriterTraceListener(Console.Out); //log console
                _dependency.TraceListener = new TextWriterTraceListener(File.Create("LogFiles\\output.txt")); //log file
                _dependency.Start();

                Console.WriteLine("Waiting for receiving notifications...");
                Console.WriteLine("Press a key to stop");
                Console.ReadKey();

                _dependency.Stop();
            }
            finally
            {
                _dependency?.Dispose();
            }
        }

        private static void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        {
            _dependency.Stop();
            Console.WriteLine("Stop ....");
            _dependency.Start();
            Console.WriteLine("Restart Success....");
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

                switch (e.ChangeType)
                {
                    case ChangeType.Insert:
                        Console.WriteLine("{0} : {1} START", MethodBase.GetCurrentMethod().Name, e.ChangeType);
                        break;

                    case ChangeType.Update:
                        Console.WriteLine("{0} : {1} START", MethodBase.GetCurrentMethod().Name, e.ChangeType);
                        ChangeTracker.TraceChangeData<Customer>(changedEntity, e.EntityOldValues);
                        break;

                    case ChangeType.Delete:
                        Console.WriteLine("{0} : {1} START", MethodBase.GetCurrentMethod().Name, e.ChangeType);
                        break;
                }

                Console.WriteLine("\n Press a key to exit");
            }
        }

        private static void TableDependency_OnStatusChanged(object sender, StatusChangedEventArgs e)
        {
            Console.WriteLine(@"Status: " + e.Status);
        }
    }
}