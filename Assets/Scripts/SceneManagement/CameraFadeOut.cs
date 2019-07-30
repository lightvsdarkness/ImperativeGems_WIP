using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFadeOut : MonoBehaviour
{
    //public Image blackScreen;
    public Image blackScreen;
    public float fadingTime = 1.0f;

    private bool isFading;                          // Flag used to determine if the Image is currently fading to or from black.

    public CanvasGroup faderCanvasGroup;            // The CanvasGroup that controls the Image used for fading to black.
    public float fadeDuration = 1f;                 // How long it should take to fade to and from black.

    //void Start () {
    //}
    //void Update () {
    //}

    public void FadeToBlack(float fadingOutTime) {
        blackScreen.color = Color.black;
        blackScreen.canvasRenderer.SetAlpha(1.0f);

        if (fadingOutTime != 0)
        {
            blackScreen.CrossFadeAlpha(1.0f, fadingOutTime, false);
        }
        else
        {
            blackScreen.CrossFadeAlpha(1.0f, fadingTime, false);
        }

    }

    public void FadeFromBlack(float fadingInTime) {
        blackScreen.color = Color.black;
        blackScreen.canvasRenderer.SetAlpha(1.0f);
        if (fadingInTime != 0)
            blackScreen.CrossFadeAlpha(0.0f, fadingInTime, false);
        else
        {
            blackScreen.CrossFadeAlpha(0.0f, fadingTime, false);
        }
    }


    //private IEnumerator Fade(float finalAlpha) {
    //    // Set the fading flag to true so the FadeAndSwitchScenes coroutine won't be called again.
    //    isFading = true;

    //    // Make sure the CanvasGroup blocks raycasts into the scene so no more input can be accepted.
    //    faderCanvasGroup.blocksRaycasts = true;

    //    // Calculate how fast the CanvasGroup should fade based on it's current alpha, it's final alpha and how long it has to change between the two.
    //    float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

    //    // While the CanvasGroup hasn't reached the final alpha yet...
    //    while (!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
    //    {
    //        // ... move the alpha towards it's target alpha.
    //        faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha,
    //            fadeSpeed * Time.deltaTime);

    //        // Wait for a frame then continue.
    //        yield return null;
    //    }

    //    // Set the flag to false since the fade has finished.
    //    isFading = false;

    //    // Stop the CanvasGroup from blocking raycasts so input is no longer ignored.
    //    faderCanvasGroup.blocksRaycasts = false;
    //}
}