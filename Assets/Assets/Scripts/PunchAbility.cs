using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class PunchAbility : AbilityController.BaseAbility
    {
        [SerializeField] public GameObject frontPunchPrefab;

        [SerializeField] public Vector2 punchSpawnOffset = new Vector2(0f, 8f);

        private Rigidbody2D carRB;
        
        // deactivation check
        public bool isActive;
        // fetch car material and change it to the ability material, then change it back when deactivated
        public PhysicsMaterial2D carMaterial;
        private GameObject frontPunch;
        
        public float punchForce = 50f;
        public float stunTime = 0.25f;
        public Sprite[] punchSprites;

        private IEnumerator delayRoutine;
        
        // moved to above

        public override void Start()
        {
            carRB = GetComponent<Rigidbody2D>();
            carMaterial = new PhysicsMaterial2D("PunchMaterial");
            if (carRB != null)
            {
                carMaterial.bounciness = carRB.sharedMaterial.bounciness;
                carMaterial.friction = carRB.sharedMaterial.friction; 
                carRB.sharedMaterial = carMaterial;
            }
        }

        public override void Activate(MonoBehaviour owner, int spriteIndex)
        {
            if (!isAvailable) return;
            if (isActive ) return; // this is to prevent multiple activations

            GameObject prefabToSpawn = frontPunchPrefab;

            isAvailable = false;
            isActive = true;
            carMaterial.bounciness = 1f;
            
            Vector2 localScale = owner.transform.localScale;
            Vector2 localOffset = punchSpawnOffset * localScale;
            Vector2 spawnPoint =   (Vector2) owner.transform.position+ (Vector2)owner.transform.up * localOffset.y+ (Vector2)owner.transform.right * localOffset.x;

            frontPunch = GameObject.Instantiate(prefabToSpawn, spawnPoint, owner.transform.rotation, owner.transform);

            AbilityHitbox hitbox = frontPunch.GetComponent<AbilityHitbox>();
            frontPunch.transform.Find("Glove").GetComponent<SpriteRenderer>().sprite = punchSprites[spriteIndex];
            if (hitbox != null) hitbox.onCollisionEnter = PunchOther;


            Debug.Log($"Bump Made: bounciness = {carMaterial.bounciness}");
            // Start a coroutine to deactivate the ability after the duration
            delayRoutine = DeactivateAfterDelay(owner);
            StartCoroutine(delayRoutine); // FIGURE OUT WHY THIS ISN'T WORKING; it was a non-static method issue
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
            isActive = false;
            StartCoroutine(DoCooldown(cooldownTime));
        }

        void PunchOther(Collision2D collision)
        {
            if (!isActive) return;
            if (!collision.gameObject.CompareTag("Player")) return;

            Rigidbody2D otherRB = collision.gameObject.GetComponent<Rigidbody2D>();
            if (otherRB == null) return;

            Vector2 punchDirection = (collision.gameObject.transform.position - frontPunch.transform.position).normalized;
            //otherRB.linearVelocity += punchDirection * punchForce;
            otherRB.AddForceAtPosition(punchDirection*punchForce, collision.GetContact(0).point, ForceMode2D.Impulse);
            collision.gameObject.GetComponent<CarController>().GetStunned(stunTime);
            StopCoroutine(delayRoutine);
            Deactivate(this);
        }
        
        
    }
