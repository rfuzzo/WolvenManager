using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolvenKit.Common;
using WolvenKit.Common.Services;
using Microsoft.Extensions.Logging;
using ILogger = Splat.ILogger;
using LogLevel = Splat.LogLevel;


namespace WolvenManager.App.Services
{
    public class LoggerService : ILoggerService
    {
        public void LogString(string msg, Logtype type)
        {
            
        }

        public void Log(string msg, Logtype type = Logtype.Normal)
        {
            
        }

        public void Info(string s)
        {
            
        }

        public void Warning(string s)
        {
            
        }

        public void Error(string msg)
        {
            
        }

        public void Important(string msg)
        {
            
        }

        public void Success(string msg)
        {
            
        }

    }
}
