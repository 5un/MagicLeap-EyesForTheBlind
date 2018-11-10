// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.MagicLeap;
using System;

namespace MagicKit
{
    /// <summary>
    /// Provides an input module which does a raycast from the viewpoint center. Intended to provide standardization
    /// with Unity's default input modules.
    /// </summary>
    public class HeadposeInputModule : PointerInputModule
    {
        //----------- Public Events -----------

        public Action<GameObject> OnRaycastHitChange;

        //----------- Public Members -----------

        //----------- Private Members -----------

        [SerializeField] private GameObject _uiCursorPrefab;
        private GameObject _uiCursor;
        private PointerEventData _pointerEventData;
        private MouseButtonEventData _inputButtonEventData;
        private GameObject _currentRaycastObject;

        //----------- MonoBehaviour Methods -----------

        protected override void OnEnable()
        {
            // Try to set input override
            BaseInput headposeInput = GetComponent<BaseInput>();
            if (headposeInput != null)
            {
                m_InputOverride = headposeInput;
            }

            // Create cursor instance
            if (_uiCursor == null)
            {
                _uiCursor = Instantiate(_uiCursorPrefab, transform);
                _uiCursor.SetActive(false);
            }

            // Create structures to hold pointer data
            _pointerEventData = new PointerEventData(EventSystem.current);
            _inputButtonEventData = new MouseButtonEventData
            {
                buttonData = _pointerEventData,
            };
            base.OnEnable();
        }

        //----------- Public Methods -----------

        public override void Process()
        {
            // Raycast from headpose
            Camera headposeCamera = Camera.main;
            _pointerEventData.position = headposeCamera.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
            eventSystem.RaycastAll(_pointerEventData, m_RaycastResultCache);
            RaycastResult raycastResult = FindFirstRaycast(m_RaycastResultCache);
            _pointerEventData.pointerCurrentRaycast = raycastResult;
            m_RaycastResultCache.Clear();
            SetCurrentRaycastObject(_pointerEventData.pointerCurrentRaycast.gameObject);
            ProcessHeadposeCursor(headposeCamera);

            // Call pointer handlers if necessary
            HandlePointerExitAndEnter(_pointerEventData, _pointerEventData.pointerCurrentRaycast.gameObject);

            // Call input handlers if necessary
            _inputButtonEventData.buttonState = StateForInput();
            ProcessPointerPress(_inputButtonEventData);
        }

        //----------- Private Methods -----------

        private void SetCurrentRaycastObject(GameObject currentObject)
        {
            if (_currentRaycastObject != currentObject)
            {
                OnRaycastHitChange?.Invoke(currentObject);
            }

            _currentRaycastObject = currentObject;
        }

        private void ProcessHeadposeCursor(Camera headposeCamera)
        {
            if (_currentRaycastObject != null && _currentRaycastObject.GetComponent<RectTransform>() != null)
            {
                // turn on UI headpose cursor
                if (!_uiCursor.activeSelf)
                {
                    _uiCursor.SetActive(true);
                }

                RaycastResult result = _pointerEventData.pointerCurrentRaycast;
                Ray ray = headposeCamera.ScreenPointToRay(result.screenPosition);

                // position cursor just slightly in front of whatever ui element we hit
                Vector3 endPoint = headposeCamera.transform.position +
                                   ray.direction * (result.distance + headposeCamera.nearClipPlane) * 0.98f;
                _uiCursor.transform.position = endPoint;
            }
            else
            {
                if (_uiCursor.activeSelf)
                {
                    _uiCursor.SetActive(false);
                }
            }
        }

        private PointerEventData.FramePressState StateForInput()
        {
            if (input == null)
                return PointerEventData.FramePressState.NotChanged;

            bool tapDown = input.GetMouseButtonDown((int) MLInputControllerButton.Bumper);
            bool tapUp = input.GetMouseButtonUp((int) MLInputControllerButton.Bumper);

            if (tapDown && tapUp)
                return PointerEventData.FramePressState.PressedAndReleased;
            if (tapDown)
                return PointerEventData.FramePressState.Pressed;
            return tapUp ? PointerEventData.FramePressState.Released : PointerEventData.FramePressState.NotChanged;
        }

        private void ProcessPointerPress(MouseButtonEventData inputEvent)
        {
            PointerEventData pointerEvent = inputEvent.buttonData;
            GameObject currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

            if (inputEvent.PressedThisFrame())
            {
                ProcessPointerPressed(pointerEvent, currentOverGo);
            }

            if (inputEvent.ReleasedThisFrame())
            {
                ProcessPointerReleased(pointerEvent, currentOverGo);
            }
        }

        private void ProcessPointerPressed(PointerEventData pointerEvent, GameObject currentOverGo)
        {
            FillOutPointerEventData(pointerEvent);
            DeselectIfSelectionChanged(currentOverGo, pointerEvent);
            GameObject pressedHandler = ExecutePointerDownOrClicked(currentOverGo, pointerEvent);
            pointerEvent.pointerPress = pressedHandler;
            pointerEvent.rawPointerPress = currentOverGo;
            IncrementOrResetClickCounter(pointerEvent);
            ProcessPointerDrag(pointerEvent, currentOverGo);
        }

        private static void FillOutPointerEventData(PointerEventData pointerEvent)
        {
            pointerEvent.eligibleForClick = true;
            pointerEvent.delta = Vector2.zero;
            pointerEvent.dragging = false;
            pointerEvent.useDragThreshold = true;
            pointerEvent.pressPosition = pointerEvent.position;
            pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;
        }

        private static GameObject ExecutePointerDownOrClicked(GameObject currentOverGo, PointerEventData pointerEvent)
        {
            GameObject pressHandler =
                ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);
            if (pressHandler == null)
            {
                pressHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);
            }
            return pressHandler;
        }

        private static void IncrementOrResetClickCounter(PointerEventData pointerEvent)
        {
            float time = Time.unscaledTime;
            if (pointerEvent.pointerPress == pointerEvent.lastPress)
            {
                float deltaTime = time - pointerEvent.clickTime;
                if (deltaTime < 0.3f)
                {
                    pointerEvent.clickCount += 1;
                }
                else
                {
                    pointerEvent.clickCount = 1;
                }
            }
            else
            {
                pointerEvent.clickCount = 1;
            }

            pointerEvent.clickTime = time;
        }

        private static void ProcessPointerDrag(PointerEventData pointerEvent, GameObject currentOverGo)
        {
            pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);
            if (pointerEvent.pointerDrag != null)
            {
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
            }
        }

        private void ProcessPointerReleased(PointerEventData pointerEvent, GameObject currentOverGo)
        {
            ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

            GameObject pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

            if (pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick)
            {
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
            }
            else if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
            {
                ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
            }

            pointerEvent.eligibleForClick = false;
            pointerEvent.pointerPress = null;
            pointerEvent.rawPointerPress = null;

            if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
            {
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
            }

            pointerEvent.dragging = false;
            pointerEvent.pointerDrag = null;

            NotifyPreviouslyIgnoredHandlers(pointerEvent, currentOverGo);
        }

        private void NotifyPreviouslyIgnoredHandlers(PointerEventData pointerEvent, GameObject currentOverGo)
        {
            // if we ignored an element due to having pressed be handled by a differente element,
            // the previously ignored element now gets notified of the enter/exit events
            if (currentOverGo != pointerEvent.pointerEnter)
            {
                HandlePointerExitAndEnter(pointerEvent, null);
                HandlePointerExitAndEnter(pointerEvent, currentOverGo);
            }
        }
    }
}
