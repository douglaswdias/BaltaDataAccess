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
  // ReadProcedure(connection);
  // ExecuteScalar(connection);
  // ReadView(connection);
  // OneToOne(connection);
  // OneToMany(connection);
  // QueryMultiple(connection);
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

static void ExecuteScalar(SqlConnection connection)
  {
    var category = new Category();
    category.Title = "Amazon AWS";
    category.Url = "Scalar";
    category.Summary = "Scalar";
    category.Order = 9;
    category.Description = "Scalar";
    category.Featured = false;

    var insertSql = @"INSERT INTO 
                        [Category] 
                        OUTPUT inserted.[Id]
                      VALUES 
                        (NEWID(), @Title, @Url, @Summary, @Order, @Description, @Featured)";

    var id = connection.ExecuteScalar<Guid>(insertSql, new 
    {
      category.Title,
      category.Url,
      category.Summary,
      category.Order,
      category.Description,
      category.Featured
    });
    Console.WriteLine($"Id da Categoria Criada: {id}");
  }

static void ReadView(SqlConnection connection)
{
  var sql = "SELECT * FROM [vwCourses]";
  var courses = connection.Query(sql);

    foreach (var item in courses)
    {
      Console.WriteLine($"{item.Id} - {item.Title}");
    }
}

static void OneToOne(SqlConnection connection)
{
  var sql = @"SELECT * FROM [CareerItem]
            INNER JOIN [Course] ON [CareerItem].[CourseId] = [Course].[Id]";
  
  var items = connection.Query<CareerItem, Course, CareerItem>(
    sql,
    (careerItem, course) =>
    {
      careerItem.Course = course;
      return careerItem;
    },
    splitOn: "Id");

  foreach (var item in items)
  {
    Console.WriteLine($"{item.Title} - Curso: {item.Course.Title}");
  }
}

static void OneToMany(SqlConnection connection)
{
  var sql = @"SELECT 
              [Career].[Id], [Career].[Title],[CareerItem].[CareerId], [CareerItem].[Title]
            FROM 
              [Career]
            INNER JOIN 
              [CareerItem] ON [CareerItem].[CareerId] = [Career].[Id]
            ORDER BY 
              [Career].[Title]";
  
  var careerList = new List<Career>();
  var sqlCareers = connection.Query<Career, CareerItem, Career>(
    sql,
    (career, careerItem) =>
    {
      var careerSql = careerList.Where(x => x.Id == career.Id).FirstOrDefault();

      if(careerSql == null)
      {
        careerSql = career;
        careerSql.CareerItems.Add(careerItem);
        careerList.Add(careerSql);
      }
      else
      {
        careerSql.CareerItems.Add(careerItem);
      }

      return career;
    },
    splitOn: "CareerId");

  foreach (var career in careerList)
  {
    Console.WriteLine($"{career.Title}");
    foreach (var item in career.CareerItems)
    {
      Console.WriteLine($"- {item.Title}");
    }
  }
}

static void QueryMultiple(SqlConnection connection)
{
  var query = @"SELECT * FROM [Category];
              SELECT * FROM [Course]";

  using(var multi = connection.QueryMultiple(query))
  {
    var categories = multi.Read<Category>();
    var courses = multi.Read<Course>();

    foreach(var item in categories)
    {
      Console.WriteLine(item.Title);
    }

    foreach(var item in courses)
    {
      Console.WriteLine(item.Title);
    }
  }
}
