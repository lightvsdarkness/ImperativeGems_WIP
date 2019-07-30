using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IG.CGrid {
    public class PaletteInput : MonoBehaviour {
        public bool Debugging;

        [Space]
        [SerializeField] private CellLogic Logic;
        private CellData Data;

        [Space]
        public bool Color;
        [Space]
        public bool Form;


        private void Start() {
            if (Logic == null)
                Logic = GetComponent<CellLogic>();

            if (Data == null)
                Data = Logic.Data;
        }

        private void OnMouseDown() {
            if (ManagerCGridGame.I.GamePlaying) {
                if (Debugging)
                    Debug.Log("Palette Cell was clicked", this);
                ManagerPalette.I.SelectCellColor(Logic);
                ManagerPalette.I.SelectCellForm(Logic);

            }

        }


    }
}