using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NuelLib.Utils.Scene
{
    /// <summary>
    /// Scene Manager for NuelLib
    /// </summary>
    public class SceneManageSupporter : SingletonMonoBehaviour<SceneManageSupporter>
    {
        private string loadingSceneName = "sc99_Loading";
        public float minLoadingTime = 1.0f; // Minimum time to show the loading screen

        public SceneData sceneNameData;

        // Event that is triggered when the loading progress changes
        public event Action<float> OnLoadingProgressChanged;

        /// <summary>
        /// �񵿱�� �� ��ȯ
        /// CustomAudio, CustomDebug ��� �߰�(���� ���ȭ �ʿ��� ��)
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadSceneSimple(string sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }

        /// <summary>
        /// �񵿱�� �� ��ȯ + �ε� ��
        /// A, B -> B, C -> C ����
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadSceneWithLoading(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }
        public string GetCurrentSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }


        /// <summary>
        /// �ڷ�ƾ �񵿱�� �� ��ȯ + �ε� ��
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        private IEnumerator LoadSceneAsync(string sceneName)
        {
            AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);
            loadingOperation.allowSceneActivation = false; // �� �ڵ� Ȱ��ȭ ����

            // �ε� �Ǻ�
            while (!loadingOperation.isDone)
            {
                // �� �ε� ���� �� Ȱ��ȭ ���
                if (loadingOperation.progress >= 0.9f)
                {
                    loadingOperation.allowSceneActivation = true;
                }

                yield return null;
            }

            // ���� �� ����
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            // Start loading the new scene asynchronously
            AsyncOperation sceneLoadingOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            sceneLoadingOperation.allowSceneActivation = false; // Prevent the new scene from being activated automatically

            // Wait until the new scene is loaded
            while (!sceneLoadingOperation.isDone)
            {
                if (sceneLoadingOperation.progress >= 0.9f)
                {
                    //(here you can do other stuff, like custom loading or something)
                    //then when done, you must call
                    sceneLoadingOperation.allowSceneActivation = true;
                }

                // Update the loading bar or spinner based on the loading progress
                float progress = Mathf.Clamp01(sceneLoadingOperation.progress / 0.9f); // Divide by 0.9f to account for the final 10% of loading

                // Update the loading UI
                OnLoadingProgressChanged?.Invoke(progress);

                yield return new WaitForEndOfFrame();
            }

            // Wait for a minimum amount of time to show the loading screen, regardless of loading progress
            float startTime = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup < startTime + minLoadingTime)
            {
                yield return new WaitForEndOfFrame();
            }

            // Unload the loading scene
            SceneManager.UnloadSceneAsync(loadingSceneName);

            // Activate the new scene
            //sceneLoadingOperation.allowSceneActivation = true;
        }
    }

}
