using UnityEngine;

public class Theater : MonoBehaviour
{
    public Sprite[] slides;

    Vector3 pos;
    RectTransform rect;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        pos = rect.position;
    }

    private void Update()
    {
        float sx = Mathf.PerlinNoise(Time.time * 2f, 0) * 3 - 1.5f;
        float sy = Mathf.PerlinNoise(0, Time.time * 2f) * 3 - 1.5f;

        rect.position = pos + new Vector3(sx, 0, sy);
    }
}
