using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Composition;
using System.Linq;

namespace AutofacDemo.AttributeMetadataDemo
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public class AppenderNameAttribute:Attribute
    {
        public AppenderNameAttribute(string appenderName)
        {
            this.AppenderName = appenderName;
        }
        public string AppenderName { get; set; }
    }
}
