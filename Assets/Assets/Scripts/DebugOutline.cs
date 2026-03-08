using UnityEngine;

public class DebugOutline : MonoBehaviour
{
    public Color outlineColor = Color.red;

    void Update()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null) return;

        Bounds b = col.bounds;

        Vector3 topLeft = new Vector3(b.min.x, b.max.y, 0);
        Vector3 topRight = new Vector3(b.max.x, b.max.y, 0);
        Vector3 bottomLeft = new Vector3(b.min.x, b.min.y, 0);
        Vector3 bottomRight = new Vector3(b.max.x, b.min.y, 0);

        Debug.DrawLine(topLeft, topRight, outlineColor);
        Debug.DrawLine(topRight, bottomRight, outlineColor);
        Debug.DrawLine(bottomRight, bottomLeft, outlineColor);
        Debug.DrawLine(bottomLeft, topLeft, outlineColor);
    }
}