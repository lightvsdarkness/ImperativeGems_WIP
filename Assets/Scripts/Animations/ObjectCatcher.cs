using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter {
    void ForcedMove(Vector2 moveVector);
}

namespace IG {
    [Serializable]
    public class CaughtObject {
        public Rigidbody2D rigidbody;
        public Collider2D collider;
        public ICharacter character;
        public bool inContact;
        public bool checkedThisFrame;

        public void Move(Vector2 movement) {
            if (!inContact) return;

            if (character != null)
                character.ForcedMove(movement);
            else
                rigidbody.MovePosition(rigidbody.position + movement);
        }
    }

    public class ObjectCatcher : MonoBehaviour, IObjectCatcher {
        public Rigidbody2D platformRigidbody;
        public ContactFilter2D contactFilter;

        protected List<CaughtObject> ObjectsCaught = new List<CaughtObject> (128);
        protected ContactPoint2D[] PointsOfContact = new ContactPoint2D[20];

        protected Collider2D CatcherCollider;
        protected ObjectCatcher ParentCatcher; // Make a List

        protected Action<Vector2> OnMovingDelegate = null;    // For additional complex moving

        public int CaughtObjectsCount {
            get {
                int count = 0;
                for (int i = 0; i < ObjectsCaught.Count; i++) {
                    if (ObjectsCaught[i].inContact)
                        count++;
                }
                return count;
            }
        }

        public float CaughtObjectsMass {
            get {
                float mass = 0f;
                for (int i = 0; i < ObjectsCaught.Count; i++)
                {
                    if (ObjectsCaught[i].inContact)
                        mass += ObjectsCaught[i].rigidbody.mass;
                }
                return mass;
            }
        }


        void Awake () {
            if (platformRigidbody == null)
                platformRigidbody = GetComponent<Rigidbody2D>();

            if (CatcherCollider == null)
                CatcherCollider = GetComponent<Collider2D>();


            ParentCatcher = null;
            Transform currentParent = transform.parent;

            while (currentParent != null) {
                ObjectCatcher parentCatcher = currentParent.GetComponent<ObjectCatcher>();

                if (parentCatcher != null)
                    ParentCatcher = parentCatcher;
                currentParent = currentParent.parent;
            }

            // if we have a parent platform catcher, we make it's move "bubble down" to that catcher, so any object caught by that platform catcher will also be moved by the parent catacher (e.g. a platform catacher on a pressure plate on top of a moving platform)
            if (ParentCatcher != null)
                ParentCatcher.OnMovingDelegate += MoveCaughtObjects;
        }

        void FixedUpdate () {
            for (int i = 0, count = ObjectsCaught.Count; i < count; i++) {
                CaughtObject caughtObject = ObjectsCaught[i];
                caughtObject.inContact = false;
                caughtObject.checkedThisFrame = false;
            }

            CheckRigidbodyContacts(platformRigidbody);

            bool checkAgain;
            do {
                for (int i = 0, count = ObjectsCaught.Count; i < count; i++) {
                    CaughtObject caughtObject = ObjectsCaught[i];

                    if (caughtObject.inContact)
                    {
                        if (!caughtObject.checkedThisFrame)
                        {
                            CheckRigidbodyContacts(caughtObject.rigidbody);
                            caughtObject.checkedThisFrame = true;
                        }
                    }
                    // Some cases will remove all contacts (collider resize etc.) leading to loosing contact with the platform
                    // so we check the distance of the object to the top of the platform.
                    if(!caughtObject.inContact)
                    {
                        Collider2D caughtObjectCollider = ObjectsCaught[i].collider;

                        // check if we are aligned with the moving paltform, otherwise the yDiff test under would be true even if far from the platform as long as we are on the same y level...
                        bool verticalAlignement = (caughtObjectCollider.bounds.max.x > CatcherCollider.bounds.min.x) && (caughtObjectCollider.bounds.min.x < CatcherCollider.bounds.max.x);

                        if (verticalAlignement)
                        {
                            float yDiff = ObjectsCaught[i].collider.bounds.min.y - CatcherCollider.bounds.max.y;

                            if (yDiff > 0 && yDiff < 0.05f)
                            {
                                caughtObject.inContact = true;
                                caughtObject.checkedThisFrame = true;
                            }
                        }
                    }
                }

                checkAgain = false;

                for (int i = 0, count = ObjectsCaught.Count; i < count; i++)
                {
                    CaughtObject caughtObject = ObjectsCaught[i];
                    if (caughtObject.inContact && !caughtObject.checkedThisFrame)
                    {
                        checkAgain = true;
                        break;
                    }
                }
            }
            while (checkAgain);
        }

        private void CheckRigidbodyContacts (Rigidbody2D rb) {
            int contactsCount = rb.GetContacts(contactFilter, PointsOfContact);

            for (int j = 0; j < contactsCount; j++) {
                ContactPoint2D contactPoint2D = PointsOfContact[j];

                Rigidbody2D contactRigidbody = contactPoint2D.rigidbody == rb ? contactPoint2D.otherRigidbody : contactPoint2D.rigidbody;
                int listIndex = -1;

                for (int k = 0; k < ObjectsCaught.Count; k++) {

                    if (contactRigidbody == ObjectsCaught[k].rigidbody) {
                        listIndex = k;
                        break;
                    }
                }

                if (listIndex == -1)
                {
                    if (contactRigidbody != null)
                    {
                        if (contactRigidbody.bodyType != RigidbodyType2D.Static && contactRigidbody != platformRigidbody)
                        {
                            float dot = Vector2.Dot(contactPoint2D.normal, Vector2.down);
                            if (dot > 0.8f)
                            {
                                CaughtObject newCaughtObject = new CaughtObject
                                {
                                    rigidbody = contactRigidbody,
                                    character = contactRigidbody.GetComponent<ICharacter>(),
                                    collider = contactRigidbody.GetComponent<Collider2D>(),
                                    inContact = true,
                                    checkedThisFrame = false
                                };

                                ObjectsCaught.Add(newCaughtObject);
                            }
                        }
                    }
                }
                else
                {
                    ObjectsCaught[listIndex].inContact = true;
                }
            }
        }

        public void MoveCaughtObjects (Vector2 velocity){
            // For complex moving
            if (OnMovingDelegate != null)
                OnMovingDelegate.Invoke(velocity);

            for (int i = 0, count = ObjectsCaught.Count; i < count; i++) {
                CaughtObject caughtObject = ObjectsCaught[i];
                if (ParentCatcher != null && ParentCatcher.ObjectsCaught.Find((CaughtObject A) => { return A.rigidbody == caughtObject.rigidbody; }) != null)
                    continue;

                ObjectsCaught[i].Move(velocity);
            }
        }

        public bool HasCaughtObject (GameObject gameObject) {
            for (int i = 0; i < ObjectsCaught.Count; i++)
            {
                if (ObjectsCaught[i].collider.gameObject == gameObject && ObjectsCaught[i].inContact)
                    return true;
            }

            return false;
        }
    }
}