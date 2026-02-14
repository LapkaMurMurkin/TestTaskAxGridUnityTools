using AxGrid;
using AxGrid.FSM;

using UnityEngine;

namespace AxGridUnityTools_TestTask
{
    [State("StopState")]
    public class StopState : FSMState
    {
        [Enter]
        private void Enter()
        {
            Debug.Log("Enter StopState");
        }

        [Loop(0)]
        private void Update(float dt)
        {
            Settings.Invoke("StopStateUpdate", dt);
        }

        [Exit]
        private void Exit()
        {
            Debug.Log("Exit StopState");
        }
    }
}