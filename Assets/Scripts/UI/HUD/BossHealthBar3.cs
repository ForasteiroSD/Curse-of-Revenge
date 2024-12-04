using UnityEngine;
using UnityEngine.UI;
public class BossHealthBar3 : MonoBehaviour
{
    public Slider RedBarSlider;
    public Slider YellowBarSlider;

    [SerializeField] public float lerpSpeed;
    private FireKnightScript _bossPrefab;
    void OnEnable()
    {
        print("Entrou");
        _bossPrefab = FindFirstObjectByType<FireKnightScript>();
        RedBarSlider.maxValue = _bossPrefab._health;
        YellowBarSlider.maxValue = _bossPrefab._health;
        RedBarSlider.value = _bossPrefab._health;
        YellowBarSlider.value = _bossPrefab._health;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (enabled)
        {
            if (RedBarSlider.value != _bossPrefab._health)
            {
                RedBarSlider.value = _bossPrefab._health;
            }

            if (RedBarSlider.value != YellowBarSlider.value)
            {
                YellowBarSlider.value = Mathf.Lerp(YellowBarSlider.value, _bossPrefab._health, lerpSpeed);
            }
        }
    }
    
}
