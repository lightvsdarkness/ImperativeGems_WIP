using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IG.CGrid {
    public class UILevel : MonoBehaviour {
        [SerializeField] private Button LevelButton;

        public TextMeshProUGUI NumberText;
        public TextMeshProUGUI MatrixSizeText;

        public string Number;
        public string MatrixSize;

        void Start() {

        }


        public void Construct(int number, Vector2Int matrixLength) {
            Number = number.ToString();
            MatrixSize = matrixLength.x + " x " + matrixLength.y;

            NumberText.text = Number;
            MatrixSizeText.text = MatrixSize;
        }


    }
}