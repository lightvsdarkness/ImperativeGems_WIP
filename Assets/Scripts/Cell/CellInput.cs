using UnityEngine;
using System.Collections;
using IG.General;

namespace IG.CGrid {
    public class CellInput : MonoBehaviour {
        public bool Debugging;

        [SerializeField] int Row;
        [SerializeField] int Column;

        [SerializeField] private CellLogic _cellLogic;


        private void Start() {
            if (_cellLogic == null)
                _cellLogic = GetComponent<CellLogic>();

            //Row = _cellLogic.Data.Row;
            //Column = _cellLogic.Data.Column;
        }

        private void OnMouseDown() {
            if (ManagerCGridGame.I.GamePlaying) {
                if (Debugging)
                    Debug.Log("Cell was clicked", this);
                _cellLogic.RespondToInput();

            }
        }
    }
}