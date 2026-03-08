using UnityEngine;

public class AbilityHitbox : MonoBehaviour
{
    public System.Action<Collision2D> onCollisionEnter;
     void OnCollisionEnter2D (Collision2D collision)
    {
        onCollisionEnter?.Invoke(collision);
    }
}
