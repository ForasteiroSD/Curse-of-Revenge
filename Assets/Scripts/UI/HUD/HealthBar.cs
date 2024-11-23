using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider RedBarSlider;
    public Slider YellowBarSlider;
    public Adventurer adventurer;

    [SerializeField] public float lerpSpeed;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Pegando vida máxima do player
        adventurer = FindFirstObjectByType<Adventurer>();
        RedBarSlider.maxValue = adventurer.life;
        YellowBarSlider.maxValue = adventurer.life;
        RedBarSlider.value = adventurer.life;
        YellowBarSlider.value = adventurer.life;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (RedBarSlider.value != adventurer.life)
        {
            if(RedBarSlider.value > adventurer.life) RedBarSlider.value = adventurer.life;
            else {
                RedBarSlider.value = Mathf.Lerp(RedBarSlider.value, adventurer.life, lerpSpeed*5);
            }
        }

        if (RedBarSlider.value != YellowBarSlider.value)
        {
            YellowBarSlider.value = Mathf.Lerp(YellowBarSlider.value, adventurer.life, lerpSpeed);
        }
    }
}
