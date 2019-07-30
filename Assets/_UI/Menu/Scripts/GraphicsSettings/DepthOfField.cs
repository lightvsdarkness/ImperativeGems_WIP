using UnityEngine.Rendering.PostProcessing;

namespace IG.Graphics
{
    public class DepthOfField : GraphicsSlider {

        protected override void GraphicsPresetLow() {
            Value = System.Convert.ToInt16(false);
        }

        protected override void GraphicsPresetMedium() {
            Value = System.Convert.ToInt16(false);
        }

        protected override void GraphicsPresetHigh() {
            Value = System.Convert.ToInt16(false);
        }

        protected override void GraphicsPresetUltra() {
            Value = System.Convert.ToInt16(true);
        }

        protected override void OnSliderValueChange() {
            SetDof(System.Convert.ToBoolean(Value));
        }

        void SetDof(bool value) {
            //cam.GetComponent<PostProcessingBehaviour>().profile.depthOfField.enabled = value;
        }
    }
}
