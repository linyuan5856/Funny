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

        public void Create(CreateUiParam param)
        {
            IsTickWindow = param.IsTick;
            IsAutoOpen = param.IsAutoOpen;
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