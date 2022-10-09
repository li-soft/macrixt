# Macrix tech task

## MacrixT = Macrix Table

### xml data file location, please see 'PeopleService' constructor
```csharp
        // should come from cofig ideally
        _dirPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\macrixt";
        _solutionDataPath = Path.Combine(_dirPath, "dalalatata.max");
```