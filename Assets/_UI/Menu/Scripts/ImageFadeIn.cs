using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace IG {
    public class ImageFadeIn : MonoBehaviour {

        Image image;

        void OnEnable() {
            image = GetComponent<Image>();
            StartCoroutine("FadeInImage");
        }

        IEnumerator FadeInImage() {
            float t = 0;
            while (t < 30f) {
                image.color = Color.Lerp(new Color(0, 0, 0, 0), Color.white, t / 30f);
                t += Time.deltaTime;
                yield return null;
            }
        }
    }
}
