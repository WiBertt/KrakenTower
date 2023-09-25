using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WibertStudio
{
    /// <summary>
    /// This is a template script if you want to make new behaviours based on your current character state.
    /// </summary>
    public class TemplateScript : MonoBehaviour, IStateAccessable
    {
        //Called once the script is activated from a player state
        public void EnterState()
        {
            //Logic to implement
        }

        //Called like a normal update function if activated from the state
        public void UpdateState()
        {
            //Logic to implement
        }

        //Called like a normal fixed update function if activated from the state
        public void FixedUpdateState()
        {
            //Logic to implement
        }

        //Called once you exit a state
        public void ExitState()
        {
            //Logic to implement
        }
    }
}