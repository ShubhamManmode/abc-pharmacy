using System.Text.Json;

namespace ABCPharmacy.Api.Services;

public class JsonFileService
{
    private readonly IWebHostEnvironment _environment;
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public JsonFileService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<List<T>> ReadAsync<T>(string fileName)
    {
        var path = GetPath(fileName);

        if (!File.Exists(path))
        {
            await WriteAsync(fileName, new List<T>());
            return [];
        }

        await using var stream = File.OpenRead(path);

        try
        {
            return await JsonSerializer.DeserializeAsync<List<T>>(stream, _options)
                   ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }

    public async Task WriteAsync<T>(string fileName, List<T> data)
    {
        var path = GetPath(fileName);
        var directory = Path.GetDirectoryName(path)!;

        Directory.CreateDirectory(directory);

        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, data, _options);
    }

    private string GetPath(string fileName)
    {
        return Path.Combine(_environment.ContentRootPath, "Data", fileName);
    }
}
