using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class AbilityController : MonoBehaviour

{
    // NB: ANYTHING GIVING NON-STATIC METHOD ISSUES SHOULD BE HERE 

    public PunchAbility punchAbility; 

    [SerializeField] public GameObject frontPunchPrefab;

    [SerializeField] public Vector2 punchSpawnOffset = new Vector2(0f, 3f);

    private Rigidbody2D carRB;


    void Awake()
    {
        carRB = GetComponent<Rigidbody2D>();
        punchAbility = new PunchAbility();
        punchAbility.Start(carRB);
    }

    // base class for abilities, will be used for the punch and boost abilities for now, but can be expanded to more in the future
    public abstract class BaseAbility
    {
        // deactivation check
        public bool isPunch;
        // fetch car material and change it to the ability material, then change it back when deactivated
        public PhysicsMaterial2D carMaterial;
        public GameObject frontPunch;

        // timer for ability duration
        public float punchDuration = 2f;
        public float punchForce = 50f;

        public void Start(Rigidbody2D carRB)
        {
           
            carMaterial = new PhysicsMaterial2D("PunchMaterial");
            if (carRB != null)
            {
                carMaterial.bounciness = carRB.sharedMaterial.bounciness;
                carMaterial.friction = carRB.sharedMaterial.friction; 
                carRB.sharedMaterial = carMaterial;
            }
        }

        public virtual void Activate(MonoBehaviour owner) { }
        public virtual void Deactivate(MonoBehaviour owner) { }

        public IEnumerator DeactivateAfterDelay(MonoBehaviour owner)
        {
            yield return new WaitForSeconds(punchDuration);
            Deactivate(owner);
        }
    }

    public class PunchAbility : BaseAbility
    {

        // moved to above

        public override void Activate(MonoBehaviour owner)
        {

            if (isPunch ) return; // this is to prevent multiple activations

            GameObject prefabToSpawn = (owner.GetComponent<AbilityController>()).frontPunchPrefab;

            isPunch = true;
            carMaterial.bounciness = 1f;

            Vector2 spawnPoint =    (Vector2) owner.transform.position 
                                  + (Vector2)owner.transform.up * owner.GetComponent<AbilityController>().punchSpawnOffset.y
                                  + (Vector2)owner.transform.right * owner.GetComponent<AbilityController>().punchSpawnOffset.x;

            frontPunch = GameObject.Instantiate(prefabToSpawn, spawnPoint, owner.transform.rotation, owner.transform);

            AbilityHitbox hitbox = frontPunch.GetComponent<AbilityHitbox>();
            if (hitbox != null) hitbox.onCollisionEnter = PunchOther;


            Debug.Log($"Bump Made: bounciness = {carMaterial.bounciness}");
            // Start a coroutine to deactivate the ability after the duration
            owner.StartCoroutine(DeactivateAfterDelay(owner)); // FIGURE OUT WHY THIS ISN'T WORKING; it was a non-static method issue


        }
        public override void Deactivate(MonoBehaviour owner)
        {
            carMaterial.bounciness = 0.8f;

            if (frontPunch != null)
            {

                GameObject.Destroy(frontPunch);
                frontPunch = null;
            }

            Debug.Log($"Bump Made: bounciness = {carMaterial.bounciness}");
            isPunch = false;
        }

        void PunchOther(Collision2D collision)
        {
            if (!isPunch) return;
            if (!collision.gameObject.CompareTag("Player")) return;

            Rigidbody2D otherRB = collision.gameObject.GetComponent<Rigidbody2D>();
            if (otherRB == null) return;

            Vector2 punchDirection = (collision.gameObject.transform.position - frontPunch.transform.position).normalized;
            otherRB.linearVelocity += punchDirection * punchForce;
        }
    }

}


