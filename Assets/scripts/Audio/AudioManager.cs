using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Utility;

namespace Audio
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        [FMODUnity.EventRef]
        public string InvisFootsteps, MovementChatter;
        [FMODUnity.EventRef]
        public string YetiRoar, YetiSwipe, Gunshot;

        public Dictionary<GameObject, FMODUnity.StudioEventEmitter> Runners = new Dictionary<GameObject, FMODUnity.StudioEventEmitter>();

        protected override void Awake()
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.transform.position = transform.position;
            base.Awake();
        }

        public void SetupRunEffect(GameObject go, UnityEvent<float> speedUpdate)
        {
            FMODUnity.StudioEventEmitter emitter = go.AddComponent<FMODUnity.StudioEventEmitter>();
            emitter.Event = MovementChatter;
            emitter.Play();
            if (Runners.ContainsKey(go))
            {
                Debug.LogWarning("Duplicate run sfx being attached! Don't do this!");
                Destroy(go.GetComponent<FMODUnity.StudioEventEmitter>());
            }
            Runners[go] = emitter;
            speedUpdate.AddListener((speed) => { emitter.SetParameter("Speed", speed); });
        }
        public void RevokeRunEffect(GameObject go)
        {
            // Listener removed in Player.Die (I know, lazy, it's a game jam tho)
            if (Runners.ContainsKey(go))
            {
                Destroy(Runners[go]);
                Runners.Remove(go);
            }
            else
            {
                Debug.LogWarning("No run sfx on object: " + go.name + " are you sure you got this right?");
            }
        }
        

        public void TriggerFootstep(Vector3 position, float speed, float materialHardness)
        {
            FMODUnity.RuntimeManager.PlayOneShot(InvisFootsteps, transform.position);

        }

    }
}