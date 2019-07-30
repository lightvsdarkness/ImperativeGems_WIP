using System;
using UnityEngine;
using System.Collections;
using IG.General;

namespace IG.CGrid {

    [Serializable]
    public class ScoreDetails {
        public int ScoreTotal {
            get { return ScoreFromGems + ScoreFromGems; }
        }

        public int ScoreFromGems;
        public int ScoreBonusWave;
        public int ScoreBonusOther;

        public int CurrentTurnScore;

        public void ClearScore() {
            ScoreFromGems = 0;
            ScoreBonusWave = 0;
            ScoreBonusOther = 0;

            CurrentTurnScore = 0;
        }
    }

    /// <summary>
    /// Counting time for ManagerCGRPGame
    /// </summary>
    public class ManagerClockScore : SingletonManager<ManagerClockScore> {
        [Space]
        [SerializeField] private int _currentTurn = 1; // So when game starts it increases to 1 == first Turn
        public int CurrentTurn { get { return _currentTurn; } private set { _currentTurn = value; } }

        [Header("Set Initial Amount of Turns")]
        public int TurnsInitialAmount = 20;
        public int TurnsLeft;


        [Space]
        public int SecondsBeforeStart = 3;
        public int SecondsBeforeStartLeft = 3;
        [Space]
        public float SecondsInTurn = 1f;

        [Header("Score Bonuses")]
        public int TurnsToAddWhenDoingGood = 2; // TODO: For incrementing turns
        public float WaveScoreCoefficient = 0.3f;

        [Space]
        public ScoreDetails ScoreInfo;

        //[SerializeField] private int _totalScore;
        //public int ScoreBonusAmount;
        //public int ScoreTotal {
        //    get { return ScoreDetails.ScoreFromGems + ScoreDetails.ScoreFromGems; }
        //}


        private WaitForSeconds _waitTimer;
        protected Coroutine RoutineCountdown;

        [Space]
        [SerializeField] private UIClock UIClock;
        private DataColorScheme CSData;


        //private bool _firstRound = false; // dirty fix for first round because external forces start before rounds start. so, we fit both in 1st round

        public Action<int> OnTurnStart;
        public Action<ScoreDetails> OnScoreChanged;
        public Action<int> OnTimerChanged;


        protected void Start() {

            _waitTimer = new WaitForSeconds(SecondsInTurn);
            CSData = ManagerCGridGame.I.ColorScheme;

            if (UIClock == null)
                UIClock = gameObject.GetComponent<UIClock>();

            ManagerChecks.I.OnPlayerDidSomethingGood += OnPlayerDidSomethingVeryGood;
        }


        public void StartGame() {
            if (CSData != null)
                Camera.main.backgroundColor = CSData.RuntimeCurrentColorBackground;

            StopCountdown();

            ScoreInfo.ClearScore();
        }


        public void UIShowTimeBeforeStart() {
            if (Debugging) Debug.Log("UIClock.UIShowTimeBeforeStart", this);
            UIClock.UIShowTimeBeforeStart(SecondsBeforeStartLeft);
        }

        public void StartCountdown() {
            //_firstRound = true;
            UIClock.UIAudioVisuals(TurnsLeft);

            if (RoutineCountdown == null)
                RoutineCountdown = StartCoroutine(CountDown());
        }
        public void StopCountdown() {
            if (RoutineCountdown != null)
                StopCoroutine(RoutineCountdown);

            RoutineCountdown = null;
        }

        /// <summary>
        /// Countdown until game turns run out
        /// </summary>
        private IEnumerator CountDown() {
            while (TurnsLeft > 0) {
                // ALL THE TURN STUFF
                ManagerCGridGame.I.TurnStart();

                // Additional stuff for turn
                OnTurnStart?.Invoke(CurrentTurn);

                // TURN END
                CurrentTurn++;
                TurnsLeft--;

                // UI & AudioVisuals for Turn End
                UIClock.UIAudioVisuals(TurnsLeft);

                yield return _waitTimer;
            }
            
            ManagerCGridGame.I.GameLostUnlessAd(ScoreInfo.ScoreTotal);    // When Time is up - game is lost
        }

        private void AddTurns(int turnsToAdd) {
            TurnsLeft += turnsToAdd;
            OnTimerChanged?.Invoke(TurnsLeft);
        }


        // TODO: 
        public void OnPlayerDidSomethingVeryGood(float howGood) {
            int timeToAdd = (int) (howGood * WaveScoreCoefficient);
            if (Debugging) Debug.Log("Adding time: " + timeToAdd, this);

            AddTurns(timeToAdd);
        }


        public void SetScoreFromGems(float score) {
            //CurrentTurnScore = ;
            ScoreInfo.ScoreFromGems = (int) score;

            OnScoreChanged?.Invoke(ScoreInfo);
        }

        public void AddScoreBonusFromRowsOrColumns(int cellsEnergizedInRowOrColumn) {
            ScoreInfo.ScoreBonusOther += cellsEnergizedInRowOrColumn * 2;

            OnScoreChanged?.Invoke(ScoreInfo);
        }

        public void AddScoreBonusWave(float scoreToAdd) {
            ScoreInfo.ScoreBonusWave += (int) scoreToAdd;

            OnScoreChanged?.Invoke(ScoreInfo);
        }

        public int GetHighScore() {
            return ScoreInfo.ScoreTotal > ManagerSaveLoad.I.Data.HighScore ? ScoreInfo.ScoreTotal : ManagerSaveLoad.I.Data.HighScore;
        }
    }
}