using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IG.CGrid {

    [Serializable]
    public struct EnergyReceived {
        public EnergyType EnergyType;

        public int LastTurnWhenEnergyExternal;
        public float EnergyFlowInExternal;
        public float EnergyExcess;
        public float EnergyFlowInInternal;
    }

    [CreateAssetMenu(fileName = "CellData", menuName = "Scriptable Objects/!Cell/CellData")]
    public class CellData : ScriptableObject, ICloneable {
        public CellStateColor StateColor;
        //public CellStateColor ImprovedStateColor;

        [Space]
        public CellStateFormT CellStateFormT;
        public float EnergyCurrent;

        [Space]
        public EnergyReceived EnergyReceived;

        [Space]
        public int Row;
        public int Column;


        //protected virtual void Start() {
        
        //}
        //protected virtual void Update() {
        
        //}
        public virtual void Construct(int row, int column) {    //, float energyMax
            Row = row;
            Column = column;
            if (ManagerCGridGame.I.FixedDataCellsImage) {

            }
            else {
                if(ManagerCGridGame.I.DataCellsStates.CellStatesColor != null) {
                    //Debug.Log("Current CellStates: " + ManagerCGridGame.I.DataCellsStates.CellStates);
                    StateColor = ManagerCGridGame.I.DataCellsStates.CellStatesColor.FirstOrDefault();

                    CellStateFormT = ManagerCGridGame.I.DataCellsStates.CellStatesFormT.FirstOrDefault();
                }
            }
                
        }

        //public virtual object Clone() {
        //    return new CellData { StateColor = this.StateColor, CellStateFormT = this.CellStateFormT, EnergyReceived = this.EnergyReceived, Row = this.Row, Column = this.Column };
        //}
        public virtual object Clone() {
            var cellData = CellData.CreateInstance<CellData>();
            cellData.StateColor = this.StateColor;
            cellData.CellStateFormT = this.CellStateFormT;
            cellData.EnergyReceived = this.EnergyReceived;
            cellData.Row = this.Row;
            cellData.Column = this.Column;

            return cellData;
        }

        public void ReceiveEnergyExternal(float addEnergy) {
            EnergyCurrent += addEnergy;

            EnergyReceived.EnergyFlowInExternal += addEnergy;
            EnergyReceived.LastTurnWhenEnergyExternal = ManagerClockScore.I.CurrentTurn;
            EnergyReceived.EnergyExcess = Mathf.Clamp(EnergyCurrent - CellStateFormT.EnergyMaxStored, 0, float.MaxValue);

            // Because we already store excess energy in EnergyReceived.EnergyExcess, clamp EnergyCurrent
            EnergyCurrent = Mathf.Clamp(EnergyCurrent, 0, CellStateFormT.EnergyMaxStored);
        }

        public void ReceiveEnergyInternal(float howMuchEnergyFromNeighbor) {
            EnergyCurrent += howMuchEnergyFromNeighbor;

            EnergyReceived.EnergyFlowInInternal += howMuchEnergyFromNeighbor;
            EnergyReceived.EnergyExcess = Mathf.Clamp(EnergyCurrent - CellStateFormT.EnergyMaxStored, 0, float.MaxValue);

            // Because we already store excess energy in EnergyReceived.EnergyExcess, clamp EnergyCurrent
            EnergyCurrent = Mathf.Clamp(EnergyCurrent, 0, CellStateFormT.EnergyMaxStored);
        }

        public virtual void SetState(CellStateColor cellStateColor) {
            StateColor = cellStateColor;
        }

        /// <summary>
        /// Only Color for now, as there is only 2 states
        /// </summary>
        public virtual void RandomizeState() {
            int currentIndex = ManagerCGridGame.I.DataCellsStates.CellStatesColor.FindIndex(x => x == StateColor);
            int randomIndex = UnityEngine.Random.Range(0, ManagerCGridGame.I.DataCellsStates.CellStatesColor.Count);
            while (currentIndex == randomIndex) {
                randomIndex = UnityEngine.Random.Range(0, ManagerCGridGame.I.DataCellsStates.CellStatesColor.Count);
            }
            StateColor = ManagerCGridGame.I.DataCellsStates.CellStatesColor[randomIndex];
        }
        
    }
}