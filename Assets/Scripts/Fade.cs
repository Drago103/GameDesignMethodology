using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public float speed = 1f;

    private Image fadeImage;
    private float targetAlpha = 0f;

    private void Awake()
    {
        fadeImage = GetComponent<Image>();

        // Force start transparent
        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;
    }

    private void Update()
    {
        Color c = fadeImage.color;
        c.a = Mathf.MoveTowards(c.a, targetAlpha, speed * Time.deltaTime);
        fadeImage.color = c;
    }

    public void FadeOut() => targetAlpha = 1f;
    public void FadeIn() => targetAlpha = 0f;
}
