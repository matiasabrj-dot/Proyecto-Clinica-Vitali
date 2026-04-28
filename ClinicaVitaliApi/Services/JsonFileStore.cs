using System.Text.Json;
using ClinicaVitaliApi.Models;

namespace ClinicaVitaliApi.Services;

public class JsonFileStore<T> where T : class, IEntity
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public JsonFileStore(string filePath)
    {
        _filePath = filePath;
    }

    public async Task<List<T>> ReadAllAsync()
    {
        await _lock.WaitAsync();
        try
        {
            await EnsureFileAsync();
            var json = await File.ReadAllTextAsync(_filePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<T>();
            }

            return JsonSerializer.Deserialize<List<T>>(json, _serializerOptions) ?? new List<T>();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task WriteAllAsync(List<T> data)
    {
        await _lock.WaitAsync();
        try
        {
            await EnsureFileAsync();
            var json = JsonSerializer.Serialize(data, _serializerOptions);
            await File.WriteAllTextAsync(_filePath, json);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task EnsureFileAsync()
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(_filePath))
        {
            await File.WriteAllTextAsync(_filePath, "[]");
        }
    }
}
