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
        // �����̴� �ʱ�ȭ
        loadingSlider.value = 0f;
        loadingPercentageText.text = "0%";

        // ���� �ִϸ��̼� ���
        if (sliderTween != null && sliderTween.IsActive())
        {
            sliderTween.Kill();
        }
    }

    private void OnDisable()
    {
        // �ִϸ��̼� ���
        if (sliderTween != null && sliderTween.IsActive())
        {
            sliderTween.Kill();
        }
    }

    /// <summary>
    /// �����̴��� ��ǥ ���൵�� �����ϰ� �ִϸ��̼��մϴ�.
    /// </summary>
    /// <param name="progress">��ǥ ���൵ (0 ~ 100)</param>
    public void SetTargetProgress(float progress)
    {
        float clampedProgress = Mathf.Clamp(progress, 0f, 100f) / 100f;

        // ���� �ִϸ��̼��� �ִٸ� ���
        if (sliderTween != null && sliderTween.IsActive())
        {
            sliderTween.Kill();
        }

        // �����̴� ���� Tween
        sliderTween = loadingSlider.DOValue(clampedProgress, 1f)
            .SetEase(Ease.Linear)
            .OnUpdate(UpdatePercentageText);
        UpdateDescription($"{progress}%");
    }

    /// <summary>
    /// �����̴� ���� ���� �ۼ�Ƽ�� �ؽ�Ʈ�� ������Ʈ�մϴ�.
    /// </summary>
    private void UpdatePercentageText()
    {
        float displayedValue = loadingSlider.value * 100f;
        loadingPercentageText.text = $"{Mathf.RoundToInt(displayedValue)}%";
    }

    /// <summary>
    /// ���� �ε� ���� �۾��� ������Ʈ�մϴ�.
    /// </summary>
    /// <param name="description">�ε� ���� �۾� ����</param>
    public void UpdateDescription(string description)
    {
        loadingDescriptionText.text = description;
    }
}
