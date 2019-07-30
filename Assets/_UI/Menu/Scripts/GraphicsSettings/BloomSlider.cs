using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace IG.Graphics
{
    public class BloomSlider : GraphicsSlider {

        protected override void Start() {
            base.Start();
            DisplayValue.text = Value.ToString() + "%";
        }

        protected override void OnSliderValueChangeSetDisplayText() {
            if (Value == 0) {
                DisplayValue.text = "Off";
            }
            else {
                DisplayValue.text = Value.ToString() + "%";
            }
        }

        protected override void GraphicsPresetLow() {
            Value = 0;
        }

        protected override void GraphicsPresetMedium() {
            Value = 100;
        }

        protected override void GraphicsPresetHigh() {
            Value = 100;
        }

        protected override void GraphicsPresetUltra() {
            Value = 100;
        }

        protected override void OnSliderValueChange() {
            SetBloom(Value);
        }



        void SetBloom(float value) {
            //Bloom bloom;
            //PostProcessingVolume.sharedProfile.TryGetSettings(out bloom);

            //bloom.intensity.value = 0.2f * (value / 100);
            //bloom.color.value = Color.white;
            //if (value <= 0)
            //    bloom.enabled.value = false;
            //else
            //    bloom.enabled.value = true;

            //BloomModel bloom = cam.GetComponent<PostProcessingBehaviour>().profile.bloom;

            //BloomModel.Settings tempSettings = bloom.settings;
            //tempSettings.bloom.intensity = 0.2f * (value / 100);
            //bloom.settings = tempSettings;

            //if (value <= 0)
            //    bloom.enabled = false;
            //else
            //    bloom.enabled = true;
        }
    }
}
