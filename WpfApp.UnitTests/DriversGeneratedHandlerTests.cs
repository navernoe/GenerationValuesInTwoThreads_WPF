using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Moq;
using WpfApp.Domain;
using WpfApp.Logic.GeneratedEntities;
using WpfApp.Logic.GeneratedValueHandlers;

namespace WpfApp.UnitTests;

public class DriversGeneratedHandlerTests
{
    [Fact]
    public void ShouldFindAllPossibleMatches()
    {
        var allGeneratedValues = new ConcurrentBag<IGeneratedProperties>();

        var handler = new DriversGeneratedHandler(
            new Mock<IGeneratedEntityProvider<Driver>>().Object,
            new Mock<IGeneratedEntitiesLinkingProvider<Car, Driver>>().Object,
            allGeneratedValues);
        var handlerWrapper = new HandlerWrapper(handler, new HandleState());

        var generatedDrivers = new ObservableCollection<Driver>();
        generatedDrivers.CollectionChanged += handlerWrapper.GeneratedValues_CollectionChanged;

        var dateNow = DateTimeOffset.Now;

        allGeneratedValues.Add(new Car()
        {
            Name = "matched_car",
            GeneratedDate = dateNow
        });

        generatedDrivers.Add(new Driver()
        {
            Name = "matched_driver",
            GeneratedDate = dateNow
        });

        allGeneratedValues.Add(new Car()
        {
            Name = "not_matched_car",
            GeneratedDate = dateNow - TimeSpan.FromMinutes(1)
        });

        generatedDrivers.Add(new Driver()
        {
            Name = "not_matched_driver",
            GeneratedDate = dateNow - TimeSpan.FromMinutes(2)
        });

        while (handlerWrapper.HandleState.WorkCount < 2)
        {
            Thread.Sleep(100);
        }

        handler.FoundMatches.Count().Should().Be(1);
    }

    private class HandlerWrapper
    {
        public DriversGeneratedHandler OriginalHandler { get; init; }

        public HandleState HandleState { get; init; }

        public HandlerWrapper(DriversGeneratedHandler originalHandler, HandleState handleState)
        {
            OriginalHandler = originalHandler;
            HandleState = handleState;
        }

        public void GeneratedValues_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            var task = Task.Run(async () =>
            {
                await OriginalHandler.GeneratedValues_CollectionChanged(sender, e);
            });

            task.Wait();
            HandleState.WorkCount++;
        }
    }

    private class HandleState
    {
        public int WorkCount { get; set; }
    }
}