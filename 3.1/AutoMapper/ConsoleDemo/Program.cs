using AutoMapper;
using System;

namespace ConsoleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                //cfg.CreateMap<Foo, FooDto>();
                cfg.CreateMap<Foo, FooDto>()
                .ForMember(dest => dest.Birthday, act => act.MapFrom(src => src.Birthday.ToString("yyyy-MM-dd HH:mm:ss")))
                .ForMember(dest => dest.CreateTime ,act=>act.MapFrom(src=>DateTime.Now));
                //.AfterMap((foo,foodto)=> {
                //    foodto.CreateTime = DateTime.Now;
                //});
            });
            configuration.AssertConfigurationIsValid();
            var mapper = configuration.CreateMapper();

            var foo = new Foo()
            {
                Age = DateTime.Now.Second,
                Name = "张" + DateTime.Now.Hour,
                Birthday = DateTime.Now.AddDays(-1)
            };
            var fooDto = mapper.Map<FooDto>(foo);


            Console.WriteLine("Hello World!");
        }
    }

    public class Foo
    {
        public int Age { get; set; }

        public string Name { get; set; }

        public DateTime Birthday { get; set; }
    }

    public class FooDto
    {
        public int Age { get; set; }

        public string Name { get; set; }

        public string Birthday { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
