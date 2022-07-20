using System;
using System.Collections.Generic;
using Blauhaus.Analytics.Abstractions;
using Blauhaus.Common.Utils.Disposables;
using Microsoft.Extensions.Logging;

namespace Blauhaus.Push.Runner
{
    public class DummyLogger<T>: IAnalyticsLogger<T>
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            
        }

        public bool IsEnabled(LogLevel logLevel)
        {

            return false;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new ActionDisposable(() => { });
        }

        public IAnalyticsLogger SetValue(string key, object value)
        {
            return this;
        }

        public IAnalyticsLogger SetValues(Dictionary<string, object> newProperties)
        {
            return this;
        }

        public IDisposable BeginScope()
        {
            return new ActionDisposable(() => { });
        }

        public IDisposable BeginTimedScope(LogLevel logLevel, string messageTemplate, params object[] args)
        {
            return new ActionDisposable(() => { });
        }

        public IDisposable LogTimed(LogLevel logLevel, string messageTemplate, params object[] args)
        {
            return new ActionDisposable(() => { });
        }
    }
}