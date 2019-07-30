using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IG.General;
using UnityEngine.Audio;

namespace IG {
    public partial class AudioManager : SingletonManager<AudioManager> {

        protected override void Awake() {
            //listener = Camera.main.GetComponent<AudioListener>();

            if(_UIAudioSource == null)
                _UIAudioSource = GetComponent<AudioSource>();

            // Devdog part
            base.Awake();

            //StartCoroutine(WaitFramesAndEnable(5));
            //enabled = false;
            //// Set to enabled at start, initialize, then enable (to avoid playing sound during initialization)

            //_audioQueue = new Queue<AudioClipInfo>(GeneralSettingsManager.instance.settings.reserveAudioSources);
            //CreateAudioSourcePool();

            mixer.SetFloat("MasterVolume", ConvertToDecibel(17));
        }

        #region UI part
        public bool Muted;

        public AudioMixer mixer;

        public AudioClip hoverSound;
        public AudioClip sliderSound;
        public AudioClip clickSound;

        public AudioSource _UIAudioSource;

        public void PlayClickSound() {
            _UIAudioSource.clip = clickSound;
            _UIAudioSource.Play();
        }
        public void PlayHoverSound() {
            _UIAudioSource.clip = hoverSound;
            _UIAudioSource.Play();
        }
        public void PlaySliderSound() {
            _UIAudioSource.clip = sliderSound;
            _UIAudioSource.Play();
        }

        public void MuteMasterVolumeSwitch() {
            Muted = !Muted;
            if (Muted)
                mixer.SetFloat("MasterVolume", ConvertToDecibel(0));
            else
                mixer.SetFloat("MasterVolume", ConvertToDecibel(135));

        }

        public void SetMusicVolume(float value) {
            mixer.SetFloat("MusicVolume", ConvertToDecibel(value));
        }
        public void SetSoundEffectsVolume(float value) {
            mixer.SetFloat("SoundEffectsVolume", ConvertToDecibel(value));
            //Debug.LogError("SoundEffectsVolume: " + ConvertToDecibel(value), this);
        }
        public void SetMasterVolume(float value) {
            mixer.SetFloat("MasterVolume", ConvertToDecibel(value));
        }
        public void SetAmbientVolume(float value) {
            mixer.SetFloat("AmbientVolume", ConvertToDecibel(value));
        }
        public void SetUIVolume(float value) {
            mixer.SetFloat("UIVolume", ConvertToDecibel(value));
        }

        public float GetMusicVolume() {
            float musicVolume;
            mixer.GetFloat("MusicVolume", out musicVolume);
            return musicVolume;
        }
        public float GetSoundEffectsVolume() {
            float soundEffectsVolume;
            mixer.GetFloat("SoundEffectsVolume", out soundEffectsVolume);
            return soundEffectsVolume;
        }

        /* Convert the value coming from our sliders to a decibel value we can feed into the audio mixer. */
        public float ConvertToDecibel(float value) {
            // Log(0) is undefined so we just set it by default to -80 decibels which is 0 volume in the audio mixer.
            float decibel = -80f;

            // I think the correct formula is Mathf.Log10(value / 100f) * 20f. Using that yields -6dB at 50% on the slider which is I think is half volume, but I don't feel like it sounds like half volume. :p And I also felt this homemade formula sounds more natural/linear when you go towards 0.
            if (value > 0) {
                decibel = Mathf.Log(value / 100f) * 17f;
            }

            return decibel;
        }
        #endregion
    }
}
