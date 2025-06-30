using Catalogue.Shared.Interfaces;
using Catalogue.Shared.Models;


namespace Catalogue.Data.Validator;

public static class CsvFileValidatorFactory
{
    public static IDataReader<Category> Create(string filePath)
        => new CsvFileValidator(filePath);
}
