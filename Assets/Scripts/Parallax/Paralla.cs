using UnityEngine;

public class Paralla : MonoBehaviour
{
    private float length;

    private float StartPos;

    private Transform cam;
    
    public float ParallaxEffect;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        float Distance = cam.transform.position.x * ParallaxEffect;
        
        transform.position = new Vector3(StartPos + Distance, transform.position.y, transform.position.z);
    }
}
