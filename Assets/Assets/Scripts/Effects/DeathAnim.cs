using UnityEngine;

public class DeathAnim : MonoBehaviour
{
    //A script that will simply detect once the player has died and execute the animation as well as the particle effect.
    //need to reference the animator and the particle system for the death animation and effect respectively. not the ai finishinh my comments bruh.

    [Header("References")]
    public Animator deathAnim;
    public ParticleSystem smokeEffect;


    void Start()
    {
        //when the game starts, smoke mustnt play ofc
       smokeEffect.Stop();    
        //if this don't work imma try and reference this as gameObjects and use setActive
    }

 
    public void EffectiveDeath ()
    {
        //when the player dies, call this script in the main script where death takes place so that the animation and particle effect play at the right time.
        deathAnim.SetTrigger("Die");//dear God i hope this is right LOL. 
        //turning on the animation trigger so that the death animation triggers....yeah.

        smokeEffect.Play();
    }

    //realised we might need a reset animation and particle effect when the players respawn for the next round
    public void ResetDeathAnim ()
    {
        deathAnim.Play("IdleNormalAnim");//hope this is right LOL. 
        smokeEffect.Stop();
    }

}
