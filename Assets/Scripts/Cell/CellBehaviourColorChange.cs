using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace IG.CGrid {

    [CreateAssetMenu(fileName = "CellBehaviourColorChange", menuName = "Scriptable Objects/!Cell/CellBehaviourColorChange")]
    public class CellBehaviourColorChange : CellBehaviour {
        public override void Initialize(CellLogic logic) {
            base.Initialize(logic);

        }

        public override void Act(CellLogic tank) {

        }

        public override void React(CellLogic logic) {

        }

        public override void Reset(CellLogic logic) {

        }

        public override void RespondToInput(CellLogic logic) {
            if (logic.Data == null) Debug.LogError("This cell doesn't have CellData", this);

            if (logic.Data.StateColor.NextStateColorWhenClicked != null) {
                logic.Data.StateColor = logic.Data.StateColor.NextStateColorWhenClicked;
                //logic.CellVisuals.

                //yield break;

                logic.AuVisuals.ShowClicked();
            }

        }
    }
}