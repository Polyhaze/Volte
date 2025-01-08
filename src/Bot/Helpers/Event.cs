using System.Collections.Immutable;

namespace Volte.Helpers;

public class Event<T>
{
    private readonly object _subLock = new();
    private ImmutableArray<Action<T>> _subscriptions = [];
    
    private readonly Queue<T> _handlerlessEvents;

    public Event(bool enableHandlerlessQueue = false)
    {
        _handlerlessEvents = enableHandlerlessQueue ? [] : null;
    }
    
    public bool HasSubscribers
    {
        get
        {
            lock (_subLock)
                return _subscriptions.Length != 0;
        }
    }

    public ImmutableArray<Action<T>> Subscriptions
    {
        get
        {
            lock (_subLock)
                return _subscriptions;
        }
    }

    public void Add(Action<T> subscriber)
    {
        Guard.Require(subscriber, nameof(subscriber));
        lock (_subLock)
            _subscriptions = _subscriptions.Add(subscriber);
    }

    public void Remove(Action<T> subscriber)
    {
        Guard.Require(subscriber, nameof(subscriber));
        lock (_subLock)
            _subscriptions = _subscriptions.Remove(subscriber);
    }

    public void Clear()
    {
        lock (_subLock)
            _subscriptions = [];
    }

    public void CallHandlers(T arg)
    {
        lock (_subLock)
        {
            if (_subscriptions.Length == 0 && _handlerlessEvents is not null)
            {
                _handlerlessEvents.Enqueue(arg);
                return;
            }
            
            if (_handlerlessEvents is not null)
                while (_handlerlessEvents.TryDequeue(out var queuedArg))
                    this.Call(queuedArg);
            
            this.Call(arg);
        }
    }
}

public static class EventExtensions
{
    public static void Call<T>(this Event<T> eventHandler,
        T arg
    ) => eventHandler.Subscriptions.ForEach(x => x(arg));
}