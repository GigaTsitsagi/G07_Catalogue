using Catalogue.Shared.Interfaces;
using Catalogue.Shared.Models;
using Microsoft.Data.SqlClient;

namespace Catalogue.Database.Importer;

internal class DatabaseWriter : IDataWriter<Category>
{
    private readonly string _connectionString;

    public DatabaseWriter(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void WriteData(IEnumerable<Category> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        using (SqlConnection connection = new(_connectionString))
        {
            connection.Open();

            foreach (Category category in data)
            {
                using (SqlCommand commandInsertCategory = connection.CreateCommand())
                {
                    commandInsertCategory.CommandText =
                        @"INSERT INTO Categories (CategoryName, IsActive) VALUES (@name, @isActive)";
                    commandInsertCategory.Parameters.AddWithValue("@name", category.Name);
                    commandInsertCategory.Parameters.AddWithValue("@isActive", category.IsActive);

                    commandInsertCategory.ExecuteNonQuery();
                }

                int categoryId;

                
                using (SqlCommand commandGetCategoryId = connection.CreateCommand())
                {
                    commandGetCategoryId.CommandText =
                        "SELECT CategoryID FROM Categories WHERE CategoryName = @name";
                    commandGetCategoryId.Parameters.AddWithValue("@name", category.Name);

                    object? result = commandGetCategoryId.ExecuteScalar();
                    if (result == null)
                    {
                        throw new Exception($"Failed to retrieve CategoryID for category '{category.Name}'");
                    }

                    categoryId = Convert.ToInt32(result);
                }

                foreach (Product product in category.Products)
                {
                    using (SqlCommand commandInsertProduct = connection.CreateCommand())
                    {
                        commandInsertProduct.CommandText =
                            "INSERT INTO Products (ProductName, ProductCode, Price, IsActive, CategoryID) " +
                            "VALUES (@name, @code, @price, @isActive, @categoryId)";
                        commandInsertProduct.Parameters.AddWithValue("@name", product.Name);
                        commandInsertProduct.Parameters.AddWithValue("@code", product.Code);
                        commandInsertProduct.Parameters.AddWithValue("@price", product.Price);
                        commandInsertProduct.Parameters.AddWithValue("@isActive", product.IsActive);
                        commandInsertProduct.Parameters.AddWithValue("@categoryId", categoryId);

                        commandInsertProduct.ExecuteNonQuery();
                    }
                }
            }
        }
    }

}