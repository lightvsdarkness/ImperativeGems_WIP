using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using IG.General;
using UnityEngine;

namespace IG.CGrid {
    public class ManagerPalette : SingletonManager<ManagerPalette> {
        [Space]
        public bool GetCellDataFromScene = true;

        [Header("Actual values for brush")]
        public CellData DrawData;

        [Space]
        public EffectController SelectionColorVisualEffect;
        public EffectController SelectionFormVisualEffect;

        [Space]
        public List<CellLogic> ListLogicColor = new List<CellLogic>();
        public List<CellData> PaletteDataColor = new List<CellData>();
        public CellLogic SelectedCellLogicColor;
        public CellData SelectedCellDataColor;
        public CellStateColor SelectedCellStateColor;

        [Space]
        public List<CellLogic> ListLogicForm = new List<CellLogic>();
        public List<CellData> PaletteDataForm = new List<CellData>();
        public CellLogic SelectedCellLogicForm;
        public CellData SelectedCellDataForm;
        public CellStateFormT SelectedCellStateForm;



        private void Start() {
            if(SelectionColorVisualEffect == null)
                Debug.LogError("Effect isn't in scene and/or set in ManagerPalette", this);
            SelectionColorVisualEffect.gameObject.SetActive(false);

            if (GetCellDataFromScene) {
                RefreshPalette();

            }
            else if (false) { // 
                
            }
            else if (PaletteDataColor.Count == 0) {
                RefreshPalette();

            }

            if (ListLogicColor.Count > 0)
                SelectCellColor(ListLogicColor[0]);
            if (ListLogicForm.Count > 0)
                SelectCellForm(ListLogicForm[0]);
        }

        private void RefreshPalette() {
            ListLogicColor = ManagerGrids.I.GridPaletteVisuals.GetComponentsInChildren<CellLogic>().ToList();
            ListLogicColor.RemoveAll(x => x.GetComponent<PaletteInput>().Color == false);
            foreach (var logic in ListLogicColor)
                PaletteDataColor.Add(logic.Data);

            ListLogicForm = ManagerGrids.I.GridPaletteVisuals.GetComponentsInChildren<CellLogic>().ToList();
            ListLogicForm.RemoveAll(x => x.GetComponent<PaletteInput>().Form == false);
            foreach (var logic in ListLogicForm)
                PaletteDataForm.Add(logic.Data);
        }

        public void SelectCellColor(CellLogic logic) {
            if (!ListLogicColor.Contains(logic)) return;

            SelectedCellLogicColor = logic;
            SelectedCellDataColor = SelectedCellLogicColor.Data;
            SelectedCellStateColor = SelectedCellDataColor.StateColor;

            DrawData.StateColor = SelectedCellStateColor;

            VisualizeCellColor(logic);
        }

        private void VisualizeCellColor(CellLogic logic) {
            SelectionColorVisualEffect.transform.position = logic.transform.position;
            SelectionColorVisualEffect.gameObject.SetActive(true);

            SelectionColorVisualEffect.MoveEffects(logic.transform);
        }


        public void SelectCellForm(CellLogic logic) {
            if (!ListLogicForm.Contains(logic)) return;

            SelectedCellLogicForm = logic;
            SelectedCellDataForm = SelectedCellLogicForm.Data;
            SelectedCellStateForm = SelectedCellDataForm.CellStateFormT;

            DrawData.CellStateFormT = SelectedCellStateForm;

            VisualizeCellForm(logic);
        }
        private void VisualizeCellForm(CellLogic logic) {
            SelectionFormVisualEffect.transform.position = logic.transform.position;
            SelectionFormVisualEffect.gameObject.SetActive(true);

            SelectionFormVisualEffect.MoveEffects(logic.transform);
        }

        public CellData GetCurrentData() {
            return DrawData;
        }
    }
}