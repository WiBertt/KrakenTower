using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WibertStudio
{
    public interface IStateAccessable
    {
        void EnterState();
        void UpdateState();
        void FixedUpdateState();
        void ExitState();
    }
}