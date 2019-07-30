using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IG.General;
using IG.CGrid;
using TMPro;
using UnityEngine.UI;

namespace IG.CGrid {
    /// <summary>
    /// Main class for most of UI
    /// </summary>
    public class ManagerUI : SingletonManager<ManagerUI> {
        public Camera MainCamera;
        [Space]
        public GameOverUI UIGameOver;
        public GameObject PanelPause;

        [Space]
        public GameObject PanelHelp;
        public List<GameObject> PanelHelpPages = new List<GameObject>();
        public int PanelHelpPagesIndex;

        [Space]
        public Text PauseText;
        public TextMeshProUGUI PauseButtonText;
        public Image PauseButtonImage;

        [Space]
        public Image PlayButtonImage;
        public Text PlayButtonText;
        public Image RestartButtonImage;
        public TextMeshProUGUI RestartButtonText;
        public Image MenuButtonImage;
        public Text MenuButtonText;

        [Space]
        public Image HelpButtonImage;
        public TextMeshProUGUI HelpButtonText;


        public DataColorScheme ColorScheme; // Colors


        //protected override void Awake() {
        //    base.Awake();
        //}

        private void Start() {
            ColorScheme = ManagerCGridGame.I.ColorScheme;

            if (MainCamera == null)
                MainCamera = Camera.main;
            if (MainCamera == null)
                MainCamera = FindObjectOfType<Camera>();

            if (UIGameOver == null)
                UIGameOver = FindObjectOfType<GameOverUI>();

            PauseText.color = ColorScheme.RuntimeCurrentColorButtonNotClicked;

            PlayButtonImage.color = ColorScheme.RuntimeCurrentColorButtonNotClicked;
            PlayButtonText.color = ColorScheme.RuntimeCurrentColorUIText;

            RestartButtonImage.color = ColorScheme.RuntimeCurrentColorButtonNotClicked;
            RestartButtonText.color = ColorScheme.RuntimeCurrentColorUIText;

            PauseButtonImage.color = ColorScheme.RuntimeCurrentColorButtonNotClicked;
            PauseButtonText.color = ColorScheme.RuntimeCurrentColorUIText;

            MenuButtonImage.color = ColorScheme.RuntimeCurrentColorButtonNotClicked;
            MenuButtonText.color = ColorScheme.CurrentColorUIText;
        }

        public void UIGameStart() {
            UIReset();
        }

        private void UIReset() {
            UIGameOver.GameStart();
            PanelPause.SetActive(false);
            PanelHelp.SetActive(false);
        }

        public void UIGameLost() {
            UIGameOver.GameLost();
        }


        public void UIPauseSwitch() {
            if (ManagerCGridGame.I.GamePlaying) {
                PanelPause.SetActive(false);
                //PanelPause.GetComponent<Animator>().SetTrigger("Hide");
                UIMenuPanel.I.UIPauseMenuSwitch(false);
            }
            else {
                PanelPause.SetActive(true);
                PanelPause.GetComponent<Animator>().SetTrigger("Show");
                UIMenuPanel.I.UIPauseMenuSwitch(true);
            }
                
        }

        public void UIHelpSwitch() {
            if (PanelHelp.activeSelf) {
                PanelPause.SetActive(false);
                //PanelPause.GetComponent<Animator>().SetTrigger("Hide");
            }
            else {
                PanelPause.SetActive(true);
                PanelPause.GetComponent<Animator>().SetTrigger("Show");
            }

            PanelHelp.SetActive(!PanelHelp.activeSelf);

            if (PanelHelpPagesIndex == 0)
                UIMenuPanel.I.UIPauseMenuSwitch(false);
            else
                UIMenuPanel.I.UIPauseMenuSwitch(true);

            PanelHelpPagesIndex = 0;
            PanelHelpPages[PanelHelpPagesIndex].SetActive(true);
            for (int i = 1; i < PanelHelpPages.Count; i++) {
                PanelHelpPages[i].SetActive(false);
            }
        }
        public void UIHelpNext() {
            PanelHelpPages[PanelHelpPagesIndex].SetActive(false);
            PanelHelpPagesIndex++;
            if (PanelHelpPagesIndex >= PanelHelpPages.Count) {
                UIHelpSwitch();
            }
            else
                PanelHelpPages[PanelHelpPagesIndex].SetActive(true);
        }
    }
}