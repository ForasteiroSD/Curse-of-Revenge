using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    TextMeshProUGUI _revengeText;
    float _revengeFontSize;
    Color _revengeFontColor;
    int _currentScore;
    
    private void Awake()
    {
        _revengeText = GetComponent<TextMeshProUGUI>();
        _revengeFontSize = _revengeText.fontSize;
        _revengeFontColor = _revengeText.color;
    }

    public async void  UpdateScore()
    {
        _currentScore++;
        _revengeText.text = $"{_currentScore.ToString("D4")}";
        _revengeText.color = Color.yellow;
        _revengeText.fontSize = _revengeFontSize * 1.1f;
        await Task.Delay(500);
        _revengeText.fontSize = _revengeFontSize;
        _revengeText.color = _revengeFontColor;
        
    }
}
