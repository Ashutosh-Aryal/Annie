using System.Collections;
using System.Collections.Generic;
using Doublsb.Dialog;
using UnityEngine;

public class FinalDialogue : MyDialogBase
{
    private List<DialogData> m_DialogDatas = new List<DialogData>();
    new void Start() {

        base.Start();

        m_DialogDatas.Add(new DialogData("Mundo: Alright, let's go before someone catches up.", "Mundo"));

        m_DialogManager.Show(m_DialogDatas);
    }

    new void Update()
    {
        base.Update();
    }
}
