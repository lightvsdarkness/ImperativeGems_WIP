using System.Collections;
using System.Collections.Generic;
using IG.General;
using UnityEngine;

namespace IG.CGrid {
    [CreateAssetMenu(fileName = "CellStateFormT", menuName = "Scriptable Objects/!Cell/CellStateFormT")]
    public class CellStateFormT : ScriptableObject {
        public string Name;
        public Sprite Sprite;

        [Space]
        public float EnergyMaxStored;

        [Space]
        public CellStateFormT NextStateColorWhenClicked;

        private void Start() {

        }

    }
}