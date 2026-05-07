using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Fade : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
    }

    public IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);

            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;

            yield return null;
        }
    }

    public IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);

            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;

            yield return null;
        }
    }
}
