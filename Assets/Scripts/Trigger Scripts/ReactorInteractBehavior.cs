using System.Collections;
using System.Collections.Generic;
using Doublsb.Dialog;
using UnityEngine;

public class ReactorInteractBehavior : MonoBehaviour
{
    private const string POP_UP_TEXT = "Press F to Interact With Reactor!";
    private const int TOTAL_NUM_BATTERIES = 3;

    private static List<DialogData> s_DialogWithoutBatteries = new List<DialogData>();
    private static List<DialogData> s_DialogWithBatteries = new List<DialogData>();

    public static bool s_PlayerInTrigger = false;

    [SerializeField]
    private GameObject m_DialogueObject;

    private TunnelDialogue m_TunnelDialogue;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            BatteryBehavior.s_PopUpTextObject.SetActive(true);
            s_PlayerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            BatteryBehavior.s_PopUpTextObject.SetActive(false);
            s_PlayerInTrigger = false;
        }
    }

    private static List<DialogData> GetDialogData(int numBatteriesHeld)
    {
        if (numBatteriesHeld  < TOTAL_NUM_BATTERIES)
        {
            return s_DialogWithoutBatteries;
        }
        else 
        {
            CheckWinStateBehavior.s_GateAnimator.SetBool("openGate", true);
            return s_DialogWithBatteries;
        }
    }

    private void Start()
    {
        m_TunnelDialogue = m_DialogueObject.GetComponent<TunnelDialogue>();
        s_DialogWithBatteries.Clear();
        s_DialogWithoutBatteries.Clear();

        s_DialogWithoutBatteries.Add(new DialogData("Mundo: Hm.....", "Mundo"));
        s_DialogWithoutBatteries.Add(new DialogData("Mundo: It looks like this reactor connects to that door on the right...", "Mundo"));
        s_DialogWithoutBatteries.Add(new DialogData("Mundo: ...and there it looks like there are some batteries missing from the power?", "Mundo"));
        s_DialogWithoutBatteries.Add(new DialogData("Annie: Leave it up to me! I'm the best seeker in all of hide & seek!", "Annie"));
        s_DialogWithoutBatteries.Add(new DialogData("Mundo: Ha! Well okay then, looks like we're looking for ... /speed = 0.2/ three giant batteries?", "Mundo"));
        s_DialogWithoutBatteries.Add(new DialogData("Annie: Annie's on the case!", "Annie"));

        s_DialogWithBatteries.Add(new DialogData("Mundo: Hey, look at that! We got the door open!", "Mundo"));
        s_DialogWithBatteries.Add(new DialogData("Annie: Hmph. Where's my thank you, huh?", "Annie"));
        s_DialogWithBatteries.Add(new DialogData("Mundo: Good point! Thank you Annie. ", "Mundo"));
        s_DialogWithBatteries.Add(new DialogData("Annie: Let's go before they catch up!!", "Annie"));
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && s_PlayerInTrigger && m_TunnelDialogue.CanPlayerMove())
        {
            m_TunnelDialogue.DisplayDialogue(GetDialogData(MundoMovement.s_NumHeldBatteries));
        }
    }
}
