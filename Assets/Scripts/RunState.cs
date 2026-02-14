using AxGrid;
using AxGrid.FSM;

using UnityEngine;

namespace AxGridUnityTools_TestTask
{
    [State("RunState")]
    public class RunState : FSMState
    {
        [Enter]
        private void Enter()
        {
            Debug.Log("Enter RunState");
        }

        [Loop(0)]
        private void Update(float dt)
        {
            Settings.Invoke("RunStateUpdate", dt);
        }

        [Exit]
        private void Exit()
        {
            Debug.Log("Exit RunState");
        }
    }
}