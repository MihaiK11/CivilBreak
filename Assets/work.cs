using UnityEngine;

public class ScrollText : MonoBehaviour
{
    public float speed = 50f;
    public float stopPositionY = 9000f; // Ajustează în funcție de poziția finală
    private RectTransform textTransform;

    void Start()
    {
        textTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (textTransform != null)
        {
            if (textTransform.anchoredPosition.y < stopPositionY)
            {
                textTransform.anchoredPosition += Vector2.up * speed * Time.deltaTime;
            }
        }
    }
}
