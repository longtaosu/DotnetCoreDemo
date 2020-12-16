using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticModule
{
    public class ESClientProvider : IESClientProvider
    {
        private readonly ElasticSetting _elasticSetting;

        public ESClientProvider(IOptions<ElasticSetting> elasticSetting)
        {
            _elasticSetting = elasticSetting.Value;
        }

        public ElasticClient GetClient()
        {
            var uri = new Uri(_elasticSetting.uri);
            return new ElasticClient(new ConnectionSettings(uri).DefaultIndex(_elasticSetting.defaultIndex));
        }

        public ElasticClient GetClient(string index)
        {
            var uri = new Uri(_elasticSetting.uri);
            return new ElasticClient(new ConnectionSettings(uri).DefaultIndex(index));
        }
    }
}
