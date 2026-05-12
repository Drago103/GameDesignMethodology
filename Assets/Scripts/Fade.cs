using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public Image fadeImage;
    public float speed = 1f;

    private float targetAlpha = 0f;

    private void Awake()
    {
        if (fadeImage == null)
            fadeImage = GetComponent<Image>();

        // Force alpha to 0 at start
        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;

        Debug.Log("FADE: Starting alpha = " + fadeImage.color.a);
    }

    void Update()
    {
        Color c = fadeImage.color;
        float oldA = c.a;

        c.a = Mathf.MoveTowards(c.a, targetAlpha, speed * Time.deltaTime);
        fadeImage.color = c;

        if (Mathf.Abs(oldA - c.a) > 0.001f)
            Debug.Log("FADE: Alpha = " + c.a);
    }

    public void FadeOut()
    {
        Debug.Log("FADE: FadeOut() called");
        targetAlpha = 1f;
    }

    public void FadeIn()
    {
        Debug.Log("FADE: FadeIn() called");
        targetAlpha = 0f;
    }
}
