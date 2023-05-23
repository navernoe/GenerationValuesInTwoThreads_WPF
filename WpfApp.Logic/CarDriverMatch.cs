using WpfApp.Domain;
using WpfApp.Logic.GeneratedEntities;

namespace WpfApp.Logic;

public class CarDriverMatch : IGeneratedValuesMatch<Car, Driver>
{
    public Car Entity1 { get; set; }
    public Driver Entity2 { get; set; }
    public string Key { get; set; }
}