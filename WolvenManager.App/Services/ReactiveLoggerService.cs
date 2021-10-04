using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Microsoft.Extensions.Logging.Abstractions;
using WolvenKit.Common;
using WolvenKit.Common.Services;

namespace WolvenManager.App.Services
{
    public class ReactiveLoggerService : ILoggerService
    {
        private const int Limit = 100000;

        private readonly SourceList<LogEntry> _log = new SourceList<LogEntry>();
        public IObservable<IChangeSet<LogEntry>> Connect() => _log.Connect();


        public void LogString(string msg, Logtype type)
        {
            var x = new LogEntry(msg, type, DateTime.Now);
            if (_log.Count > Limit)
            {
                _log.RemoveAt(0);
            }
            _log.Add(x);
        }

        // Normal
        public void Log(string msg, Logtype type = Logtype.Normal) => LogString(msg, type);


        public void Success(string msg) => LogString(msg, Logtype.Success);

        public void Info(string s) => LogString(s, Logtype.Important);
        public void Important(string s) => LogString(s, Logtype.Important);

        public void Warning(string s) => LogString(s, Logtype.Error);

        public void Error(string msg) => LogString(msg, Logtype.Error);
        public void Error(Exception exception)
        {
            var msg =
                $"========================\r\n" +
                $"{exception.ToString()}" +
                $"\r\n========================";
            Error(msg);
        }
    }
    
}
