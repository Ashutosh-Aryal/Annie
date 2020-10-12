using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Restart : MonoBehaviour
{

    // Start is called before the first frame update
    public void RestartGame()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
        EnemyBehavior.s_EndGameMenu.SetActive(false);
        EnemyBehavior.s_HasPlayerLost = false;
        CheckWinStateBehavior.s_PlayerDidWin = false;
        MundoMovement.s_NumHeldBatteries = 0;
    }

    public void Quit()
    {
        Application.Quit();
    }

}
