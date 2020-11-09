using System.Collections;
using System.Collections.Generic;
using Doublsb.Dialog;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

public class PinkDrinkFactoryDialogue : MyDialogBase
{
    private List<DialogData> m_DialogDatas = new List<DialogData>();
    
    void Start() {

        base.Start(); 

        m_DialogDatas.Add(new DialogData("Annie: Mundo?", "Annie"));
        m_DialogDatas.Add(new DialogData("Mundo: Yes?", "Mundo"));
        m_DialogDatas.Add(new DialogData("Annie: Where exactly are we going? I know we can't go back. but what are we moving to?", "Annie"));
        m_DialogDatas.Add(new DialogData("Mundo: I'm not sure, but maybe we could ask that person up ahead what they know.", "Mundo"));
        m_DialogDatas.Add(new DialogData("Annie: Okay, but how will we know we've found what we're looking for?", "Annie"));
        m_DialogDatas.Add(new DialogData("Mundo: I don't know Annie. I'll settle for not being surrounded by enemies...", "Mundo"));
        m_DialogDatas.Add(new DialogData("Annie: Okay.... I hope we can find some good people...", "Annie"));
        m_DialogDatas.Add(new DialogData("Mundo: Me too....", "Mundo"));

        m_DialogManager.Show(m_DialogDatas);
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }
}
