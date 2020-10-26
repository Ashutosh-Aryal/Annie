using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;

public class AnniesHouseDialogue : MyDialogBase
{
    private const float FINAL_ANIMATION_Y_POSITION = 10.58f;
    private const float Y_POSITION_TO_LOAD_NEXT_LEVEL_ON = 12.5f;
    private const string SHOULD_PLAY_ANNIE_MOVING_UP_ANIMATION = "shouldMoveUpAndStop";
    private Animator m_AnnieAnimator;
    private Animator m_MundoAnimator;

    private bool m_HasAnnieComeDownstairs = false;
    private bool m_MundoHasComeInside = false;
    private bool m_IsOnFinalDialogue = false;

    [SerializeField]
    private GameObject m_MundoObject;

    [SerializeField]
    private GameObject m_AnnieObject;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        m_DialogManager = gameObject.GetComponent<DialogManager>();
        m_MundoAnimator = m_MundoObject.GetComponent<Animator>();
        m_AnnieAnimator = m_AnnieObject.GetComponent<Animator>();

        List<DialogData> myDialogScript = new List<DialogData>();

        myDialogScript.Add(new DialogData("Annie's Mom: Annie! Can you come down please?!", "Annie's Mom", () => m_AnnieAnimator.SetBool(SHOULD_PLAY_ANNIE_MOVING_UP_ANIMATION, true)));

        m_DialogScript.Add(myDialogScript);

        m_DialogManager.Show(m_DialogScript[0]);
    }

    private void ShowAnnieDialogue()
    {
        List<DialogData> myDialogScript = new List<DialogData>();

        myDialogScript.Add(new DialogData("Annie: What is it mom?", "Annie"));
        myDialogScript.Add(new DialogData("Annie's Mom: Mundo's been wondering where you've been...", "Annie's Mom"));
        myDialogScript.Add(new DialogData("Annie: Mundo? I want to play with Mundo!", "Annie"));
        myDialogScript.Add(new DialogData("Annie's Dad: Well then! You're in luck because he's right outside!", "Annie's Dad"));
        myDialogScript.Add(new DialogData("Annie's Dad: Mundo! Come on in!", "Annie's Dad", () => MoveMundoInsideHouse()));

        m_DialogScript.Add(myDialogScript);

        m_DialogManager.Show(m_DialogScript[0]);
    }

    private void ShowMundoDialogue()
    {
        List<DialogData> myDialogScript = new List<DialogData>();

        myDialogScript.Add(new DialogData("Mundo: Hey Annie! Are you ready to go?", "Mundo"));
        myDialogScript.Add(new DialogData("Annie: Yes. Can I be carried?", "Annie"));
        myDialogScript.Add(new DialogData("Annie's Dad: Now Annie...", "Annie's Dad"));
        myDialogScript.Add(new DialogData("Mundo: Well, aren't you a spoiled one!", "Mundo"));
        myDialogScript.Add(new DialogData("Annie: PLLEEEEEEEEEEEEAAASE!", "Annie"));
        myDialogScript.Add(new DialogData("Mundo: Alright, fine! Let's go then!", "Mundo"));

        m_DialogScript.Add(myDialogScript);

        m_DialogManager.Show(m_DialogScript[0]);
    }

    private void ShowFinalDialogue()
    {
        List<DialogData> myDialogScript = new List<DialogData>();

        myDialogScript.Add(new DialogData("Mundo: Alright! We're off!", "Mundo", () => m_MundoAnimator.SetTrigger("shouldLeaveHouse")));

        m_DialogScript.Add(myDialogScript);

        m_DialogManager.Show(m_DialogScript[0]);
    }

    private void MoveMundoInsideHouse()
    {
        m_MundoObject.SetActive(true);
        m_MundoAnimator.SetBool("shouldWalkInHouse", true);
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if(!m_HasAnnieComeDownstairs && Mathf.Approximately(m_AnnieObject.transform.position.y, FINAL_ANIMATION_Y_POSITION))
        {
            m_DialogScript.RemoveAt(0);
            m_HasAnnieComeDownstairs = true;
            ShowAnnieDialogue();
        } else if(!m_MundoHasComeInside && Mathf.Approximately(m_MundoObject.transform.position.y, FINAL_ANIMATION_Y_POSITION))
        {
            m_DialogScript.RemoveAt(0);
            m_MundoHasComeInside = true;
            ShowMundoDialogue();
        } else if(!m_IsOnFinalDialogue && Input.GetKeyUp(KeyCode.F) && CanPlayerMove())
        {
            m_IsOnFinalDialogue = true;
            m_DialogScript.RemoveAt(0);
            ShowFinalDialogue();
        } else if(m_IsOnFinalDialogue && m_MundoObject.transform.position.y >= Y_POSITION_TO_LOAD_NEXT_LEVEL_ON)
        {
            m_LevelLoader.LoadNextLevel();
        }
    }
}
