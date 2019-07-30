using UnityEngine;
using System.Collections;

namespace IG.General {
    [CreateAssetMenu(fileName = "DataColorScheme", menuName = "Scriptable Objects/!AuVisuals/DataColorScheme", order = 1)]
    public partial class DataColorScheme : ScriptableObject, ISerializationCallbackReceiver {
        private byte ColorButtonClicked_R, ColorButtonClicked_G, ColorButtonClicked_B;
        private byte ColorButtonNotClicked_R, ColorButtonNotClicked_G, ColorButtonNotClicked_B;
        private byte BackgroundRed, BackgroundGreen, BackgroundBlue;

        public ColorData DefaultDataButtonClicked;
        public Color DefaultColorButtonClicked;

        public ColorData DefaultDataButtonNotClicked;
        public Color DefaultColorButtonNotClicked;

        public ColorData DefaultDataBackground;
        public Color DefaultColorBackground;

        public ColorData DefaultDataUIText;
        public Color DefaultColorUIText;


        [Space]
        public ColorData CurrentDataColorButtonClicked;
        public Color CurrentColorButtonClicked;

        public ColorData CurrentDataColorButtonNotClicked;
        public Color CurrentColorButtonNotClicked;

        public ColorData CurrentDataColorBackground;
        public Color CurrentColorBackground;

        public ColorData CurrentDataUIText;
        public Color CurrentColorUIText;

        // We can change Runtime colors however we want (procedural generation, color change effects, etc), but original color will stay the same
        [Space] public Color RundtimeCurrentColorButtonClicked;
        public Color RuntimeCurrentColorButtonNotClicked;
        public Color RuntimeCurrentColorBackground;
        public Color RuntimeCurrentColorUIText;

        private void Start() {
            UpdateColors();
        }

        [ContextMenu("UpdateColors")]
        public void UpdateColors() {
            UpdateDefaultColorsFromData();

            UpdateCurrentColorsFromData();

            RundtimeCurrentColorButtonClicked = CurrentColorButtonClicked;
            RuntimeCurrentColorButtonNotClicked = CurrentColorButtonNotClicked;
            RuntimeCurrentColorBackground = CurrentColorBackground;
            RuntimeCurrentColorUIText = CurrentColorUIText;
        }

        private void UpdateDefaultColorsFromData() {
            DefaultColorButtonClicked = new Color32(DefaultDataButtonClicked.Red, DefaultDataButtonClicked.Green,
                DefaultDataButtonClicked.Blue, 255);
            DefaultColorButtonNotClicked = new Color32(DefaultDataButtonNotClicked.Red,
                DefaultDataButtonNotClicked.Green, DefaultDataButtonNotClicked.Blue, 255);
            DefaultColorBackground = new Color32(DefaultDataBackground.Red, DefaultDataBackground.Green,
                DefaultDataBackground.Blue, 255);
            DefaultColorUIText = new Color32(DefaultDataUIText.Red, DefaultDataUIText.Green,
                DefaultDataUIText.Blue, 255);
        }
        private void UpdateCurrentColorsFromData() {
            CurrentColorButtonClicked = new Color32(CurrentDataColorButtonClicked.Red,
                CurrentDataColorButtonClicked.Green, CurrentDataColorButtonClicked.Blue, 255);
            CurrentColorButtonNotClicked = new Color32(CurrentDataColorButtonNotClicked.Red,
                CurrentDataColorButtonNotClicked.Green, CurrentDataColorButtonNotClicked.Blue, 255);
            CurrentColorBackground = new Color32(CurrentDataColorBackground.Red, CurrentDataColorBackground.Green,
                CurrentDataColorBackground.Blue, 255);
            CurrentColorUIText = new Color32(CurrentDataUIText.Red, CurrentDataUIText.Green,
                CurrentDataUIText.Blue, 255);
        }

        [ContextMenu("InstallColors from Default if Current are missing")]
        public void InstallColors() {
            if (CurrentDataColorButtonClicked == null || CurrentDataColorButtonNotClicked == null ||
                CurrentDataColorBackground == null || CurrentDataUIText == null)
            {
                CurrentDataColorButtonClicked = DefaultDataButtonClicked;
                CurrentDataColorButtonNotClicked = DefaultDataButtonNotClicked;
                CurrentDataColorBackground = DefaultDataBackground;
                CurrentDataUIText = DefaultDataUIText;
            }


            // TODO: This code probably doesn't work
            CurrentDataColorButtonClicked.OnColorChanged = UpdateColors;
            CurrentDataColorButtonNotClicked.OnColorChanged = UpdateColors;
            CurrentDataColorBackground.OnColorChanged = UpdateColors;
            CurrentDataUIText.OnColorChanged = UpdateColors;
        }


        /// <summary>
        /// OBSOLETE
        /// </summary>
        public void ChangeTheme() {
            switch (Random.Range(0, 2)) {
                case 0:
                    ColorButtonClicked_R = 255;
                    ColorButtonClicked_G = 90;
                    ColorButtonClicked_B = 24;

                    ColorButtonNotClicked_R = 255;
                    ColorButtonNotClicked_G = 255;
                    ColorButtonNotClicked_B = 0;

                    BackgroundRed = 77;
                    BackgroundGreen = 66;
                    BackgroundBlue = 109;
                    break;

                case 1:
                    ColorButtonClicked_R = 121;
                    ColorButtonClicked_G = 183;
                    ColorButtonClicked_B = 225;

                    ColorButtonNotClicked_R = 117;
                    ColorButtonNotClicked_G = 94;
                    ColorButtonNotClicked_B = 210;

                    BackgroundRed = 163;
                    BackgroundGreen = 234;
                    BackgroundBlue = 122;
                    break;

                case 2:
                    ColorButtonClicked_R = DefaultDataButtonClicked.Red;
                    ColorButtonClicked_G = DefaultDataButtonClicked.Green;
                    ColorButtonClicked_B = DefaultDataButtonClicked.Blue;

                    ColorButtonNotClicked_R = DefaultDataButtonNotClicked.Red;
                    ColorButtonNotClicked_G = DefaultDataButtonNotClicked.Green;
                    ColorButtonNotClicked_B = DefaultDataButtonNotClicked.Blue;

                    BackgroundRed = DefaultDataBackground.Red;
                    BackgroundGreen = DefaultDataBackground.Green;
                    BackgroundBlue = DefaultDataBackground.Blue;

                    break;
            }
            UpdateColors();

        }

        /// <summary>
        /// WIP
        /// </summary>
        public void RandomizeTheme() {

        }

        public void OnBeforeSerialize() {
        }

        public void OnAfterDeserialize() {
            RundtimeCurrentColorButtonClicked = CurrentColorButtonClicked;
            RuntimeCurrentColorButtonNotClicked = CurrentColorButtonNotClicked;
            RuntimeCurrentColorBackground = CurrentColorBackground;
        }
    }
}