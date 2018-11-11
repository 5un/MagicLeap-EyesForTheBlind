// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine;
using UnityEngine.Events;

namespace UnityEngine.XR.MagicLeap
{
    public class WorldRaycastSphereSample : MonoBehaviour
    {
        #region Public Variables
        // [System.Serializable]
        // public class RaycastResultEvent : UnityEvent<MLWorldRays.MLWorldRaycastResultState, RaycastHit, float> { }

        // [Space]
        // [Tooltip("The callback handler for raycast result.")]
        // public RaycastResultEvent OnRaycastResult;

        [Tooltip("The number of horizontal rays to cast. Single raycasts set to 1.")]
        public uint Width = 1;

        [Tooltip("The number of vertical rays to cast. Single raycasts set to 1.")]
        public uint Height = 1;

        [Tooltip("The horizontal field of view in degrees to determine density of Width/Height raycasts.")]
        public float HorizontalFovDegrees;

        [Tooltip("If true the ray will terminate when encountering an unobserved area. Useful for determining unmapped areas.")]
        public bool CollideWithUnobserved = false;

        public float xAngle;
        public float yAngle;

        public int raycastId;
        #endregion

        #region Protected Variables
        protected MLWorldRays.QueryParams _raycastParams = new MLWorldRays.QueryParams();

        // Stores if previous raycast called finished (true) or not (false).
        protected bool _isReady = true;

        #endregion

        #region Public Properties
        /// <summary>
        /// Returns the last raycasts origination position.
        /// </summary>
        public Vector3 RayOrigin
        {
            get
            {
                return _raycastParams.Position;
            }
        }

        /// <summary>
        /// Returns the last raycasts origination direction.
        /// </summary>
        public Vector3 RayDirection
        {
            get
            {
                return _raycastParams.Direction;
            }
        }
        #endregion

        #region Protected Properties
        /// <summary>
        /// Returns raycast position.
        /// </summary>
        protected Vector3 Position
        {
            get
            {
                return _camera.transform.position;
            }
        }

        /// <summary>
        /// Returns raycast direction.
        /// </summary>
        protected Vector3 Direction
        {
            get
            {
                return _camera.transform.forward;
            }
        }

        /// <summary>
        /// Returns raycast up.
        /// </summary>
        protected Vector3 Up
        {
            get
            {
                return _camera.transform.up;
            }
        }
        #endregion

        #region Private Variables
        private Camera _camera;
        #endregion

        #region Unity Methods
        /// <summary>
        /// Initializes component.
        /// </summary>
        virtual protected void OnEnable()
        {
            _isReady = true;
            MLResult result = MLWorldRays.Start();
            if (!result.IsOk)
            {
                Debug.LogErrorFormat("Error: BaseRaycast failed starting MLWorldRays, disabling script. Reason: {0}", result);
                enabled = false;
                return;
            }
        }

        /// <summary>
        /// Cleans up component.
        /// </summary>
        virtual protected void OnDisable()
        {
            if (MLWorldRays.IsStarted)
            {
                MLWorldRays.Stop();
            }
        }

        void Awake()
        {
            _camera = Camera.main;
            if (_camera == null)
            {
                Debug.LogError("Error: WorldRaycastHead._camera is null, disabling script.");
                enabled = false;
                return;
            }
        }

        /// <summary>
        /// Continuously casts rays using _raycastParams.
        /// </summary>
        private void Update()
        {
            if (_isReady)
            {
                _isReady = false;

                _raycastParams.Position = Position;
                _raycastParams.Direction = Direction;
                _raycastParams.UpVector = Up;
                _raycastParams.Width = Width;
                _raycastParams.Height = Height;
                _raycastParams.HorizontalFovDegrees = HorizontalFovDegrees;
                _raycastParams.CollideWithUnobserved = CollideWithUnobserved;

                MLWorldRays.GetWorldRays(_raycastParams, HandleOnReceiveRaycast);
            }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Returns a RaycastHit based on results from callback function HandleOnReceiveRaycast.
        /// </summary>
        /// <param name="state"> The state of the raycast result.</param>
        /// <param name="point"> Position of the hit.</param>
        /// <param name="normal"> Normal of the surface hit.</param>
        /// <param name="confidence"> Confidence value on hit.</param>
        /// <returns></returns>
        protected RaycastHit GetWorldRaycastResult(MLWorldRays.MLWorldRaycastResultState state, Vector3 point, Vector3 normal, float confidence)
        {
            RaycastHit result = new RaycastHit();

            if (state != MLWorldRays.MLWorldRaycastResultState.RequestFailed && state != MLWorldRays.MLWorldRaycastResultState.NoCollision)
            {
                result.point = point;
                result.normal = normal;
                result.distance = Vector3.Distance(_raycastParams.Position, point);
            }

            return result;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Callback handler called when raycast call has a result.
        /// </summary>
        /// <param name="state"> The state of the raycast result.</param>
        /// <param name="point"> Position of the hit.</param>
        /// <param name="normal"> Normal of the surface hit.</param>
        /// <param name="confidence"> Confidence value on hit.</param>
        virtual protected void HandleOnReceiveRaycast(MLWorldRays.MLWorldRaycastResultState state, Vector3 point, Vector3 normal, float confidence)
        {
            RaycastHit result = GetWorldRaycastResult(state, point, normal, confidence);
            // OnRaycastResult.Invoke(state, result, confidence);
            //TODO: Raycast Result should send id

            _isReady = true;
        }
        #endregion
    }
}
