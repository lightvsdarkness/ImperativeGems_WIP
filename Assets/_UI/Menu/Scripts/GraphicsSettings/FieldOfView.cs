using UnityEngine;

namespace IG.Graphics
{
    public class FieldOfView : GraphicsSlider {

        protected override void Start() {
            base.Start();
            SetFieldOfViewDisplayText();
        }

        /* It's a Data and it goes to PostProcessingEffectLevel */
        protected override void GraphicsPresetLow() {
            Value = 60;
        }

        protected override void GraphicsPresetMedium() {
            Value = 60;
        }

        protected override void GraphicsPresetHigh() {
            Value = 60;
        }

        protected override void GraphicsPresetUltra() {
            Value = 60;
        }

        protected override void OnSliderValueChange() {
            SetFieldOfViewValue(Value);
            
        }

        private void SetFieldOfViewValue(int value) {
            //Cam.fieldOfView = value;
        }

        protected override void OnSliderValueChangeSetDisplayText() {
            SetFieldOfViewDisplayText();
        }

        private void SetFieldOfViewDisplayText() {
        //    float hFov = 2 * Mathf.Atan(Cam.aspect * Mathf.Tan(Mathf.Deg2Rad * Value / 2));
        //    hFov *= Mathf.Rad2Deg;
        //    DisplayValue.text = Value.ToString() + " (" + hFov.ToString("0") + ")";
        }
    }
}
