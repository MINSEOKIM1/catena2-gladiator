using System.Collections.Generic;
using UnityEngine;

namespace Bucket
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance { get { return _instance; } }
    
        private static EventManager _instance = null;
        private Dictionary<EVENT_TYPE, List<IListener>> Listeners =
            new Dictionary<EVENT_TYPE, List<IListener>>();


        //싱글톤
        void Awake()
        {
            if(_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                return;
            }
            DestroyImmediate(gameObject);
        }
    
        public void AddListener(EVENT_TYPE eventType, IListener Listener)
        {
            List<IListener> ListenList = null;
        
            if(Listeners.TryGetValue(eventType, out ListenList))
            {
                ListenList.Add(Listener);
                return;
            }
        
            ListenList = new List<IListener>();
            ListenList.Add(Listener);
            Listeners.Add(eventType, ListenList);
        }
    
        //event posting
        public void PostNotification(EVENT_TYPE eventType, Component Sender, object param = null)
        {
            List<IListener> ListenList = null;

            if (!Listeners.TryGetValue(eventType, out ListenList))
                return;


            for(int i = 0; i < ListenList.Count; i++)
                ListenList?[i].OnEvent(eventType, Sender, param);
        }
    
        //null들 지우기
        public void RemoveRedundancies()
        {
            Dictionary<EVENT_TYPE, List<IListener>> newListeners =
                new Dictionary<EVENT_TYPE,List<IListener>>();

            foreach(KeyValuePair<EVENT_TYPE, List<IListener>> Item in Listeners)
            {
                for (int i = Item.Value.Count - 1; i >= 0; i--)
                {
                    if(Item.Value[i].Equals(null))
                        Item.Value.RemoveAt(i);
                }

                if(Item.Value.Count > 0)
                    newListeners.Add(Item.Key, Item.Value);
            }

            Listeners = newListeners;
        }
    }

    public enum EVENT_TYPE
    {
        eDatePass, //param is next date
        eChallengeMail,
    };

    public interface IListener
    {
        void OnEvent(EVENT_TYPE EventType, Component Sender, object Param1 = null, object Param2 = null);
    }
}