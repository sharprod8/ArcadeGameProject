using System;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject waveCounterUI;
    public CanvasGroup waveCounterCG;
    public int waveCounter;

    private TextMeshProUGUI waveCounterUItext;
    public Transform screenCenter;
    public Transform waveCounterPos;

    private Vector3 doubleScale = new Vector3(2, 2, 1);
    private Vector3 halfScale = new Vector3(1, 1, 1);
    private bool waveCounterCentered;
    private bool RunningOpeningWaveCounterAnimation;

    private void Start()
    {
        waveCounterCG.alpha = 0f;
        waveCounter = 1;
        waveCounterUItext = waveCounterUI.GetComponentInChildren<TextMeshProUGUI>();

        ResetWaveCounterPosition();

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

    public void AdvanceWave()
    {
        if (RunningOpeningWaveCounterAnimation)
            Debug.Log("Didn't advance wave, wait for animation first");
            return;
        
        waveCounter += 1;
        StartWave();
    }

    public void StartWave()
    {
        PlayOpeningWaveCounterAnimation();
        RunningOpeningWaveCounterAnimation = true;
    }
}
