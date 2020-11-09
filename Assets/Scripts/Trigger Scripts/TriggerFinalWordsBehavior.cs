using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;

public class TriggerFinalWordsBehavior : MonoBehaviour
{
    private List<DialogData> m_FinalsWordsDialogue = new List<DialogData>();

    [SerializeField]
    private GameObject m_DialogueGameObject;

    [SerializeField]
    private GameObject m_EnemiesContainer;

    [SerializeField]
    private GameObject m_BelowGardenCollider;

    private MyDialogBase m_DialogueBase;

    private bool m_HasDialogueTriggered = false;

    private void Awake()
    {
        m_FinalsWordsDialogue.Add(new DialogData("Mundo: /speed: 0.1/Oh no...", "Mundo", null, false));
        m_FinalsWordsDialogue.Add(new DialogData("Annie: Mama? Are you- no this can't be happening...", "Annie"));
        m_FinalsWordsDialogue.Add(new DialogData("Annie's Mom: I'm so sorry. No child should have to see their mother in such a state...", "Annie's Mom"));
        m_FinalsWordsDialogue.Add(new DialogData("Annie: Mama! You can't leave me alone! What am I going to do?", "Annie"));
        m_FinalsWordsDialogue.Add(new DialogData("Annie's Mom: I know Annie, but we don't have a choice & I don't have much time left.", "Annie's Mom"));
        m_FinalsWordsDialogue.Add(new DialogData("Annie's Mom: I never expected the soldiers of Zion to strike us so ruthlessly.", "Annie's Mom"));
        m_FinalsWordsDialogue.Add(new DialogData("Annie's Mom: Mundo, I need you to protect Annie. I need you two to find a safe community of people & no matter what, stay hidden.", "Annie's Mom"));
        m_FinalsWordsDialogue.Add(new DialogData("Annie's Mom: Do you understand?", "Annie's Mom"));
        m_FinalsWordsDialogue.Add(new DialogData("Mundo: I understand. Where should we go?", "Mundo"));
        m_FinalsWordsDialogue.Add(new DialogData("Annie's Mom: There's a road past the garden. Make your way there & try to escape if you can.", "Annie's Mom"));
        m_FinalsWordsDialogue.Add(new DialogData("Annie's Mom: Oh and Mundo, I need you to take this...", "Annie's Mom", 
            () => { MundoMovement.s_NumKnifesLeft = 1; }));
        m_FinalsWordsDialogue.Add(new DialogData("You've been given a knife! When num knifes > 0, press [Space] to use the knife whenever you're not holding Annie (place or pickup Annie with [F]).", "Mundo", null, false));
        m_FinalsWordsDialogue.Add(new DialogData("Annie's Mom: Use this to keep Annie safe, but only use it when you absolutely must.", "Annie's Mom"));
        m_FinalsWordsDialogue.Add(new DialogData("Annie: Mama, why are you saying that? You're going to come with us, right?", "Annie"));
        m_FinalsWordsDialogue.Add(new DialogData("Annie's Mom: /speed: 0.1/. . . . ", "Annie's Mom"));
        m_FinalsWordsDialogue.Add(new DialogData("Annie: Mama? No, Mama!", "Annie"));
        m_FinalsWordsDialogue.Add(new DialogData("Mundo: I know Annie. But we need to go, now! C'mon before any of the people who attacked us come close!", "Mundo"));
    }

    // Start is called before the first frame update
    void Start()
    {
        m_DialogueBase = m_DialogueGameObject.GetComponent<MyDialogBase>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !m_HasDialogueTriggered)
        {
            m_HasDialogueTriggered = true;
            m_DialogueBase.DisplayDialogue(m_FinalsWordsDialogue);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(m_HasDialogueTriggered && m_DialogueBase.CanPlayerMove())
        {
            m_EnemiesContainer.SetActive(true);
            m_BelowGardenCollider.SetActive(false);
        }
    }
}
