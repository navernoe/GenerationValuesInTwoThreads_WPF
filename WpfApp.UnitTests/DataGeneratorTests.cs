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
    [InlineData(2, 4)]
    [InlineData(2, 6)]
    public void ShouldGenerateProperCountEntitiesPerInterval(int intervalInSec, int timeGenerationInSec)
    {
        var jobManager = new JobManager<TestEntity>();
        var dataGenerator = new DataGenerator<TestEntity>(jobManager, new TestGenerationSettings()
        {
            Interval = TimeSpan.FromSeconds(intervalInSec)
        });
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