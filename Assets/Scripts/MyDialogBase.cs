using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;

public class MyDialogBase : MonoBehaviour
{
    protected LevelLoader m_LevelLoader;
    protected DialogManager m_DialogManager;
    protected List<List<DialogData>> m_DialogScript = new List<List<DialogData>>();

    [SerializeField]
    private GameObject m_LevelLoaderObject;

    public void DisplayDialogue(List<DialogData> displayedDialog) {

        bool isDialogManagerCurrentlyOccupied = m_DialogManager.myDialogData.Count != 0;
        if (!isDialogManagerCurrentlyOccupied) {
            m_DialogManager.Show(displayedDialog);
        }
    }
    public bool CanPlayerMove() {
        return (null == m_DialogManager) ? true : m_DialogManager.isFinished;
    }

    protected void Start()
    {
        if (m_LevelLoaderObject != null)
        {
            m_LevelLoader = m_LevelLoaderObject.GetComponent<LevelLoader>();
        }
    }

    // Update is called once per frame
    protected void Update() {

        if (!m_DialogManager.isFinished && Input.GetKeyDown(KeyCode.Space)) {
            m_DialogManager.Click_Window();
        }
    }
}
