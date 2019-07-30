using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IG {
    public class Selectable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, ISubmitHandler, ISelectHandler {
        public UIMenuPanel UIMenu;

        [Space]
        [TextArea]
        public string tooltipText;

        // The colors for the states for this button.
        public Color normalColor;
        public Color highlightColorInitial;
        public Color highlightColorFadeTo;
        public float fadeSpeed = 0.75f;
        float t;
        bool fadeDown = true;

        Button button;
        Slider slider;

        Image image;
        Vector3 lastPointerPositon;

        void Start() {
            UIMenu = GetComponentInParent<UIMenuPanel>();
            if (UIMenu == null)
                Debug.Log("Using Selectable without parent UIMenuPanel", this);

            image = GetComponent<Image>();
            button = GetComponent<Button>();
            slider = GetComponent<Slider>();

            if (slider != null) {
                slider.onValueChanged.AddListener(delegate { OnSliderValueChange(); });
            }
        }

        /** All the fluff we're doing in the events below should allow the mouse
         * and the keyboard to interact properly together when switching between
         * mouse and keyboard for navigating our menus */
        void Update() {
            // If we're currently hovering over the button and we moved our mouse switch the selection to this button
            if (UIMenu.currentMouseOverGameObject == gameObject && Input.mousePosition != lastPointerPositon) {
                EventSystem.current.SetSelectedGameObject(gameObject);
            }

            // Set the color of the button depending on the selected state.
            if (EventSystem.current.currentSelectedGameObject == gameObject) {
                if (t <= 1f) {
                    t += Time.deltaTime / fadeSpeed;
                    if (fadeDown) {
                        image.color = Color.Lerp(highlightColorInitial, highlightColorFadeTo, t);
                    }
                    else {
                        image.color = Color.Lerp(highlightColorFadeTo, highlightColorInitial, t);
                    }

                }
                else {
                    t = 0;
                    fadeDown = !fadeDown;
                }
            }
            else {
                image.color = normalColor;
            }

            // Save the current mouse position for next frame so we can compare it.
            lastPointerPositon = Input.mousePosition;
        }


        public void OnPointerEnter(PointerEventData eventData) {
            UIMenu.currentMouseOverGameObject = gameObject;
            EventSystem.current.SetSelectedGameObject(gameObject);
            UITooltip.I.Activate(tooltipText);
        }
        public void OnPointerExit(PointerEventData eventData) {
            UIMenu.currentMouseOverGameObject = null;
            UITooltip.I.Deactivate();
        }

        // Handle navigating to the button with the arrow keys on the keyboard.
        public void OnSelect(BaseEventData eventData) {
            AudioManager.I.PlayHoverSound();
        }

        // Handle clicking the button with the mouse.
        public void OnPointerUp(PointerEventData eventData) {
            // We only need this for buttons.
            if (button == null) {
                return;
            }

            // Set the selected game object in the event system to this button so
            // it works properly if we switch to the keyboard.
            UIMenu.currentButton = button;
            AudioManager.I.PlayClickSound();
        }

        // Handle pressing ENTER or SPACE on the keyboard.
        public void OnSubmit(BaseEventData eventData) {
            // We only need this for buttons.
            if (button == null) {
                return;
            }

            UIMenu.currentButton = button;
            AudioManager.I.PlayClickSound();
        }

        // Play the 'tick' sound when we move a slider.
        public void OnSliderValueChange() {
            AudioManager.I.PlaySliderSound();
        }
    }
}
