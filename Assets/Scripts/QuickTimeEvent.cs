using UnityEngine;
using UnityEngine.UI;

public class QuickTimeEvent : MonoBehaviour
{

    public Slider slider;
   
   public void SetMaxDuration(float duration)
    {
        slider.maxValue = duration;
        slider.value = duration;
    }

    public void SetDuration(float time)
    {
        slider.value = time;
    }

    public void ResetUI()
    {
        slider.value = 0f;
        //slider.gameObject.SetActive(false);
    }
}
