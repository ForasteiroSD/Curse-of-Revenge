using System.Collections;
using TMPro;
using UnityEngine;

public class DamageUI : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 1f;
    [SerializeField] float _effectsDuration = 1.5f;
    private TextMeshPro _tmp;

    void Awake()
    {
        _tmp = GetComponent<TextMeshPro>();
        StartCoroutine(FadeAndScale());
    }

    void Update()
    {
        //slowly moves text up
        transform.position += new Vector3(0, _moveSpeed * Time.deltaTime, 0);
    }

    IEnumerator FadeAndScale()
    {
        float elapsedTime = 0f;

        Vector3 initialScale = transform.localScale;
        
        //final size
        Vector3 targetScale = initialScale * 1.5f;

        while (elapsedTime < _effectsDuration)
        {
            float t = elapsedTime / _effectsDuration;

            //changes alpha
            Color color = _tmp.color;
            color.a = Mathf.Lerp(1, 0, t);
            _tmp.color = color;

            //changes scale
            transform.localScale = Vector3.Lerp(initialScale, targetScale, t);

            elapsedTime += Time.deltaTime;

            //waits until next frame
            yield return null;
        }

        //aply last state
        _tmp.color = new Color(_tmp.color.r, _tmp.color.g, _tmp.color.b, 0);
        transform.localScale = targetScale;

        Destroy(gameObject);
    }
}
