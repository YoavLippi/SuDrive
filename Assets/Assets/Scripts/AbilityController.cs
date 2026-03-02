using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class AbilityController : MonoBehaviour

{
    // NB: ANYTHING GIVING NON-STATIC METHOD ISSUES SHOULD BE HERE 

    public BumperAbility bumperAbility = new BumperAbility(); // NEW INSTANCE

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (bumperAbility.isBumper && collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D otherRB = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector2 bumpDirection = (collision.gameObject.transform.position - collision.otherCollider.transform.position).normalized;
            otherRB.AddForce(bumpDirection * bumperAbility.bumperForce, ForceMode2D.Impulse); // MOVED TO ABOVE; dealing with monobehaviour issue

        }
    }

    void Start()
    {
        bumperAbility.Start(); // Initialise the bumper ability instance
    }

    // base class for abilities, will be used for the bumper and boost abilities for now, but can be expanded to more in the future
    public abstract class BaseAbility
    {
        // deactivation check
        public bool isBumper;
        // fetch car material and change it to the ability material, then change it back when deactivated
        public PhysicsMaterial2D carMaterial;

        // timer for ability duration
        public float bumperDuration = 2f;
        public float bumperForce = 10f;

        public void Start ()
        {
            GameObject car = GameObject.FindWithTag("Player");
            Rigidbody2D carRB = car.GetComponent<Rigidbody2D>();
            carMaterial = new PhysicsMaterial2D("BumperMaterial"); // Initialise bumper mat for session object
            carMaterial.bounciness = carRB.sharedMaterial.bounciness;
            carMaterial.friction = carRB.sharedMaterial.friction; // If needed later
        }

        public virtual void Activate(MonoBehaviour owner) { }
        public virtual void Deactivate() { }

       
    }

    public class BumperAbility : BaseAbility {

        // moved to above

        public override void Activate (MonoBehaviour owner) { 
             
            if (isBumper) return; // this is to prevent multiple activations
            isBumper = true;
            carMaterial.bounciness = 10f;
           
            Debug.Log($"Bump Made: bounciness = {carMaterial.bounciness}");
            // Start a coroutine to deactivate the ability after the duration
            owner.StartCoroutine(DeactivateAfterDelay()); // FIGURE OUT WHY THIS ISN'T WORKING; it was a non-static method issue
            

        }

        public override void Deactivate()
        {
           carMaterial.bounciness = 0.8f;
            Debug.Log($"Bump Made: bounciness = {carMaterial.bounciness}");
            isBumper = false;
        }

        public IEnumerator DeactivateAfterDelay ()
        {
            yield return new WaitForSeconds(bumperDuration);
            Deactivate();
        }
    }
}


