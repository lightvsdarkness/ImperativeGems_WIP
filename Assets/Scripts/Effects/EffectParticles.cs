using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IG.General {
    public enum MaterialColorName {
        _TintColor,
        _Color
    }

    public class EffectParticles : MonoBehaviour {
        public bool Debugging;

        [Space]
        public bool ChangeColor;
        [Tooltip("Only color, opacity is taken from EndOpacity")]
        public Color EndColor; // can be set through code prior to starting

        [Space]
        public float StartingOpacity = 0; // can be set through code prior to starting
        public float EndOpacity = 1; // can be set through code prior to starting
        public float OpacityChangePeriod = 1f;

        [Space]
        public MaterialColorName MaterialColorName;
        private string _colorName;

        [Space]
        public List<ParticleSystem> ParticleSystems = new List<ParticleSystem>();
        public List<ParticleSystemRenderer> ParticleSystemRenderers = new List<ParticleSystemRenderer>();
        public List<Color> ColorOriginalParticleSystems = new List<Color>();

        [HideInInspector] public bool Appearing;
        [HideInInspector] public bool Disappearing;

        [Header("Visible for Debugging purposes")] public float TimeAlive = 0f;
        public Color IndicatorColorStarting;
        public Color IndicatorColorEnding;
        public Color IndicatorColorLerping;

        private void Update() {
            TimeAlive += Time.deltaTime;
        }

        private void Start() {
            if (ParticleSystems.Count == 0)
                ParticleSystems = GetComponentsInChildren<ParticleSystem>().ToList();
            if (ParticleSystemRenderers.Count == 0)
                ParticleSystemRenderers = GetComponentsInChildren<ParticleSystemRenderer>().ToList();

            if (MaterialColorName == MaterialColorName._TintColor)
                _colorName = "_TintColor";
            else
                _colorName = "_Color";

            // Memorize Original Colors
            if (ChangeColor)
                foreach (var renderer in ParticleSystemRenderers) {
                    ColorOriginalParticleSystems.Add(renderer.material.GetColor(_colorName));
                }

            //SlowlyStart();
        }

        [ContextMenu("Slowly DestroyTest")]
        public void SlowlyDestroyTest() {
            SlowlyDestroy(3f);
        }

        public void SlowlyDestroy(float destructionDelay = 3f) {
            if (Debugging) Debug.Log("Destroying in: " + destructionDelay, this);

            RestoreState();

            StartCoroutine(Delay(destructionDelay, SlowlyDestroyInternal));
        }


        /// <summary>
        /// Particle system should go transparent in last second of existence (if lifetime > 3f)
        /// </summary>
        public IEnumerator Delay(float timerDelay, Action<float> action) {
            float animateDestructionTime = 0;

            if (timerDelay >= 3f) {
                timerDelay--;
                animateDestructionTime = 1f;
            }
            else if (timerDelay >= 1f) {
                timerDelay -= 0.5f;
                animateDestructionTime = 0.5f;
            }
            else if (timerDelay >= 0.3f) {
                timerDelay -= 0.1f;
                animateDestructionTime = 0.1f;
            }
            else {
                animateDestructionTime = 0f;
            }

            while (timerDelay > 0f) {
                timerDelay -= Time.deltaTime;
                yield return null;
            }

            action?.Invoke(animateDestructionTime);
        }

        /// <summary>
        /// Particle system should go transparent in last second of existence (if lifetime > 3f)
        /// </summary>
        public void SlowlyDestroyInternal(float animateDestructionTime) {
            //Disappearing = true;
            if (Debugging) Debug.Log("SlowlyDestroyInternal", this);

            if (animateDestructionTime > 0f) {
                foreach (var PSR in ParticleSystemRenderers) {
                    StartCoroutine(ChangeMaterialOpacity(PSR, EndOpacity, StartingOpacity, false,
                        () => { Disappearing = false; }, animateDestructionTime, false));
                }
            }
            Destroy(gameObject, animateDestructionTime);
        }

        [ContextMenu("Slowly Start")]
        public void SlowlyStart() {
            //Appearing = true;
            if (Debugging) Debug.Log("SlowlyStart", this);

            RestoreState();
            foreach (var PSR in ParticleSystemRenderers) {
                StartCoroutine(ChangeMaterialOpacity(PSR, StartingOpacity, EndOpacity, false, () => { Appearing = false; }, OpacityChangePeriod,
                    true));
            }
        }

        public void ChangeColorAndTurnOff() {
            SlowlyChangeColor(OpacityChangePeriod);
            TurnOff(OpacityChangePeriod);
        }


        // TODO: Probably will want in the future not to turn off after color change
        public void SlowlyChangeColor(float opacityChangePeriod, Action callback = null) {
            if (Debugging) Debug.Log("SlowlyChangeColor", this);

            foreach (var PSR in ParticleSystemRenderers) {
                StartCoroutine(ChangeMaterialOpacity(PSR, StartingOpacity, EndOpacity, ChangeColor, callback, OpacityChangePeriod, true));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private IEnumerator ChangeMaterialOpacity(ParticleSystemRenderer PSR, float startingValue, float endingValue, bool changeColor = false,
            Action callback = null, float lerpTime = -1, bool? cosinus = null) {
            //PSR.GetComponent<ParticleSystem>().Play();

            Color materialColor = PSR.material.GetColor(_colorName);
            IndicatorColorStarting = new Color(materialColor.r, materialColor.g, materialColor.b, startingValue);
            if (changeColor)
                IndicatorColorEnding = new Color(EndColor.r, EndColor.g, EndColor.b, endingValue);
            else
                IndicatorColorEnding = new Color(materialColor.r, materialColor.g, materialColor.b, endingValue);
            float CurrentLerp = 0f;

            if (lerpTime < 0) // If we are not given lerpTime, use default
                lerpTime = OpacityChangePeriod; //OpacityChangePeriod - 1
            if (lerpTime < 0)
                lerpTime = 0;

            //if (Debugging) Debug.Log("Started opacity change from " + IndicatorColorStarting.a + " To " + IndicatorColorEnding.a + ". Time to change: " + lerpTime, this);

            while (CurrentLerp < lerpTime) {
                yield return new WaitForFixedUpdate();

                CurrentLerp += Time.deltaTime;
                if (CurrentLerp > lerpTime) {
                    //if (Debugging) Debug.Log("CurrentLerp = " + CurrentLerp + ". Time: " + Time.time, this);
                    CurrentLerp = lerpTime;
                }
                float t = CurrentLerp / lerpTime;

                // --- Change t in order to get in/out effcts
                if (cosinus == true) // ...and “ease in” with coserp and division if cosinus selected
                    t = (1f - Mathf.Cos(t*Mathf.PI*0.5f))/8;
                else if (cosinus == false) // ...and "ease out" with sinerp if cosinus false
                    t = Mathf.Sin(t*Mathf.PI*0.5f);
                //if (Debugging) Debug.Log("real lerp (t) = " + t, this);

                IndicatorColorLerping = Color.Lerp(IndicatorColorStarting, IndicatorColorEnding, t);
                PSR.material.SetColor(_colorName, IndicatorColorLerping);
            }
            PSR.material.SetColor(_colorName, IndicatorColorEnding);

            //if (Debugging) Debug.Log("Ended opacity change from " + IndicatorColorStarting.a + " To " + IndicatorColorEnding.a + ". Time: " + Time.time, this);

            yield return null;
            callback?.Invoke();
        }

        public void RestoreState() {
            if (ChangeColor)
            for (int i = 0; i < ParticleSystemRenderers.Count; i++) {
                ParticleSystemRenderers[i].material.SetColor(_colorName, ColorOriginalParticleSystems[i]);
            }
            else
                for (int i = 0; i < ParticleSystemRenderers.Count; i++) {
                    ParticleSystemRenderers[i].material.SetColor(_colorName, Color.white);
                }
        }

        public void TurnOff(float opacityChangePeriod) {
            StartCoroutine(TurnPSROff(opacityChangePeriod));
        }

        private IEnumerator TurnPSROff(float opacityChangePeriod) {
            yield return new WaitForSeconds(opacityChangePeriod);
            RestoreState();
            yield return null;
            gameObject.SetActive(false);
        }
    }
}