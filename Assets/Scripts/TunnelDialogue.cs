using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;
using UnityEngine.UI;

public class TunnelDialogue : MonoBehaviour
{
    private const KeyCode SKIP_DIALOGUE_KEY = KeyCode.Space;
    private List<DialogData> allDialogScript = new List<DialogData>();
    private DialogManager dialogueManager;

    public static bool s_ShouldMove = false;

    void Start()
    {
        dialogueManager = gameObject.GetComponent<DialogManager>();

        allDialogScript.Add(new DialogData("Annie: Mundo...? Where are we? What is this place?", "Annie"));

        allDialogScript.Add(new DialogData("Mundo: I'm not sure, but we can't stay here. They're right behind us & the only way forward is forward", "Mundo"));

        allDialogScript.Add(new DialogData("Annie: I'm scared. What if we get caught? I don't want to end up like mama.", "Annie"));

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

        
        if(dialogueManager.isFinished)
        {
            s_ShouldMove = true;
        }
    }
}
