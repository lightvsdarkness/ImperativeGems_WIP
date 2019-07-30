using System.Collections;
using System.Collections.Generic;
using IG.CGrid;
using IG.General;
using UnityEngine;

namespace IG.CGrid {
    [CreateAssetMenu(fileName = "DataCampaign", menuName = "Scriptable Objects/Levels/DataCampaign")]
    public class DataCampaign : ScriptableObject {
        [Space]
        public List<DataLevel> LevelsData = new List<DataLevel>();
        public int CurrentLevel;


        public void NewGame(int levelIndex) {
            if (levelIndex < LevelsData.Count)
                CurrentLevel = levelIndex;
            else
                CurrentLevel = 0;

            ManagerScenes.I.RelocateToScene("_Game");
        }

        public void RegisterLevelWon(int levelIndex) {
            
        }
    }
}