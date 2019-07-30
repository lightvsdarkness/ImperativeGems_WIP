using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace IG.Graphics {

    [CreateAssetMenu(fileName = "PostProcessingEffectLevel", menuName = "Scriptable Objects/Graphics/PostProcessingEffectLevel")]
    public class PostProcessingEffectLevel : ScriptableObject {
        public GraphicalEffect Effect;
        [SerializeField] private GraphicsSettings _graphicsSettings = null;

        public bool On = true;

        public int ValueLow = 0;
        public int ValueMedium = 33;
        public int ValueHigh = 66;
        public int ValueUltra = 100;

        private string textToDisplay;

        public void Initialize(GraphicsSettings graphicsSettings) {
            _graphicsSettings = graphicsSettings;
        }

        public void SetEffect(GraphicalEffect effect) {
            Effect = effect;
        }

        public string GetTextToDisplay(float value) {

            //
            if (Effect == GraphicalEffect.FieldOfView)
                return SetFieldOfViewDisplayText(value);
            else if (Effect == GraphicalEffect.Bloom)
            {
                return SetBloomDisplayText(value);
            }
            else
            {

            }
            return "";
        }
        protected string SetBloomDisplayText(float value) {

            if (value == 0)
                textToDisplay = "Off";
            else
                textToDisplay = value.ToString() + "%";

            return textToDisplay;
        }


        private string SetFieldOfViewDisplayText(float value) {
            if (_graphicsSettings != null) {
                float hFov = 2 * Mathf.Atan(_graphicsSettings.Cam.aspect * Mathf.Tan(Mathf.Deg2Rad * value / 2));
                hFov *= Mathf.Rad2Deg;
                textToDisplay = value.ToString() + " (" + hFov.ToString("0") + ")";
            }


            return textToDisplay;
        }


    }
}