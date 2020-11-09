using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;

public class NPCTrigger : MonoBehaviour
{
    [SerializeField] private string[] m_DialogueText;
    [SerializeField] private string[] m_Characters;
    [SerializeField] private GameObject m_InteractTextObject;
    [SerializeField] private GameObject m_DialogueObject;
    [SerializeField] private GameObject m_ObjectToSetActive;
    [SerializeField] private bool m_ShouldStartOnInteract = true;

    private MyDialogBase m_DialogBase;
    private List<DialogData> m_DialogDatas = new List<DialogData>();
    private bool m_PlayerInTrigger = false;
    private bool m_ShouldRepopulate = false;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            if (m_ShouldStartOnInteract) {
                m_InteractTextObject.SetActive(true);
                m_PlayerInTrigger = true;
            } else {
                m_DialogBase.DisplayDialogue(m_DialogDatas);
                Destroy(gameObject);
                
                if (m_ObjectToSetActive) {
                    m_ObjectToSetActive.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player") && m_ShouldStartOnInteract) {
            BatteryBehavior.s_PopUpTextObject.SetActive(false);
            m_PlayerInTrigger = false;
        }
    }


    // Start is called before the first frame update
    void Start() {

        m_DialogBase = m_DialogueObject.GetComponent<MyDialogBase>();

        PopulateDialogData();
    }

    private void PopulateDialogData() {

        List <DialogData> temp = new List<DialogData>();

        m_DialogDatas.Clear();

        for (int x = 0; x < m_DialogueText.Length; x++) {
            temp.Add(new DialogData(m_DialogueText[x], m_Characters[x]));
        }

        m_DialogDatas = temp;
    }

    // Update is called once per frame
    void Update()
    {

        if (m_ShouldRepopulate && m_DialogBase.CanPlayerMove()) {
            PopulateDialogData();
            m_ShouldRepopulate = false;
        }


        if (m_ShouldStartOnInteract) {
            if (Input.GetKeyDown(KeyCode.E) && m_PlayerInTrigger && m_DialogBase.CanPlayerMove()) {
                m_DialogBase.DisplayDialogue(m_DialogDatas);
                m_ShouldRepopulate = true;
            }
        }
    }
}
