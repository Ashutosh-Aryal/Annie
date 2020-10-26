using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;
using UnityEngine.UI;

public class TunnelDialogue : MyDialogBase
{
   
    new void Start() {

        base.Start();

        List<DialogData> myDialogData = new List<DialogData>();

        myDialogData.Add(new DialogData("Annie: Mundo...? Where are we? What is this place?", "Annie"));

        myDialogData.Add(new DialogData("Mundo: I'm not sure, but we can't stay here. They're right behind us & the only way forward is forward", "Mundo"));

        myDialogData.Add(new DialogData("Annie: I'm scared. What if they come again? I don't want to end up like mama...", "Annie"));

        myDialogData.Add(new DialogData("Mundo: I won't let anyone hurt you, okay? Let's just keep moving. Things have had a way of working out so far, right?", "Mundo"));

        myDialogData.Add(new DialogData("Annie: /speed: 0.2/ .......", "Annie"));

        m_DialogManager.Show(myDialogData);
    }

    // Update is called once per frame
    new void Update() {

        base.Update();

        if(m_DialogManager.myDialogData.Count == 0 && ReactorInteractBehavior.s_PlayerInTrigger) {
            BatteryBehavior.s_PopUpTextObject.SetActive(true);
        }
    }
}
