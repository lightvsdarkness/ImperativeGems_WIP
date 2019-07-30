using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace IG.Graphics {
    [CreateAssetMenu(fileName = "PostProcessingEffectLevels", menuName = "Scriptable Objects/Graphics/PostProcessingEffectLevels")]
    public partial class PostProcessingEffectLevels : ScriptableObject {
        //: ScriptableObject
        public List<PostProcessingEffectLevel> PPEffectLevels = new List<PostProcessingEffectLevel>();

        //private void Awake() {
        //    PPEffectLevels.Add(new PostProcessingEffectLevel("AmbientOcclusion"));
        //    PPEffectLevels.Add(new PostProcessingEffectLevel("AutoExposure"));
        //    PPEffectLevels.Add(new PostProcessingEffectLevel("Bloom"));
        //    PPEffectLevels.Add(new PostProcessingEffectLevel("ChromaticAberration"));
        //    PPEffectLevels.Add(new PostProcessingEffectLevel("ColorGrading"));
        //    PPEffectLevels.Add(new PostProcessingEffectLevel("DepthOfField"));
        //    PPEffectLevels.Add(new PostProcessingEffectLevel("Grain"));
        //    PPEffectLevels.Add(new PostProcessingEffectLevel("LensDistortion"));
        //    PPEffectLevels.Add(new PostProcessingEffectLevel("MotionBlur"));
        //    PPEffectLevels.Add(new PostProcessingEffectLevel("ScreenSpaceReflections"));
        //    PPEffectLevels.Add(new PostProcessingEffectLevel("Vignette"));
        //}

    }
}