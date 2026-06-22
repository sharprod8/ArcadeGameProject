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
    
    private void Start()
    {
        waveCounterCG.alpha = 0f;
        waveCounter = 1;
        waveCounterUItext = waveCounterUI.GetComponentInChildren<TextMeshProUGUI>();
        waveCounterUI.transform.position = screenCenter.transform.position;

        PlayOpeningWaveCounterAnimation();
    }

    private void PlayOpeningWaveCounterAnimation()
    {
        LeanTween.alphaCanvas(waveCounterCG, 1, 1).setEase(LeanTweenType.easeOutQuint).setOnComplete(() =>
        
        );
    }
}
