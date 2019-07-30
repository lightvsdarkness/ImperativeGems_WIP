using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IG {

    [CreateAssetMenu(fileName = "SceneData_", menuName = "Scriptable Objects/IG/SceneData")]
    class IG_SceneData : ScriptableObject {
        public string SceneName = null;
        public Scene SceneFile;
        public List<string> ScenesTransition = new List<string>();
        public List<string> ScenesConnected = new List<string>();

        private void OnEnable() {
            GetSceneFile();
        }

        private void GetSceneFile() {
            if (!string.IsNullOrEmpty(SceneName))
                SceneFile = SceneManager.GetSceneByName(SceneName);
        }
    }


}