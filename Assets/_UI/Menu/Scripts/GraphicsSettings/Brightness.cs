namespace IG.Graphics
{
    public class Brightness : GraphicsSlider {

        protected override void Start() {
            base.Start();
            DisplayValue.text = Value.ToString() + "%";
        }

        protected override void GraphicsPresetLow() {
            Value = 100;
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
            SetBrightness();
        }

        protected override void OnSliderValueChangeSetDisplayText() {
            DisplayValue.text = Value.ToString() + "%";
        }

        protected void SetBrightness() {
            //Cam.GetComponent<BrightnessEffect>().brightness = Slider.value / 100f;
        }
    }
}
