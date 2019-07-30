using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IG
{
    [CreateAssetMenu(fileName = "GraphSceneData_", menuName = "Scriptable Objects/IG/GraphSceneData")]
    class IG_GraphSceneData : ScriptableObject {
        public Scene CurrentScene;

        // Leave empty if there is no persistent scene
        public string PersistentSceneName;
        // Leave empty if there is no persistent scene
        public string MainMenuSceneName;

        // Default amount of connected scenes Manager will load
        public int DefaultMaxDistance = 0;

        [Space]
        public List<IG_SceneData> ScenesDataAll = new List<IG_SceneData>();
        //public HashSet<IG_SceneData> ScenesDataList2;

        public HashSet<IG_SceneData> ScenesDataLoaded;


        private void Start() {
            ScenesDataAll.Distinct();
        }
    }

}