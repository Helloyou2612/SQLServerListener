using TableDependency.SqlClient.Base;

namespace SQLServerListener
{
    public class ModelToTableMapper
    {
        public static ModelToTableMapper<Customer> CustomerMapper()
        {
            var mapper = new ModelToTableMapper<Customer>();
            mapper.AddMapping(c => c.Id, "Id");
            mapper.AddMapping(c => c.FirstName, "FirstName");

            return mapper;
        }
    }
}