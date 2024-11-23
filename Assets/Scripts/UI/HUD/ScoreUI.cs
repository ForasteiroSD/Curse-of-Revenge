using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    TextMeshProUGUI _revengeText;
    float _revengeFontSize;
    Color _revengeFontColor;
    int _currentScore;
    private bool _isScaling = false;
    private Animator _animator;
    
    private void Awake()
    {
        _revengeText = GetComponent<TextMeshProUGUI>();
        _animator = GetComponent<Animator>();
        _revengeFontSize = _revengeText.fontSize;
        _revengeFontColor = _revengeText.color;
    }

    public void  UpdateScore(int value)
    {
        _currentScore += value;
        _revengeText.text = $"{_currentScore.ToString("D4")}";
        if(!_isScaling) {
            _isScaling = true;
            _animator.SetTrigger("GetPoint");
        }
    }

    private void FinishAnimation() {
        _isScaling = false;
    }
}
