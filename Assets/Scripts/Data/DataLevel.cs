using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IG.CGrid {
    [CreateAssetMenu(fileName = "DataLevel", menuName = "Scriptable Objects/Levels/DataLevel")]
    public class DataLevel : ScriptableObject {
        public bool TutorialIsOn;

        [Space]
        public bool WaveIsOn;

        public bool ExternalEnergyIsOn;

        [Space]
        public int DefaultEnergyChainLength = 3;

        [Space]
        public Vector2Int MatrixLength;


        private void Awake() {
            MatrixLength = new Vector2Int(7, 5);
        }

    }
}