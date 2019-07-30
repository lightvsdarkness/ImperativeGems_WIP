using System.Collections;
using System.Collections.Generic;
using IG.General;
using UnityEngine;

namespace IG.CGrid {
    public class ManagerWaves : SingletonManager<ManagerWaves> {
        [Header("CoefficientForHowManyCellsInLineToWipe")]
        public float Coefficient = 0.3f;
        [Space]
        public int HowOftenToActInTurns = 5;
        public int TurnsWithoutActing = 0;

        [Space]
        public bool VerticalWave = true;
        public bool HorizontalWave = false;

        [Header("WaveVIsuals")]
        public float PauseBetweenVisualsAndAction = 0.3f;
        public float WaveTimeActive = 1;
        public float PauseBetweenLines = 0.1f; // A nice touch / polish
        protected WaitForSeconds WaitTimerPauseBetweenLines;
        public GameObject WaveVisuals;

        public Transform StartingTransform;
        public Transform EndingTransform;
        public Vector3 StartingPoint;
        public Vector3 EndingPoint;


        private void Start() {
            WaitTimer = new WaitForSeconds(PauseBetweenVisualsAndAction);
            WaitTimerPauseBetweenLines = new WaitForSeconds(PauseBetweenLines);

            ManagerClockScore.I.OnTurnStart += StartWave;
            WaveGameStart();
        }

        public void WaveGameStart() {
            VisualTurnWaveOff();
            TurnsWithoutActing = 0;
        }

        /// <summary>
        /// Change in cell Data if they are charged
        /// </summary>
        /// <param name="currentTurn"></param>
        public void StartWave(int currentTurn) {
            if (!ManagerCGridGame.I.LevelData.WaveIsOn) return;
            //Debug.LogError("TurnsWithoutActing: " + TurnsWithoutActing, this);

            TurnsWithoutActing++;
            if (TurnsWithoutActing < HowOftenToActInTurns)
                return;

            TurnsWithoutActing = 0;
            StartCoroutine(StartWaveActually());
        }

        private IEnumerator StartWaveActually() {
            VisualizeWave();

            yield return WaitTimer;

            CellLogic cell = null;
            if (VerticalWave)
            {
                for (int i = 0; i < ManagerGrids.I.MatrixLength.x; i++)
                {
                    int max = Random.Range(0, ManagerGrids.I.MatrixLength.y);
                    int coefficientedMax = (int) (max * Coefficient);

                    int howManyToWipe = Random.Range(0, coefficientedMax);
                    int tilesLeft = howManyToWipe;

                    float sumFromWiping = 0f;

                    // Now go over elements and change
                    for (int j = 0; j < ManagerGrids.I.MatrixLength.y; j++)
                    {
                        cell = ManagerGrids.I.GridMain[i, j];

                        if (Random.Range(0, 2) == 1)
                        {
                            tilesLeft--;
                            if (!(cell.Data.EnergyCurrent > 0)) continue;

                            ManagerClockScore.I.AddScoreBonusWave(cell.Data.EnergyCurrent);

                            //Debug.Log("sumFromWiing: " + sumFromWiping, this);
                            sumFromWiping += cell.Data.EnergyCurrent;

                            cell.WipeEnergy();
                        }
                    }
                    // TODO: TEMPORARILY CALLING A METHOD INSTEAD OF CHECK IN ManagerChecks
                    ManagerChecks.I.PlayerDidSomethingGood(sumFromWiping);

                    yield return WaitTimerPauseBetweenLines;
                }
            }
            else if (HorizontalWave)
            {

            }
        }

        public void VisualizeWave() {
            WaveVisuals.transform.position = StartingTransform.position;
            WaveVisuals.SetActive(true);

            //Debug.LogError("VisualizingWave at position: " + WaveVisuals.transform.position, this);

            Invoke("VisualTurnWaveOff", WaveTimeActive);
        }

        // !!! DON'T CHANGE NAME - Invoke!
        private void VisualTurnWaveOff() {
            WaveVisuals.SetActive(false);
        }
    }
}