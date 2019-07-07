using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacDemo.AttributeMetadataDemo
{
    public class TypedManualMetadataAppender : ILogAppender
    {
        public void Write(string message)
        {
            Console.WriteLine("Strongly-Typed Metadata on Registration: {0}", message);
        }
    }
}
