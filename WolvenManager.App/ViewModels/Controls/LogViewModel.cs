using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using ReactiveUI;
using WolvenKit.Common.Services;
using DynamicData;

namespace WolvenManager.App.ViewModels.Controls
{
    public class LogViewModel : MainViewModel
    {
        private readonly ILoggerService _loggerService;

        private readonly ReadOnlyObservableCollection<LogEntry> _logEntries;
        public ReadOnlyObservableCollection<LogEntry> LogEntries => _logEntries;



        public LogViewModel(
            ILoggerService loggerService
            )
        {
            _loggerService = loggerService;

            //filter, sort and populate reactive list,
            _loggerService.Connect() //connect to the cache
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _logEntries)
                .Subscribe();

        }

    }
}
