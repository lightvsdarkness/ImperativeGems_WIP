using UnityEngine;
using System.Collections;
using IG.General;
using UnityEngine.UI;
using TMPro;

namespace IG.CGrid {
    /// <summary>
    /// UI-скрипт, показывающий счёт и создающий попап, показыывающий счёт
    /// </summary>
    public class UIClock : MonoBehaviour {
        public bool Debugging;

        [Space]
        public bool TickTocking;

        // --- Visuals
        [Space]
        [SerializeField] private Sprite _borderSprite;
        [SerializeField] private float _dangerScaleChange = 1.2f;

        [Space]
        [SerializeField] private Image _borderSpriteRenderer;
        [SerializeField] private TextMeshProUGUI _clockText;
        [Space]
        [SerializeField] private TextMeshProUGUI _waitText = null;

        [Space]
        [SerializeField] private AudioSource _audioSource;

        private DataColorScheme CSData;


        private void Start() {
            CSData = ManagerCGridGame.I.ColorScheme;

            if (_audioSource == null)
                _audioSource = gameObject.GetComponent<AudioSource>();

            if (_borderSpriteRenderer == null)
                _borderSpriteRenderer = GetComponent<Image>();
            if (_borderSpriteRenderer != null)
                _borderSpriteRenderer.color = CSData.RuntimeCurrentColorButtonNotClicked;

            if (_clockText == null)
                _clockText = GetComponent<TextMeshProUGUI>();
            if (_clockText == null)
                _clockText = GetComponentInChildren<TextMeshProUGUI>();

            if (_clockText != null)
                _clockText.color = CSData.RuntimeCurrentColorButtonNotClicked;


            if (_waitText != null) {
                //_waitText.GetComponent<MeshRenderer>().sortingOrder = 10;
                _waitText.color = CSData.RuntimeCurrentColorButtonNotClicked;
            }
            else
                Debug.LogError("No _waitText", this);

            ManagerClockScore.I.OnTimerChanged += OnTimerChanged;
            //ManagerCGridGame.I.OnGameStart += OnTimerChanged
        }

        public void UIShowTimeBeforeStart(float SecondsBeforeStart) {
            if (_waitText == null) return;

            _clockText.text = "";

            if (SecondsBeforeStart > 0) {
                _waitText.gameObject.SetActive(true);
                _waitText.text = SecondsBeforeStart.ToString();
            }
            else {
                _waitText.gameObject.SetActive(false);
                if (Debugging)
                    Debug.Log("Clock starts the ManagerCGridGame at: " + Time.realtimeSinceStartup + " CurrentTurn: " + ManagerClockScore.I.CurrentTurn);
            }
        }

        public void OnTimerChanged(int TurnsLeft) {
            _clockText.text = TurnsLeft.ToString();
        }

        public void UIAudioVisuals(int TurnsLeft) {
            // Because it's actually Rounds, and when you start having 0 rounds left (last one), you actually have 1 second left
            int textToShow = TurnsLeft + 1; 
            _clockText.text = textToShow.ToString();

            if (textToShow <= 3)
            {
                _borderSpriteRenderer.color = new Color32(255, 0, 0, 255);
                _clockText.color = Color.red;

                _borderSpriteRenderer.transform.localScale = new Vector2(_dangerScaleChange, _dangerScaleChange);
                _clockText.transform.localScale = new Vector2(_dangerScaleChange + 0.1f, _dangerScaleChange + 0.1f);

                Invoke("ResizeCircle", 0.3f);
                PlayTickTock();
            }
            else if (TurnsLeft % 2 == 0)
            {
                _borderSpriteRenderer.color = CSData.RuntimeCurrentColorButtonNotClicked;
                _clockText.color = CSData.RuntimeCurrentColorButtonNotClicked;

                StopTickTock();
            }
            else
            {
                _borderSpriteRenderer.color = CSData.RundtimeCurrentColorButtonClicked;
                _clockText.color = CSData.RundtimeCurrentColorButtonClicked;

                StopTickTock();
            }

        }
    

        private void ResizeCircle() {
            _borderSpriteRenderer.transform.localScale = new Vector2(1f, 1f);
            _clockText.transform.localScale = new Vector2(1f, 1f);
        }

        private void PlayTickTock() {
            if (!TickTocking)
            {
                _audioSource.Play();
                TickTocking = true;
            }
        }
        private void StopTickTock() {
            if (TickTocking)
            {
                _audioSource.Stop();
                TickTocking = false;
            }
        }
    }
}