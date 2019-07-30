using UnityEngine;

namespace IG {
    public class Panel : MonoBehaviour {

        public UnityEngine.UI.Selectable firstSelected;

        /// <summary>
        /// Select the specified element so that we can navigate through the panel using a keyboard or gamepad
        /// </summary>
        public void SelectFirstElement() {
            firstSelected.Select();
        }
    }
}
