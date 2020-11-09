using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;

public class TriggerBombingBehavior : MonoBehaviour
{
    private List<DialogData> preBombingDialogData = new List<DialogData>();
    private List<DialogData> postBombingDialogData = new List<DialogData>();
    private bool m_TriggeredBombingDialogue = false;
    private bool m_TriggeredPostBombingDialogue = false;

    private AudioSource m_RingingSoundEffect;

    [SerializeField]
    private GameObject m_DialogueGameObject;

    [SerializeField]
    private GameObject m_DeadParentsContainer;

    [SerializeField]
    private GameObject m_FiresContainer;

    [SerializeField]
    private GameObject m_PreBombMusicObject;

    [SerializeField]
    private GameObject m_PostBombMusicObject;

    [SerializeField]
    private GameObject m_PreBombAmbientObject;

    [SerializeField]
    private GameObject m_PostBombAmbientObject;

    private MyDialogBase m_DialogueBase;

    private void Start()
    {
        m_DialogueBase = m_DialogueGameObject.GetComponent<MyDialogBase>();

        preBombingDialogData.Add(new DialogData("Mundo: We're here!", "Mundo"));
        preBombingDialogData.Add(new DialogData("Annie: Yay! Finally!", "Annie"));
        preBombingDialogData.Add(new DialogData("Mundo: It would have taken less time if we'd both walked instead...", "Mundo"));
        preBombingDialogData.Add(new DialogData("Annie: Yeah! But that's not as fun.", "Annie", () => m_RingingSoundEffect.PlayOneShot(m_RingingSoundEffect.clip)));
        preBombingDialogData.Add(new DialogData("/speed: 0.1/Mundo:*sigh* do you have to be so- wait... can you hear that too?", "Mundo", () => { m_PreBombAmbientObject.SetActive(false); m_PreBombMusicObject.SetActive(false); }, false));
        
        preBombingDialogData.Add(new DialogData("/speed: 0.1/. . . . .", "Mundo", () => { m_PostBombMusicObject.SetActive(true); m_PostBombAmbientObject.SetActive(true); }, false));

        postBombingDialogData.Add(new DialogData("Mundo: Annie! Annie, are you okay?", "Mundo"));
        postBombingDialogData.Add(new DialogData("Annie: I think so. What was that?", "Annie"));
        postBombingDialogData.Add(new DialogData("Mundo: I don't know, but it sounds like it can from the village! We need to get back home!", "Mundo"));
        postBombingDialogData.Add(new DialogData("Annie: /speed: 0.2/ I'm scared...", "Annie"));

        m_RingingSoundEffect = gameObject.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && !m_TriggeredBombingDialogue)
        {
            m_DialogueBase.DisplayDialogue(preBombingDialogData);
            m_TriggeredBombingDialogue = true;
        }
    }

    private void Update()
    {
        bool canPlayerMove = m_DialogueBase.CanPlayerMove();
        if (m_TriggeredPostBombingDialogue && canPlayerMove) {
            m_DeadParentsContainer.SetActive(true);
            m_FiresContainer.SetActive(true);
        } else if (m_TriggeredBombingDialogue && canPlayerMove)
        {
            m_DialogueBase.DisplayDialogue(postBombingDialogData);
            m_TriggeredPostBombingDialogue = true;
        }
    }
}
