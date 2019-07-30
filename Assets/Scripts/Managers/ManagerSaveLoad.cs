using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using IG.General;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace IG.CGrid {
    public class ManagerSaveLoad : SingletonManager<ManagerSaveLoad> {
        public PlayerData Data;

        //[Space]
        //public int CurrentScore;
        //public int GameMode;

        private void Start() {
            Debugging = false;

            DontDestroyOnLoad(gameObject);
            Load();
        }


        //public void SetHighScore(int NewScore) {
        //    _highScore = NewScore;
        //}

        public void Save(int newHighScore) {
            if (Debugging)
                Debug.Log("Saving Data in PlayerData.dat to Application.persistentDataPath: " + Application.persistentDataPath + "/PlayerData.dat", this);

            if (Data == null)
                Load();
            if (Data == null)
                Data = new PlayerData();

            // Save only if we have new data
            if (newHighScore > Data.HighScore) { 
                Data.HighScore = newHighScore;

                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Create(Application.persistentDataPath + "/PlayerData.dat");

                bf.Serialize(file, Data);
                file.Close();
            }
        }

        public PlayerData Load() {
            if (File.Exists(Application.persistentDataPath + "/PlayerData.dat")) {
                if (Debugging)
                    Debug.Log("Loading PlayerData.dat from Application.persistentDataPath: " + Application.persistentDataPath + "/PlayerData.dat", this);

                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/PlayerData.dat", FileMode.Open);

                Data = (PlayerData) bf.Deserialize(file);
                file.Close();

                if (Debugging)
                    Debug.Log("PlayerData.dat contents: " + Data, this);

                return Data;
            }

            return null;
        }
        
    }


    [Serializable]
    public class PlayerData {

        public int HighScore;
    }
}