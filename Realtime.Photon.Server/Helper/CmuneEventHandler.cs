using System;
using System.Collections.Generic;

namespace Cmune.Util
{
    /// <summary>
    /// This is the basic Event Handling Class of CMUNE
    /// 
    /// There should always be 1 static instantiation of this class per room, to guarantee a closed and unique message enviroment
    /// 
    /// Every call to AddListener has to be paired with a call to RemoveListener.
    /// Call AddListener when you want to _start_ to listen to an eventType, call RemoveListener respectivly.
    /// 
    /// In Unity, normally the Monobehaivor methods Start/OnEnable are used to call AddListener
    /// and the method OnDisable is used to call RemoveListener
    /// 
    /// You can only cast Events of type IEventMessage, so implement the interface IEventMessage to create a customized Event.
    /// Use Route to cast a customized Event.
    /// </summary>
    public static class CmuneEventHandler
    {
        private static Dictionary<System.Type, IEventContainer> eventContainer = new Dictionary<System.Type, IEventContainer>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback"></param>
        public static void AddListener<T>(Action<T> callback)
        {
            IEventContainer container;
            if (!eventContainer.TryGetValue(typeof(T), out container))
            {
                container = new EventContainer<T>();
                eventContainer.Add(typeof(T), container);
            }

            EventContainer<T> c = container as EventContainer<T>;
            if (c != null) c.AddCallbackMethod(callback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback"></param>
        public static void RemoveListener<T>(Action<T> callback)
        {
            IEventContainer container;
            if (eventContainer.TryGetValue(typeof(T), out container))
            {
                EventContainer<T> c = container as EventContainer<T>;
                if (c != null) c.RemoveCallbackMethod(callback);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void Route(object message)
        {
            IEventContainer container;
            if (eventContainer.TryGetValue(message.GetType(), out container))
            {
                container.CastEvent(message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private interface IEventContainer
        {
            void CastEvent(object m);
        }

        /// <summary>
        /// This Container class holds a list of all delegates (function pointers) concerning the events of type "T"
        /// To properly add and remove listeners for specific events, we have to store them in a list after instantiation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class EventContainer<T> : IEventContainer
        {
            private Dictionary<string, Action<T>> _dictionary = new Dictionary<string, Action<T>>();

            public event Action<T> Sender;

            public void AddCallbackMethod(Action<T> callback)
            {
                string methodId = GetCallbackMethodId(callback);

                if (!_dictionary.ContainsKey(methodId))
                {
                    _dictionary.Add(methodId, callback);
                    Sender += callback;
                }
            }

            public void RemoveCallbackMethod(Action<T> callback)
            {
                if (_dictionary.Remove(GetCallbackMethodId(callback)))
                {
                    Sender -= callback;
                }
            }

            public string DebugCheck(Action<T> callback)
            {
                System.Text.StringBuilder b = new System.Text.StringBuilder("Check for ");
                b.AppendLine("function: " + GetCallbackMethodId(callback));

                foreach (string t in _dictionary.Keys)
                    b.AppendLine("- Found: " + t);

                return b.ToString();
            }

            private string GetCallbackMethodId(Action<T> callback)
            {
                string s = string.Format("{0}{1}", callback.Method.DeclaringType.FullName, callback.Method.Name);

                if (callback.Target != null)
                {
                    s = string.Format("{0}{1}", s, callback.Target.GetHashCode().ToString());
                }

                return s;
            }

            public void CastEvent(object m)
            {
                if (Sender != null)
                    Sender((T)m);
            }
        }
    }
}