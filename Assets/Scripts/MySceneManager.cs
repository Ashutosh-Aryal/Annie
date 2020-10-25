using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    public enum Scene
    {
        MAIN_MENU = 0,
        INTRO_SCENE = 1,
        ANNIE_HOME = 2,
        NORMAL_VILLAGE = 3,
        TUNNEL = 4,
        PINK_DRINK_FACTORY = 5,
        END_SCENE = 6
    }

    public static Scene s_CurrentlyDisplayedScene = Scene.MAIN_MENU;

    public static void LoadScene(Scene scene)
    {
        s_CurrentlyDisplayedScene = scene;
        SceneManager.LoadScene((int)scene, LoadSceneMode.Single);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
