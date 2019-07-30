using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IG.General;
using UnityEngine;

namespace IG.CGrid {
    public class CellAuVisual : MonoBehaviour {
        public bool UIIndication;
        [Space]
        public GameObject EffectTouched;
        public GameObject EffectReceivingExternal;
        public GameObject EffectCharging;
        public EffectParticles EffectEnergized;

        [Space]
        public Color ColorBackgroundDefault = new Color(0f, 0f, 0f, 0.8f);

        //
        [Space]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private SpriteRenderer _backgroundSpriteRenderer;

        //
        [Space]
        public CellLogic Logic;

        [Space]
        public GameObject ChargingEffectCurrent;
        public EffectParticles EnergizedEffectCurrent;

        [SerializeField] private List<UICellIndication> UIIndicators = new List<UICellIndication>();

        //private void Awake() {
        //}

        private void Start() {
            if (_spriteRenderer == null)
                _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (_spriteRenderer == null) Debug.LogError("Cell's _spriteRenderer is null", this);

            if (_backgroundSpriteRenderer == null)
                _backgroundSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (_backgroundSpriteRenderer == null) Debug.Log("Cell's _backgroundSpriteRenderer is null", this);

            if (Logic == null)
                Logic = gameObject.GetComponent<CellLogic>();

            if (EffectTouched == null) Debug.LogError("EffecTouched is null", this);
            if (EffectReceivingExternal == null) Debug.LogError("EffectReceivingExternal is null", this);
            if (EffectEnergized == null) Debug.LogError("EffectEnergized is null", this);
            if (EffectCharging == null) Debug.LogError("EffectCharging is null", this);

            // UI Indication
            if (UIIndication)
                UIIndicators = GetComponentsInChildren<UICellIndication>().ToList();

            Initialize();
        }

        private void Initialize() {
            if (UIIndication)
                IndicateValues();

            ClearAllEffects();
        }


        public void ShowClicked() {
            ManagerAuVisuals.I.PlayAudio();
            Instantiate(EffectTouched, transform.position, Quaternion.Euler(0.0f, 0.0f, 45.0f));
        }


        public void UpdateCellEndOfTurn(CellData cellData) {
            UpdateCellEnergyReceived(cellData);

            UpdateCellAuVisuals(cellData);
        }

        [ContextMenu("UpdateCellDataChanged")]
        public void UpdateCellAuVisuals(CellData cellData) {
            if (UIIndication)
                IndicateValues();

            UpdateCellDataChanged(cellData);


            // TODO: Make it a gradient
            // If Cell is energized, make background flashy and shiny and beatiful and gay (still straight ofc) and wonderful ^^
            if (cellData.EnergyCurrent > 0 && Mathf.Approximately(cellData.EnergyCurrent, cellData.CellStateFormT.EnergyMaxStored)) {
                // Background
                _backgroundSpriteRenderer.color = new Color(0.65f, 0.65f, 1f, 0.5f);

                //
                // If it's Energized but doesn't have Energized effect - add one
                if (EnergizedEffectCurrent == null) {
                    EnergizedEffectCurrent = Instantiate(EffectEnergized, transform.position, Quaternion.identity, this.transform);
                    EnergizedEffectCurrent.gameObject.SetActive(true);
                    EnergizedEffectCurrent.SlowlyStart();
                }
                else if (!EnergizedEffectCurrent.gameObject.activeSelf) {
                    EnergizedEffectCurrent.gameObject.SetActive(true);
                    EnergizedEffectCurrent.RestoreState();
                }
            }
            else {
                // Background
                _backgroundSpriteRenderer.color = ColorBackgroundDefault;

                // 
                //Debug.Log("cellData.EnergyCurrent: " + cellData.EnergyCurrent, this);
                //Debug.Log("Math.Abs(cellData.EnergyCurrent - cellData.CellStateFormT.EnergyMaxStored): " + Math.Abs(cellData.EnergyCurrent - cellData.CellStateFormT.EnergyMaxStored), this);
                //Debug.Log("Turning off EnergizedEffectCurrent of Cell: " + gameObject.name, this);
                EnergizedEffectCurrent?.gameObject.SetActive(false);
            }

        }

        protected void UpdateCellEnergyReceived(CellData cellData) {
            // In Cells Energy External stuff
            if (cellData.EnergyReceived.EnergyFlowInExternal > 0)
            {
                ChargingEffectCurrent = Instantiate(EffectCharging, transform.position, Quaternion.identity, this.transform);
                // TODO: Set Intensity to cellData.EnergyReceived.EnergyFlowInExternal

                ChargingEffectCurrent.GetComponent<EffectParticles>().SlowlyDestroy(1f);
            }

            // In Cells Energy Internal stuff
            if (cellData.EnergyReceived.EnergyFlowInInternal > 0)
            {
                ChargingEffectCurrent = Instantiate(EffectCharging, transform.position, Quaternion.identity, this.transform);
                // TODO: Set Intensity to cellData.EnergyReceived.EnergyFlowInInternal

                ChargingEffectCurrent.GetComponent<EffectParticles>().SlowlyDestroy(1f);
            }
        }


        public void VisualizeEnergyLost(CellData cellData) {
            if (UIIndication)
                IndicateValues();

            UpdateCellDataChanged(cellData);

            // If it should show losing Energy but doesn't have Energized effect - add one
            if (EnergizedEffectCurrent == null)
                EnergizedEffectCurrent = Instantiate(EffectEnergized, transform.position, Quaternion.identity, this.transform);

            EnergizedEffectCurrent.gameObject.SetActive(true);
            EnergizedEffectCurrent.ChangeColorAndTurnOff();
            //EnergizedEffectCurrent?.gameObject.SetActive(false);
        }

        [ContextMenu("VisualizeReceivingExternalEffect")]
        public void VisualizeReceivingExternalEffect(float forHowLong) {
            //
            Vector3 delta = Vector3.zero;

            if (Logic.Data.Row == ManagerGrids.I.MatrixLength.x - 1) //
                delta = new Vector3(0, -_spriteRenderer.bounds.size.y, 0);
            else if (Logic.Data.Column == ManagerGrids.I.MatrixLength.y - 1)
                delta = new Vector3(_spriteRenderer.bounds.size.x, 0, 0);
            else if (Logic.Data.Row == 0)
                delta = new Vector3(0, _spriteRenderer.bounds.size.y, 0);
            else if (Logic.Data.Column == 0)
                delta = new Vector3(-_spriteRenderer.bounds.size.x, 0, 0);
            else
            {
                Debug.LogError("It's not a border cell, can't receive external flow input", this);
                return;
            }

            var receivingPoint = transform.position + delta; //_spriteRenderer.bounds.size.x
            //Debug.Log("Spawning receiving effect. Transform of Cell: " + transform.position, this);
            //Debug.Log("Spawning receiving effect. delta: " + delta, this);
            //Debug.Log("Spawning receiving effect. forHowLong: " + forHowLong, this);

            var activeEffect = Instantiate(EffectReceivingExternal, receivingPoint, Quaternion.identity);
            //activeEffect.transform.parent = ManagerGrids.I.GridMainVisuals.transform;
            activeEffect.GetComponent<EffectParticles>().SlowlyDestroy(forHowLong);
        }

        public void UpdateCellDataChanged(CellData cellData) {
            if (_spriteRenderer.color != cellData.StateColor.ColorData.Color)
                _spriteRenderer.color = cellData.StateColor.ColorData.Color;
            if (_spriteRenderer.sprite != cellData.CellStateFormT.Sprite)
                _spriteRenderer.sprite = cellData.CellStateFormT.Sprite;
        }

        public void IndicateValues() {
            foreach (var uiIndicator in UIIndicators) {
                if (Logic == null)
                    Logic = gameObject.GetComponent<CellLogic>();
                uiIndicator.Indicate(Logic);
            }
        }

        // TODO: Pool manager
        public void ClearAllEffects() {
            Destroy(ChargingEffectCurrent);
            //Destroy(EnergizedEffectCurrent);
        }

        public void VisualizeInternalEnergyTransfer(CellLogic otherCell) {
            
        }


        //public void ShowInactive() {
        //    _spriteRenderer.color = Logic.Data.State.ColorData.Color;    //ManagerGrids.I.CSData.ColorButtonNotClicked;
        //}
    }
}