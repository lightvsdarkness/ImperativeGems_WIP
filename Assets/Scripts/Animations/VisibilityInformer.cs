using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IG {
    /// <summary>
    /// Notifies parent object (without renderer) be notified when a child become visible
    /// </summary>
    public class VisibilityInformer : MonoBehaviour {
        public System.Action<VisibilityInformer> objectBecameVisible;

        private void OnBecameVisible() {
            objectBecameVisible(this);
        }
    }
}