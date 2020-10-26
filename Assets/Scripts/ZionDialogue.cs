using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;

public class ZionDialogue : MyDialogBase
{
    private List<DialogData> m_ZionStartDialogue = new List<DialogData>();

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        m_ZionStartDialogue.Add(new DialogData("Mundo: Alright, looks like we've arrived in Zion without any hiccups.", "Mundo"));
        m_ZionStartDialogue.Add(new DialogData("Annie: We should make sure that none of those bad men see us.", "Annie"));
        m_ZionStartDialogue.Add(new DialogData("Mundo: Yeah, let's try & use that hacking device we picked up on the truck!", "Mundo"));
        m_ZionStartDialogue.Add(new DialogData("You must be holding Annie in order to use the hacking device. To use, right click on an enemy & drag to location you want enemy to move to.", "Mundo"));

        m_DialogManager.Show(m_ZionStartDialogue);
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }
}
