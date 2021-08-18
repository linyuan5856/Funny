namespace GFrame.StateMachine
{
    public class HotUpdateState : BaseState
    {
        public override void OnEnter()
        {
            ChangeState(GameDefine.LoginState);
        }

        public override void OnExit()
        {
        }
    }
}