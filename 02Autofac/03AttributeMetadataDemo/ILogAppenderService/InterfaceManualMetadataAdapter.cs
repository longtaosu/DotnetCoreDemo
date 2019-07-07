using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacDemo.AttributeMetadataDemo
{
    public class InterfaceManualMetadataAdapter : ILogAppender
    {
        public void Write(string message)
        {
            Console.WriteLine("Interface Metadata on Registration: {0}", message);
        }
    }
}
