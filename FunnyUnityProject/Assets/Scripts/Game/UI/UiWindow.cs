using UnityEngine;

namespace GFrame.Ui
{
    public class UiWindow : MonoBehaviour
    {
        public void OpenWindow()
        {
            OnOpenWindow();
        }

        public void Update()
        {
            OnUpdate();
        }

        public void CloseWindow()
        {
            OnCloseWindow();
        }

        protected virtual void OnOpenWindow()
        {
        }

        protected virtual void OnUpdate()
        {
        }


        protected virtual void OnCloseWindow()
        {
        }
    }
}