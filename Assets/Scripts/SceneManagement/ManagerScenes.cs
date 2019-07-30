using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using IG.CGrid;
using IG.General;

namespace IG {
    /// <summary>
    /// 
    /// </summary>
    [AddComponentMenu("Scene Streamer/Scene Streamer")]
    public class ManagerScenes : SingletonManager<ManagerScenes> {
        [SerializeField]
        private IG_GraphSceneData _graphSceneData = null; // Scriptable object

        [Space]
        [Tooltip("Name of the persistent main scene, containing managers and player, if any. Leave empty if it doesn't exists")]
        [SerializeField] private string _namePersistentScene = "_Player";
        public string PersistentSceneName {
            get { return _namePersistentScene = _graphSceneData.PersistentSceneName; } 
            private set { _namePersistentScene = _graphSceneData.PersistentSceneName = value; }
        } 

        public static Vector3 PositionDefaultRelocation = Vector3.zero;

        // --- General settings
        public string MainMenuSceneName { get { return _graphSceneData.MainMenuSceneName; } }
        public string CurrentSceneName { get { return _currentSceneName; } }
        [SerializeField] private string _currentSceneName;

        [Tooltip("If scene doesn't load after this many seconds, stop waiting")]
        public float MaxLoadWaitTime = 10f;

        [Tooltip("Tag for 1st level object in scene, containing scripts related to ManagerScenes")]
        public string Tag_LevelParentObject = "LevelParentObject";

        // --- Collections of scenes
        /// <summary>
        /// The names of all loaded scenes
        /// </summary>
        private HashSet<string> _scenesLoaded = new HashSet<string>();
        /// <summary>
        /// The names of all scenes that are in the process of being loaded
        /// </summary>
        private HashSet<string> _scenesLoading = new HashSet<string>();
        /// <summary>
        /// 
        /// </summary>
        private HashSet<string> _scenesShouldBeLoaded = new HashSet<string>();

        // Unity Events
        public StringEvent OnSceneLoading = new StringEvent();
        public StringEvent OnSceneLoaded = new StringEvent();

        public StringEvent OnSceneLoadingSync = new StringEvent();
        public StringEvent OnSceneLoadedSync = new StringEvent();

        public StringEvent OnSceneLoadingAsyncSingle;
        public StringEvent OnSceneLoadedAsyncSingle;

        public StringEvent OnSceneLoadingAsyncAdditive = new StringEvent();
        public StringAsyncEvent OnSceneLoadedAsyncAdditive = new StringAsyncEvent();

        //
        public string DefaultPath;


        protected void OnEnable() {
            // React to loading save
            //ManagerSaveLoad.OnLoadingGame += LoadSceneWhenSavedGameLoads;

            //// For developing purposes & Debugging
            //if (!string.IsNullOrEmpty(ManuallyLoadSceneWithName))
            //    LoadCurrentScene(ManuallyLoadSceneWithName, LoadSceneMode.Additive);
        }

        private void Start() {
            if (_graphSceneData == null) {
                Debug.LogError("No _graphSceneData", this);
            }

        }

        protected void OnDisable() {
            // React to loading save
            // ManagerSaveLoad.OnLoadingGame -= LoadSceneWhenSavedGameLoads;
        }


        public void LoadSceneWhenSavedGameLoads(string currentSceneInSave) {
            if (Debugging) Debug.Log("SavedGame Loaded, loading scene");

            ChangeCurrentScene(currentSceneInSave);
        }

        /// <summary>
        /// Main method for changing scene. Suggested way of calling through checking colliders placed at exits from scene.
        /// But it's public so anything can call it. For example, "Teleportation" script will change player coordinates and optionally loads scene.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="loadSceneMode"></param>
        public void RelocateToScene(string sceneName, GameObject player = null, Vector3? relocationWorldPosition = null,
        LoadSceneMode loadSceneMode = LoadSceneMode.Additive) {
            if (Debugging) Debug.Log("Changing current scene to " + sceneName);

            ChangeCurrentScene(sceneName, loadSceneMode);
            // TODO: Should make sceneName Active if it's already loaded and LoadCurrentScene return at first row

            if (relocationWorldPosition == null)
                relocationWorldPosition = PositionDefaultRelocation;
            if (player != null)
                player.transform.position = (Vector3) relocationWorldPosition;
        }

        private void ChangeCurrentScene(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Additive) {
            if (string.IsNullOrEmpty(sceneName) || sceneName == _currentSceneName) return;
            
            _currentSceneName = sceneName;  // Saving the name of current scene

            _scenesLoaded = GetLoadedScenes();    // Searching if we have scenes loaded for some reason
            _graphSceneData.ScenesDataLoaded = new HashSet<IG_SceneData>(_graphSceneData.ScenesDataAll);
            //_graphSceneData.ScenesDataLoaded.IntersectWith(_scenesLoaded);

            Action action = () => ActivateScene(_currentSceneName);
            // If isn't loaded, load. Then make current scene active
            if (!_scenesLoaded.Contains(_currentSceneName)) {
                StartCoroutine(LoadSceneAsync(_currentSceneName, loadSceneMode, action));
            }
            
            
            // --- After loading main scene, load optional additional scenes up to SceneLoadDistance, keeping track of them in the near list:
            if (Debugging) Debug.Log("Loading scenes connected with: " + _currentSceneName);

            IG_SceneData sceneData = _graphSceneData.ScenesDataAll.Find(x => x.SceneName == _currentSceneName); // Get Scene Data for current scene

            // 
            LoadScenes(GetScenesShouldBeLoaded(sceneData, 1));

            // UnloadScene any scenes not in the near list
            UnloadScenesExcessive();
        }

        /// <summary>
        /// Generate collections with related scenes' names for loading and load them
        /// </summary>
        /// <param name="sceneData"></param>
        /// <param name="sceneLoadDistance"></param>
        /// <returns></returns>
        private HashSet<string> GetScenesShouldBeLoaded(IG_SceneData sceneData, int sceneLoadDistance) {
            _scenesShouldBeLoaded.Clear();
            GetScenesShouldBeLoadedInternal(sceneData, sceneLoadDistance);
            _scenesShouldBeLoaded.Add(PersistentSceneName);

            return _scenesShouldBeLoaded;
        }

        private void GetScenesShouldBeLoadedInternal(IG_SceneData sceneData, int sceneLoadDistance) {
            if (sceneLoadDistance > _graphSceneData.DefaultMaxDistance) return;
            sceneLoadDistance++;

            // Add scenes from current sceneData
            HashSet<string> scenesConnected = new HashSet<string>(sceneData.ScenesConnected); 
            _scenesShouldBeLoaded.UnionWith(scenesConnected);
            _scenesShouldBeLoaded.UnionWith(new HashSet<string>(sceneData.ScenesTransition));

            foreach (var sceneName in scenesConnected) {
                IG_SceneData cSceneData = _graphSceneData.ScenesDataAll.Find(x => x.name == sceneName);

                GetScenesShouldBeLoadedInternal(cSceneData, sceneLoadDistance);
            }
        }

        /// <summary>
        /// Loads neighbor scenes within SceneLoadDistance, adding them to the near list
        /// 
        /// </summary>
        /// <param name="sceneName">Scene to activate</param>
        private void LoadScenes(HashSet<string> scenesToLoad) {
            //StartCoroutine(FailsafeTimer(MaxLoadWaitTime, "Too much time spended loading scenes, connected with: " + sceneName));
            foreach (var sceneName in scenesToLoad)
            {
                if (_scenesLoaded.Contains(sceneName)) {
                    if (!SceneManager.GetSceneByName(sceneName).isLoaded)
                        break;
                }

                LoadSceneAsync(sceneName);
            }
        }

        /// <summary>
        /// Sync runs LoadScene() and calls FinishSceneLoading() when done. Coroutine waits two frames to wait for the load to complete
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="loadSceneMode"></param>
        /// <param name="loadingWaitingTime"></param>
        private IEnumerator LoadSceneSync(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single, float loadingWaitingTime = 1.0f) {
            if (_scenesLoaded.Contains(sceneName) || string.IsNullOrEmpty(sceneName))
                yield break;

            OnSceneLoadingSync.Invoke(sceneName); // Diff methods like screen fading to black
            SceneManager.LoadScene(sceneName, loadSceneMode);

            yield return new WaitForSeconds(loadingWaitingTime); // Some delay after scene loaded so we can activate objects, teleport Player, etc
            OnSceneLoadedSync.Invoke(sceneName); // Diff methods like screen fading from black

            FinishSceneLoading(sceneName);
        }

        /// <summary>
        /// Loads Scene asynchronically and calls FinishSceneLoading() when done
        /// </summary>
        private IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Additive, Action callback = null, float loadingWaitingTime = 1.0f) {
            if (_scenesLoaded.Contains(sceneName) || string.IsNullOrEmpty(sceneName))
                yield break;

            _scenesLoading.Add(sceneName);
            if (Debugging) Debug.Log("Loading " + sceneName);

            //StartCoroutine(FailsafeTimer(MaxLoadWaitTime, "Too much time spended loading scene: " + sceneName)); // Timer for loading current scene

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            //asyncOperation.allowSceneActivation = false;

            if (loadSceneMode == LoadSceneMode.Single)
                OnSceneLoadingAsyncSingle.Invoke(sceneName);

            // Wait until the asynchronous scene fully loads
            while (!asyncOperation.isDone) {
                Debug.Log("Async Scene Loading: " + asyncOperation.progress);
                yield return null;
            }
            //asyncOperation.allowSceneActivation = true;

            //if (loadSceneMode == LoadSceneMode.Single)
            //    yield return new WaitForSeconds(loadingWaitingTime);

            yield return asyncOperation;


            FinishSceneLoading(sceneName);
            callback?.Invoke();

            if (loadSceneMode == LoadSceneMode.Single)
                OnSceneLoadedAsyncSingle?.Invoke(sceneName);
            if (loadSceneMode == LoadSceneMode.Additive)
                OnSceneLoadedAsyncAdditive?.Invoke(sceneName, asyncOperation);
        }

        /// <summary>
        /// Called when a level is done loading. Updates the loaded and loading lists, and  calls the loaded handler
        /// </summary>
        private void FinishSceneLoading(string sceneName) {
            _scenesLoading.Remove(sceneName);
            _scenesLoaded.Add(sceneName);

            OnSceneLoaded.Invoke(sceneName);
        }

        /// <summary>
        /// Unloads scenes beyond maxNeighborDistance. Assumes the near list has already been populated
        /// </summary>
        private void UnloadScenesExcessive() {
            HashSet<string> scenesExcessive = new HashSet<string>(_scenesLoaded);
            scenesExcessive.ExceptWith(_scenesShouldBeLoaded);

            if (Debugging && scenesExcessive.Count > 0) Debug.Log("Unloading scenes not connected with current scene " + _currentSceneName);

            foreach (var sceneName in scenesExcessive) {
                UnloadScene(sceneName);
            }
        }

        /// <summary>
        /// Unloads a scene, and removes it from the list
        /// </summary>
        private void UnloadScene(string sceneName) {
            if (Debugging) Debug.Log("Unloading scene " + sceneName);

            SceneManager.UnloadSceneAsync(sceneName);
            _scenesLoaded.Remove(sceneName);
        }

        /// <summary>
        /// Changing Active scene, e.g. if we need to change lighting and objects' spawn location
        /// </summary>
        /// <param name="sceneName">Scene to activate</param>
        private void ActivateScene(string sceneName) {
            // Check for it being loaded
            if (_scenesLoaded.Contains(sceneName))
            {
                if (SceneManager.GetSceneByName(sceneName).isLoaded)
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

                else if (DebugLogging) Debug.Log("Scene wasn't loaded: " + sceneName);
            }
            else // Shouldn't ever get here, but just to be sure
            {
                if (Debugging) Debug.LogError("Scene wasn't marked as viable for activation" + sceneName + ". Activating main scene");

                if (SceneManager.GetSceneByName(sceneName).isLoaded) {
                    _scenesLoaded.Add(sceneName);
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
                }
                else if (DebugLogging) Debug.Log("Scene wasn't loaded: " + sceneName);
            }
        }

        /// <summary>
        /// Search for some scenes being loaded for some reason
        /// </summary>
        /// <returns></returns>
        private HashSet<string> GetLoadedScenes() {
            //var levelObjects = GameObject.FindGameObjectsWithTag(Tag_LevelParentObject);
            //HashSet<string> scenesLoaded = new HashSet<string>();

            //if (levelObjects.Length > 0)
            //    foreach (var levelObject in levelObjects) {
            //        scenesLoaded.Add(levelObject.name);
            //    }
            //return scenesLoaded;

            return new HashSet<string>();
        }

        ///// <summary>
        ///// Until Count > 0 && failsafe, do nothing but wait (so, it's a wait until current scene loads, with failsafe)
        ///// </summary>
        ///// <param name="maxLoadWaitTime">Maximum time, after which we log debug error</param>
        ///// <param name="message">For Debug logging</param>
        //public IEnumerator FailsafeTimer(float maxLoadWaitTime, string message) {
        //    float failsafeTime = Time.realtimeSinceStartup + maxLoadWaitTime;

        //    while ((_scenesLoading.Count > 0) && (Time.realtimeSinceStartup < failsafeTime)) {
        //        if (Time.realtimeSinceStartup >= failsafeTime && Debug.isDebugBuild)
        //            Debug.LogError(message);

        //        yield return null;
        //    }
        //}
    }
}