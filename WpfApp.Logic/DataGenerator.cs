using System.Collections.ObjectModel;
using WpfApp.Domain;

namespace WpfApp.Logic;

/// <summary>
/// Генератор данных.
/// </summary>
/// <typeparam name="T">Сущность, для которой нужно сгенерировать данные</typeparam>
public class DataGenerator<T> where T: IGeneratedProperties, new()
{
    private readonly ObservableCollection<IGeneratedProperties> _generatedValues = new ();
    private readonly JobManager<T> _jobManager;
    private readonly IGenerationSettings<T> _generationSettings;
    private readonly Random _randomizer;

    public DataGenerator(JobManager<T> jobManager, IGenerationSettings<T> generationSettings)
    {
        _jobManager = jobManager;
        _generationSettings = generationSettings;
        _randomizer = new Random();
    }

    public ObservableCollection<IGeneratedProperties> GeneratedValues => _generatedValues;

    public int? ThreadId => _jobManager.ThreadId;

    public void StartGenerate()
    {
        _jobManager.StartRepeatJob(() =>
            {
                _generatedValues.Add(GenerateNextValue());
            },
            _generationSettings.Interval);
    }

    public void StopGenerate()
    {
        _jobManager.StopRepeatJob();
    }

    private IGeneratedProperties GenerateNextValue()
    {
        return new T()
        {
            Name = _generationSettings.AvailableNameValues[_randomizer.Next(_generationSettings.AvailableNameValues.Length)],
            GeneratedDate = DateTimeOffset.UtcNow,
        };
    }
}