using System;
using System.Collections.Generic;
using GFrame.Ui;

namespace GFrame.System
{
    public class UiSystem : BaseSystem
    {
        private Dictionary<Type, UiWindow> _openedUiDic;

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
        }

        public void OpenWindow<T>() where T : UiWindow
        {
            Type key = typeof(T);
            if (HasUiOpened(key))
                return;
        }

        public void CloseWindow<T>() where T : UiWindow
        {
        }

        private bool HasUiOpened(Type key)
        {
            return _openedUiDic.ContainsKey(key);
        }
    }
}