using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;

public class TriggerFromVillageToTruck : MonoBehaviour
{
    private List<DialogData> m_FinalWordsDialogue = new List<DialogData>();

    [SerializeField]
    private GameObject m_DialogueGameObject;

    [SerializeField]
    private GameObject m_LevelLoaderObject;

    private MyDialogBase m_DialogueBase;
    private LevelLoader m_LevelLoader;

    private bool m_HasDialogueTriggered = false;

    // Start is called before the first frame update
    void Start()
    {
        m_FinalWordsDialogue.Add(new DialogData("Mundo: Okay, c'mon. Lets sneak aboard that cargo truck & see if we can stay hidden.", "Mundo"));
        m_FinalWordsDialogue.Add(new DialogData("Annie: /speed: 0.1/. . . . . . . . okay", "Annie", null, false));

        m_LevelLoader = m_LevelLoaderObject.GetComponent<LevelLoader>();
        m_DialogueBase = m_DialogueGameObject.GetComponent<MyDialogBase>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!m_HasDialogueTriggered && collision.CompareTag("Player"))
        {
            m_DialogueBase.DisplayDialogue(m_FinalWordsDialogue);
            m_HasDialogueTriggered = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(m_HasDialogueTriggered && m_DialogueBase.CanPlayerMove()) {
            m_LevelLoader.LoadNextLevel();
        }        
    }
}
