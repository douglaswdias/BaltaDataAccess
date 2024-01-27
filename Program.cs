using Microsoft.Data.SqlClient;
const string connectionString = "server=localhost\\sqlexpress;database=balta;trusted_connection=True;TrustServerCertificate=True";

using (var connection = new SqlConnection(connectionString))
{
  connection.Open();
  using(var command = new SqlCommand())
  {
    command.Connection = connection;
    command.CommandType = System.Data.CommandType.Text;
    command.CommandText = "SELECT [Id], [Title] FROM [Category]";

    var reader = command.ExecuteReader();
    while(reader.Read())
    {
      Console.WriteLine($"{reader.GetGuid(0)} - {reader.GetString(1)}");
    }
  }
}