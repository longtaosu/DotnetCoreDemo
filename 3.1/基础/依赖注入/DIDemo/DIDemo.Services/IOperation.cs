using System;
using System.Collections.Generic;
using System.Text;

namespace DIDemo.Services
{
    public interface IOperation
    {
        Guid OperationId { get; }
    }

    public interface IOperationSingleton : IOperation { }

    public interface IOperationTransient : IOperation { }

    public interface IOperationScoped : IOperation { }

    public interface IOperationSingletonInstance : IOperation { }


    public class Operation : IOperationScoped,IOperationSingleton,IOperationTransient, IOperationSingletonInstance
    {
        private Guid _guid;

        public Guid OperationId => _guid;

        public Operation()
        {
            _guid = Guid.NewGuid();
        }

        public Operation(Guid guid)
        {
            _guid = guid;
        }
    }
}
