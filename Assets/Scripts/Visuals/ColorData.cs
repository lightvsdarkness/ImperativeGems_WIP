using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IG.General {
    [CreateAssetMenu(fileName = "ColorData", menuName = "Scriptable Objects/!AuVisuals/ColorData")]
    public class ColorData : ScriptableObject {
        public string Name;

        [Header("Game uses Color, but you can type numeric values and use UpdateColor context menu command")] [Tooltip("Color can be changed to numeric values through UpdateColor context menu command")] public Color32
            Color;

        [Space] [Tooltip("Numeric value can be changed to Color's value through UpdateRedGreenBlue context menu command")] public byte Red;
        [Tooltip("Numeric value can be changed to Color's value through UpdateRedGreenBlue context menu command")] public byte Green;
        [Tooltip("Numeric value can be changed to Color's value through UpdateRedGreenBlue context menu command")] public byte Blue;

        [Tooltip("Numeric value can be changed to Color's value through UpdateAlpha context menu command")] public byte
            Alpha = 255;

        public Action OnColorChanged;

        protected virtual void Start() {
            UpdateColor();
        }

        //protected virtual void Update() {

        //}

        [ContextMenu("UpdateColor")]
        protected void UpdateColor() {
            Color = new Color32(Red, Green, Blue, Alpha);

            OnColorChanged?.Invoke();
        }

        [ContextMenu("UpdateRedGreenBlue")]
        public void UpdateRedGreenBlue() {
            Red = Color.r;
            Green = Color.g;
            Blue = Color.b;

            OnColorChanged?.Invoke();
        }

        [ContextMenu("UpdateAlpha")]
        public void UpdateAlpha() {
            Alpha = Color.a;

            OnColorChanged?.Invoke();
        }

        [ContextMenu("RandomizeColor")]
        public void RandomizeColor() {
            Red = (byte) Random.Range(0, 255);
            Green = (byte) Random.Range(0, 255);
            Blue = (byte) Random.Range(0, 255);
            UpdateColor();
        }
    }
}