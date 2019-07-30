using UnityEngine;
using UnityEngine.UI.Extensions;

namespace IG.UI {
    public abstract class MenuBase<T> : MenuBase where T : MenuBase<T> {

        public static T Instance { get; private set; }

        protected virtual void Awake() {
            Instance = (T)this;
        }

        protected virtual void OnDestroy() {
            Instance = null;
        }


        public void OpenFromPrefab() {
            
        }
        public static void Open() {
            OpenActually();
        }

        protected static void OpenActually() {
            if (Instance == null) {
                //Debug.LogError("Opening menu: " + typeof(T).Name);
                MainMenuManager.I.CreateInstance(typeof(T).Name);
            }
            else
                Instance.gameObject.SetActive(true);

            MainMenuManager.I.OpenMenu(Instance);
            Instance.VisualizeOpeningDetails();
        }

        public virtual void VisualizeOpeningDetails() {
        }

        public static void Close() {
            CloseActually();
        }
        protected static void CloseActually() {
            if (Instance == null)
            {
                Debug.LogErrorFormat("Trying to close menu {0} but Instance is null", typeof(T));
                return;
            }

            Instance.VisualizeClosingDetails();
            MainMenuManager.I.CloseMenu(Instance);
        }

        public virtual void VisualizeClosingDetails() {
        }

        public override void OnBackPressed() {
            Close();
        }
    }

    public abstract class MenuBase : MonoBehaviour
    {
        [Tooltip("Destroy the Game Object when menu is closed (reduces memory usage)")]
        public bool DestroyWhenClosed = true;

        [Tooltip("Disable menus that are under this one in the stack")]
        public bool DisableMenusUnderneath = true;

        public abstract void OnBackPressed();
    }
}