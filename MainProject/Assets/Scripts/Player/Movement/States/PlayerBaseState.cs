using UnityEngine;

namespace WibertStudio
{
    public abstract class PlayerBaseState
    {
        protected PlayerManager context;
        public abstract void EnterState(PlayerManager ctx);

        public abstract void UpdateState();

        public abstract void FixedUpdateState();

        public abstract void ExitState();

        public abstract void SwitchConditions();
    }
}