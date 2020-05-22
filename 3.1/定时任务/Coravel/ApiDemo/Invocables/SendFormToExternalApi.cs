using Coravel.Invocable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiDemo.Invocables
{
    public class SendFormToExternalApi : IInvocable
    {
        public SendFormToExternalApi WithForm(SampleForm form)
        {
            return this;
        }
        public Task Invoke()
        {
            throw new NotImplementedException();
        }
    }

    public class SampleForm
    {

    }
}
