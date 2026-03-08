using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.Serialization;

public class AbilityController : MonoBehaviour

{
    // NB: ANYTHING GIVING NON-STATIC METHOD ISSUES SHOULD BE HERE

    // base class for abilities, will be used for the punch and boost abilities for now, but can be expanded to more in the future
    public abstract class BaseAbility : MonoBehaviour
    {
        public float duration;
        public float cooldownTime;
        public bool isAvailable = true;
        public PlayerController _playerController;
        public virtual void Start()
        {
            throw new NotImplementedException("Please use an inherited class");
        }

        public virtual void Activate(MonoBehaviour owner, int playerIndex) { }
        public virtual void Deactivate(MonoBehaviour owner) { }

        public IEnumerator DeactivateAfterDelay(MonoBehaviour owner)
        {
            yield return new WaitForSeconds(duration);
            Deactivate(owner);
        }

        public IEnumerator DoCooldown(float time)
        {
            isAvailable = false;
            float timeElapsed = 0f;
            do
            {
                _playerController.SetCooldownSlider((time-timeElapsed)/time);
                yield return new WaitForEndOfFrame();
                timeElapsed += Time.deltaTime;
            } while (timeElapsed < time);
            isAvailable = true;
        }
    }

}


