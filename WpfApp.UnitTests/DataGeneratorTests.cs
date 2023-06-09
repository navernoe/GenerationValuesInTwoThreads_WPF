using WpfApp.Domain;
using WpfApp.Logic;
using Timer = System.Timers.Timer;

namespace WpfApp.UnitTests;

public class DataGeneratorTests
{
    [Theory]
    [InlineData(2, 10)]
    [InlineData(3, 10)]
    [InlineData(1, 5)]
    [InlineData(2, 5)]
    [InlineData(2, 6)]
    public void ShouldGenerateProperCountEntitiesPerInterval(int intervalInSec, int timeGenerationInSec)
    {
        var dataGenerator = CreateDataGeneratorWithInterval(intervalInSec);
        var isTimerRunning = true;
        var timer = new Timer(TimeSpan.FromSeconds(timeGenerationInSec));
        timer.Elapsed += (source, e) =>
        {
            dataGenerator.StopGenerate();
            timer.Stop();
            timer.Close();
            isTimerRunning = false;
        };

        timer.Start();
        dataGenerator.StartGenerate();

        while (isTimerRunning)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        var count = (double)timeGenerationInSec / intervalInSec;
        var expectedGeneratedCount = (int)Math.Ceiling(count);
        dataGenerator.GeneratedValues.Count.Should().Be(expectedGeneratedCount);
    }

    [Theory]
    [InlineData(3, 10)]
    [InlineData(1, 5)]
    [InlineData(2, 5)]
    [InlineData(2, 6)]
    public void ShouldGenerateInTwoThreads(int intervalInSec, int timeGenerationInSec)
    {
        var dataGenerator1 = CreateDataGeneratorWithInterval(intervalInSec);
        var dataGenerator2 = CreateDataGeneratorWithInterval(intervalInSec);
        var isTimerRunning = true;
        var timer = new Timer(TimeSpan.FromSeconds(timeGenerationInSec));
        timer.Elapsed += (source, e) =>
        {
            dataGenerator1.StopGenerate();
            dataGenerator2.StopGenerate();
            timer.Stop();
            timer.Close();
            isTimerRunning = false;
        };

        timer.Start();
        dataGenerator1.StartGenerate();
        dataGenerator2.StartGenerate();

        while (isTimerRunning)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        var count = (double)timeGenerationInSec / intervalInSec;
        var expectedGeneratedCount = (int)Math.Ceiling(count);
        dataGenerator1.GeneratedValues.Count.Should().Be(expectedGeneratedCount);
        dataGenerator2.GeneratedValues.Count.Should().Be(expectedGeneratedCount);
    }

    private DataGenerator<TestEntity> CreateDataGeneratorWithInterval(int interval)
    {
        return new DataGenerator<TestEntity>(
            new JobManager<TestEntity>(),
            new TestGenerationSettings()
            {
                Interval = TimeSpan.FromSeconds(interval)
            });
    }

    private class TestEntity : IGeneratedProperties
    {
        public string Name { get; set; }
        public DateTimeOffset GeneratedDate { get; set; }
    }

    private class TestGenerationSettings : IGenerationSettings<TestEntity>
    {
        public string[] AvailableNameValues { get; } =
        {
            "one", "two", "three", "four", "five", "six", "seven"
        };

        public TimeSpan Interval { get; init; }
    }
}