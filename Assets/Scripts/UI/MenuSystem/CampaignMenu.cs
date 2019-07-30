using System.Collections;
using System.Collections.Generic;
using IG.General;
using IG.UI;
using UnityEditor;
using UnityEngine;

namespace IG.CGrid {
    public class CampaignMenu : GenericMenu<CampaignMenu> {
        public Transform ParentTransform;
        public UILevel LevelPrefab;

        private void Start() {
            
        }

        public static void Show(DataCampaign campaignData) {
                Open();
                Instance.CreateItems(campaignData);
        }

        //public override void VisualizeOpeningDetails() {
        //    base.VisualizeOpeningDetails();
        //}



        public void CreateItems(DataCampaign campaignData) {
            int number = 0;
            foreach (var levelData in campaignData.LevelsData) {
                CreateItem(number++, levelData);
            }

        }


        private void CreateItem(int number, DataLevel levelData) {
            var inst = Instantiate(LevelPrefab, ParentTransform);
            inst.Construct(number, levelData.MatrixLength);


        }

        public void StartLevel(int levelNumber) {
            
        }
    }
}