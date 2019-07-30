using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace IG.CGrid {
    public class CellLogic : SerializedMonoBehaviour {
        //
        public CellData Data;
        public string CellStateColorName;   //Debugging
        public CellAuVisual AuVisuals;
        public CellBehaviour CellBehaviour;

        public List<CellLogic> CellNeighbors = new List<CellLogic>();
        public List<CellLogic> CellNeighborsThatSentEnergy = new List<CellLogic>();
        public List<CellLogic> CellNeighborsThatDidntSentEnergy = new List<CellLogic>();

        public static event Action<CellLogic> OnCellClicked;

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

        public void CreateNeisghborList(CellLogic[,] grid) {
            int rowIndex = Data.Row;
            int columnIndex = Data.Column;
            bool topRow = rowIndex == 0;
            bool bottomRow = rowIndex == grid.GetLength(0) - 1;
            bool leftColumn = columnIndex == 0;
            bool rightColumn = columnIndex == grid.GetLength(1) - 1;
            
            if (topRow && leftColumn) {
                CellNeighbors.Add(grid[rowIndex + 1, columnIndex]);
                CellNeighbors.Add(grid[rowIndex, columnIndex + 1]);
            }
            else if (topRow && rightColumn) {
                CellNeighbors.Add(grid[rowIndex + 1, columnIndex]);
                CellNeighbors.Add(grid[rowIndex, columnIndex - 1]);
            }

            else if (bottomRow && leftColumn) {
                CellNeighbors.Add(grid[rowIndex - 1, columnIndex]);
                CellNeighbors.Add(grid[rowIndex, columnIndex + 1]);
            }
            else if (bottomRow && rightColumn) {
                CellNeighbors.Add(grid[rowIndex - 1, columnIndex]);
                CellNeighbors.Add(grid[rowIndex, columnIndex - 1]);
            }

            else if(topRow) {
                CellNeighbors.Add(grid[rowIndex, columnIndex - 1]);
                CellNeighbors.Add(grid[rowIndex, columnIndex + 1]);
                CellNeighbors.Add(grid[rowIndex + 1, columnIndex]);
            }
            else if (bottomRow) {
                CellNeighbors.Add(grid[rowIndex, columnIndex - 1]);
                CellNeighbors.Add(grid[rowIndex, columnIndex + 1]);
                CellNeighbors.Add(grid[rowIndex - 1, columnIndex]);
            }
            else if (leftColumn) {
                CellNeighbors.Add(grid[rowIndex - 1, columnIndex]);
                CellNeighbors.Add(grid[rowIndex + 1, columnIndex]);
                CellNeighbors.Add(grid[rowIndex, columnIndex + 1]);
            }
            else if (rightColumn) {
                CellNeighbors.Add(grid[rowIndex - 1, columnIndex]);
                CellNeighbors.Add(grid[rowIndex + 1, columnIndex]);
                CellNeighbors.Add(grid[rowIndex, columnIndex - 1]);
            }
            else {
                CellNeighbors.Add(grid[rowIndex - 1, columnIndex]);
                CellNeighbors.Add(grid[rowIndex + 1, columnIndex]);
                CellNeighbors.Add(grid[rowIndex, columnIndex - 1]);
                CellNeighbors.Add(grid[rowIndex, columnIndex + 1]);
            }
        }

        public void InstallCellData(CellData cellData) {
            Data = cellData;
            AuVisuals.UpdateCellAuVisuals(Data);
        }

        public void RespondToInput() {
            CellBehaviour?.RespondToInput(this);
            OnCellClicked?.Invoke(this);

            AuVisuals.UpdateCellAuVisuals(Data);
        }
        

        /// <summary>
        /// Checks for identical states with original Grid
        /// </summary>
        /// <param name="energy"></param>
        /// <param name="howLong"></param>
        public void TryReceiveEnergyExternal(float energy, float howLong) {
            AuVisuals.VisualizeReceivingExternalEffect(howLong);

            bool stateIsright = Data.StateColor == ManagerGrids.I.GridOriginal[Data.Row, Data.Column].Data.StateColor;
            if (stateIsright)
                ReceiveEnergyExternal(energy, howLong);

            // TODO: Effect for not receiving energy
        }

        /// <summary>
        /// Cell receives External Energy
        /// </summary>
        /// <param name="energy"></param>
        /// <param name="howLong"></param>
        public void ReceiveEnergyExternal(float energy, float howLong) {
            if (energy < 0.00000f) {
                Debug.Log("Not really receiving energy", this);
                return;
            }
            //Debug.Log("ManagerExternalForces starts the Force at: " + Time.realtimeSinceStartup + " CurrentTurn: " + ManagerClockScore.I.CurrentTurn);

            //Debug.Log("Receiving External energy, X = " + Data.Row + " Y = " + Data.Column + " at: " + Time.realtimeSinceStartup + " CurrentTurn: " + ManagerClockScore.I.CurrentTurn, this);
            if (ManagerChecks.I.Debugging)
                Debug.Log(this.gameObject.name + " receiving External Energy " + " at: " + Time.realtimeSinceStartup + " Current Turn: " + ManagerClockScore.I.CurrentTurn, this);

            Data.ReceiveEnergyExternal(energy);


            AuVisuals.UpdateCellAuVisuals(Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="howMuchEnergyFromNeighbor"></param>
        /// <param name="neighbor"></param>
        /// <param name="chainLength">How far from Cell received External Energy this Cell located</param>
        public void ReceiveEnergyInternal(float howMuchEnergyFromNeighbor, CellLogic neighbor, int chainLength) {
            if (howMuchEnergyFromNeighbor < 0.00000f) {
                Debug.Log("Not really receiving energy", this);
                return;
            }
            float howMuchEnergyToEachNeighbor = 0f;
            chainLength++;

            // We received energy internal
            if (ManagerChecks.I.Debugging)
                Debug.Log(this.gameObject.name + " receiving Internal Energy from " + neighbor.gameObject.name + " at: " + Time.realtimeSinceStartup + " Current Turn: " + ManagerClockScore.I.CurrentTurn + " Chain length: " + chainLength, this);

            Data.ReceiveEnergyInternal(howMuchEnergyFromNeighbor);

            CellNeighborsThatSentEnergy.Add(neighbor);  // maintaining List
            ManagerChecks.I.CellsReceivedInternal.Add(this); // maintaining List

            //CellNeighborsThatDidntSentEnergy = CellNeighbors.Except(CellNeighborsThatSentEnergy).ToList();
            CellNeighborsThatDidntSentEnergy.RemoveAll(x => CellNeighborsThatSentEnergy.Contains(x));

            // If we didn't yet received more energy than we can hold - do nothing else
            if (Data.EnergyReceived.EnergyExcess <= 0)
                return;

            if (!ManagerChecks.I.CellsEnergized.Contains(this)) // maintaining List
                ManagerChecks.I.CellsEnergized.Add(this);

            // We received energy and if cell is too far away from original energy source - stop sending energy through cell chain
            if (ManagerCGridGame.I.ActualEnergyChainLength == chainLength) // <=
                return;


            // --- Sending energy through chain
            if (CellNeighborsThatDidntSentEnergy.Count > 0) { 
                howMuchEnergyToEachNeighbor = Data.EnergyReceived.EnergyExcess / CellNeighborsThatDidntSentEnergy.Count;

                // GAME DESIGN DECISION: we split energy among all neighbors, but send it only to the ones that are able to receive due to State
                //foreach (var otherNeighbor in CellNeighborsThatDidntSentEnergy) {
                //    bool stateIsRight = otherNeighbor.Data.State ==
                //                        ManagerGrids.I.GridOriginal[otherNeighbor.Data.Row, otherNeighbor.Data.Column].Data.State;
                //    if(stateIsRight)
                //        otherNeighbor.ReceiveEnergyInternal(howMuchEnergyToEachNeighbor, this, chainLength++);
                //}

                // Copy for this cycle
                List<CellLogic> list = new List<CellLogic>(CellNeighborsThatDidntSentEnergy);

                while (list.Count > 0) {
                    bool stateIsRight = list[0].Data.StateColor == ManagerGrids.I.GridOriginal[list[0].Data.Row, list[0].Data.Column].Data.StateColor;
                    if (stateIsRight)
                        list[0].ReceiveEnergyInternal(howMuchEnergyToEachNeighbor, this, chainLength);
                    list.RemoveAt(0);
                }
            }
        }

        public void WipeEnergy() {
            Data.EnergyCurrent = 0f;

            Data.RandomizeState();
            AuVisuals.VisualizeEnergyLost(Data);
        }

        public void Clear() {
            AuVisuals.ClearAllEffects();

            CellBehaviour.Reset(this);

            //Destroy(Data);
            //if (Data != null)
            //    Debug.LogError("Scriptable Object isn't null" + Data, this);
        }
    }
}