using Autofac.Features.AttributeFilters;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacDemo.AttributeMetadataDemo
{
    public class LogWithFilter
    {
        private readonly ILogAppender _appender;

        public LogWithFilter([MetadataFilter("AppenderName", "attributed")] ILogAppender appender)
        {
            this._appender = appender;
        }

        public void Write(string message)
        {
            this._appender.Write(message);
        }
    }
}
