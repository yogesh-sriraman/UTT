using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.medcare360.utt
{
    public class SingletonManager<T> : MonoBehaviour
        where T : MonoBehaviour
    {
        private static T _instance;
        public static T Instance
        {
            get { return FindInstance(); }
            private set { _instance = value; }
        }

        private static T FindInstance()
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject go = new GameObject($"{typeof(T).Name} (singleton)");
                    _instance = go.AddComponent<T>();
                }
            }
            return _instance;
        }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            else
            {
                if (Instance != this)
                {
                    Destroy(gameObject);
                }
            }
            DontDestroyOnLoad(gameObject);
        }
    }

}