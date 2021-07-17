namespace GFrame.Ui
{
    public class UiWindow
    {
        public bool IsTickWindow;
        public void OpenWindow()
        {
            OnOpenWindow();
        }


        public void CloseWindow()
        {
            OnCloseWindow();
        }

        protected virtual void OnOpenWindow()
        {
        }


        protected virtual void OnCloseWindow()
        {
        }
    }
}