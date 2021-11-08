using System;
using System.Collections.Generic;
using GFrame.Service;
using GFrame.Ui;
using UnityEngine;

namespace GFrame.System
{
    public class UiSystem : BaseSystem
    {
        private Dictionary<Type, UiController> _openedUiDic;
        private readonly List<UiController> _tickList = new List<UiController>();

        private LoaderService _loaderService;
        private Canvas _canvasRoot;

        private LoaderService GetLoadService()
        {
            return _loaderService ??= GetService<LoaderService>();
        }

        private Canvas GetCanvas()
        {
            if (_canvasRoot == null)
            {
                var loader = GetLoadService();
                loader.SetLoader(ELoadType.Resource);
                var root = loader.Instantiate<GameObject>("prefab/uiroot");
                _canvasRoot = root.GetComponentInChildren<Canvas>();
            }

            return _canvasRoot;
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            _openedUiDic = new Dictionary<Type, UiController>(16);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            for (int i = _tickList.Count - 1; i >= 0; i--)
            {
                var controller = _tickList[i];
                if (!controller.IsTickWindow)
                {
                    _tickList.RemoveAt(i);
                    continue;
                }

                controller.Update();
            }
        }

        public void OpenWindow<T>(bool isAutoOpen = true, bool isTick = true) where T : UiWindow
        {
            Type key = typeof(T);
            UiController controller = null;
            if (HasUiLoaded(key))
                controller = _openedUiDic[key];
            else
            {
                controller = new UiController();
                controller.Create(GetLoadService(), new CreateUiParam
                {
                    Path = @"Assets/~Test/~UI/" + key.Name + ".prefab",
                    IsTick = isTick,
                    IsAutoOpen = isAutoOpen,
                });
                _openedUiDic[key] = controller;
                if (isTick)
                    _tickList.Add(controller);
                controller.LoadSync();
                var window = controller.Window;
                window.transform.SetParent(GetCanvas().transform);
                window.transform.localPosition = Vector3.zero;
            }

            controller.Open();
        }

        public void CloseWindow<T>() where T : UiWindow
        {
            Type key = typeof(T);
            if (!HasUiLoaded(key)) return;
            var controller = _openedUiDic[key];
            controller.Close();
            if (controller.IsTickWindow)
                _tickList.Remove(controller);
        }

        public void DestroyWindow<T>() where T : UiWindow
        {
            Type key = typeof(T);
            if (!HasUiLoaded(key)) return;
            var controller = _openedUiDic[key];
            controller.Destroy();
            _openedUiDic.Remove(key);
            if (controller.IsTickWindow)
                _tickList.Remove(controller);
        }

        private bool HasUiLoaded(Type key)
        {
            return _openedUiDic.ContainsKey(key);
        }
    }
}