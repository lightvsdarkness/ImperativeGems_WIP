using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IG.General;

namespace IG.CGrid {
    public class ManagerChecks : SingletonManager<ManagerChecks> {
        [Header("Used for Score")]
        public float TotalEnergy; // TODO: Make it grow during round?
        [Space]
        //
        public List<CellLogic> CellsReceivedExternal = new List<CellLogic>();
        public List<CellLogic> CellsReceivedInternal = new List<CellLogic>();

        public List<CellLogic> CellsWithCorrectState = new List<CellLogic>();
        public List<CellLogic> CellsEnergized = new List<CellLogic>();
        //public List<CellLogic> CellsEnergizedThisRound = new List<CellLogic>();


        public event Action<float> OnPlayerDidSomethingGood;

        private ManagerCGridGame _managerCGridGame;


        // TODO: Replace with Check of range of cells being Energized
        public void PlayerDidSomethingGood(float howGood) {
            if (Debugging) Debug.Log("Adding Energy: " + howGood, this);

            OnPlayerDidSomethingGood.Invoke(howGood);
        }

        //protected override void Awake() {
        //    base.Awake();
        //}

        private void Start() {

            if (_managerCGridGame == null)
                _managerCGridGame = ManagerCGridGame.I;

        }


        public void StartTurn() {

            EnergyshouldFlow();
        }

        public void EnergyshouldFlow() {
            if (Debugging)
                Debug.Log("Clock at StartRound starts the CheckForMatchScript at: " + Time.realtimeSinceStartup + " CurrentTurn: " + ManagerClockScore.I.CurrentTurn);
            CellLogic cell = null;

            // First iteration по всем клеткам в этом ходу с целью логики обработки получения клетками EnergyExternal
            // Defining if received External Energy
            for (int i = 0; i < ManagerGrids.I.MatrixLength.x; i++) {
                for (int j = 0; j < ManagerGrids.I.MatrixLength.y; j++) {
                    cell = ManagerGrids.I.GridMain[i, j];

                    //// Cleaning stuff because new Round
                    //cell.CellNeighborsThatSentEnergy.Clear();
                    //cell.CellNeighborsThatDidntSentEnergy.Clear();
                    //foreach (var neighbor in cell.CellNeighbors) {
                    //    cell.CellNeighborsThatDidntSentEnergy.Add(neighbor);
                    //}

                    // Iterating Logic, to learn if it received external input
                    bool externalEnergyReceivedThisTurn = ManagerClockScore.I.CurrentTurn == cell.Data.EnergyReceived.LastTurnWhenEnergyExternal;
                    bool stateIsRight = cell.Data.StateColor == ManagerGrids.I.GridOriginal[i, j].Data.StateColor;

                    if (externalEnergyReceivedThisTurn && stateIsRight) {
                        //if(Debugging)
                        //Debug.Log("State is right! Main cell: " + cell.name+" " + cell.Data.State + " Original: " + cell.name + " " + ManagerGrids.I.GridOriginal[i, j].Data.State, this);

                        CellsReceivedExternal.Add(cell);
                    }
                }
            }

            // Second iteration only over cells that received External Energy so it could be propagated
            foreach (var cellReceived in CellsReceivedExternal) {
                float howMuchEnergyToEachNeighbor = 0f;

                if (cellReceived.Data.EnergyReceived.EnergyExcess < 0)
                    return;

                if (!CellsEnergized.Contains(cellReceived))
                        CellsEnergized.Add(cellReceived);

                    //cell.SpreadExcessenergy();
                    if (cellReceived.CellNeighbors.Count > 0)
                        howMuchEnergyToEachNeighbor = cellReceived.Data.EnergyReceived.EnergyExcess / cellReceived.CellNeighbors.Count;

                    // Because we distributed External Energy Excess, currently it's 0
                    cellReceived.Data.EnergyReceived.EnergyExcess = 0f;

                    foreach (var neighbor in cellReceived.CellNeighbors) {
                        var originalCell = ManagerGrids.I.GridOriginal[neighbor.Data.Row, neighbor.Data.Column];
                        bool stateIsRight = neighbor.Data.StateColor == originalCell.Data.StateColor;

                        if(stateIsRight)
                            neighbor.ReceiveEnergyInternal(howMuchEnergyToEachNeighbor, cellReceived, 0);
                    }

            }
            
        }

        /// <summary>
        /// Clear Lists at the start of Turn
        /// </summary>
        public void ClearInfoForNewRound() {
            TotalEnergy = 0;
            // 
            CellsReceivedExternal.Clear();
            CellsReceivedInternal.Clear();
            // 
            CellsWithCorrectState.Clear();
            CellsEnergized.Clear();

            //
            RefreshInfoListsCells();

            // Communicate new Score value
            ManagerClockScore.I.SetScoreFromGems(TotalEnergy);
        }

        public void RefreshInfoListsCells() {
            //CellsEnergized
            CellLogic cell = null;
            for (int i = 0; i < ManagerGrids.I.MatrixLength.x; i++) {
                for (int j = 0; j < ManagerGrids.I.MatrixLength.y; j++) {
                    cell = ManagerGrids.I.GridMain[i, j];

                    // Initial Energy Amount at the start of Round
                    TotalEnergy += Mathf.Clamp(cell.Data.EnergyCurrent, 0, cell.Data.CellStateFormT.EnergyMaxStored);

                    ClearCellData(cell);
                    ClearCellNeighbors(cell);

                    if(cell.Data.CellStateFormT.EnergyMaxStored <= cell.Data.EnergyCurrent)
                        CellsEnergized.Add(cell);

                    if (cell.Data.StateColor == ManagerGrids.I.GridOriginal[i, j].Data.StateColor)
                        CellsWithCorrectState.Add(cell);
                }
            }
        }

        public void ClearCellData(CellLogic cell) {
            cell.Data.EnergyReceived.EnergyExcess = 0f;
            cell.Data.EnergyReceived.EnergyFlowInExternal = 0f;
            cell.Data.EnergyReceived.EnergyFlowInInternal = 0f;
        }

        private void ClearCellNeighbors(CellLogic cell) {
            cell.CellNeighborsThatSentEnergy.Clear();

            cell.CellNeighborsThatDidntSentEnergy.Clear();
            foreach (var neighbor in cell.CellNeighbors) {
                cell.CellNeighborsThatDidntSentEnergy.Add(neighbor);
            }
        }

        public void CheckForMatchAll() {
            if (ManagerCGridGame.I.CGridGameMode == CGridGameMode.EnergyFlowSimple) {
                for (int x = 0; x < ManagerGrids.I.MatrixLength.x; x++) {
                    for (int y = 0; y < ManagerGrids.I.MatrixLength.y; y++) {
                        if (ManagerGrids.I.GridMain[x, y] == ManagerGrids.I.GridOriginal[x, y]) {
                            CellsWithCorrectState.Add(ManagerGrids.I.GridMain[x, y]);
                        }
                    }
                }
            }
        }


        public bool CheckForWin() {
            //if (ManagerCGridGame.I.CGridGameMode == CGridGameMode.FixMainGrid) {
            //    if (ManagerGrids.I.TotalTiles == ManagerChecks.I.CellsWithCorrectState.Count) {
            //        if (DebugLogging)
            //            Debug.LogWarning("Game Won!", this);
            //        return true;
            //    }  
            //}

            /// ---
            if (ManagerCGridGame.I.CGridGameMode == CGridGameMode.EnergyFlowSimple) {
                //
                if (ManagerGrids.I.TotalTiles == ManagerChecks.I.CellsEnergized.Count) {
                    if (DebugLogging)
                        Debug.LogWarning("Game Won!", this);
                    return true;
                }
                    
            }

            return false;
        }
    }
}