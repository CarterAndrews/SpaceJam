using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SceneNavigator : MonoSingleton<SceneNavigator>
{
    public Image screen;
    public float fadeTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //fadeOut();
    }
    public static IEnumerator fadeInTimed(string name)
    {
        for(float alpha = 0; alpha <= 1; alpha += Time.deltaTime)
        {
            Instance.screen.color = new Color(1, 1, 1, alpha);
            yield return null;
        }
        SceneManager.LoadScene(name);
        while (SceneManager.GetActiveScene().name!=name)
        {
            yield return null;
        }
        for (float alpha = 1; alpha >=0; alpha -= Time.deltaTime)
        {
            Instance.screen.color = new Color(1, 1, 1, alpha);
            yield return null;
        }
    }
    public static void fadeIn(string name)
    {
        SceneNavigator.Instance.StartCoroutine(fadeInTimed(name));
    }
    private void Awake()
    {
        base.Awake();
        //GoToScene("menu");
        GoToScene("menu");
        //fadeOut();
    }
    public static void GoToScene(string sceneName)
    {
        fadeIn(sceneName);
    }
    public static void Quit()
    {
        Application.Quit();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
