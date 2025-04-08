using System.Collections.Concurrent;

namespace Wxck.AdminTemplate.CrossCutting.EventBus {

    public class EventAggregator {
        private static readonly Lazy<EventAggregator> _instance = new(() => new EventAggregator());

        public static EventAggregator Instance => _instance.Value;

        private readonly ConcurrentDictionary<Type, ConcurrentBag<Action<object>>> _eventSubscribers = new();

        public void Publish<TEventType>(TEventType eventData) {
            var eventType = typeof(TEventType);
            if (_eventSubscribers.TryGetValue(eventType, out var eventSubscriber)) {
                foreach (var subscriber in eventSubscriber) {
                    if (eventData != null) subscriber.Invoke(eventData);
                }
            }
        }

        public void Subscribe<TEventType>(Action<TEventType> action) {
            var eventType = typeof(TEventType);
            if (!_eventSubscribers.ContainsKey(eventType)) {
                _eventSubscribers[eventType] = [];
            }
            _eventSubscribers[eventType].Add(obj => action((TEventType)obj));
        }
    }
}