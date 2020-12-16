using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticModule
{
    public interface IESClientProvider
    {
        ElasticClient GetClient();

        ElasticClient GetClient(string index);
    }
}
