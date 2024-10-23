using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    TextMeshProUGUI _revengeText;
    int _currentScore;
    
    private void Awake()
    {
        _revengeText = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateScore()
    {
        _currentScore++;
        _revengeText.text = $"{_currentScore.ToString("D4")}";
    }
}
