using System.Collections.Concurrent;
using System.Collections.ObjectModel;
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

        handlerWrapper.WaitWhenHandlerEndWork(2); // Ждем пока он обработает добавленные сущности водителей.

        handler.FoundMatches.Count().Should().Be(1);
    }

    [Fact]
    public void ShouldSaveToDb()
    {
        var allGeneratedValues = new ConcurrentBag<IGeneratedProperties>();
        var driversDb = new ConcurrentBag<Driver>();

        var driversProviderMock = new Mock<IGeneratedEntityProvider<Driver>>();
        driversProviderMock
            .Setup(x => x.Add(It.IsAny<Driver>()))
            .Callback((Driver driver) =>
            {
                driversDb.Add(driver);
            })
            .Returns((Driver driver) => Task.CompletedTask);

        var handler = new DriversGeneratedHandler(
            driversProviderMock.Object,
            new Mock<IGeneratedEntitiesLinkingProvider<Car, Driver>>().Object,
            allGeneratedValues);
        var handlerWrapper = new HandlerWrapper(handler, new HandleState());

        var generatedDrivers = new ObservableCollection<Driver>();
        generatedDrivers.CollectionChanged += handlerWrapper.GeneratedValues_CollectionChanged;

        var drivers = new List<Driver>()
        {
            new Driver()
            {
                Name = "driver1",
                GeneratedDate = DateTimeOffset.Now
            },
            new Driver()
            {
                Name = "driver2",
                GeneratedDate = DateTimeOffset.Now - TimeSpan.FromSeconds(5)
            }
        };
        generatedDrivers.Add(drivers[0]);
        generatedDrivers.Add(drivers[1]);

        handlerWrapper.WaitWhenHandlerEndWork(drivers.Count); // Ждем пока он обработает добавленные сущности водителей.

        driversDb.Count(d => d.Name == drivers[0].Name).Should().Be(1);
        driversDb.Count(d => d.Name == drivers[1].Name).Should().Be(1);
    }
}