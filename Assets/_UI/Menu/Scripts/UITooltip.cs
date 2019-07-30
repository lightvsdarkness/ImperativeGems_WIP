using IG.General;
using UnityEngine;
using UnityEngine.UI;

namespace IG
{
    public class UITooltip : SingletonManager<UITooltip>
    {
        public GameObject tooltipGameObject;
        [SerializeField]
        private RectTransform rect;
        [SerializeField]
        private Image imageComponent;
        [SerializeField]
        private Text textComponent;

        Color originalColor;
        Color originalTextColor;
        bool mouseover;
        bool wait;

        protected void Start() {
            if (tooltipGameObject == null) tooltipGameObject = GameObject.Find("Tooltip");
            if (rect == null) rect = tooltipGameObject.GetComponent<RectTransform>();
            if (imageComponent == null) imageComponent = tooltipGameObject.GetComponentInChildren<Image>();
            if (textComponent == null) textComponent = imageComponent.GetComponentInChildren<Text>();

            originalColor = imageComponent.color;
            originalTextColor = textComponent.color;
            imageComponent.color = Color.clear;
            textComponent.color = Color.clear;
            tooltipGameObject.SetActive(false);

        }

        [SerializeField]
        bool mouseOver;
        bool fadingIn;
        bool fadingOut;
        float waitDuration = 1f;
        float fadeInDuration = 0.5f;
        float fadeOutDuration = 0.25f;
        float t;

        void Update() {
            t += Time.deltaTime;

            if (mouseOver)
            {
                if (t > waitDuration && !tooltipGameObject.activeSelf)
                {
                    tooltipGameObject.SetActive(true);
                    t = 0;
                }
                if (t < fadeInDuration && tooltipGameObject.activeSelf)
                {
                    imageComponent.color = Color.Lerp(imageComponent.color, originalColor, t / fadeInDuration);
                    textComponent.color = Color.Lerp(textComponent.color, originalTextColor, t / fadeInDuration);
                }
            }
            else
            {
                if (t < fadeOutDuration)
                {
                    imageComponent.color = Color.Lerp(imageComponent.color, Color.clear, t / fadeOutDuration);
                    textComponent.color = Color.Lerp(textComponent.color, Color.clear, t / fadeOutDuration);
                }
                else
                {
                    tooltipGameObject.SetActive(false);
                }
            }

            if (tooltipGameObject.activeSelf)
            {
                rect = tooltipGameObject.GetComponent<RectTransform>();

                Vector3 offset = new Vector3(rect.sizeDelta.x / 2, rect.sizeDelta.y / 2, 0);
                tooltipGameObject.transform.position = Input.mousePosition + offset;

                // Make sure the tooltip can't leave the screen boundries.
                // We don't have to worry about the left side or the bottom because our tooltip is always on the top right.
                if (tooltipGameObject.transform.position.x + offset.x > Screen.width)
                {
                    float diff = (tooltipGameObject.transform.position.x + offset.x) - Screen.width;
                    tooltipGameObject.transform.position = new Vector3(tooltipGameObject.transform.position.x - diff, tooltipGameObject.transform.position.y, tooltipGameObject.transform.position.z);
                }
                if (tooltipGameObject.transform.position.y + offset.y > Screen.height)
                {
                    float diff = (tooltipGameObject.transform.position.y + offset.y) - Screen.height;
                    tooltipGameObject.transform.position = new Vector3(tooltipGameObject.transform.position.x, tooltipGameObject.transform.position.y - diff, tooltipGameObject.transform.position.z);
                }
            }

            if (!UIMenuPanel.I.panelOpened && !UIMenuPanel.I.menuOpened)
                Deactivate(1);
        }

        public void Activate(string text) {
            mouseOver = true;
            textComponent.text = text;
            t = 0;
        }

        public void Deactivate() {
            mouseOver = false;
            t = 0;
        }
        /// <summary>
        /// Deactivate with fading out during specified time, not working for now
        /// </summary>
        /// <param name="time"></param>
        public void Deactivate(float time) {
            mouseOver = false;
            t = 0;
        }
    }
}
