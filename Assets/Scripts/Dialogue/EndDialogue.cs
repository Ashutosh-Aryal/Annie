using System.Collections;
using System.Collections.Generic;
using Doublsb.Dialog;
using UnityEngine;

public class EndDialogue : MyDialogBase
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        List<DialogData> myDialogScript = new List<DialogData>();

        myDialogScript.Add(new DialogData("...................", "Dad"));
        myDialogScript.Add(new DialogData("Annie's Mom: Was that....Annie?", "Mom"));
        myDialogScript.Add(new DialogData("Annie's Dad: I think so...", "Dad"));
        myDialogScript.Add(new DialogData(". . . . . . . . . . . . .", "Dad"));
        myDialogScript.Add(new DialogData("Annie's Mom: That must have been fake, there's no way that-", "Mom"));
        myDialogScript.Add(new DialogData("Annie's Dad: But what if it was?", "Dad"));
        myDialogScript.Add(new DialogData("Annie's Mom: What are you saying?", "Mom"));
        myDialogScript.Add(new DialogData("Annie's Dad: I don't know. Should we have this child if that is its future?", "Dad"));
        myDialogScript.Add(new DialogData("Annie's Mom: Eric....life is a gift. There's no other choice here.", "Mom"));
        myDialogScript.Add(new DialogData("Annie's Dad: But-", "Dad"));
        myDialogScript.Add(new DialogData("Annie's Mom: Now that we know that's a possibility for our child, don't you think we can now plan ahead to ensure it doesn't happen?", "Mom"));
        myDialogScript.Add(new DialogData("Annie's Mom: Besides... the alternative would also kill her, just a lot sooner. And I will never accept that because I'm not a murderer.", "Mom"));
        myDialogScript.Add(new DialogData("Annie's Dad: But our daughter will suffer... how can we-", "Dad"));
        myDialogScript.Add(new DialogData("Annie's Mom: We'll make sure she's okay. Okay? Don't let that mysterious figure influence what is OUR decision to make.", "Mom"));
        myDialogScript.Add(new DialogData("Annie's Dad: And what about Annie's choice?", "Dad"));
        myDialogScript.Add(new DialogData("Annie's Mom: What choice? She hasn't even been born yet, how can she understand her position & make a choice?", "Mom"));
        myDialogScript.Add(new DialogData("Annie's Dad: Exactly. Right now, we have all the choice & she has none. How is that fai-", "Dad"));
        myDialogScript.Add(new DialogData("Annie's Mom: Honey.....relax. Think of how long we've waited to have this kid. And you know my parents have been waiting for grandchildren...", "Mom"));
        myDialogScript.Add(new DialogData("Annie's Dad: /speed: 0.1/ ........I know. Okay then. We'll protect her. We HAVE to.", "Dad"));
        myDialogScript.Add(new DialogData("Annie's Mom: That's the spirit. And regardless, it's HER life to live. Perhaps that's just the start of God's plan for her...", "Mom"));
        myDialogScript.Add(new DialogData("Annie's Dad: Okay, yes. We can do this. We will do this. We will give her a better ending than that, I swear it.", "Dad"));
        myDialogScript.Add(new DialogData("Annie's Dad: I can't wait to see her...", "Dad"));
        myDialogScript.Add(new DialogData("Annie's Mom: Me neither. And don't worry. I'm sure this story has a happy ending!", "Mom"));

        m_DialogManager.Show(myDialogScript);
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        bool shouldLoadNextLevel = m_DialogManager.isFinished || m_DialogManager == null;
        if (shouldLoadNextLevel) {
            m_LevelLoader.LoadNextLevel();
        }
    }
}
