using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    public GameObject waveCounterUI;
    public CanvasGroup waveCounterCG;

    public TextMeshProUGUI waveTypeText;
    public CanvasGroup waveTypeCG;

    private TextMeshProUGUI waveCounterUItext;
    public Transform screenCenter;
    public Transform waveCounterPos;
    
    public Image[] heartImages;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private Vector3 doubleScale = new Vector3(2, 2, 1);
    private Vector3 halfScale = new Vector3(1, 1, 1);
    private bool waveCounterCentered;
    private bool RunningOpeningWaveCounterAnimation;

    private int currentWaveNumber = 1;

    public TextMeshProUGUI coinCountUItext;

    public Action OnWaveNumberAnimationComplete;

    private void Awake()
    {
        waveCounterCG.alpha = 0f;
        waveCounterUItext = waveCounterUI.GetComponent<TextMeshProUGUI>();

        if (waveCounterUItext == null)
            waveCounterUItext = waveCounterUI.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        RunningOpeningWaveCounterAnimation = true;
        ResetWaveCounterPosition();
        PlayOpeningWaveCounterAnimation();
    }

    public void ShowWave(int waveNumber)
    {
        waveCounterUItext.text = $"WAVE {waveNumber}";
        PlayOpeningWaveCounterAnimation();
    }

    private void PlayOpeningWaveCounterAnimation()
    {

        ResetWaveCounterPosition();

        if (waveCounterCentered)
        {
            LeanTween.scale(waveCounterUI, doubleScale, 0.1f);

            LeanTween.alphaCanvas(waveCounterCG, 1, 1).setEase(LeanTweenType.easeOutQuint).setOnComplete(() =>
            {
                LeanTween.scale(waveCounterUI, halfScale, 0.5f);
                LeanTween.move(waveCounterUI, waveCounterPos, 1).setEase(LeanTweenType.easeInOutCirc).setOnComplete(() =>
                {
                    RunningOpeningWaveCounterAnimation = false;
                    OnWaveNumberAnimationComplete?.Invoke();
                }
                );
            }
            );
        }
    }

    void ResetWaveCounterPosition()
    {
        waveCounterUI.transform.position = screenCenter.transform.position;

        waveCounterCentered = true;
    }

    public void ShowNextWaveUI()
    {
        currentWaveNumber++;
        UpdateWaveUI();
        PlayOpeningWaveCounterAnimation();
    }

    private void UpdateWaveUI()
    {
        waveCounterUItext.text = $"WAVE {currentWaveNumber}";
    }

    public void OnWaveStarted(int waveNumber)
    {
        currentWaveNumber = waveNumber;
        UpdateWaveUI();
        PlayOpeningWaveCounterAnimation();
    }

    public void ShowNewCoinUI(int coin)
    {
        coinCountUItext.text = coin.ToString();
    }

    public void UpdateHeartsUI(int hearts)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < hearts)
                heartImages[i].sprite = fullHeart;
            else
                heartImages[i].sprite = emptyHeart;
        }
    }

    public void ShowWaveType(WaveData.WaveType type)
    {
        switch (type)
        {
            case WaveData.WaveType.Normal:
                return;

            case WaveData.WaveType.Boss:
                waveTypeText.text = "BOSS WAVE!";
                break;

            case WaveData.WaveType.Special:
                waveTypeText.text = "SPECIAL WAVE!";
                break;
        }

        waveTypeCG.alpha = 0f;
        LeanTween.alphaCanvas(waveTypeCG, 1f, 0.5f).setEaseOutExpo().setOnComplete(() =>
        {
            LeanTween.alphaCanvas(waveTypeCG, 0f, 0.5f).setEaseInExpo().setDelay(1f);
        });
    }

}
