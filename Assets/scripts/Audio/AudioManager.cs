using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Audio
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        [FMODUnity.EventRef]
        public string SampleEvent = "";

        private void Start()
        {
            FMODUnity.RuntimeManager.PlayOneShot(SampleEvent, transform.position);
        }


    }
}