using System.Collections;
using System.Collections.Generic;
using Doublsb.Dialog;
using UnityEngine;

public class VillageDialogue : MyDialogBase
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        List<DialogData> villageStartDialog = new List<DialogData>();

        villageStartDialog.Add(new DialogData("Mundo: Alright. Let's head to the garden!", "Mundo"));
        villageStartDialog.Add(new DialogData("Annie: Which way do we go?", "Annie"));
        villageStartDialog.Add(new DialogData("Mundo: We just follow the road headed east until we get there, remember? It's not too far.", "Mundo"));
        villageStartDialog.Add(new DialogData("Annie: Okay! Let's go!", "Annie"));

        m_DialogManager.Show(villageStartDialog);
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }
}
