using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace IG.VFX {
    [RequireComponent(typeof (LineRenderer))]
    public class F3DBeam1 : MonoBehaviour {
        public Texture[] BeamFrames; // Animation frame sequence
        public float FrameStep; // Animation time

        public float beamScale; // Default beam scale to be kept over distance

        public bool AnimateUV = true; // UV Animation
        public float UVTime; // UV Animation speed

        //public Transform rayImpact; // Impact transform
        //public Transform rayMuzzle; // Muzzle flash transform

        private LineRenderer lineRenderer; // Line rendered component
        public Vector3[] LineRenderersPositions;

        private float animateUVTime;

        private int frameNo; // Frame counter
        private int FrameTimerID; // Frame timer reference
        private float beamLength; // Current beam length
        private float initialBeamOffset; // Initial UV offset  

        void Awake() {

        }

        // OnSpawned called by pool manager 
        void Start() {
            if (lineRenderer == null)
                lineRenderer = GetComponent<LineRenderer>();

            // Assign first frame texture
            if (!AnimateUV && BeamFrames.Length > 0)
                lineRenderer.material.mainTexture = BeamFrames[0];

            // Randomize uv offset
            initialBeamOffset = Random.Range(0f, 5f);

            //AdjustMaterialToLength();

            if (BeamFrames.Length > 1)
                StartAnimation();
        }

        void Update() {
            // Animate texture UV
            if (AnimateUV) {
                animateUVTime += Time.deltaTime;

                if (animateUVTime > 1.0f)
                    animateUVTime = 0f;

                lineRenderer.material.SetTextureOffset("_MainTex", new Vector2(animateUVTime * UVTime + initialBeamOffset, 0f));
            }

            AdjustMaterialToLength();
        }

        void AdjustMaterialToLength() {
            // Get current beam length and update line renderer accordingly
            beamLength = 0;
            LineRenderersPositions = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(LineRenderersPositions);

            for (int i = 1; i < LineRenderersPositions.Length; i++) {
                beamLength += Vector3.Distance(LineRenderersPositions[i - 1], LineRenderersPositions[i]);
            }

            // Calculate default beam proportion multiplier based on default scale and current length
            var propMult = beamLength * (beamScale / 10f);

            // Set beam scaling according to its length
            lineRenderer.material.SetTextureScale("_MainTex", new Vector2(propMult, 1f));
        }

        void StartAnimation() {
            if (BeamFrames.Length > 1) {
                // Set initial frame
                frameNo = 0;
                lineRenderer.material.mainTexture = BeamFrames[frameNo];

                // Add timer 
                FrameTimerID = F3DTime1.time.AddTimer(FrameStep, BeamFrames.Length - 1, OnFrameStep);

                frameNo = 1;
            }
        }

        // Advance texture frame
        void OnFrameStep() {
            // Set current texture frame based on frame counter
            lineRenderer.material.mainTexture = BeamFrames[frameNo];
            frameNo++;

            // Reset frame counter
            if (frameNo == BeamFrames.Length)
                frameNo = 0;
        }

    }
}