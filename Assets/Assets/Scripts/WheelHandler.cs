using UnityEngine;
using UnityEngine.Serialization;

public class WheelHandler : MonoBehaviour
{
    [Header("Parent Setup")]
    [SerializeField] private GameObject parentObj;
    [SerializeField] private Rigidbody2D parentBody;

    [Header("Physics")] 
    [SerializeField] [Range(0,1f)] private float gripFactor = 1f;
    void Start()
    {
        parentObj = transform.parent.gameObject;
        if (parentObj)
        {
            parentBody = parentObj.GetComponent<Rigidbody2D>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (UnityEngine.Debug.isDebugBuild)
        {
            //Accel
            Debug.DrawRay(transform.position, transform.up, Color.blue);
            //Traction
            Debug.DrawRay(transform.position,transform.right, Color.green);
        }
        
        Vector2 tireWorldVel = parentBody.GetPointVelocity(transform.position);

        #region Physics
        
        //Referencing code from Yoav's previous code, which pulls from
        //https://www.youtube.com/watch?v=CdPYlj5uZeI&t=273s

        //Traction (Right/Left and slipping)
        #region Traction

        Vector2 steeringDir = transform.right;
        
        //getting x velocity, to cancel later
        float steeringVel = Vector2.Dot(steeringDir, tireWorldVel);

        //cancelling velocity
        //Grip factor is a range from 0-1: 0=no grip, 1=full grip
        float desiredVelocityChange = -steeringVel * gripFactor;
        
        parentBody.AddForceAtPosition(steeringDir*desiredVelocityChange/parentBody.mass, transform.position);
        
        #endregion
        
        //Acceleration

        #region Acceleration

        Vector2 accelDir = transform.up;
        
        //We want to accelerate constantly, then the main physics body can clamp the speed based on the throttle input

        #endregion

        #endregion
    }

    private void OnDrive()
    {
        Debug.Log("TEstststs");
    }
}
