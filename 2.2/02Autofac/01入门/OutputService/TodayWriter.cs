using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacDemo
{
    public class TodayWriter : IDateWriter
    {
        private IOutput _output;
        public TodayWriter(IOutput output)
        {
            this._output = output;
        }
        public void WriteDate()
        {
            _output.Write(DateTime.Now.ToString("HH:mm:ss-ffff"));
        }
    }
}
