using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckWinStateBehavior : MonoBehaviour
{
    private Animator myAnimator;

    public static bool s_PlayerDidWin = false;

    [SerializeField]
    private GameObject m_EndGameTextObject;

    private void Start()
    {
        myAnimator = gameObject.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(myAnimator.GetBool("openGate"))
        {
            m_EndGameTextObject.GetComponent<TextMeshProUGUI>().text = "You Win!";
            EnemyBehavior.s_EndGameMenu.SetActive(true);
            s_PlayerDidWin = true;
        }
    }

}
