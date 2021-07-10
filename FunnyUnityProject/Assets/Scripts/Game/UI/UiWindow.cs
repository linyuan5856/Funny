namespace GFrame.Ui
{
    public class UiWindow
    {
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