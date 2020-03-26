﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Lts.Services
{
    public interface ILogService
    {
        void PrintLog(string msg);
    }
    public class LogService:ILogService
    {
        public void PrintLog(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
