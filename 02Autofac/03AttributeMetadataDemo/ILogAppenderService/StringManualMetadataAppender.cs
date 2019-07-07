using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacDemo.AttributeMetadataDemo
{
    public class StringManualMetadataAppender:ILogAppender
    {
        public void Write(string message)
        {
            Console.WriteLine("String Metadata on Registration: {0}", message);
        }
    }
}
