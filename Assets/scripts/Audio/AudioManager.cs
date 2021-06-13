using FMOD.Studio;
using FMODUnity;
using System;
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
        public string YetiRoar, YetiSwipe, Gunshot, GunFilled, VictoryCheer, VictorySong, Death;

        public Dictionary<GameObject, FMODUnity.StudioEventEmitter> Runners = new Dictionary<GameObject, FMODUnity.StudioEventEmitter>();

        private float _maxConventionalSpeed = 12f; // Scuffed af

        protected override void Awake()
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.transform.position = transform.position;
            base.Awake();
        }

        public void SetupRunEffect(GameObject go, UnityEvent<float> speedUpdate)
        {
            StudioEventEmitter emitter = go.AddComponent<FMODUnity.StudioEventEmitter>();
            emitter.Event = MovementChatter;
            emitter.Play();
            if (Runners.ContainsKey(go))
            {
                Debug.LogWarning("Duplicate run sfx being attached! Don't do this!");
                Destroy(go.GetComponent<StudioEventEmitter>());
            }
            Runners[go] = emitter;
            speedUpdate.AddListener((speed) => { emitter.SetParameter("Speed", speed); /*Debug.Log("Players: " + speed);*/ });
        }
        public void RevokeRunEffect(GameObject go)
        {
            // Listener removed in Player.Die (I know, lazy, it's a game jam tho)
            if (Runners.ContainsKey(go))
            {
                Runners[go].Stop();
                Destroy(Runners[go]);
                Runners.Remove(go);
            }
        }
        

        public void TriggerFootstep(Vector3 position, float speed, float materialHardness)
        {
            RuntimeManager.PlayOneShot(InvisFootsteps, transform.position);
            EventInstance e = RuntimeManager.CreateInstance(InvisFootsteps);
            e.set3DAttributes(RuntimeUtils.To3DAttributes(position));
            e.setParameterByName("Speed", speed);
            e.start();
            e.release();
            //Debug.Log("Monster: " + speed);
        }

        public void TriggerSound(TriggerSoundType sound, Vector3 position)
        {
            string foundSound = "";

            switch (sound)
            {
                case TriggerSoundType.GUNSHOT:
                    foundSound = Gunshot;
                    break;
                case TriggerSoundType.YETI_ROAR:
                    foundSound = YetiRoar;
                    break;
                case TriggerSoundType.YETI_SWIPE:
                    foundSound = YetiSwipe;
                    break;
                case TriggerSoundType.GUN_FILLED:
                    foundSound = GunFilled;
                    break;
                case TriggerSoundType.VICTORY_SONG:
                    foundSound = VictorySong;
                    break;
                case TriggerSoundType.VICTORY_CHEER:
                    foundSound = VictoryCheer;
                    break;
                case TriggerSoundType.DEATH:
                    foundSound = Death;
                    break;
            }

            if (foundSound != "")
                FMODUnity.RuntimeManager.PlayOneShot(foundSound, position);
            else
                Debug.LogWarning("Invalid trigger sound: " + Enum.GetName(typeof(TriggerSoundType), sound));
        }

        public enum TriggerSoundType
        {
            UNDEFINED = 0,
            GUNSHOT = 1,
            YETI_ROAR = 2,
            YETI_SWIPE = 3,
            GUN_FILLED = 4,
            VICTORY_SONG = 5,
            VICTORY_CHEER = 6,
            DEATH = 7,
        }
    }
}