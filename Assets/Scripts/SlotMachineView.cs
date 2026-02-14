using System.Collections.Generic;

using AxGrid.Base;
using AxGrid.Model;
using AxGrid.Path;
using AxGrid.Tools.Binders;

using UnityEngine;

namespace AxGridUnityTools_TestTask
{
    public class SlotMachineView : MonoBehaviourExtBind
    {
        [SerializeField] private UIButtonDataBind _startButton;
        [SerializeField] private UIButtonDataBind _stopButton;
        [SerializeField] private List<ParticleSystem> _particles;

        [SerializeField] private List<RectTransform> _slotItems;
        [SerializeField] private float _itemSize;
        [SerializeField] private float _maxSpeed;
        [SerializeField] private float _accelerationTime;
        [SerializeField] private float _accelerationDelay;
        [SerializeField] private float _stopButtonDelay;
        [SerializeField] private float _particlesIntensity;

        private CPath[] _slotAnimation;
        private CPath _particleAnimation;


        [OnAwake]
        private void Init()
        {
            _slotAnimation = new CPath[_slotItems.Count];
        }

        [OnStart]
        private void Subscribe()
        {
            Model.EventManager.AddAction($"On{_startButton.buttonName}Click", StartMachine);
            Model.EventManager.AddAction($"On{_stopButton.buttonName}Click", StopMachine);
        }

        [OnDestroy]
        private void Unsubscribe()
        {
            Model.EventManager.RemoveAction($"On{_startButton.buttonName}Click", StartMachine);
            Model.EventManager.RemoveAction($"On{_stopButton.buttonName}Click", StopMachine);
        }

        private void StartMachine()
        {
            Model.Set(_startButton.enableField, false);
            Model.Set(_stopButton.enableField, false);

            SetStartAnimation();
        }

        private void StopMachine()
        {
            Model.Set(_startButton.enableField, true);
            Model.Set(_stopButton.enableField, false);

            SetStopAnimation();
        }

        [Bind("RunStateUpdate")]
        [Bind("StopStateUpdate")]
        private void UpdateAnimation(float dt)
        {
            foreach (CPath anim in _slotAnimation)
                anim.Update(dt);

            _particleAnimation.Update(dt);
        }

        private void SetStartAnimation()
        {
            for (int i = 0; i < _slotItems.Count; i++)
            {
                int localIndex = i;

                _slotAnimation[i] = new CPath()
                    .Wait(_accelerationDelay * i)
                    .EasingLinear(_accelerationTime, 0, _maxSpeed, (speed) =>
                     {
                         Vector2 newPosition = _slotItems[localIndex].anchoredPosition - Vector2.up * Time.deltaTime * speed;
                         UpdateSlotPosition(localIndex, newPosition);
                     })
                     .Add((partPath) =>
                     {
                         Vector2 newPosition = _slotItems[localIndex].anchoredPosition - Vector2.up * Time.deltaTime * _maxSpeed;
                         UpdateSlotPosition(localIndex, newPosition);

                         if (partPath.PathStartTimeF >= _stopButtonDelay)
                             Model.Set(_stopButton.enableField, true);

                         return Status.Continue;
                     });
            }

            _particleAnimation = new CPath()
                .Add((partPath) =>
                {
                    foreach (ParticleSystem ps in _particles)
                        ps.Play();
                    return Status.OK;
                })
                .EasingQuadEaseIn(_accelerationTime, 0, _particlesIntensity, (intensity) =>
                     {
                         foreach (ParticleSystem ps in _particles)
                         {
                             ParticleSystem.EmissionModule emission = ps.emission;
                             emission.rateOverTime = intensity;
                         }
                     });
        }

        private void SetStopAnimation()
        {
            for (int i = 0; i < _slotItems.Count; i++)
            {
                int localIndex = i;
                float currentY = _slotItems[localIndex].anchoredPosition.y;
                float targetY = Mathf.Round(currentY / _itemSize) * _itemSize;

                _slotAnimation[i] = new CPath()
                    .EasingElasticEaseOut(0.7f, currentY, targetY, (offset) =>
                    {
                        _slotItems[localIndex].anchoredPosition = Vector2.up * offset;
                    });

                foreach (ParticleSystem ps in _particles)
                    ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        private void UpdateSlotPosition(int index, Vector2 position)
        {
            RectTransform slot = _slotItems[index];
            slot.anchoredPosition = position;
            if (slot.anchoredPosition.y <= -_itemSize)
                SetFirstChildToLast(slot);
        }

        private void SetFirstChildToLast(RectTransform rectTransform)
        {
            //set child
            Transform lastChild = rectTransform.GetChild(rectTransform.childCount - 1);
            lastChild.SetAsFirstSibling();

            //offset by child size
            Vector2 newPos = rectTransform.anchoredPosition;
            newPos.y += _itemSize;
            rectTransform.anchoredPosition = newPos;
        }
    }
}