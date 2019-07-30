using System.Collections;
using System.Collections.Generic;
using IG.CGrid;
using UnityEngine;

namespace IG.CGrid {
    public abstract class CellBehaviour : ScriptableObject {
        public virtual void Initialize(CellLogic logic) {
        }

        public abstract void Act(CellLogic logic);
        public abstract void React(CellLogic logic);
        public abstract void RespondToInput(CellLogic logic);   //IEnumerator

        public abstract void Reset(CellLogic logic);
    }
}