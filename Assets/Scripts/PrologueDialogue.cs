using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;
using System.Transactions;

public class PrologueDialogue : MyDialogBase
{   
    private Animator m_MysteriousFigureAnimator;
    
    [SerializeField]
    private GameObject m_MysteriousFigureObject;

    [SerializeField]
    private GameObject m_TerminalObject;

    [SerializeField]
    private GameObject m_DadObject;

    new void Start() {

        base.Start();

        m_DialogManager = gameObject.GetComponent<DialogManager>();
        m_MysteriousFigureAnimator = m_MysteriousFigureObject.GetComponent<Animator>();

        List<DialogData> myDialogScript = new List<DialogData>();

        myDialogScript.Add(new DialogData("Unknown Man: There's truly nothing like camping in the morning...", "Dad"));
        myDialogScript.Add(new DialogData("Unknown Woman: I'd have to disagree with you on account of my swollen everything. What possessed you to take your overly pregnant wife on a camping trip???", "Mom"));
        myDialogScript.Add(new DialogData("Unknown Man: Why, love, of course!", "Dad"));
        myDialogScript.Add(new DialogData("Unknown Woman: Ha! You're lucky your wife loves you enough to put up with your heaps of bullshit.", "Mom"));
        myDialogScript.Add(new DialogData("Unknown Man: Hey! No swearing around Annie!!! Do you want her to come out the womb already able to call out my bullshit?", "Dad"));
        myDialogScript.Add(new DialogData("Unknown Woman: Huh. I guess you have a point.", "Mom"));
        myDialogScript.Add(new DialogData("Unknown Woman: We should give her a couple years to naturally get used to your bullshit on her own.", "Mom"));
        myDialogScript.Add(new DialogData("Unknown Man: Hey!!!", "Dad"));
        myDialogScript.Add(new DialogData("Mysterious Voice: If only you knew what you were doing...", "Mysterious Figure",
            () => m_MysteriousFigureAnimator.SetTrigger("FadeIn"), false));
        myDialogScript.Add(new DialogData("Unknown Man: Who's there?!", "Dad", () => FlipDad()));

        myDialogScript.Add(new DialogData("Mysterious Figure: Unimportant. A better question is what I am about to give you. /speed:0.2/...", 
            "Mysterious Figure"));

        myDialogScript.Add(new DialogData("Unknown Woman: Excuse me?! What gives you the righ-/close/", "Mom"));
        myDialogScript.Add(new DialogData("Mysterious Figure: I know, I know. Humans and their natalist bullshit...", "Mysterious Figure"));
        myDialogScript.Add(new DialogData("Mysterious Figure: So, don't take my word for it.", "Mysterious Figure"));
        
        myDialogScript.Add(new DialogData("Mysterious Figure: Take this...", "Mysterious Figure", 
            () => m_TerminalObject.SetActive(true)));
        
        myDialogScript.Add(new DialogData("Mysterious Figure: ...and see what the future holds for your bloodline.", "Mysterious Figure", 
            () => m_MysteriousFigureAnimator.SetTrigger("FadeOut")));
        
        myDialogScript.Add(new DialogData("/speed:0.1/ . . . . . . . . . . .", "Dad")); //, null, false));

        m_DialogScript.Add(myDialogScript);

        m_DialogManager.Show(m_DialogScript[0]);
    }

    private void FlipDad()
    {
        SpriteRenderer spriteRenderer = m_DadObject.GetComponent<SpriteRenderer>();

        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    // Update is called once per frame
    new void Update() {

        base.Update();

        bool shouldLoadNextLevel = m_DialogManager.isFinished || m_DialogManager == null;
        if(shouldLoadNextLevel) {
            m_LevelLoader.LoadNextLevel();
        }
    }
}
