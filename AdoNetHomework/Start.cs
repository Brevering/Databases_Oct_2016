namespace AdoNetHomework
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.OleDb;
    using System.Data.SqlClient;
    using System.IO;
    using System.Text;

    using MySql.Data.MySqlClient;

    public class Start
    {
        private static readonly ConnectionStringSettings SqlServerSettings =
            ConfigurationManager.ConnectionStrings["SQL Server Connection"];

        public static void Main()
        {
            // Problem 1: Write a program that retrieves from the Northwind sample database in MS SQL Server 
            // the number of rows in the Categories table.

            int numberOfCategories = GetNumberOfCategoriesFromNorthwind(SqlServerSettings);
            Console.WriteLine("---------- NUMBER OF CATEGORIES -----------------------");
            Console.WriteLine("Number of categories: {0}", numberOfCategories);

            // Problem 2: Write a program that retrieves the name and description of all categories in the Northwind DB.
            var categories = GetCategoriesNamesAndDescription(SqlServerSettings);
            Console.WriteLine("---------- CATEGORIES - DESCRIPTION -----------------------");
            foreach (var cat in categories)
            {
                Console.WriteLine("Category: {0}\nDescription: {1}", cat.Key, cat.Value);
                Console.WriteLine();
            }

            // Problem 3: Write a program that retrieves from the Northwind database all product categories 
            // and the names of the products in each category.

            var productsAndCategories = GetProductsAndCategories(SqlServerSettings);
            Console.WriteLine("---------- CATEGORIES - PRODUCTS -----------------------");
            foreach (var cat in productsAndCategories)
            {
                Console.WriteLine("Category: {0}\nProducts: {1}", cat.Key, string.Join(", ", cat.Value));
                Console.WriteLine();
            }

            // Problem 4: Write a method that adds a new product in the products table in the Northwind database.
            // Use a parameterized SQL command.

            var rowsAffected = InsertProduct("New test product", 1, 1, "5 boxes x 10 bottles", 10.0M, 5, 50, 25, false, SqlServerSettings);
            Console.WriteLine(rowsAffected);

            // Problem 5: Write a program that retrieves the images for all categories in 
            // the Northwind database and stores them as JPG files in the file system.

            var images = ExtractImagesFromDb(SqlServerSettings);
            int counter = 1;
            foreach (var image in images)
            {
                WriteBinaryFile("../../image" + counter + ".jpg", 78, image);
                counter++;
            }

            // Problem 6: Create an Excel file with 2 columns: name and score...
            // Write a program that reads your MS Excel file through the OLE DB data provider and displays the name and score row by row.

            var excelData = ReadExcelFile();
            var reader = excelData.CreateDataReader();
            while (reader.Read())
            {
                var name = reader["Name"];
                var score = reader["Score"];
                Console.WriteLine("Name - {0}; Score - {1}", name, score);
            }

            // Problem 7: 

            using (OleDbConnection connection = new OleDbConnection(GetExcelConnectionString()))
            {
                connection.Open();
                for (int i = 0; i < 10; i++)
                {
                    var command = new OleDbCommand("INSERT INTO [Sheet1$] Values(@name, @score)", connection);

                    command.Parameters.AddWithValue("@name", "User" + i);
                    command.Parameters.AddWithValue("@score", 100 + (10 * i) + (i % 10));
                    command.ExecuteNonQuery();
                }
            }

            // Problem 8: Write a program that reads a string from the console and finds all products that contain this string.

            SqlConnection dbCon = new SqlConnection(SqlServerSettings.ConnectionString);

            dbCon.Open();
            using (dbCon)
            {
                int newProjectId = InsertProduct(
                                    dbCon,
                                    "Starbucks Coffee",
                                    20,
                                    1,
                                    "150 ml",
                                    3.3m,
                                    500,
                                    400,
                                    20,
                                    false);
                Console.WriteLine("Inserted new product with ProductID = {0}", newProjectId);
            }

            // Problem 9: Download and install MySQL database, MySQL Connector/Net (.NET Data Provider for MySQL) + MySQL Workbench GUI administration tool.
            // Create a MySQL database to store Books(title, author, publish date and ISBN).
            // Write methods for listing all books, finding a book by name and adding a book.

            // Create the database by running the sql script in WorkBench first
            Console.Write("Enter MySQL password: ");
            string pass = Console.ReadLine();

            string connectionStr = "Server=localhost;Database=library;Uid=root;Pwd=" + pass + ";";
            MySqlConnection mySqlConnection = new MySqlConnection(connectionStr);

            ListAllBooks(mySqlConnection);
            Console.WriteLine();
            FindBookById(2, mySqlConnection);
            Console.WriteLine();
            AddBook("Nemo", "Mark Twain", "222-222-222-22", mySqlConnection);
            AddBook("Harry Potter 2", "J. K. Rolling", "111-111-111-11", mySqlConnection);

            // Problem 10: Re-implement the previous task with SQLite embedded DB


        }

        private static SortedDictionary<string, IList<string>> GetProductsAndCategories(ConnectionStringSettings settings)
        {
            var con = new SqlConnection(settings.ConnectionString);

            con.Open();

            var result = new SortedDictionary<string, IList<string>>();

            using (con)
            {
                var command = new SqlCommand(
                "SELECT c.CategoryName, p.ProductName FROM Categories c JOIN Products p ON c.CategoryID = p.CategoryID",
                con);

                var resultReader = command.ExecuteReader();

                while (resultReader.Read())
                {
                    if (result.ContainsKey(resultReader["CategoryName"].ToString()))
                    {
                        result[resultReader["CategoryName"].ToString()].Add(resultReader["ProductName"].ToString());
                    }
                    else
                    {
                        result.Add(resultReader["CategoryName"].ToString(), new List<string> { resultReader["ProductName"].ToString() });
                    }
                }
            }

            return result;
        }

        private static int GetNumberOfCategoriesFromNorthwind(ConnectionStringSettings settings)
        {
            var con = new SqlConnection(settings.ConnectionString);

            con.Open();

            using (con)
            {
                var command = new SqlCommand(
                "select count(*) from Categories",
                con);

                int result = (int)command.ExecuteScalar();
                return result;
            }
        }

        private static Dictionary<string, string> GetCategoriesNamesAndDescription(ConnectionStringSettings settings)
        {
            var con = new SqlConnection(settings.ConnectionString);

            con.Open();

            var result = new Dictionary<string, string>();

            using (con)
            {
                var command = new SqlCommand(
                "select CategoryName, Description from Categories",
                con);

                var resultReader = command.ExecuteReader();

                while (resultReader.Read())
                {
                    result.Add(resultReader["CategoryName"].ToString(), resultReader["Description"].ToString());
                }

                return result;
            }
        }

        private static string GetExcelConnectionString()
        {
            Dictionary<string, string> props = new Dictionary<string, string>();

            // XLSX - Excel 2007, 2010, 2012, 2013
            props["Provider"] = "Microsoft.ACE.OLEDB.12.0;";
            props["Extended Properties"] = "Excel 12.0 XML";
            props["Data Source"] = "../../scores.xls";

            // XLS - Excel 2003 and Older
            //props["Provider"] = "Microsoft.Jet.OLEDB.4.0";
            //props["Extended Properties"] = "Excel 8.0";
            //props["Data Source"] = "C:\\MyExcel.xls";

            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> prop in props)
            {
                sb.Append(prop.Key);
                sb.Append('=');
                sb.Append(prop.Value);
                sb.Append(';');
            }

            return sb.ToString();
        }

        private static DataSet ReadExcelFile()
        {
            DataSet ds = new DataSet();

            string connectionString = GetExcelConnectionString();

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;

                // Get all Sheets in Excel File
                DataTable dtSheet = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                // Loop through all Sheets to get data
                foreach (DataRow dr in dtSheet.Rows)
                {
                    string sheetName = dr["TABLE_NAME"].ToString();

                    if (!sheetName.EndsWith("$"))
                        continue;

                    // Get all rows from the Sheet
                    cmd.CommandText = "SELECT * FROM [" + sheetName + "]";

                    DataTable dt = new DataTable();
                    dt.TableName = sheetName;

                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    da.Fill(dt);

                    ds.Tables.Add(dt);
                }

                cmd = null;
                conn.Close();
            }

            return ds;
        }

        private static int InsertProduct(
            string name, 
            int supplier,
            int category, 
            string quantityPerProduct,
            decimal unitPrice,
            short unitsInStock,
            short unitsInOrder,
            short reorderLevel,
            bool discontinued,
            ConnectionStringSettings settings)
        {
            var con = new SqlConnection(settings.ConnectionString);

            con.Open();

            int result;

            using (con)
            {
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Products " +
                    "VALUES(@name, @supplier, @cat, @quantity, @price, @unitsInStock, @unitsInOrder, @reorderLevel, @discontinued)", con);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@supplier", supplier);
                cmd.Parameters.AddWithValue("@cat", category);
                cmd.Parameters.AddWithValue("@quantity", quantityPerProduct);
                cmd.Parameters.AddWithValue("@price", unitPrice);
                cmd.Parameters.AddWithValue("@unitsInStock", unitsInStock);
                cmd.Parameters.AddWithValue("@unitsInOrder", unitsInOrder);
                cmd.Parameters.AddWithValue("@reorderLevel", reorderLevel);
                cmd.Parameters.AddWithValue("@discontinued", discontinued);
                result = cmd.ExecuteNonQuery();
            }
            return result;
        }

        private static IList<byte[]> ExtractImagesFromDb(ConnectionStringSettings settings)
        {
            SqlConnection dbConn = new SqlConnection(settings.ConnectionString);
            List<byte[]> result = new List<byte[]>();
            dbConn.Open();
            using (dbConn)
            {
                SqlCommand cmd = new SqlCommand("SELECT Picture FROM Categories", dbConn);

                SqlDataReader reader = cmd.ExecuteReader();
                
                using (reader)
                {
                    while (reader.Read())
                    {
                        result.Add((byte[])reader["Picture"]);
                    }
                }
            }

            return result;
        }

        private static void WriteBinaryFile(string fileName, int offset, byte[] fileContents)
        {
            FileStream stream = File.OpenWrite(fileName);
            using (stream)
            {
                stream.Write(fileContents, offset, fileContents.Length - offset);
            }
        }

        private static int InsertProduct(
                            SqlConnection dbCon,
                            string productName,
                            int supplierId,
                            int categoryId,
                            string quantityPerUnit,
                            decimal unitPrice,
                            int unitsInStock,
                            int unitsOnOrder,
                            int reorderLevel,
                            bool discontinued)
        {
            SqlCommand cmdInsertProject = new SqlCommand(
                "INSERT INTO Products([ProductName], [SupplierID], [CategoryID], [QuantityPerUnit]" +
                ",[UnitPrice], [UnitsInStock], [UnitsOnOrder], [ReorderLevel], [Discontinued]) " +
                "VALUES (@productName, @supplierId, @categoryId, @quantityPerUnit, " +
                "@unitPrice, @unitsInStock, @unitsOnOrder, @reorderLevel, @discontinued)", dbCon);
            cmdInsertProject.Parameters.AddWithValue("@productName", productName);
            cmdInsertProject.Parameters.AddWithValue("@supplierId", supplierId);
            cmdInsertProject.Parameters.AddWithValue("@categoryId", categoryId);
            cmdInsertProject.Parameters.AddWithValue("@quantityPerUnit", quantityPerUnit);
            cmdInsertProject.Parameters.AddWithValue("@unitPrice", unitPrice);
            cmdInsertProject.Parameters.AddWithValue("@unitsInStock", unitsInStock);
            cmdInsertProject.Parameters.AddWithValue("@unitsOnOrder", unitsOnOrder);
            cmdInsertProject.Parameters.AddWithValue("@reorderLevel", reorderLevel);
            cmdInsertProject.Parameters.AddWithValue("@discontinued", discontinued ? 1 : 0);
            cmdInsertProject.ExecuteNonQuery();

            SqlCommand cmdSelectIdentity = new SqlCommand("SELECT @@Identity", dbCon);
            int insertedRecordId = (int)(decimal)cmdSelectIdentity.ExecuteScalar();
            return insertedRecordId;
        }

        private static void ListAllBooks(MySqlConnection connection)
        {
            connection.Open();
            using (connection)
            {
                MySqlCommand command = new MySqlCommand("select b.Name as Book, a.Name as Author from books as b " +
                                                        "join authors as a " +
                                                        "on a.Id = b.AuthorId", connection);
                MySqlDataReader reader = command.ExecuteReader();

                Console.WriteLine("Books currently in library: ");
                Console.WriteLine();

                using (reader)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(string.Format("- {0} -\t by {1}", reader["Book"], reader["Author"]));
                    }
                }
            }
        }

        private static void FindBookById(int id, MySqlConnection connection)
        {
            connection.Open();
            using (connection)
            {
                MySqlCommand command = new MySqlCommand("select b.Name as Book, a.Name as Author from books as b " +
                                                        "join authors as a " +
                                                        "on a.Id = b.AuthorId " +
                                                        "where b.Id=@id", connection);
                command.Parameters.AddWithValue("@id", id);
                MySqlDataReader reader = command.ExecuteReader();
                using (reader)
                {
                    if (reader.Read())
                    {
                        Console.WriteLine("The book with id of " + id + " is: ");
                        Console.WriteLine(string.Format("- {0} -\t by {1}", reader["Book"], reader["Author"]));
                    }
                    else
                    {
                        Console.WriteLine("There is no book with id of " + id + " in the library.");
                    }
                }
            }
        }

        private static void AddBook(string name, string author, string isbn, MySqlConnection connection)
        {
            connection.Open();
            using (connection)
            {
                MySqlCommand checkAuthor = new MySqlCommand("select a.Id as Id from authors as a " +
                                                            "join books as b " +
                                                            "on a.Id = b.AuthorId " +
                                                            "where a.Name = @author", connection);
                checkAuthor.Parameters.AddWithValue("@author", author);
                var reader = checkAuthor.ExecuteReader();

                int authorId;
                if (reader.Read())
                {
                    authorId = (int)reader["Id"];
                    reader.Close();
                }
                else
                {
                    reader.Close();
                    MySqlCommand createAuthor = new MySqlCommand("insert into authors(Name) " +
                                                                 "values (@author)", connection);
                    createAuthor.Parameters.AddWithValue("@author", author);
                    createAuthor.ExecuteNonQuery();

                    Console.WriteLine("Author " + author + " added successfully!");

                    var cmdSelectIdentity = new MySqlCommand("select @@Identity", connection);
                    authorId = int.Parse(cmdSelectIdentity.ExecuteScalar().ToString());
                }

                MySqlCommand command = new MySqlCommand("insert into books(Name, AuthorId, ISBN) " +
                                                        "values (@name, @authorId, @isbn)", connection);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@authorId", authorId);
                command.Parameters.AddWithValue("@isbn", isbn);
                command.ExecuteNonQuery();

                Console.WriteLine("Book " + name + " added successfully!");
            }
        }
    }

}
