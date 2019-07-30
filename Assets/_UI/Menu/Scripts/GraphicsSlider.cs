using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

namespace IG.Graphics {
    public class GraphicsSlider : MonoBehaviour {
        public GraphicsSettings graphicsSettings; // Settings to set
        
        [SerializeField] protected PostProcessingEffectLevel ControlledEffect;

        // The text we display to the user for the slider value.
        public TextMeshProUGUI DisplayValue;

        protected Slider Slider;

        protected int Value {
            get { return (int)Slider.value; }
            set { Slider.value = System.Convert.ToInt16(value); }
        }

        public string[] DisplayLabels;


        private void Awake() {
            if (Slider == null)
                Slider = GetComponent<Slider>();

            if (DisplayValue == null)
                DisplayValue = transform.Find("Value").GetComponent<TextMeshProUGUI>();

            // Register the graphics preset listeners
            if (graphicsSettings == null)
                graphicsSettings = FindObjectOfType<GraphicsSettings>();

            graphicsSettings.LowPresetEvent.AddListener(GraphicsPresetLow);
            graphicsSettings.MediumPresetEvent.AddListener(GraphicsPresetMedium);
            graphicsSettings.HighPresetEvent.AddListener(GraphicsPresetHigh);
            graphicsSettings.UltraPresetEvent.AddListener(GraphicsPresetUltra);
        }

        protected virtual void Start() {
            // Listeners for when the slider value changes
            Slider.onValueChanged.AddListener(delegate { OnSliderValueChange(); });
            Slider.onValueChanged.AddListener(delegate { OnSliderValueChangeCheckPreset(); });
            Slider.onValueChanged.AddListener(delegate { OnSliderValueChangeSetDisplayText(); });

            // Initialize it to the current slider value.
            DisplayValue.text = Slider.value.ToString();

            if (DisplayLabels.Length > 0) {
                DisplayValue.text = DisplayLabels[Value];
            }
        }

        /* The settings to apply when a preset is selected. Overriden in each respective settings class. Here you can turn off an effect on a lower quality setting or adjust some of it's values, lower shadow distance perhaps or whatever you want. */
        protected virtual void GraphicsPresetLow() {
            var ss = graphicsSettings.PostProcessingEffectLevels.PPEffectLevels.Find(x => x.Effect == GraphicalEffect.FieldOfView).ValueLow;


        }
        protected virtual void GraphicsPresetMedium() {
        }
        protected virtual void GraphicsPresetHigh() {
        }
        protected virtual void GraphicsPresetUltra() {
        }

        /* Whenever we change a setting check to see if we're not in the custom preset and if so change it so that we're in the custom preset. You're not supposed to be able to change anything while having any other preset selected. */
        protected virtual void OnSliderValueChangeCheckPreset() {
            if (graphicsSettings.presetSlider == null) return;

            if (graphicsSettings.presetSlider.value != 4 && !graphicsSettings.ChangedByPreset) {
                graphicsSettings.presetSlider.value = 4;
            }
        }

        /* Each setting class overrides this and changes whatever it wants changed when we modify the slider. For example turns on/off an image effect or adjusts the volume in the audio mixer. */
        protected virtual void OnSliderValueChange() {
            graphicsSettings.SetSettings(ControlledEffect, Value);
            //if (true)
            //    SetBloom(Value);
        }

        /* Set the text value to display in the menu for this settings slider. A  setting class can override this to display whatever it wants in the menu. */
        protected virtual void OnSliderValueChangeSetDisplayText() {
            //ControlledEffect
            string textToDisplay = ControlledEffect.GetTextToDisplay(Value);

            if (textToDisplay != null & textToDisplay != "") {
                DisplayValue.text = textToDisplay;
                return;
            }

            if (DisplayLabels.Length > 0) {
                DisplayValue.text = DisplayLabels[Value];
            }
            else {
                DisplayValue.text = Value.ToString();
            }
        }


        //protected string SetBloomDisplayText(float value) {
        //    string textToDisplay;
        //    if (value == 0)
        //        DisplayValue.text = textToDisplay = "Off";
        //    else
        //        DisplayValue.text = textToDisplay = value.ToString() + "%";

        //    return textToDisplay;
        //}


        //private string SetFieldOfViewDisplayText(float value) {
        //    string textToDisplay;
        //    float hFov = 2 * Mathf.Atan(graphicsSettings.Cam.aspect * Mathf.Tan(Mathf.Deg2Rad * value / 2));
        //    hFov *= Mathf.Rad2Deg;
        //    DisplayValue.text = textToDisplay = value.ToString() + " (" + hFov.ToString("0") + ")";

        //    return textToDisplay;
        //}
    }
}