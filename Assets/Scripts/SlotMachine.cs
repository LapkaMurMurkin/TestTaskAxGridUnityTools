using AxGrid.Base;
using AxGrid.FSM;

using UnityEngine;

namespace AxGridUnityTools_TestTask
{
    public class SlotMachine : MonoBehaviourExt
    {
        private FSM _fsm;

        [OnAwake]
        public void Init()
        {
            _fsm = new FSM();
            _fsm.Add(new RunState());
            _fsm.Add(new StopState());
            _fsm.Start("StopState");

            Model.EventManager.AddAction("OnStartClick", StartMachine);
            Model.EventManager.AddAction("OnStopClick", StopMachine);
        }

        [OnUpdate]
        private void UpdateFsm()
        {
            _fsm.Update(Time.deltaTime);
            //Debug.Log("ipdate");
        }

        [OnDestroy]
        private void Destroy()
        {
            Model.EventManager.RemoveAction("OnStartClick", StartMachine);
            Model.EventManager.RemoveAction("OnStopClick", StopMachine);
        }

        private void StartMachine()
        {
            _fsm.Change("RunState");
        }

        private void StopMachine()
        {
            _fsm.Change("StopState");
        }
    }
}