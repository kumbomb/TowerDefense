using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class LoadingUI : MonoBehaviour
{
    public Slider loadingSlider;
    public TextMeshProUGUI loadingPercentageText;
    public TextMeshProUGUI loadingDescriptionText;

    private Tween sliderTween;

    private void OnEnable()
    {
        loadingSlider.value = 0f;
        loadingPercentageText.text = "0%";

        if (sliderTween != null && sliderTween.IsActive())
        {
            sliderTween.Kill();
        }
    }

    private void OnDisable()
    {
        if (sliderTween != null && sliderTween.IsActive())
        {
            sliderTween.Kill();
        }
    }
    public void SetTargetProgress(float progress)
    {
        float clampedProgress = Mathf.Clamp(progress, 0f, 100f) / 100f;

        if (sliderTween != null && sliderTween.IsActive())
        {
            sliderTween.Kill();
        }

        sliderTween = loadingSlider.DOValue(clampedProgress, 1f)
            .SetEase(Ease.Linear)
            .OnUpdate(UpdatePercentageText);
        UpdateDescription($"{progress}%");
    }
    private void UpdatePercentageText()
    {
        float displayedValue = loadingSlider.value * 100f;
        loadingPercentageText.text = $"{Mathf.RoundToInt(displayedValue)}%";
    }

    public void UpdateDescription(string description)
    {
        loadingDescriptionText.text = description;
    }
}
