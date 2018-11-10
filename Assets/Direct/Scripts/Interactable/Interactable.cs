// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using System;
using UnityEngine.XR.MagicLeap;
using UnityEngine;
using UnityEngine.Events;

namespace MagicKit
{
    /// <summary>
    /// Attach this script to any objects which are interactable in your scene and InteractableFrameData will save
    /// relevant information for user focus on these objects.  
    /// </summary>

    public class Interactable : MonoBehaviour
    {
        [Flags]
        public enum InteractableType
        {
            ChannelOne = (1 << 0),
            ChannelTwo = (1 << 1),
            ChannelThree = (1 << 2)
        };

        public enum InteractableState
        {
            Targeting,
            Activating,
            Manipulating,
            Deactivating,
            Integrating
        };

        //----------- Public Events -----------

        [Serializable] public class TargetStateEvent : UnityEvent { };
        [Serializable] public class ActivateStateEvent : UnityEvent { };
        [Serializable] public class ManipulateStateEvent : UnityEvent { };
        [Serializable] public class DeactivateStateEvent : UnityEvent { };
        [Serializable] public class IntegrateStateEvent : UnityEvent { };
        [Serializable] public class FrameDataCreated : UnityEvent<InteractableType, Interactable> { };
        [Serializable] public class FrameDataDestroyed : UnityEvent<InteractableType, Interactable> { };

        [SerializeField, BitMask(typeof(InteractableType))] public InteractableType interactableType;

        public TargetStateEvent OnTarget;
        public ActivateStateEvent OnActivate;
        public ManipulateStateEvent OnManipulate;
        public DeactivateStateEvent OnDeactivate;
        public IntegrateStateEvent OnIntegrate;
        public static FrameDataCreated OnCreated = new FrameDataCreated();
        public static FrameDataDestroyed OnDestroyed = new FrameDataDestroyed();

        //----------- Public Members -----------

        public InteractableState CurrentState
        {
            get
            {
                return _currentState;
            }
        }

        //----------- Private Members -----------

        private InteractableState _currentState;

        //----------- MonoBehaviour Methods -----------

        private void Start()
        {
            OnCreated.Invoke(interactableType, this);
            _currentState = InteractableState.Integrating;
        }

        private void OnDestroy()
        {
            OnDestroyed.Invoke(interactableType, this);
        }

        //----------- Event Handlers ------------

        public void ChangeState(InteractableState newState)
        {
            if(newState == _currentState)
            {
                return;
            }
            switch (newState)
            {
                case InteractableState.Targeting:
                    OnTarget?.Invoke();
                    break;
                case InteractableState.Activating:
                    OnActivate?.Invoke();
                    break;
                case InteractableState.Manipulating:
                    OnManipulate?.Invoke();
                    break;
                case InteractableState.Deactivating:
                    OnDeactivate?.Invoke();
                    break;
                case InteractableState.Integrating:
                    OnIntegrate?.Invoke();
                    break;
            }
            _currentState = newState;
        }
        public void Activate()
        {
            ChangeState(InteractableState.Activating);
        }
        public void Deactivate()
        {
            ChangeState(InteractableState.Deactivating);
        }
        public void Target()
        {
            ChangeState(InteractableState.Targeting);
        }
        public void Manipulate()
        {
            ChangeState(InteractableState.Manipulating);
        }
        public void Integrate()
        {
            ChangeState(InteractableState.Integrating);
        }
    }
}