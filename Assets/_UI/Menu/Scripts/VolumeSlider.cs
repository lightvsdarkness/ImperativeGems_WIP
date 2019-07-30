using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IG {
    public enum AudioCategory {
        Master,
        Music,
        SoundEffects,
        UI
    }

    public class VolumeSlider : MonoBehaviour {
        public AudioCategory SliderCategory;
        public float DefaultValue = 50f;

        // The text we display to the user for the slider value
        [Space]
        public TextMeshProUGUI TextField;

        protected Slider slider;
        // The slider value as an int
        protected int Value {
            get { return (int)slider.value; }
            set { slider.value = System.Convert.ToInt16(value); }
        }

        [Space]
        public string[] displayLabels;

        void Awake() {
            slider = GetComponent<Slider>();
            ClearSettingAndUI();
        }

        protected virtual void Start() {
            // Attach the listener for the method we call when the slider value changes.
            slider.onValueChanged.AddListener(delegate { OnSliderValueChange(); });
            slider.onValueChanged.AddListener(delegate { OnSliderValueChangeSetDisplayText(); });

            // Find the Text component for the display value
            if (TextField == null)
                TextField = transform.Find("Value").GetComponent<TextMeshProUGUI>();
            if (TextField == null)
                TextField = gameObject.GetComponentInChildren<TextMeshProUGUI>();

            // Initialize it to the current slider value
            TextField.text = slider.value.ToString();

            if (displayLabels.Length > 0) {
                TextField.text = displayLabels[Value];
            }

            //RefreshUI();
        }

        private void ClearSettingAndUI() {
            switch (SliderCategory) {
                case AudioCategory.Music:
                    //Debug.LogError("SoundEffectsVolume: " + AudioManager.I.GetMusicVolume(), this);
                    slider.value = DefaultValue;
                    AudioManager.I.SetMusicVolume(DefaultValue);
                    break;
                case AudioCategory.SoundEffects:
                    //Debug.LogError("MusicVolume: " + AudioManager.I.GetSoundEffectsVolume(), this);
                    slider.value = DefaultValue;
                    AudioManager.I.SetSoundEffectsVolume(DefaultValue);
                    break;
            }

        }

        // Each setting class overrides this and changes whatever it wants changed when we modify the slider
        protected virtual void OnSliderValueChange() {
            //TextValue.text = Value.ToString();
        }

        // Set the text value to display in the menu for this settings slider. A setting class can override this
        protected virtual void OnSliderValueChangeSetDisplayText() {
            if (displayLabels.Length > 0) {
                TextField.text = displayLabels[Value];
            }
            else {
                TextField.text = Value.ToString();
            }
        }
    }
}
