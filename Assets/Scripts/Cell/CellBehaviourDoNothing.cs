using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace IG.CGrid {
    [CreateAssetMenu(fileName = "CellBehaviourDoNothing", menuName = "Scriptable Objects/!Cell/CellBehaviourDoNothing")]
    public class CellBehaviourDoNothing : CellBehaviour
    {
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

        }
    }
}