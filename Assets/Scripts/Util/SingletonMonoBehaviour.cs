namespace DFSW
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [System.Serializable]
    internal abstract class SingletonMonoBehaviour<T> : Sirenix.OdinInspector.SerializedMonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        private static T _instance = null;
        public static T instance
        {
            get
            {
                if (_instance == null && (_instance = FindObjectOfType<T>()) == null)
                {
                    _instance = new GameObject { name = typeof(T).Name + "Singleton" }.AddComponent<T>();
                    Debug.LogError(string.Format("{0} could not be found! Creating a dummy to prevent a crash.", typeof(T).Name));
                }
                return _instance;
            }
            protected set
            {
                _instance = value;
            }
        }
        public static bool Exists() => _instance;
        public static bool IsNull() => !_instance;
        public static void Toggle()
        {
            Toggle(!instance.gameObject.activeInHierarchy);
        }
        public static void Toggle(bool state)
        {
            instance.gameObject.SetActive(state);
        }
        public static void ToggleOn()
        {
            instance.gameObject.SetActive(true);
        }
        public static void ToggleOff()
        {
            instance.gameObject.SetActive(false);
        }
        public static bool IsOn => instance.gameObject.activeInHierarchy;
    }
}