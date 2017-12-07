using System;
using System.Threading.Tasks;

namespace Satochat.Client.Service.Api {
    public class SatoApiEventHandler {
        public class EventReceivedEventArg {
            public string Uuid { get; private set; }
            public string Name { get; private set; }
            public string Data { get; private set; }

            public EventReceivedEventArg(string uuid, string name, string data) {
                Uuid = uuid;
                Name = name;
                Data = data;
            }
        }
        
        public delegate Task RaiseEventReceivedEventHandler(object sender, EventReceivedEventArg arg);
        public event RaiseEventReceivedEventHandler EventReceivedEvent;
        public Task RaiseEventReceivedEvent(object sender, EventReceivedEventArg arg) => EventReceivedEvent?.Invoke(sender, arg);

        public class ExceptionOccurredEventArg {
            public Exception Exception { get; private set; }

            public ExceptionOccurredEventArg(Exception exception) {
                Exception = exception;
            }
        }

        public delegate void ExceptionOccurredEventHandler(object sender, ExceptionOccurredEventArg arg);
        public event ExceptionOccurredEventHandler ExceptionOccurredEvent;
        public void RaiseExceptionOccurredEvent(object sender, ExceptionOccurredEventArg arg) => ExceptionOccurredEvent?.Invoke(sender, arg);
    }
}
