using Autofac.Features.Metadata;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AutofacDemo.AttributeMetadataDemo
{
    public class Log
    {
        private readonly IEnumerable<Meta<ILogAppender>> _appenders;

        public Log(IEnumerable<Meta<ILogAppender>> appenders)
        {
            this._appenders = appenders;
        }

        public void Write(string destination, string message)
        {
            var appender = this._appenders.First(a => a.Metadata["AppenderName"].Equals(destination));
            appender.Value.Write(message);
        }
    }
}
