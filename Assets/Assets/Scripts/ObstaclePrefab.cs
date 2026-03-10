using UnityEngine;

public class ObstaclePrefab : MonoBehaviour
{
   public enum ObstacleType
    {
        Bounce,
        // Kill,
        SpeedControl
    }

    [SerializeField] ObstacleType obstacleType = ObstacleType.SpeedControl; // speedControl is just the default, we can swap out behaviours in inspector

    [SerializeField] float neededSpeed = 5f;
    [SerializeField] float neededRotationSpeed = 10f;

    [SerializeField] float bounciness = 1.5f;
    [SerializeField] float minBounceForce = 3f;

    //GameController _gameController;

    [SerializeField] private ParticleSystem bounceParticles;
    [SerializeField] private ParticleSystem breakParticles;

    void Start()
    {
       // _gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        Rigidbody2D rbOther = other.gameObject.GetComponent<Rigidbody2D>();
        if (rbOther == null) return; // forgot a "="

        Vector2 contactPoint = GetContactPoint(other);
        Quaternion contactRotation = GetContactRotation(contactPoint, other.transform.position);

        switch (obstacleType)
        {
            case ObstacleType.Bounce:
                BouncePlayer(rbOther, contactPoint, contactRotation);
                break;

           // case ObstacleType.Kill:
              //  KillPlayer(other, contactPoint, contactRotation);
               // break;

            case ObstacleType.SpeedControl:
                bool isFast = rbOther.linearVelocity.magnitude >= neededSpeed
                    || Mathf.Abs(rbOther.angularVelocity) >= neededRotationSpeed;

                if (isFast)
                
                    //KillPlayer(other, contactPoint, contactRotation);

                    //else
                        BouncePlayer(rbOther, contactPoint, contactRotation);
                    break;
                
        }

        void BouncePlayer (Rigidbody2D rb, Vector2 contactPoint, Quaternion rot)
        {
            float forceMag = Mathf.Max((rb.linearVelocity * bounciness / rb.mass).magnitude, minBounceForce);

            rb.AddForce(forceMag * -rb.linearVelocity.normalized, ForceMode2D.Impulse);

            if (bounceParticles != null)
                Instantiate(bounceParticles, contactPoint, rot);
        }

       // void KillPlayer(Collider2D other, Vector2 contactPoint, Quaternion rot)
       // {
         //   if (breakParticles !=null)
              //  Instantiate(breakParticles, contactPoint, rot);

           //if (_gameController != null)
               //_gameController.KillPlayer(other.gameObject);

        //    else
        //        Debug.LogWarning("[ObstaclePrefab] KillPlayer called but no GameController assigned.");
       // }

        Vector2 GetContactPoint (Collider2D other)
        {
            Collider2D col = GetComponent<Collider2D>();
            return col != null ? col.ClosestPoint(other.transform.position) : (Vector2)transform.position;

        }

        Quaternion GetContactRotation (Vector2 contactPoint, Vector3 targetPosition)
        {
            GameObject temp = new GameObject("_ContactHelper");
            temp.transform.position = contactPoint;
            temp.transform.LookAt(targetPosition);
            Quaternion rot = temp.transform.rotation;
            Destroy(temp);
            return rot;
        }
    }
    
}
