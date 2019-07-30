using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using IG.Graphics;
using System;
using UnityEngine.Rendering.PostProcessing;

namespace IG {
    public enum GraphicalEffect {
        AmbientOcclusion,
        AutoExposure,
        Bloom,
        ChromaticAberration,
        ColorGrading,
        DepthOfField,
        Grain,
        LensDistortion,
        MotionBlur,
        ScreenSpaceReflections,
        Vignette,
        FieldOfView

    }
    public enum EffectPreset {
        None,
        LowPreset,
        MediumPreset,
        HighPreset,
        UltraPreset
    }

    public class GraphicsSettings : MonoBehaviour {
        public float Multiplayer = 1f;
        public EffectPreset SelectedPreset;
        public bool ChangedByPreset;

        [Space]
        public PostProcessingEffectLevels PostProcessingEffectLevels;
        [SerializeField] protected PostProcessVolume PostProcessingVolume;
        public Camera Cam; // The camera in use

        public UnityEvent LowPresetEvent = new UnityEvent();
        public UnityEvent MediumPresetEvent = new UnityEvent();
        public UnityEvent HighPresetEvent = new UnityEvent();
        public UnityEvent UltraPresetEvent = new UnityEvent();

        public Slider presetSlider; // A reference to the slider for setting graphics preset so we can update it

        // All the settings register their listeners in Awake() so we can safely invoke them in Start().
        private void Awake() {
            if (Cam == null)
                Cam = Camera.main;
            if (PostProcessingVolume == null)
                PostProcessingVolume = Cam.GetComponent<PostProcessVolume>();
        }

        private void Start() {
            SetPresetSettings();

            //Debug.Log("Flashing MainMenu for GraphicsSettings", this);
            // We need to "flash" the graphics panel on the screen because all the graphics settings are part
            // of the graphics panel and won't be called if it's not active. Not sure how to best fix this
            //UIMenuPanel.I.panels[0].SetActive(true);
            //ultraPresetEvent.Invoke();
            //presetSlider.value = 3;
            //UIMenuPanel.I.panels[0].SetActive(false);
        }

        private void Update() {
            //if (ChangedByPreset) {
            //    ChangedByPreset = false;
            //}
        }

        public void SetPresetSettings() {
            foreach (var ppEffectLevel in PostProcessingEffectLevels.PPEffectLevels) {
                InitializeEffects(ppEffectLevel);
                SetSettings(ppEffectLevel);
            }
        }

        public void InitializeEffects(PostProcessingEffectLevel controlledEffect, float? value = null) {
            controlledEffect.Initialize(this);
        }

        public void SetSettings(PostProcessingEffectLevel controlledEffect, float? value = null) {
            Debug.Log("value" + value, this);
            if (value == null)
                return;


            if (controlledEffect.Effect == GraphicalEffect.Bloom)
                SetBloom((float)value);
            
        }
        

        public void SetGraphicsPreset(int value) {
            ChangedByPreset = true;
            SelectedPreset = (EffectPreset)value;

            switch (SelectedPreset) {
                case EffectPreset.LowPreset:
                    LowPresetEvent.Invoke();
                    break;
                case EffectPreset.MediumPreset:
                    MediumPresetEvent.Invoke();
                    break;
                case EffectPreset.HighPreset:
                    HighPresetEvent.Invoke();
                    break;
                case EffectPreset.UltraPreset:
                    UltraPresetEvent.Invoke();
                    break;
                default:
                    break;
            }
        }

        protected void SetBloom(float value) {
            Bloom bloom;
            PostProcessingVolume.sharedProfile.TryGetSettings(out bloom);
            //Debug.Log("value" + value, this);
            //Debug.Log("Multiplayer" + Multiplayer, this);
            var v = 0.2f * (value * Multiplayer); // / 10
            Debug.Log(v, this);
            bloom.intensity.value = v;
            bloom.color.value = Color.white;
            if (value <= 0)
                bloom.enabled.value = false;
            else
                bloom.enabled.value = true;
        }

        private void SetFieldOfViewValue(int value) {
            Cam.fieldOfView = value;
        }

        protected void SetBrightness(int value) {
            Cam.GetComponent<BrightnessEffect>().brightness = value / 100f;
        }
    }
}