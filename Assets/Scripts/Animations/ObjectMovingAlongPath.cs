using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IG {
    public interface IObjectMovingAlongPath {
        void TeleportObject();
    }
    public interface IObjectCatcher {
        void MoveCaughtObjects(Vector2 moveVector);
    }

    [SelectionBase]
    [RequireComponent(typeof(Rigidbody2D))]
    public class ObjectMovingAlongPath : MonoBehaviour, IObjectMovingAlongPath {
        public enum MovingPlatformType {
            BACK_FORTH,
            LOOP,
            ONCE
        }

        public bool Debugging;

        public MovingPlatformType PlatformType;
        public bool UseLocalSpace;
        public bool UseObjectCatcher;

        public float Speed = 1.0f;


        [Space]
        public bool StartMovingOnlyWhenVisible;
        public bool StartMovingAtStart = true;


        [Space]
        [HideInInspector]
        public Vector3[] NodesLocal = new Vector3[1];
        public Vector3[] NodesWorld { get { return _NodesWorld; } }
        protected Vector3[] _NodesWorld;

        public float[] NodesWaitTimes = new float[1];

        protected float CurrentNodeWaitTime = -1.0f;
        protected int CurrentNode = 0;
        protected int NextNode = 0;
        protected int MovingDirection = 1;
        public Vector3 StartRelativeToParent; // TODO: ACHTUNG! visible in inspector because bug - doesn't set properly


        [Space] //[SerializeField]
        protected Vector2 PlatformVelocity;
        public Vector2 Velocity { get { return PlatformVelocity; } }


        protected bool Moving = false;
        protected bool VeryFirstStart = false;

        protected Rigidbody2D PlatformRigidbody;
        public ObjectCatcher ObjectCatcher;


        private void Start() {
            PlatformRigidbody = GetComponent<Rigidbody2D>();
            PlatformRigidbody.isKinematic = true;

            if (ObjectCatcher == null)
                ObjectCatcher = GetComponent<ObjectCatcher>();

            StartRelativeToParent = transform.localPosition;
            //Debug.LogError("local pos: " + transform.localPosition, this);

            // Allow to make platform only move when they became visible
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; ++i)
            {
                var visibilityInformer = renderers[i].gameObject.AddComponent<VisibilityInformer>();
                visibilityInformer.objectBecameVisible = BecameVisible;
            }

            CalculateNodesWorldPositions();

            Initialize();
        }

        private void FixedUpdate() {
            if (!Moving) return;

            if (CurrentNode == NextNode) return; // no need to update we have a single node in the path

            if (CurrentNodeWaitTime > 0)
            {
                CurrentNodeWaitTime -= Time.deltaTime;
                return; // this update - waiting
            }

            // How much platform can cover
            float distancePossibleToCover = Speed * Time.deltaTime;

            while (distancePossibleToCover > 0)
            {
                // How much (and where) platform should go
                Vector2 direction = _NodesWorld[NextNode] - transform.position;

                float distance = distancePossibleToCover;

                // IF WE CAN REACH NEXT NODE
                if (direction.sqrMagnitude < distance * distance)
                {
                    // we have to go farther than our current goal point, so we set the distance to the remaining distance
                    // then we change the current & next indexes
                    distance = direction.magnitude;

                    CurrentNode = NextNode;

                    CurrentNodeWaitTime = NodesWaitTimes[CurrentNode];

                    // MOVING FORWARD
                    if (MovingDirection > 0)
                    {
                        NextNode += 1;

                        // REACHED THE END
                        if (NextNode >= _NodesWorld.Length)
                        {
                            switch (PlatformType)
                            {
                                case MovingPlatformType.BACK_FORTH:
                                    NextNode = _NodesWorld.Length - 2; // move to node before last one
                                    MovingDirection = -1;
                                    break;
                                case MovingPlatformType.LOOP:
                                    NextNode = 0;
                                    break;
                                case MovingPlatformType.ONCE:
                                    NextNode -= 1;
                                    StopMoving();
                                    break;
                            }
                        }

                    }
                    // MOVING BACKWARD
                    else
                    {
                        NextNode -= 1;

                        // REACHED THE BEGINNING
                        if (NextNode < 0)
                        {
                            switch (PlatformType)
                            {
                                case MovingPlatformType.BACK_FORTH:
                                    NextNode = 1;
                                    MovingDirection = 1;
                                    break;
                                case MovingPlatformType.LOOP:
                                    NextNode = _NodesWorld.Length - 1;
                                    break;
                                case MovingPlatformType.ONCE:
                                    NextNode += 1;
                                    StopMoving();
                                    break;
                            }
                        }

                    }
                }

                //transform.position +=  direction.normalized * distance; // If no Rigidbody

                PlatformVelocity = direction.normalized * distance;
                PlatformRigidbody.MovePosition(PlatformRigidbody.position + PlatformVelocity);

                if (UseObjectCatcher)
                    ObjectCatcher?.MoveCaughtObjects(PlatformVelocity);

                // We remove the distance we moved. That way if we didn't had enough distance to the next goal, we will do a new loop to finish
                // the remaining distance we have to cover this frame toward the new goal
                distancePossibleToCover -= distance;

                // we have some wait time set, that mean we reach a point where we have to wait. So no need to continue to move the platform, early exit.
                if (CurrentNodeWaitTime > 0.001f)
                    break;
            }
        }

        protected void Initialize() {
            CurrentNode = 0;
            MovingDirection = 1;
            NextNode = NodesLocal.Length > 1 ? 1 : 0;

            CurrentNodeWaitTime = NodesWaitTimes[0];

            VeryFirstStart = false;
            if (StartMovingAtStart) {
                Moving = !StartMovingOnlyWhenVisible;
                VeryFirstStart = true;
            }
            else
                Moving = false;
        }

        public void TeleportObject() {
            transform.localPosition = StartRelativeToParent;

            Initialize();

            CalculateNodesWorldPositions();
        }

        private void Reset() {
            //we always have at least a node which is the local position
            NodesLocal[0] = Vector3.zero;
            NodesWaitTimes[0] = 0;
        }

        /// <summary>
        /// We make point in the path being defined in local space so game designer can move the platform & path together
        /// but as the object will move during gameplay, that would also move the node. So we convert the local nodes to world position
        /// </summary>
        public void CalculateNodesWorldPositions() {
            if (Debugging) Debug.LogWarning("Current WorldPosition: " + transform.TransformPoint(transform.position));
            _NodesWorld = new Vector3[NodesLocal.Length];

            //for (int i = 0; i < _NodesWorld.Length; i++) {
            //    Debug.LogWarning("Calculating _NodesWorld. Node " + i + ": " + _NodesWorld[i]);
            //}

            for (int i = 0; i < _NodesWorld.Length; i++) {
                if (Debugging) Debug.LogWarning("Using NodesLocal to calculate _NodesWorld. Node " + i + ": " + NodesLocal[i]);
            }

            for (int i = 0; i < _NodesWorld.Length; ++i)
                _NodesWorld[i] = transform.TransformPoint(NodesLocal[i]);

            for (int i = 0; i < _NodesWorld.Length; i++) {
                if (Debugging) Debug.LogWarning("Calculated _NodesWorld. Node " + i + ": " + _NodesWorld[i]);
            }

            if (Debugging) Debug.LogWarning("Current WorldPosition: " + transform.TransformPoint(transform.position));
        }

        public void StartMoving() {
            Moving = true;
        }

        public void StopMoving() {
            Moving = false;
        }

        public void ResetPlatform() {
            transform.position = _NodesWorld[0];
            Initialize();
        }

        private void BecameVisible(VisibilityInformer obj) {
            if (VeryFirstStart)
            {
                Moving = true;
                VeryFirstStart = false;
            }
        }
    }
}