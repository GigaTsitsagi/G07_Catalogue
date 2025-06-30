using Catalogue.Data.Reader;
using Catalogue.Shared.Interfaces;
using Catalogue.Shared.Models;


namespace Catalogue.Data.Validator;

internal sealed class CsvFileValidator : IDataReader<Category>
{
    private readonly string _filePath; 
    private readonly string _outputPath = "C:\\Users\\giga\\Desktop\\FilteredData.csv";
    private readonly HashSet<string> _seenCodes = new();

    public CsvFileValidator(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        if (!File.Exists(filePath)) throw new FileNotFoundException($"The file '{filePath}' does not exist.", filePath);
        _filePath = filePath;
    }

    public void FilterInvalidData()
    {
        using var reader = new StreamReader(_filePath);
        using var writer = new StreamWriter(_outputPath);

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            if (IsValidLine(line))
            {
                writer.WriteLine(line);
            }
        }
    }

    private bool IsValidLine(string line)
    {
        var parts = line.Split('\t');

        if (parts.Length < 6)
            return false;

        if (string.IsNullOrWhiteSpace(parts[0]) || parts[0].Any(char.IsDigit))
            return false;

        if (parts[1] != "0" && parts[1] != "1")
            return false;

        string code = parts[3].Trim();
        if (string.IsNullOrEmpty(code) || !_seenCodes.Add(code))
            return false;

        if (!decimal.TryParse(parts[4], out _))
            return false;

        if (parts[5] != "0" && parts[5] != "1")
            return false;

        return true;
    }

    public IEnumerable<Category> GetData()
    {
      throw new NotImplementedException();
    }
}



