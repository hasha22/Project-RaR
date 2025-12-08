using UnityEngine;
using UnityEngine.UI;

public class ResourceBarUI : MonoBehaviour
{
    [Header("Setup")]
    public Slider slider;
    public float lerpSpeed = 5f;
    private float targetValue;
    public void SetMaxValue(float max)
    {
        slider.maxValue = max;
    }
    public void SetValue(int current)
    {
        targetValue = (float)current;
    }
    private void Update()
    {
        slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * lerpSpeed);
    }
}
