using UnityEngine;

namespace NuelLib
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        private static T instance;

        public static bool isQuit = false;

        public static T Instance
        {
            get
            {
                if (instance == null && !isQuit)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject();
                        instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";
                        DontDestroyOnLoad(singletonObject);
                    }
                }
                return instance;
            }
        }

        protected virtual void Awake()
        {
            Debug.Log("GameManager Awake called on: " + gameObject.name);
            if (instance == null)
            {
                instance = this as T;
                isQuit= false;
                DontDestroyOnLoad(gameObject);
                Debug.Log("Instance created and set to not destroy on load: " + gameObject.name);
            }
        
            else
            {
            Debug.Log("Instance already exists, destroying duplicate: " + gameObject.name);
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 종료 시 에러 예외처리
        /// </summary>
        private void OnApplicationQuit()
        {
            isQuit = true;
            Destroy(gameObject);
        }
    }
}