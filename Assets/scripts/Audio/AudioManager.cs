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

        public StudioEventEmitter Wind;

        public Dictionary<GameObject, FMODUnity.StudioEventEmitter> Runners = new Dictionary<GameObject, FMODUnity.StudioEventEmitter>();

        private float _initialDampening = 1f;
        private float _elapsedTime = 0f;
        private float _timeSinceInput = -4f;
        private float _inputTimeLerper = -4f;
        private const float _maxInputDelay = 10f;

        protected override void Awake()
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.transform.position = transform.position;
            base.Awake();
        }

        private void Update()
        {
            if (_initialDampening > 0)
            {
                _initialDampening -= Time.deltaTime/3f;
                if (_initialDampening < 0)
                    _initialDampening = 0;
            }
            _elapsedTime += Time.unscaledDeltaTime;
            _timeSinceInput += Time.unscaledDeltaTime;
            _inputTimeLerper = Mathf.Lerp(_inputTimeLerper,_timeSinceInput, 4f*Time.unscaledDeltaTime);

            Wind.SetParameter("Speed", Mathf.PerlinNoise(_elapsedTime / 4f, 0f));
            Wind.SetParameter("Atten", Mathf.Clamp01(Mathf.PerlinNoise(_elapsedTime / 8f, 0f)+ _initialDampening));
            Wind.SetParameter("Action", 1f- Mathf.Clamp01(_inputTimeLerper / _maxInputDelay));
        }
        public void ResetInputTimer()
        {
            _timeSinceInput = 0;
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
            string foundSound = GetSound(sound);
            if (foundSound != "")
                FMODUnity.RuntimeManager.PlayOneShot(foundSound, position);
        }
        public void TriggerSoundAttached(TriggerSoundType sound, GameObject obj)
        {
            string foundSound = GetSound(sound);
            if (foundSound != "")
                FMODUnity.RuntimeManager.PlayOneShotAttached(foundSound, obj);
        }

        public string GetSound(TriggerSoundType sound) // Not a dictionary because I wanted easy assignment
        {
            switch (sound)
            {
                case TriggerSoundType.GUNSHOT:
                    return Gunshot;
                case TriggerSoundType.YETI_ROAR:
                    return YetiRoar;
                case TriggerSoundType.YETI_SWIPE:
                    return YetiSwipe;
                case TriggerSoundType.GUN_FILLED:
                    return GunFilled;
                case TriggerSoundType.VICTORY_SONG:
                    return VictorySong;
                case TriggerSoundType.VICTORY_CHEER:
                    return VictoryCheer;
                case TriggerSoundType.DEATH:
                    return Death;
            }
            Debug.LogWarning("Invalid trigger sound: " + Enum.GetName(typeof(TriggerSoundType), sound));
            return "";
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