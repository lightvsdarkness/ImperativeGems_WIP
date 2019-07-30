using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace IG.CGrid {

    [CreateAssetMenu(fileName = "CellBehaviourReplaceStateFromPalette", menuName = "Scriptable Objects/!Cell/CellBehaviourReplaceStateFromPalette")]
    public class CellBehaviourReplaceStateFromPalette : CellBehaviour {
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

            var paletteData = ManagerPalette.I.GetCurrentData();
            if (paletteData == null) return;

            if (logic.Data.StateColor != paletteData.StateColor || logic.Data.CellStateFormT != paletteData.CellStateFormT) {
                logic.Data.StateColor = paletteData.StateColor;
                logic.Data.CellStateFormT = paletteData.CellStateFormT;

                logic.AuVisuals.ShowClicked();
            }

        }
    }
}