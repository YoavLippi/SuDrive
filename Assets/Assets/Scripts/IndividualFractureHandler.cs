using UnityEngine;

public class IndividualFractureHandler : MonoBehaviour
{
    [SerializeField] private Collider2D[] allColliders;

    public void SetCollidersActive(bool isActive)
    {
        foreach (var col in allColliders)
        {
            col.enabled = isActive;
        }
    }
}
