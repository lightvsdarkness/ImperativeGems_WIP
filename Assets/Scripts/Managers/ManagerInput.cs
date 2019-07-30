using System.Collections;
using System.Collections.Generic;
using IG.General;
using UnityEngine;

namespace IG {
    public class ManagerInput : SingletonManager<ManagerInput> {

        void Start() {

        }


        void Update() {
            if (Input.GetButtonDown("Cancel"))
            { }
        }

        public void ShowCursorBecauseOfMenu() {
            //vp_LocalPlayer.ShowMouseCursor();
            //vp_Utility.LockCursor = false;
        }

        public void HideAndLockCursorBecauseOfMenu() {
            //vp_LocalPlayer.HideMouseCursor();
            //vp_Utility.LockCursor = true;
        }
    }
}