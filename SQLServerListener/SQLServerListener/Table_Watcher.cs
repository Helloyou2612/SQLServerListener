using TableDependency;
using TableDependency.Enums;
using TableDependency.SqlClient;

namespace SQLServerListener
{
    public class Table_Watcher
    {
        public string _connectionString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=test;Data Source=.;";

        // System.Configuration.ConfigurationManager.ConnectionStrings["constring"].ConnectionString;
        private SqlTableDependency<Stock> _dependency;

        public void WatchTable()
        {
            var mapper = new ModelToTableMapper<Stock>();
            mapper.AddMapping(model => model.Symbol, "Code");
            _dependency = new SqlTableDependency<Stock>(_connectionString, "Code", mapper);
            _dependency.OnChanged += _dependency_OnChanged;
            _dependency.OnError += _dependency_OnError;
        }

        public void StartTableWatcher()
        {
            _dependency.Start();
        }

        public void StopTableWatcher()
        {
            _dependency.Stop();
        }

        private void _dependency_OnError(object sender, TableDependency.EventArgs.ErrorEventArgs e)
        {
            throw e.Error;
        }

        private void _dependency_OnChanged(object sender, TableDependency.EventArgs.RecordChangedEventArgs<Stock> e)
        {
            if (e.ChangeType != ChangeType.None)
            {
                switch (e.ChangeType)
                {
                    case ChangeType.Delete:

                        break;

                    case ChangeType.Insert:
                        if (e.Entity.AttendaceState.ToLower() == "p")
                        {
                            // Send Present Message Here
                            MessageBox.Show("Student is present");
                        }
                        else if (e.Entity.AttendaceState.ToLower() == "l")
                        {
                            // send Leave Message here
                            MessageBox.Show("Student is Leave");
                        }
                        else if (e.Entity.AttendaceState.ToLower() == "a")
                        {
                            // send absent Message here
                            MessageBox.Show("Student is absent");
                        }
                        else
                        {
                            // send Other Message here
                        }
                        MessageBox.Show(e.Entity.StudentID + " Inserted");
                        break;

                    case ChangeType.Update:

                        break;
                }
            }
        }
    }
}