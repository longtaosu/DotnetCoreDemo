using Microsoft.Extensions.DependencyInjection;
using System;

/// <summary>
/// https://dotnetcoretutorials.com/2019/10/15/the-factory-pattern-in-net-core/
/// </summary>
namespace _01FactoryPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddTransient<MyService>(MyServiceFactory.Create);
            services.AddTransient<MyService>((serviceProvider) => new MyService(true));

            var provider = services.BuildServiceProvider();

        }
    }


    public interface IVehicle
    {
        VehicleType VehicleType { get; }
        int WheelCount { get; }
    }

    public class Car : IVehicle
    {
        public VehicleType VehicleType => VehicleType.Car;
        public int WheelCount => 4;
    }

    public class Motorbike : IVehicle
    {
        public VehicleType VehicleType => VehicleType.Motorbike;
        public int WheelCount => 2;
    }

    public enum VehicleType
    {
        Car,
        Motorbike
    }

    class VehicleFactory
    {
        public IVehicle Create(VehicleType vehicleType)
        {
            switch (vehicleType)
            {
                case VehicleType.Car:
                    return new Car();
                case VehicleType.Motorbike:
                    return new Motorbike();
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public class MyService
    {
        public MyService(bool constructorParam)
        {

        }
    }
    
    

    public static class MyServiceFactory
    {
        public static MyService Create(IServiceProvider serviceProvider)
        {
            return new MyService(true);
        }
    }
}
