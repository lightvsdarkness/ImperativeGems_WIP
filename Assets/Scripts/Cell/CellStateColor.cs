using System.Collections;
using System.Collections.Generic;
using IG.General;
using UnityEngine;

namespace IG.CGrid {
    [CreateAssetMenu(fileName = "CellStateColor", menuName = "Scriptable Objects/!Cell/CellStateColor")]
    public class CellStateColor : ScriptableObject {
        public string Name;
        public ColorData ColorData;
        //public Sprite StateSprite;

        [Space]
        public CellStateColor NextStateColorWhenClicked;

        void Start() {

        }

    }
}