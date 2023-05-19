using System.Collections.ObjectModel;

namespace WpfApp.Logic;

public class DataGenerator<T> where T: IGenerationSettings
{
    private readonly ObservableCollection<GeneratedValue> _generatedValues = new ();
    private readonly JobManager<T> _jobManager;
    private readonly T _entity;
    private readonly Random _randomizer;
    private readonly string _typeName;

    
    public DataGenerator(JobManager<T> jobManager, T entity)
    {
        _jobManager = jobManager;
        _entity = entity;
        _typeName = typeof(T).Name;
        _randomizer = new Random();
    }

    public string TypeName => _typeName;

    public ObservableCollection<GeneratedValue> GeneratedValues => _generatedValues;

    public void StartGenerate()
    {
        _jobManager.StartRepeatJob(() =>
            {
                _generatedValues.Add(GenerateNextValue());
            },
            _entity.Interval);
    }

    public void StopGenerate()
    {
        _jobManager.StopRepeatJob();
    }
    
    private GeneratedValue GenerateNextValue()
    {
        return new GeneratedValue()
        {
            Name = _entity.AvailableValues[_randomizer.Next(_entity.AvailableValues.Length)],
            GeneratedDate = DateTimeOffset.UtcNow,
            Type = _typeName
        };
    }
}