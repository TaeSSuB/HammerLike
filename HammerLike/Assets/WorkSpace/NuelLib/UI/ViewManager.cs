using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace NuelLib.UI
{

    /// <summary>
    /// UI View 오브젝트 정보
    /// 인스펙터 상에서 바인딩 및 활용 용도
    /// </summary>
    [System.Serializable]
    public class IViewWrapper
    {
        [SerializeField]
        public GameObject gameobject;

        public IView IView
        {
            get { return gameobject.GetComponent<IView>(); }
        }
    }

    public class ViewManager : MonoBehaviour
    {
        private static ViewManager instance;

        [SerializeField]
        private IViewWrapper startingView;

        public IViewWrapper mainView;

        [SerializeField]
        private IViewWrapper[] views;

        private IView currentView;

        private readonly Stack<IView> history = new Stack<IView>();

        private GameObject currentTooltipObj;

        private void Awake() => instance = this;

        /// <summary>
        /// 설정된 View들 중 T와 같은 값을 판별하고 반환
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetView<T>() where T : IView
        {
            for (int i = 0; i < instance.views.Length; i++)
            {
                if (instance.views[i].IView is T tView)
                {
                    return tView;
                }
            }
            return default(T);
        }

        public static void Show<T>(bool remember = true) where T : IView
        {
            for (int i = 0; i < instance.views.Length; i++)
            {
                if (instance.views[i].IView is T)
                {
                    if (instance.currentView != null)
                    {
                        if (remember)
                        {
                            instance.history.Push(instance.currentView);
                        }
                        instance.currentView.Hide();
                    }

                    instance.views[i].IView.Show();

                    instance.currentView = instance.views[i].IView;
                }
            }
        }

        public static void Show(IView view, bool remember = true)
        {
            if (instance.currentView != null)
            {
                if (remember)
                {
                    instance.history.Push(instance.currentView);

                }
                instance.currentView.Hide();
            }

            view.Show();

            instance.currentView = view;
        }

        public static void ShowLast()
        {
            if (instance.history.Count != 0)
            {
                Show(instance.history.Pop(), false);
            }
        }

        public static void ShowFirst()
        {
            Show(instance.startingView.IView, false);
        }

        public static void ShowMain()
        {
            Show(instance.mainView.IView, false);
        }

        public void ShowTooltip(GameObject newTooltipObj)
        {
            currentTooltipObj?.SetActive(false);

            //var pos = Input.mousePosition;

            newTooltipObj.SetActive(!newTooltipObj.activeSelf);
            currentTooltipObj = newTooltipObj;
        }

        private void Start()
        {
            int trashNum = 0;

            List<IView> listView = new List<IView>();

            for (int i = 0; i < views.Length; i++)
            {
                if (views[i].IView != null)
                {
                    listView.Add(views[i].IView);
                }
                else trashNum++;
            }

            for (int i = 0; i < listView.Count; i++)
            {
                listView[i].Initialize();

                listView[i].Hide();
            }


            if (startingView != null)
            {
                Show(startingView.IView, true);
            }
        }

    }

}