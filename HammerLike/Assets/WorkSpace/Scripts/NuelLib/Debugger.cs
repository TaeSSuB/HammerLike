using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NuelLib.CustomDebug
{
    public class Debugger : SingletonMonoBehaviour<Debugger>
    {
        public string currentSceneName = "Default";

        private List<string> logMessages = new List<string>();
        private bool isDevMode = false;
        private float deltaTime = 0.0f;
        private float sliderValue = 1.0f;

        protected override void Awake()
        {
            base.Awake();
            //Debug.unityLogger.filterLogType = LogType.Exception;
#if !(UNITY_EDITOR)
            Debug.unityLogger.filterLogType = LogType.Exception;
            Destroy(this);
#endif
        }

        private void Start()
        {
            isDevMode = true;
            Application.logMessageReceivedThreaded += HandleLog;

            InvokeRepeating("RemoveOldLog", 0f, 2f);
        }
        private void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        private void OnDestroy()
        {
            Application.logMessageReceivedThreaded -= HandleLog;
            isDevMode = false;
        }

        public void UpdateSceneName(string _currentSceneName)
        {
            Debug.Log("Scene update: " + currentSceneName + " to " + _currentSceneName);

            currentSceneName = _currentSceneName;
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (!isDevMode) return;

            // Truncate log message if it exceeds the maximum length
            if (logString.Length > 20)
            {
                logString = logString.Substring(0, 20) + "...";
            }

            logMessages.Add(logString);
            if (logMessages.Count > 10)
            {
                logMessages.RemoveAt(logMessages.Count - 1);
            }

            logMessages.Reverse();

            //Invoke("RemoveOldLog", 5f);
        }

        private void RemoveOldLog()
        {
            if(logMessages.Count > 0) logMessages.RemoveAt(0);
        }

        private void OnGUI()
        {
            if (isDevMode)
            {

            }
        }
    }
}