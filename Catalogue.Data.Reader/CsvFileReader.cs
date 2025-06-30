using Catalogue.Shared.Interfaces;
using Catalogue.Shared.Models;

namespace Catalogue.Data.Reader;

internal sealed class CsvFileReader : IDataReader<Category>
{
    private readonly string _filePath;

    public CsvFileReader(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        if (!File.Exists(filePath)) throw new FileNotFoundException($"The file '{filePath}' does not exist.", filePath);
        _filePath = filePath;
    }

    public IEnumerable<Category> GetData()
    {
        var categoryMap = new Dictionary<string, Category>();

        using (StreamReader reader = new StreamReader(_filePath))
        {
            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split('\t');

                string categoryName = parts[0];
                bool categoryIsActive = parts[1] == "1";

                string productName = parts[2];
                string productCode = parts[3];
                decimal productPrice = decimal.Parse(parts[4]);
                bool productIsActive = parts[5] == "1";

                Product product = new Product
                {
                    Name = productName,
                    Code = productCode,
                    Price = productPrice,
                    IsActive = productIsActive
                };

                if (!categoryMap.TryGetValue(categoryName, out var category))
                {
                    category = new Category
                    {
                        Name = categoryName,
                        IsActive = categoryIsActive
                    };
                    categoryMap[categoryName] = category;
                }
                    
                category.AddProduct(product);
            }

            return categoryMap.Values;
        }

    }
}