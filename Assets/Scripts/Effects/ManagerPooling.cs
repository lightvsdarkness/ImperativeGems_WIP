using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace IG.General {
    public class ManagerPooling : SingletonManager<ManagerPooling> {
        [System.Serializable]
        public class PreloadedPrefab {
            public GameObject Prefab = null; // prefab to check for pooling
            public int Amount = 15; // amount of objects to instantiate at start
        }

        public List<PreloadedPrefab> PreloadedPrefabs = new List<PreloadedPrefab>();
        // The transform to which parent pooled objects
        protected Transform PoolTransform;
        // the pooled objects currently available
        protected Dictionary<string, List<GameObject>> AvailableObjects = new Dictionary<string, List<GameObject>>();
        // used to distinquish between different prefabs with identical names
        protected Dictionary<string, int> UniquePrefabNames = new Dictionary<string, int>();

        protected void Start() {
            foreach (PreloadedPrefab obj in PreloadedPrefabs) {
                if (obj != null && obj.Prefab != null && obj.Amount > 0)
                    AddToPool(obj.Prefab, Vector3.zero, Quaternion.identity, obj.Amount);
            }
        }

        /// <summary>
        /// adds one or more deactivated instances of a prefab to the pool
        /// </summary>
        public virtual void AddToPool(GameObject prefab, Vector3 position, Quaternion rotation, int amount = 1) {
            if (prefab == null) return;

            string uniqueName = GetUniqueNameOf(prefab);

            // add this prefab type to the available objects dictionary under a unique name
            if (!AvailableObjects.ContainsKey(uniqueName))
                AvailableObjects.Add(uniqueName, new List<GameObject>());

            // create amount of objects
            for (int i = 0; i < amount; i++) {
                GameObject newObj = GameObject.Instantiate(prefab, position, rotation) as GameObject;
                newObj.name = uniqueName;
                newObj.transform.parent = PoolTransform;
                newObj.SetActive(false);
                AvailableObjects[uniqueName].Add(newObj);
            }
        }

        /// <summary>
        /// If prefab object in in Dictionary pool, it will use it. If it isn't, a new object will be instantiated and added to the pool
        /// </summary>
        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation) {
            //, GameObject newParent = null
            if (prefab == null) return null;

            GameObject go = null;
            List<GameObject> availableObjects = null;

            string uniqueName = GetUniqueNameOf(prefab);

            // check if we have objects like 'prefab' in the pool
            if (AvailableObjects.TryGetValue(uniqueName, out availableObjects)) {

                // get the first available pooled object of same type as 'original'
                while (availableObjects.Count > 0 && availableObjects[0] == null) {
                    availableObjects.Remove(availableObjects[0]);
                }
                go = availableObjects[0];

                go.transform.position = position;
                go.transform.rotation = rotation;

                // remove the object from the 'available' list
                availableObjects.Remove(go);

                go.SetActive(true);

                //for (int i = 0; i < go.transform.childCount; i++) { // Let's not control inner state
                //    Transform child = go.transform.GetChild(i);
                //    if (child != null)
                //        child.gameObject.SetActive(true);
                //}

                //if (go.transform.parent == m_Transform) // Let's not unparent for now. TODO: Parent to something
                //    go.transform.parent = null;

                return go;
            }

            // add a new object if this type of object isn't being pooled
            AddToPool(prefab, position, rotation);

            // return the new object by calling this method again
            return Spawn(prefab, position, rotation);
        }

        /// <summary>
        /// Puts the object back into the pool if it's being pooled or destroys it if not
        /// </summary>
        public void DespawnWithTimer(GameObject obj, float delay = 0) {
            if (delay > 0)
                Invoke("DespawnInternal", delay);
            else
                Despawn(obj);
        }

        public void Despawn(GameObject obj) {
            // NOTE: we don't fetch object instance ids or use 'GetUniqueNameOf'in this method, since we deal with scene instances with different ids from the original (project view) prefabs

            if (obj == null) return;

            // --- We need to check if object we are despawning is part of some pooled prefab object
            List<GameObject> availableObjects = null;
            string uniqueName = GetUniqueNameOf(obj);

            AvailableObjects.TryGetValue(uniqueName, out availableObjects);

            // --- If no prefabs with that we either deactivate object or delete altogether
            if (availableObjects == null) {
                bool pooled = CheckIfParentPooled(obj);

                if (pooled)
                    obj.SetActive(false);
                else
                // If we end up here, neither the object nor any of its ancestors were pooled: destroy it for real
                    UnityEngine.Object.Destroy(obj);
                return;
            }

            // --- If such prefabs are pooled, deactivate it, parent to pool and add to pool's list
            obj.SetActive(false);
            obj.transform.parent = PoolTransform;
            availableObjects.Add(obj);
        }

        /// <summary>
        /// Вызывается только если объекта нет в пуле префабов, но может его родитель - в пуле префабов
        /// </summary>
        public bool CheckIfParentPooled(GameObject obj) {
            // First should check if parent exists
            var parentTransform = obj.transform.parent;
            if (parentTransform == null) return false;

            List<GameObject> availableObjects = null;
            string uniqueParentName = GetUniqueNameOf(parentTransform.gameObject);
            AvailableObjects.TryGetValue(uniqueParentName, out availableObjects);

            if (availableObjects != null)
                return true;
            else
            // Parent of checked object isn't pooled, but parent of parent could be pooled
                return CheckIfParentPooled(parentTransform.gameObject);
        }

        /// <summary>
        /// used to distinguish between different prefabs with identical names. basically: when a prefab is instantiated with the same name as another (previously spawned) prefab, the new instance will have the unique instance id of the prefab added to the in-world gameobject name. All instances of that prefab will go by the modified name
        /// </summary>
        protected string GetUniqueNameOf(GameObject prefab) {
            int id;
            // --- We have a pooled prefab with the same name + id: return the name
            if (UniquePrefabNames.TryGetValue(prefab.name, out id)) {

                if (prefab.GetInstanceID() == id)
                    return prefab.name;

                // we have a pooled prefab with the same name but a different id, so make sure we store this prefab under a new unique name (the prefab name + id)
                string newName = string.Join(" ", new string[] {prefab.name, prefab.GetInstanceID().ToString()});
                if (!UniquePrefabNames.ContainsKey(newName))
                    UniquePrefabNames.Add(newName, prefab.GetInstanceID());
                return newName;
            }

            // --- We don't have a previously spawned prefab with this name: store and return the name
            UniquePrefabNames.Add(prefab.name, prefab.GetInstanceID());

            return prefab.name;
        }

    }
}