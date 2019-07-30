using System.Collections;
using System.Collections.Generic;
using IG.General;
using UnityEngine;

namespace IG.CGrid {
    public class ManagerExternalForces : SingletonManager<ManagerExternalForces> {
        [Space]
        public bool RandomPereskok;
        [Space]
        public float EnergyEachTick = 1f;
        //public float EnergyHowLong = 3f;
        //public float WaitTime = 3;


        private int matrixLengthX;
        private int matrixLengthY;

        protected Coroutine RoutineExternalEnergy;
        protected bool Energizing;
        //protected Coroutine RoutineRow;
        //protected Coroutine RoutineColumn;


        private void Start() {
            WaitTimer = new WaitForSeconds(ManagerClockScore.I.SecondsInTurn);
            
        }
        //private void Update() {
        //}


        // Must be 100% sure that it executes earlier than startRound
        public void StartExternalEnergyFlow() {
            if (RoutineExternalEnergy == null)
                RoutineExternalEnergy = StartCoroutine(EnergyFlow());
            else {
                Debug.LogError("Starting second Coroutine while first still active", this);
            }
        }

        public void StopExternalEnergyFlow() {
            if (RoutineExternalEnergy != null)
                StopCoroutine(RoutineExternalEnergy);
            RoutineExternalEnergy = null;
            //StopCoroutine("AffectColumn");
            //StopCoroutine("AffectRow");
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator EnergyFlow() {
            //if (ManagerClockScore.I.enabled)
            //    yield return new WaitForSeconds(ManagerClockScore.I.SecondsBeforeStart);
            //Debug.Log("ManagerExternalForces starts the Force at: " + Time.realtimeSinceStartup + " CurrentTurn: " + ManagerClockScore.I.CurrentTurn);

            Energizing = true;
            // If we are starting, let's randomize a bit, otherwise - let's not
            bool flagFirstCycle = true;
            int i, j = 0;
            matrixLengthX = ManagerGrids.I.MatrixLength.x - 1;
            matrixLengthY = ManagerGrids.I.MatrixLength.y - 1;

            while (Energizing) {
                if (flagFirstCycle) { // If we are starting, let's randomize a bit, otherwise - let's not
                    j = RandomEasedMax(0, matrixLengthY);
                    i = RandomEasedMax(0, matrixLengthX);
                    flagFirstCycle = false;
                }

                // Started from Up
                if (Random.value > 0.5f) {
                    i = 0;
                    yield return AffectRow(i, j, true);
                }
                // Started from Down
                else {
                    i = matrixLengthX;
                    yield return AffectRow(i, j, false);
                }

                // Started from Up - Continue with Right
                if (i == 0) {
                    j = matrixLengthY;
                    if (RandomPereskok)
                        i = RandomEasedMax(0, matrixLengthX);
                    else
                        i = 0;
                    yield return AffectColumn(j, i, true);
                }
                // Started from Down - Continue with Left
                else {
                    j = 0;
                    if (RandomPereskok)
                        i = RandomEasedMax(0, matrixLengthX);
                    else
                        i = matrixLengthX;
                    yield return AffectColumn(j, i, false);
                }

                // Continued with Right - Continue with Down
                if (j == matrixLengthY) {
                    i = matrixLengthX;
                    if (RandomPereskok)
                        j = RandomEasedMax(0, matrixLengthY);
                    else 
                        j = matrixLengthY;
                    yield return AffectRow(i, j, false);
                }
                // Continued with Left - Continue with Up
                else {
                    i = 0;
                    if (RandomPereskok)
                        j = RandomEasedMax(0, matrixLengthY);
                    else
                        j = 0;
                    yield return AffectRow(i, j, true);
                }

                // Continued with Down - Continue with Left
                if (i == matrixLengthX) {
                    j = 0;
                    if (RandomPereskok)
                        i = RandomEasedMax(0, matrixLengthX);
                    else
                        i = matrixLengthX;
                    yield return AffectColumn(j, i, false);
                }
                // Continued with Up - Continue with Right
                else {
                    j = matrixLengthY;
                    if (RandomPereskok)
                        i = RandomEasedMax(0, matrixLengthX);
                    else
                        i = 0;
                    yield return AffectColumn(j, i, true);
                }
            }
        }

        private IEnumerator AffectRow(int rowIndex, int startingColumn, bool isClockwise) {
            if (isClockwise) {
                for (int i = startingColumn; i <= matrixLengthY; i++) {
                    //if (!Energizing)
                        //yield return ;
                    //Debug.Log("Sending energy, X = " + rowIndex + " Y = " + i + " Clockwise: " + isClockwise, this);
                    ManagerChecks.I.ClearInfoForNewRound();

                    // We send energy anyway, if it's received - depends on CellLogic & ManagerChecks logic
                    ManagerGrids.I.GridMain[rowIndex, i].TryReceiveEnergyExternal(EnergyEachTick, ManagerClockScore.I.SecondsInTurn);
                    yield return WaitTimer;
                }
            }
            else {
                for (int i = startingColumn; i >= 0; i--) {
                    ManagerChecks.I.ClearInfoForNewRound();

                    // We send energy anyway, if it's received - depends on CellLogic & ManagerChecks logic
                    ManagerGrids.I.GridMain[rowIndex, i].TryReceiveEnergyExternal(EnergyEachTick, ManagerClockScore.I.SecondsInTurn);
                    yield return WaitTimer;
                }
            }
        }

        private IEnumerator AffectColumn(int columnIndex, int startingRow, bool isClockwise) {
            if (isClockwise) {
                for (int i = startingRow; i <= matrixLengthX; i++) {
                    //Debug.Log("Sending energy, X = " + i + " Y = " + columnIndex + " Clockwise: " + isClockwise, this);
                    ManagerChecks.I.ClearInfoForNewRound();

                    // We send energy anyway, if it's received - depends on CellLogic & ManagerChecks logic
                    ManagerGrids.I.GridMain[i, columnIndex].TryReceiveEnergyExternal(EnergyEachTick, ManagerClockScore.I.SecondsInTurn);
                    yield return WaitTimer;
                }
            }
            else {
                for (int i = startingRow; i >= 0; i--) {
                    ManagerChecks.I.ClearInfoForNewRound();

                    // We send energy anyway, if it's received - depends on CellLogic & ManagerChecks logic
                    ManagerGrids.I.GridMain[i, columnIndex].TryReceiveEnergyExternal(EnergyEachTick, ManagerClockScore.I.SecondsInTurn);
                    yield return WaitTimer;
                }
            }
        }

        private int RandomEasedMax(int minValue, int maxValue, float maxPercent = 0.7f) {
            float result = (float) Random.Range(0, matrixLengthX)*maxPercent;

            return (int) result;
        }
    }
}