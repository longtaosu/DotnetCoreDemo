using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AutofacDemo.AttributeMetadataDemo
{
    [AppenderName("attributed")]
    public class AttributeMetadataAppender : ILogAppender
    {
        void ILogAppender.Write(string message)
        {
            Console.WriteLine("Attribute Metadata : {0}", message);
        }
    }
}
