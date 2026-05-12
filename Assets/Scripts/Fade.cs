using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public Image fadeImage;
    public float speed = 1f;

    private float targetAlpha = 0f;

    void Update()
    {
        Color c = fadeImage.color;
        c.a = Mathf.MoveTowards(c.a, targetAlpha, speed * Time.deltaTime);
        fadeImage.color = c;
    }

    public void FadeOut() => targetAlpha = 1f;
    public void FadeIn() => targetAlpha = 0f;
}
