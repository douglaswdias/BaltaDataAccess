using System.Data;
using BaltaDataAccess.Models;
using Dapper;
using Microsoft.Data.SqlClient;

const string connectionString = "server=localhost\\sqlexpress;database=balta;trusted_connection=True;TrustServerCertificate=True";

// using (var connection = new SqlConnection(connectionString))
// {
//   connection.Open();
//   using(var command = new SqlCommand())
//   {
//     command.Connection = connection;
//     command.CommandType = System.Data.CommandType.Text;
//     command.CommandText = "SELECT [Id], [Title] FROM [Category]";

//     var reader = command.ExecuteReader();
//     while(reader.Read())
//     {
//       Console.WriteLine($"{reader.GetGuid(0)} - {reader.GetString(1)}");
//     }
//   }
// }


using (var connection = new SqlConnection(connectionString))
{
  // CreateCategory(connection);  
  // UpdateCategory(connection);
  // DeleteCategory(connection);
  // ListCategories(connection);
  // ExecuteProcedure(connection);
  ReadProcedure(connection);
}

static void ListCategories(SqlConnection connection)
  {
    var categories = connection.Query<Category>("SELECT [Id], [Title] FROM [Category]");

    foreach (var item in categories)
    {
      Console.WriteLine($"{item.Id} - {item.Title}");
    }
  }

static void CreateCategory(SqlConnection connection)
  {
    var category = new Category();
    category.Id = Guid.NewGuid();
    category.Title = "Amazon AWS";
    category.Url = "amazon";
    category.Summary = "AWS Cloud";
    category.Order = 8;
    category.Description = "Categoria destinada a serviços do AWS";
    category.Featured = false;

    var insertSql = @"INSERT INTO 
                        [Category] 
                      VALUES 
                        (@Id, @Title, @Url, @Summary, @Order, @Description, @Featured)";

    var rows = connection.Execute(insertSql, new 
    {
      category.Id,
      category.Title,
      category.Url,
      category.Summary,
      category.Order,
      category.Description,
      category.Featured
    });
  }

static void UpdateCategory(SqlConnection connection)
{
  var updateCategory = "UPDATE [Category] SET [Title] = @title WHERE [Id] = @id";
  var rows = connection.Execute(updateCategory, new 
    {
      id = new Guid("99CEB1F4-8A24-47F3-9E75-C15212FE4E08"),
      title = "AWS Amazon"
    }
  );
}

static void DeleteCategory(SqlConnection connection)
{
  var deleteCategory = "DELETE FROM [Category] WHERE [Id] = @id";
  var rows = connection.Execute(deleteCategory, new 
    {
      id = new Guid("99CEB1F4-8A24-47F3-9E75-C15212FE4E08")
    }
  );
}

static void ExecuteProcedure(SqlConnection connection)
{
  var procedure = "[spDeleteStudent]";
  var parameters = new { StudentId = "BDC6E888-C50F-406B-93E1-E156DCF6FA50" };
  var affectedRows = connection.Execute(procedure, parameters, commandType: CommandType.StoredProcedure);
}

static void ReadProcedure(SqlConnection connection)
{
  var procedure = "[spGetCoursesByCategory]";
  var parameters = new { CategoryId = "09CE0B7B-CFCA-497B-92C0-3290AD9D5142" };
  var courses = connection.Query(procedure, parameters, commandType: CommandType.StoredProcedure);

  foreach(var item in courses)
  {
    Console.WriteLine(item.Id);
  }
}
