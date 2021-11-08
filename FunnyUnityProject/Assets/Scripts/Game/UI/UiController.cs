using GFrame.Service;
using UnityEngine;

namespace GFrame.Ui
{
    public struct CreateUiParam
    {
        public string Path;
        public bool IsAutoOpen;
        public bool IsTick;
    }

    public class UiController
    {
        public bool IsTickWindow;
        private bool IsAutoOpen;
        private UiWindow _window;
        private LoaderService _loader;
        private string _uiPath;

        public UiWindow Window => _window;

        public void Create(LoaderService loader, CreateUiParam param)
        {
            _loader = loader;
            IsTickWindow = param.IsTick;
            IsAutoOpen = param.IsAutoOpen;
            _uiPath = param.Path;
        }

        public void LoadSync()
        {
            _loader.SetLoader(ELoadType.AssetBundle);
            var prefab = _loader.Instantiate<GameObject>(_uiPath);
            _window = prefab.GetComponent<UiWindow>();
        }

        public void Open()
        {
            if (_window != null)
                _window.OpenWindow();
        }

        public void Update()
        {
            if (_window != null)
                _window.Update();
        }

        public void Close()
        {
            _window.CloseWindow();
        }

        public void Destroy()
        {
            _window.CloseWindow();
            _window = null;
        }
    }
}