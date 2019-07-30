using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IG.General;
using IG.UI;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace IG.CGrid {
    public class GameModesMenu : GenericMenu<GameModesMenu> {
        public List<DataCampaign> CampaignDataList = new List<DataCampaign>();

        [Space]
        [SerializeField] private DataColorScheme TCD = null;

        public CampaignMenu CampaignVis;

        public Image SoundImage;

        //[SerializeField] private GameObject GameModes = null;

        [Space]
        public Image LogoImage;
        public TextMeshProUGUI LogoText;

        [Space]
        public Image BackButtonImage;
        public TextMeshProUGUI BackButtonText;


        private void Start() {
            if (TCD == null) Debug.LogError("Set DataColorScheme", this);
            //Debug.Log("Opened", this);

            //TCD.RandomizeTheme();
            if (LogoImage != null)
                LogoImage.color = TCD.RundtimeCurrentColorButtonClicked;
            if (LogoText != null)
                LogoText.color = TCD.RundtimeCurrentColorButtonClicked;

            if (BackButtonImage != null)
                BackButtonImage.color = TCD.RundtimeCurrentColorButtonClicked;
            if (BackButtonText != null)
                BackButtonText.color = TCD.RundtimeCurrentColorButtonClicked;

            if (SoundImage != null) { 
            if (AudioListener.volume == 1)
                SoundImage.color = new Color32(0, 255, 0, 255);
            else
                SoundImage.color = new Color32(255, 0, 0, 255);
            }
        }

        public void OpenCampaign(int campaignIndex) {
            //Buttons.SetActive(false);
            if (CampaignDataList.Count > campaignIndex)
                CampaignMenu.Show(CampaignDataList[campaignIndex]);
            else 
                Debug.LogError("There is no DataCampaign in CampaignDataList with such index, please refresh List or use right index", this);
        }

        public void OpenTutorial() {

        }

        public void OpenBattle() {

        }
        
        public override void OnBackPressed() {
            Application.Quit();
        }

        //public void Back() {
        //    //Buttons.SetActive(true);
        //    CampaignVis.Activate(false);
        //    GameModes.SetActive(true);
        //}

        //public void SelectGameMode(int gameMode) {
        //    //ManagerCGridGame.I.CGridGameMode = (CGridGameMode) gameMode;
        //    SceneManager.LoadScene(2);
        //}
    }
}