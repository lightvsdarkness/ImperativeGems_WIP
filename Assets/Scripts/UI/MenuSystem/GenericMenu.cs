using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI.Extensions;

namespace IG.UI {
    public abstract class GenericMenu<T> : MenuBase<T> where T : GenericMenu<T> {
        [Space]
        public Canvas MainCanvas;
        public List<GameObject> SubMenuGameObjects = new List<GameObject>();

        protected override void Awake() {
            base.Awake();

            if(MainCanvas == null)
                MainCanvas = GetComponent<Canvas>();
            if (MainCanvas == null)
                Debug.LogError("Can't find MainCanvas", this);
        }


        /// <summary>
        /// Do some visual stuff after opening menu
        /// </summary>
        public override void VisualizeOpeningDetails() {
            if (MainMenuManager.I.ParentTransform != null)
                Instance.transform.parent = MainMenuManager.I.ParentTransform;

            var topMenu = MainMenuManager.I.GetTopMenu() as T;
            if (topMenu != null) { 
                var previousCanvas = topMenu.MainCanvas;
                MainCanvas.sortingOrder = previousCanvas.sortingOrder + 1;
            }

            foreach (var go in SubMenuGameObjects) {
                go.SetActive(true);
            }

            base.VisualizeOpeningDetails();
        }

        public static void Hide() {
            Close();
        }
        /// <summary>
        /// Do some visual stuff, and after that - close menu in base method
        /// </summary>
        public override void VisualizeClosingDetails() {

            base.VisualizeOpeningDetails();
        }

    }
}