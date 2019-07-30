using System.Collections.Generic;
using System.Diagnostics;
using IG.CGrid;
using UnityEngine;
using UnityEngine.UI;
using IG.General;

namespace IG {
    public class UIMenuPanel : SingletonManager<UIMenuPanel> {

        [Space]
        public Canvas pauseMenuCanvas; //Pause Menu
        public Canvas backgroundMenuCanvas; //Pause Menu
        public Animator Anim;

        [Space]
        public float currentTimeScale;

        //private bool isInPauseState; // control variable for toggling pause menu
        [Space]
        public static int guiDepth = 0; // define  a static gui depth variable in all other gui classes and use in OnGui() as shown ahead

        [Space]
        public GameObject menu;
        public List<GameObject> panels = new List<GameObject>();

        // Used by the buttons which open panels so we can return to the same button after closing the panel
        [HideInInspector] public Button currentButton;
        // Used to determine which game object our mouse pointer is currently hovering over
        [HideInInspector] public GameObject currentMouseOverGameObject;

        [Space]
        public bool menuOpened = false;
        public bool panelOpened = false;



        protected void Start() {
            currentTimeScale = Time.timeScale;

            //Turns off the Pause Menu
            for (int i = 0; i < panels.Count; i++) {
                panels[i].SetActive(false);
            }
            menu.SetActive(false);
        }

        // Gradual turning off
        public void UIPanelPauseMenuSwitch() {
            if (panelOpened) {
                ReturnBackToMenu();
            }
            else if (menuOpened) {
                HideMenu();
            }
            else {
                ShowMenu();
            }
        }

        // Complete instant turning off
        public void UIPauseMenuSwitch(bool shouldShowMenu) {
            //Debug.Log("PauseMenu");

            if (shouldShowMenu) {
                ShowMenu();
            }
            else {
                HideAllPanels();
                HideMenu();
            }
        }

        private void onGui() {
            GUI.depth = guiDepth;//set the gui depth, if any, of gui elements  available in pause menu and same statement should be available in other gui classes
        }

        public void ReturnBackToMenu() {
            HideAllPanels();
            ShowMenu();
        }

        private void HideAllPanels() {
            foreach (GameObject panel in panels) {
                panel.SetActive(false);
            }
            panelOpened = false;
            currentButton?.Select();
        }

        public void ShowPanel(int id) {
            panels[id].SetActive(true);
            //backgroundMenuCanvas.enabled = true;

            pauseMenuCanvas.renderMode = RenderMode.WorldSpace; // 
            pauseMenuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            panels[id].GetComponent<Panel>().SelectFirstElement();
            panelOpened = true;
            HideMenu();
        }

        public void ShowMenu() {
            menu.SetActive(true);
            if (backgroundMenuCanvas != null)
                backgroundMenuCanvas.enabled = true;
            pauseMenuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            menuOpened = true;
            Anim.SetTrigger("Show");

            //Time.timeScale = 0; // TODO: UI classes shouldn't change game state
            ManagerInput.I.ShowCursorBecauseOfMenu();

            //m_Input.MouseCursorForced = true; // hm
            //Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;

            //guiDepth = 0; //set a lower value for guis  (if present) to show on top of other
            //GuiSurvival.guiDepth = 1; // other gui element class
        }

        public void HideMenu() {
            Invoke("TurnOffObject", 1f);
            menuOpened = false;
            if (backgroundMenuCanvas != null)
                backgroundMenuCanvas.enabled = false;
            Anim.SetTrigger("Hide");

            //Time.timeScale = currentTimeScale; // TODO: UI classes shouldn't change game state
            ManagerInput.I.HideAndLockCursorBecauseOfMenu();
            //if (ManagerGame.Instance != null)
            //{
            //    ManagerGame.I.CurrentPlayerForm.transform.GetComponent<vp_FPInput>().MouseCursorForced = true;
            //    //Cursor.visible = false; //Unity 5.2 turn off the cursor
            //}
            //else
            //    Debug.LogError("ManagerGame instance isn't present in the scene");

            //Cursor.lockState = CursorLockMode.Locked; //Unity 5.2 lock the cursor
            //m_Input.MouseCursorForced = false; //Change cursor back to normal in game mode

            //Cursor.lockState = CursorLockMode.None;
            //m_Input.MouseCursorForced = true;
            //Cursor.visible = true;

            //guiDepth = 1; //set a lower value for guis  (if present) to show on top other
            //GuiSurvival.guiDepth = 0;  // other gui element class 
        }

        public void TurnOffObject() {
            menu.SetActive(false); //Turn off Pause Menu
        }

        public void Quit() {
            //If we are running in a standalone build of the game
#if UNITY_STANDALONE
            if (ManagerScenes.I.MainMenuSceneName != ManagerScenes.I.CurrentSceneName)
                ManagerScenes.I.ChangeCurrentScene(ManagerScenes.MainMenuName);     // Load MainMenuName
            else
                Application.Quit(); //Quit the application
#endif

            //If we are running in the editor
#if UNITY_EDITOR
            if (ManagerScenes.I.MainMenuSceneName != ManagerScenes.I.CurrentSceneName)
                ManagerScenes.I.RelocateToScene(ManagerScenes.I.MainMenuSceneName);
            else
                UnityEditor.EditorApplication.isPlaying = false; // Stop playing the scene
#endif
        }
    }
}