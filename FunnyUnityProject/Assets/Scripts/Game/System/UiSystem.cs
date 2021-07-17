using System;
using System.Collections.Generic;
using GFrame.Ui;

namespace GFrame.System
{
    public class UiSystem : BaseSystem
    {
        private Dictionary<Type, UiController> _openedUiDic;
        private List<UiController> _tickList = new List<UiController>();

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
                if (controller.IsTickWindow)
                    controller.Update();
            }
        }

        public void OpenWindow<T>(bool isAutoOpen, bool isTick) where T : UiWindow
        {
            Type key = typeof(T);
            UiController controller = null;
            if (HasUiLoaded(key))
            {
                controller = _openedUiDic[key];
                return;
            }

            controller = new UiController();
            controller.Create(new CreateUiParam
            {
                Path = key.Name,
                IsTick = isTick,
                IsAutoOpen = isAutoOpen
            });
            _openedUiDic[key] = controller;
            if (isTick)
                _tickList.Add(controller);
        }

        public void CloseWindow<T>() where T : UiWindow
        {
            Type key = typeof(T);
            if (!HasUiLoaded(key))
                return;
            var controller = _openedUiDic[key];
            controller.Close();
            if (controller.IsTickWindow)
                _tickList.Remove(controller);
        }

        public void DestroyWindow<T>() where T : UiWindow
        {
            Type key = typeof(T);
            if (!HasUiLoaded(key))
                return;
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