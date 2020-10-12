using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;
using UnityEngine.UI;

public class TunnelDialogue : MonoBehaviour
{
    private const KeyCode SKIP_DIALOGUE_KEY = KeyCode.Space;
    private List<DialogData> allDialogScript = new List<DialogData>();
    private static DialogManager dialogueManager;

    public static bool s_ShouldMove = false;

    public static bool ShouldMove() {

        s_ShouldMove = (null == dialogueManager)? true: dialogueManager.isFinished;
        return s_ShouldMove;
    }

    public static void DisplayDialogue(List<DialogData> displayedDialog) {
        if (dialogueManager.myDialogData.Count == 0)
        {
            dialogueManager.Show(displayedDialog);
        }
    }

    void Start() {

        dialogueManager = gameObject.GetComponent<DialogManager>();

        allDialogScript.Add(new DialogData("Annie: Mundo...? Where are we? What is this place?", "Annie"));

        allDialogScript.Add(new DialogData("Mundo: I'm not sure, but we can't stay here. They're right behind us & the only way forward is forward", "Mundo"));

        allDialogScript.Add(new DialogData("Annie: I'm scared. What if they come again? I don't want to end up like mama...", "Annie"));

        allDialogScript.Add(new DialogData("Mundo: I won't let anyone hurt you, okay? Let's just keep moving. Things have had a way of working out so far, right?", "Mundo"));

        allDialogScript.Add(new DialogData("Annie: /speed: 0.2/ .......", "Annie"));

        dialogueManager.Show(allDialogScript);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(SKIP_DIALOGUE_KEY))
        {
            dialogueManager.Click_Window();
        }

        if(dialogueManager.myDialogData.Count == 0 && ReactorInteractBehavior.s_PlayerInTrigger)
        {
            BatteryBehavior.s_PopUpTextObject.SetActive(true);
        }

        s_ShouldMove = dialogueManager.isFinished;
    }
}
