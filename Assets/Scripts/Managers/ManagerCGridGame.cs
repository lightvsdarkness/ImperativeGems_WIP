using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IG.General;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

namespace IG.CGrid {
    public enum CGridGameMode {
        EnergyFlowSimple,
        EnergyFlowPalette,
        MainMenu,
        GameOver
    }

    public class ManagerCGridGame : SingletonManager<ManagerCGridGame> {
        [SerializeField] private CGridGameMode _cGridGameMode;
        public CGridGameMode CGridGameMode {
            get { return _cGridGameMode; }
            set { _cGridGameMode = value;
                  SetActiveCellBehaviour(); }
        }
        public bool FixedDataCellsImage = false;    // If we use some sort of image for OriginalGrid
        public bool Monetizing;

        [Space]
        public DataCampaign CampaignData;
        public DataLevel LevelData;

        public int ActualEnergyChainLength {
            get { return LevelData.DefaultEnergyChainLength + BonusEnergyChainLength; }
        }
        public int BonusEnergyChainLength = 0;

        // 
        [Space]
        public bool StartPlayingOnStart;
        public bool GamePlaying;

        [Space]
        public RewardVideoAd RewardVideoAd;

        [Space]
        public ManagerUI UIGeneral;

        [Space]
        public DataCellStates DataCellsStates;
        public DataColorScheme ColorScheme; // Colors

        [Header("Resources paths")]
        public string CGridPrefabsPath = "Prefabs";
        public string CGridDataPath = "Data";
        public string DefaultNameColorScheme = "DefaultColorScheme";
        public string DefaultNameCellStates = "DefaultCellStates";

        [Space]
        public CellBehaviour CellBehaviourActive;
        public List<CellBehaviour> CellBehaviours;

        [Space]
        public Action OnGameStart; //<CGridGameMode>
        public Action OnGameEnd;

        private WaitForSeconds _waitTimer = new WaitForSeconds(1f);


        protected override void Awake() {
            base.Awake();

            if (ColorScheme == null)
                ColorScheme = (DataColorScheme)Resources.Load(ManagerCGridGame.I.CGridDataPath + "/" + DefaultNameColorScheme, typeof(ScriptableObject));
            if (ColorScheme == null) Debug.LogError("Set DataColorScheme", this);

            if (DataCellsStates == null)
                DataCellsStates = (DataCellStates)Resources.Load(ManagerCGridGame.I.CGridDataPath + "/" + DefaultNameCellStates, typeof(ScriptableObject));
            if (DataCellsStates == null) Debug.LogError("Set DataCellsStates", this);

            if (CampaignData == null) { 
                Debug.LogError("No CampaignData set", this);
                CampaignData = Resources.FindObjectsOfTypeAll<DataCampaign>().FirstOrDefault();
            }
            LevelData = CampaignData.LevelsData[CampaignData.CurrentLevel];
            //if (LevelData == null) {
            //    Debug.LogError("No DataLevel set. Creating new one", this);
            //    LevelData = DataLevel.CreateInstance<DataLevel>();
            //}

        }

        private void Start() {
            if (UIGeneral == null) Debug.LogError("Set UIGeneral", this);

            if (StartPlayingOnStart)
                GameStart();
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                GamePauseSwitch();
            }
        }

        /// <summary>
        /// Creates Grids, does all the Checking stuff and then starts External Forces
        /// </summary>
        public void GameStart() {
            // Managers initialization
            ManagerClockScore.I.StartGame();

            // Game mode chooses Active CellBehaviour
            SetActiveCellBehaviour();

            // 
            ManagerSaveLoad.I.Load();

            // TODO: Probably should remove those from GameLost
            ManagerExternalForces.I.StopExternalEnergyFlow(); // Before ManagerGrids.I.Clear
            ManagerGrids.I.Clear();                             // After ManagerExternalForces.I.StopExternalEnergyFlow

            // Game setup
            ManagerGrids.I.StartGame(CellBehaviourActive);
            ManagerChecks.I.CheckForMatchAll();   // At the end of turn should check if cells changed states
            ManagerWaves.I.WaveGameStart();

            Time.timeScale = 1;

            // UI and stuff
            ManagerUI.I.UIGameStart();

            ManagerMusic.I.PlayMusic();
            OnGameStart?.Invoke();

            StartCoroutine(WaitBeforeStartPlaying());  // Waiting time for processes to finish and player to get ready
        }

        // TODO: Probably should relocate to ManagerClockScore
        private IEnumerator WaitBeforeStartPlaying() {
            ManagerClockScore.I.SecondsBeforeStartLeft = ManagerClockScore.I.SecondsBeforeStart;
            ManagerClockScore.I.TurnsLeft = ManagerClockScore.I.TurnsInitialAmount;

            while (ManagerClockScore.I.SecondsBeforeStartLeft > 0) {

                ManagerClockScore.I.UIShowTimeBeforeStart();
                ManagerClockScore.I.SecondsBeforeStartLeft--;

                yield return _waitTimer;
            }
            ManagerClockScore.I.UIShowTimeBeforeStart();

            GamePlaying = true;
            // ACHTUNG!
            // NOTE: The order is super-important: first start of ExternalForces (clean data and do ExternalEnergy), then TurnStart from Countdown
            ManagerExternalForces.I.StartExternalEnergyFlow();  // Start external forces
            ManagerClockScore.I.StartCountdown();            // And finally start game clock
        }

        public void TurnStart() {
            if(ManagerChecks.I.CheckForWin())
                GameWon();

            //ManagerGrids.I.StartTurn(); // Removed as it's basically only installer and holder now
            //ManagerExternalForces.I.StartTurn(); // It's independent for now
            ManagerChecks.I.StartTurn();
            ManagerAuVisuals.I.UpdateAuVisualsEndOfTurn();
        }

        public void GamePauseSwitch() {
            if (Debugging) Debug.Log("PauseSwitch", this);

            GamePlaying = !GamePlaying;

            if (GamePlaying) {
                Time.timeScale = 1f;
                ManagerMusic.I.OnGameUnPaused();
            }
            else {
                Time.timeScale = 0f;
            }
        }

        // Called by UI button
        public void PlayerPause() {
            GamePauseSwitch();

            UIGeneral.UIPauseSwitch();
        }

        // Called by UI button
        public void PlayerHelpInitial() {
            if (Debugging) Debug.Log("HelpInitial", this);

            UIGeneral.UIHelpSwitch();
        }
        //public void PlayerHelpNext() {
        //    if (Debugging) Debug.Log("HelpSwitch", this);
        //    if () { 
        //    UIGeneral.UIHelpNext();
        //    }
        //    else {
        //        GamePauseSwitch();
        //    }
        //}

        public void GameWon() {
            if (DebugLogging) Debug.Log("Game Won", this);

            // Show UI UIGameOver / ModalWindow For NextLevel or MainMenu

        }

        public void MainMenu() {
            if (DebugLogging) Debug.Log("MainMenu", this);
            Time.timeScale = 1;
            SceneManager.LoadScene(1);
        }

        public void GameLostUnlessAd(int score) {
            if (DebugLogging) Debug.Log("Game Lost", this);
            
            GamePlaying = false;
            Time.timeScale = 0;

            // Show UIGameOver OR show ModalWindow For Advertisement

            //if (Monetizing) {
            //    if (!RewardVideoAd.CheckIfWantsToSeeAd()) {
            //        GameLostForSure(score);
            //    }
            //    else {
            //        GamePlaying = true;
            //    }
            //}
            //else {
            GameLostForSure(score);
            //}
        }

        private void GameLostForSure(int score) {
            ManagerSaveLoad.I.Save(score);
            ManagerExternalForces.I.StopExternalEnergyFlow(); // Before ManagerGrids.I.Clear
            //ManagerGrids.I.Clear();                             // After ManagerExternalForces.I.StopExternalEnergyFlow
            ManagerUI.I.UIGameLost();
            Time.timeScale = 0;
        }


        public void PlayAgainWithSceneLoad() {
            if (DebugLogging) Debug.Log("Game Over - PlayAgain", this);
            Time.timeScale = 1;
            SceneManager.LoadScene(2);
        }

        private void SetActiveCellBehaviour() {
            if (CellBehaviours.Count == 0)
                Debug.LogError("No CellBehaviours set in Main Manager", this);
            CellBehaviourActive = CellBehaviours[(int)_cGridGameMode];
        }
    }
}