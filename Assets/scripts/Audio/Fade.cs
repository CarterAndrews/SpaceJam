using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    public StudioEventEmitter emitter;
    private Coroutine fadeCor;

    private void Start()
    {
        emitter.SetParameter("Atten", 1f);
    }
    public void DoFade()
    {
        if(fadeCor == null)
            fadeCor = StartCoroutine(FadeCor());
    }
    private IEnumerator FadeCor()
    {
        float t = 1f;
        while (t >0f)
        {
            t -= Time.unscaledDeltaTime;
            emitter.SetParameter("Atten", t);
            yield return null;

        }
    }
}
