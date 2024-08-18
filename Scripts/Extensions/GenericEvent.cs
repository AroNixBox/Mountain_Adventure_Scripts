using System.Collections.Generic;
using Extensions;
using UnityEngine.Events;

namespace Extensions
{
    public class GenericEvent<T> where T : class, new()
    {
        private Dictionary<string, T> _map = new ();
        public T Get(string channel = "")
        {
            _map.TryAdd(channel, new T());
            return _map[channel];
        }
    }
}

public abstract class UIEvents
{
    //TODO: Rework this, to deal as a string instead of a bool
    public class IsControllerConnected : UnityEvent<bool>{ }
    public static readonly GenericEvent<IsControllerConnected> InputDeviceChanged = new ();
    
    public class PassStringEvent : UnityEvent<string>{ }
    public static readonly GenericEvent<PassStringEvent> PassTextToUI = new ();
    
    public class PassBoolEvent : UnityEvent<bool>{ }
    public static readonly GenericEvent<PassBoolEvent> PassBool = new ();   
    
    public class PassNumberEvent : UnityEvent<int>{ }
    public static readonly GenericEvent<PassNumberEvent> PassNumber = new ();   
    

    public class CountdownData
    {
        public string CountdownText { get; set; }
        public bool IsActive { get; set; }
    }
}
