using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using Moq;
using WpfApp.Domain;
using WpfApp.Logic.GeneratedEntities;
using WpfApp.Logic.GeneratedValueHandlers;

namespace WpfApp.UnitTests;

public class CarsGeneratedHandlerTests
{
    [Fact]
    public void ShouldSaveToDb()
    {
        var allGeneratedValues = new ConcurrentBag<IGeneratedProperties>();
        var carsDb = new ConcurrentBag<Car>();

        var carsProviderMock = new Mock<IGeneratedEntityProvider<Car>>();
        carsProviderMock
            .Setup(x => x.Add(It.IsAny<Car>()))
            .Callback((Car car) =>
            {
                carsDb.Add(car);
            })
            .Returns((Car car) => Task.CompletedTask);

        var handler = new CarsGeneratedHandler(
            carsProviderMock.Object,
            allGeneratedValues);
        var handlerWrapper = new HandlerWrapper(handler, new HandleState());

        var generatedCars = new ObservableCollection<Car>();
        generatedCars.CollectionChanged += handlerWrapper.GeneratedValues_CollectionChanged;

        var cars = new List<Car>()
        {
            new Car()
            {
                Name = "car1",
                GeneratedDate = DateTimeOffset.Now
            },
            new Car()
            {
                Name = "car2",
                GeneratedDate = DateTimeOffset.Now - TimeSpan.FromSeconds(5)
            }
        };
        generatedCars.Add(cars[0]);
        generatedCars.Add(cars[1]);

        handlerWrapper.WaitWhenHandlerEndWork(cars.Count); // Ждем пока он обработает добавленные сущности машин.

        carsDb.Count(d => d.Name == cars[0].Name).Should().Be(1);
        carsDb.Count(d => d.Name == cars[1].Name).Should().Be(1);
    }
}