using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace IG.General {
    public class SingletonManager<T> : SerializedMonoBehaviour where T : Component
    {
        protected static T instance;
        public static T I {
            get {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        instance = obj.AddComponent<T>();
                    }
                }
                if (false) { 
                    System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
                    System.Diagnostics.StackFrame frame = stackTrace.GetFrame(0);
                    UnityEngine.Debug.Log(frame + " ");
                    frame = stackTrace.GetFrame(1);
                    UnityEngine.Debug.Log(frame + " ");
                }
                return instance;
            }
        }

        //[SerializeField]


        protected WaitForSeconds WaitTimer;


        [Tooltip("Tick to log debug info to the Console window")]
        public bool Debugging = false;
        public bool DebugLogging { get { return Debugging || Debug.isDebugBuild; } }

        protected virtual void Awake() {
            if (instance == null) {
                instance = this as T;
            }
            else {
                // ACHTUNG if(this is ManagerGame) return; // ACHTUNG Dirtiest of hacks!
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy() {
            instance = null;
        }
    }
}
