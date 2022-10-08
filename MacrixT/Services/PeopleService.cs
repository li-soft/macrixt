using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MacrixT.Models;
using MacrixT.StoredModel;

namespace MacrixT.Services;

public interface IPeopleService
{
    Task<IEnumerable<Person>> GetStoredPeople();
    Task DumpPeople(IEnumerable<Person> people);
}

public class PeopleService : IPeopleService
{
    private readonly string _dirPath;
    private readonly string _solutionDataPath;

    public PeopleService()
    {
        // should come from cofig ideally
        _dirPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\macrixt";
        _solutionDataPath = Path.Combine(_dirPath, "dalalatata.max");
    }
    
    public async Task<IEnumerable<Person>> GetStoredPeople()
    {
        EnsurePath();
        
        if (!HasDataFile())
        {
            return Enumerable.Empty<Person>();
        }

        await using var file = File.OpenRead(_solutionDataPath);
        var reader = new XmlSerializer(typeof(MacrixData));  
        var data = reader.Deserialize(file) as MacrixData;
        await Task.Delay(500);
        file.Close();

        return data?.People?.Length == 0 ? Enumerable.Empty<Person>() : data!.People!.AsEnumerable();
    }

    public async Task DumpPeople(IEnumerable<Person> people)
    {
        EnsurePath();

        if (HasDataFile())
        {
            File.Delete(_solutionDataPath);
        }
        await using var writer = File.Create(_solutionDataPath);

        var data = new MacrixData { People = people.ToArray() };
        
        var serializer = new XmlSerializer(typeof(MacrixData));
        serializer.Serialize(writer, data);
        
        writer.Close();
    }

    private void EnsurePath()
    {
        if (!Directory.Exists(_dirPath))
        {
            Directory.CreateDirectory(_dirPath);
        }
    }

    private bool HasDataFile() => File.Exists(_solutionDataPath);
}