using System;
using System.Collections.Generic;
using System.Text;

namespace EasyNetQDemo.Common.Polymorphic
{
    public interface IPayment
    {
        decimal Amount { get; set; }
    }
}
