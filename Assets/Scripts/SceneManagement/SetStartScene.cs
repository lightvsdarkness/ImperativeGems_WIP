using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace IG {
    /// <summary>
    /// Sets the current scene at Start().
    /// </summary>
    [AddComponentMenu("Scene Streamer/Set Start Scene")]
	public class SetStartScene : MonoBehaviour {

        public LoadSceneMode _loadSceneMode = LoadSceneMode.Additive;

        /// <summary>
        /// The name of the scene to load at Start.
        /// </summary>
        [Tooltip("Load this scene at start")]
		public string startSceneName = "Scene 1";

		public void Start() 
		{
			ManagerScenes.I.RelocateToScene(startSceneName, null, null, _loadSceneMode);
            Destroy(this);
		}

	}

}
