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
        // 슬라이더 초기화
        loadingSlider.value = 0f;
        loadingPercentageText.text = "0%";

        // 기존 애니메이션 취소
        if (sliderTween != null && sliderTween.IsActive())
        {
            sliderTween.Kill();
        }
    }

    private void OnDisable()
    {
        // 애니메이션 취소
        if (sliderTween != null && sliderTween.IsActive())
        {
            sliderTween.Kill();
        }
    }

    /// <summary>
    /// 슬라이더의 목표 진행도를 설정하고 애니메이션합니다.
    /// </summary>
    /// <param name="progress">목표 진행도 (0 ~ 100)</param>
    public void SetTargetProgress(float progress)
    {
        float clampedProgress = Mathf.Clamp(progress, 0f, 100f) / 100f;

        // 기존 애니메이션이 있다면 취소
        if (sliderTween != null && sliderTween.IsActive())
        {
            sliderTween.Kill();
        }

        // 슬라이더 값을 Tween
        sliderTween = loadingSlider.DOValue(clampedProgress, 1f)
            .SetEase(Ease.Linear)
            .OnUpdate(UpdatePercentageText);
        UpdateDescription($"{progress}%");
    }

    /// <summary>
    /// 슬라이더 값에 따라 퍼센티지 텍스트를 업데이트합니다.
    /// </summary>
    private void UpdatePercentageText()
    {
        float displayedValue = loadingSlider.value * 100f;
        loadingPercentageText.text = $"{Mathf.RoundToInt(displayedValue)}%";
    }

    /// <summary>
    /// 현재 로딩 중인 작업을 업데이트합니다.
    /// </summary>
    /// <param name="description">로딩 중인 작업 설명</param>
    public void UpdateDescription(string description)
    {
        loadingDescriptionText.text = description;
    }
}
