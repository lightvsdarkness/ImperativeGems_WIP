using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace IG.CGrid {
    public class PaletteCell : SerializedMonoBehaviour {
        //
        public CellData Data;
        public string CellStateColorName;
        public CellAuVisual AuVisuals;
        //public CellBehaviour CellBehaviour;

        public static event Action<PaletteCell> OnCellClicked;

        private void Awake() {
            if (AuVisuals == null)
                AuVisuals = gameObject.GetComponent<CellAuVisual>();
        }

        private void Start() {

        }

        // NOTE: Change to event-based?..
        private void Update() {
            CellStateColorName = Data?.StateColor?.Name;
            //Visuals.UpdateVisuals();
        }
        //private void OnDestroy() {
        //    Debug.Break();
        //}

        public void InstallCellData(CellData cellData) {
            Data = cellData;
            AuVisuals.UpdateCellAuVisuals(Data);
        }

        public void RespondToInput() {
            //CellBehaviour?.RespondToInput(this);
            OnCellClicked?.Invoke(this);

            AuVisuals.UpdateCellAuVisuals(Data);
        }


        public void Reset() {
            //CellBehaviour.Reset(this);

        }


    }
}