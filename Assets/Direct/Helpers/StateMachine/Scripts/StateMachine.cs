// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;
using UnityEngine.Events;

namespace MagicLeap.Utilities
{
    [System.Serializable]
    public class StateChangeEvent : UnityEvent<State> { }

    ///<summary>
    /// Basic state machine that iterates through states (child objects). 
    ///</summary>
    public class StateMachine : MonoBehaviour
    {
        //----------- Public Events -----------
        public StateChangeEvent OnStateChange = new StateChangeEvent();

        //----------- Public variables -----------
        public bool startFirstStateOnStart = true;

        //----------- Private Members -----------
        private State[] _states;
        private int _currentStateIndex;

        private int CurrentStateIndex
        {
            get { return _currentStateIndex; }
            set
            {
                if (_currentStateIndex != value)
                {
                    // disable current state object only if current != next
                    _states[_currentStateIndex].gameObject.SetActive(false);
                    if (OnStateChange != null)
                    {
                        OnStateChange.Invoke(_states[value]);
                    }
                }

                _states[value].gameObject.SetActive(true);
                _currentStateIndex = value;
            }
        }

        //----------- MonoBehaviour Methods -----------
        private void Awake()
        {
            _states = GetComponentsInChildren<State>(true);
            foreach (var item in _states)
            {
                item.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            if (startFirstStateOnStart)
            {
                GoToState(0);
            }
        }

        //----------- Public Methods -----------
        public void GoToNext()
        {
            if (CurrentStateIndex + 1 >= _states.Length)
            {
                return;
            }
            CurrentStateIndex++;
        }

        public void GoToPrevious()
        {
            if (CurrentStateIndex <= 0)
            {
                return;
            }
            CurrentStateIndex--;
        }

        /// <summary>
        /// Go to a certain state using state name.
        /// </summary>
        public void GoToState(string stateName)
        {
            int index = FindStateByName(stateName);
            if (index != -1)
            {
                CurrentStateIndex = index;
            }
        }

        /// <summary>
        /// Go to a certain state using index number of state, if known.
        /// </summary>
        public void GoToState(int index)
        {
            if (index < 0 || index >= _states.Length)
            {
                return;
            }

            CurrentStateIndex = index;
        }

        public State GetCurrentState()
        {
            return _states[CurrentStateIndex];
        }

        //----------- Private Methods -----------
        private int FindStateByName(string name)
        {
            for (int i = 0; i < _states.Length; i++)
            {
                if (_states[i].name.Equals(name))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
