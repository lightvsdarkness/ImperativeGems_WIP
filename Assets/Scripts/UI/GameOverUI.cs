using UnityEngine;
using System.Collections;
using IG.General;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace IG.CGrid {
    public class GameOverUI : MonoBehaviour {
        public Canvas CanvasGamePlay;
        public Canvas CanvasGameOverUI;
        
        [Space]
        [SerializeField] private Text CurrentScoreText = null;
        [SerializeField] private Text BestScoreText =  null;

        public Button PlayButton;
        public Button MenuButton;
        private Image PlayButtonImage;
        private Image MenuButtonImage;
        [SerializeField] private Text PlayButtonText = null;
        [SerializeField] private Text MenuButtonText = null;

        [Space]
        public int TotalScore;
        public int HighScore;

        [Space]
        [SerializeField] private DataColorScheme ColorScheme;

        //private void Awake() {
        //    GameStart();
        //}

        private void Awake() {
            ColorScheme = ManagerCGridGame.I.ColorScheme;

            CurrentScoreText.color = ColorScheme.RundtimeCurrentColorButtonClicked;
            BestScoreText.color = ColorScheme.RundtimeCurrentColorButtonClicked;

            if (PlayButton != null) {
                PlayButtonImage = PlayButton.gameObject.GetComponent<Image>();
                PlayButtonImage.color = ColorScheme.RuntimeCurrentColorButtonNotClicked;

                if (PlayButtonText == null)
                    PlayButtonText = PlayButton.GetComponentInChildren<Text>();
                PlayButtonText.color = ColorScheme.RundtimeCurrentColorButtonClicked;
            }
            else
                Debug.LogError("No PlayButton set", this);

            if (MenuButton != null) {
                MenuButtonImage = PlayButton.gameObject.GetComponent<Image>();
                MenuButtonImage.color = ColorScheme.RuntimeCurrentColorButtonNotClicked;

                if (MenuButtonText == null)
                    MenuButtonText = PlayButton.GetComponentInChildren<Text>();
                MenuButtonText.color = ColorScheme.RundtimeCurrentColorButtonClicked;
            }
            else
                Debug.LogError("No MenuButton set", this);
        }

        public void GameStart() {
            CanvasGamePlay.gameObject.SetActive(true); // gameObject off == lower memory consumption (all the nasty Update callbacks and stuff)
            CanvasGameOverUI.gameObject.SetActive(false);
        }

        public void GameLost() {
            CanvasGamePlay.gameObject.SetActive(false);
            CanvasGameOverUI.gameObject.SetActive(true);

            ManagerUI.I.MainCamera.backgroundColor = ColorScheme.RuntimeCurrentColorBackground;

            TotalScore = ManagerClockScore.I.ScoreInfo.ScoreTotal;
            HighScore = ManagerClockScore.I.GetHighScore();

            CurrentScoreText.text = TotalScore.ToString();

            if (TotalScore > HighScore) {
                HighScore = TotalScore;
            }

            BestScoreText.text = HighScore.ToString();
        }

        public void GameWon() {
            CanvasGamePlay.gameObject.SetActive(false);
            CanvasGameOverUI.gameObject.SetActive(true);

            ManagerUI.I.MainCamera.backgroundColor = ColorScheme.RuntimeCurrentColorBackground;

            TotalScore = ManagerClockScore.I.ScoreInfo.ScoreTotal;
            HighScore = ManagerClockScore.I.GetHighScore();

            CurrentScoreText.text = TotalScore.ToString();

            if (TotalScore > HighScore) {
                HighScore = TotalScore;

            }

            BestScoreText.text = HighScore.ToString();
        }
    }
}