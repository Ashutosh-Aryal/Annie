using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;

public class AnniesHouseDialogue : MyDialogBase
{
    private const float BOTTOM_Y_POSITION = 10.58f;
    private const float TOP_Y_POSITION = 13.32f;
    private const float Y_POSITION_TO_LOAD_NEXT_LEVEL_ON = 12.5f;
    private const string SHOULD_PLAY_ANNIE_MOVING_UP_ANIMATION = "shouldMoveUpAndStop";

    private Animator m_AnnieAnimator;
    private Animator m_MundoAnimator;
    private Vector3 m_AnnieStartLocation;
    private Vector3 m_MundoDoorLocation;
    private Vector3 m_MundoHouseLocation;
    private Vector3 m_AnnieEndLocation;

    private bool m_HasAnnieComeDownstairs = false;
    private bool m_MundoHasComeInside = false;
    private bool m_IsOnFinalDialogue = false;

    private float m_MovementTimer = 0.0f;

    [SerializeField]
    private GameObject m_MundoObject;

    [SerializeField]
    private GameObject m_AnnieObject;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        m_MundoAnimator = m_MundoObject.GetComponent<Animator>();
        m_AnnieAnimator = m_AnnieObject.GetComponent<Animator>();

        m_MundoDoorLocation = m_MundoObject.transform.position;
        m_MundoHouseLocation = m_MundoDoorLocation;
        m_MundoHouseLocation.y = BOTTOM_Y_POSITION;

        m_AnnieStartLocation = m_AnnieObject.transform.position;

        m_AnnieEndLocation = m_AnnieStartLocation;
        m_AnnieEndLocation.y = BOTTOM_Y_POSITION;

        List<DialogData> myDialogScript = new List<DialogData>();

        myDialogScript.Add(new DialogData("Annie's Mom: Annie! Can you come down please?!", "Annie's Mom", () => m_AnnieAnimator.SetBool(SHOULD_PLAY_ANNIE_MOVING_UP_ANIMATION, true)));

        m_DialogManager.Show(myDialogScript);
    }

    private void ShowAnnieDialogue()
    {
        List<DialogData> myDialogScript = new List<DialogData>();

        myDialogScript.Add(new DialogData("Annie: What is it mom?", "Annie"));
        myDialogScript.Add(new DialogData("Annie's Mom: Mundo's been wondering where you've been...", "Annie's Mom"));
        myDialogScript.Add(new DialogData("Annie: Mundo? I want to play with Mundo!", "Annie"));
        myDialogScript.Add(new DialogData("Annie's Dad: Well then! You're in luck because he's right outside!", "Annie's Dad"));
        myDialogScript.Add(new DialogData("Annie's Dad: Mundo! Come on in!", "Annie's Dad", () => MoveMundoInsideHouse()));

        m_DialogManager.Show(myDialogScript);
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

        m_DialogManager.Show(myDialogScript);
    }

    private void ShowFinalDialogue()
    {
        List<DialogData> myDialogScript = new List<DialogData>();

        myDialogScript.Add(new DialogData("Mundo: Alright! We're off!", "Mundo", () => m_MundoAnimator.SetBool("shouldLeaveHouse", true)));

        m_DialogManager.Show(myDialogScript);
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

        if(m_AnnieAnimator.GetBool(SHOULD_PLAY_ANNIE_MOVING_UP_ANIMATION))
        {
            m_MovementTimer += Time.deltaTime;
            m_AnnieObject.transform.position = Vector3.Lerp(m_AnnieStartLocation, m_AnnieEndLocation, m_MovementTimer);
        } else if(m_MundoAnimator.GetBool("shouldWalkInHouse"))
        {
            m_MovementTimer += Time.deltaTime;
            m_MundoObject.transform.position = Vector3.Lerp(m_MundoDoorLocation, m_MundoHouseLocation, m_MovementTimer);
        } else if(m_MundoAnimator.GetBool("shouldLeaveHouse"))
        {
            m_MovementTimer += Time.deltaTime;
            m_MundoObject.transform.position = Vector3.Lerp(m_MundoHouseLocation, m_MundoDoorLocation, m_MovementTimer);
        }

        if(!m_HasAnnieComeDownstairs && Mathf.Approximately(m_AnnieObject.transform.position.y, BOTTOM_Y_POSITION))
        {
            m_HasAnnieComeDownstairs = true;
            m_MovementTimer = 0.0f;
            m_AnnieAnimator.SetBool(SHOULD_PLAY_ANNIE_MOVING_UP_ANIMATION, false);
            ShowAnnieDialogue();
        } else if(!m_MundoHasComeInside && Mathf.Approximately(m_MundoObject.transform.position.y, BOTTOM_Y_POSITION))
        {
            m_MundoHasComeInside = true;
            m_MovementTimer = 0.0f;
            m_MundoAnimator.SetBool("shouldWalkInHouse", false);
            ShowMundoDialogue();
        } else if(!m_IsOnFinalDialogue && Input.GetKeyUp(KeyCode.F) && CanPlayerMove())
        {
            m_IsOnFinalDialogue = true;
            ShowFinalDialogue();
        } else if(m_IsOnFinalDialogue && m_MundoObject.transform.position.y >= Y_POSITION_TO_LOAD_NEXT_LEVEL_ON)
        {
            m_LevelLoader.LoadNextLevel();
        }
    }
}
