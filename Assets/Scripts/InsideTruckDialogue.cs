using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;

public class InsideTruckDialogue : MyDialogBase
{
    private List<DialogData> m_TruckDialogData = new List<DialogData>();

    [SerializeField]
    private GameObject m_HackingDeviceObject;

    [SerializeField]
    private GameObject m_AnnieObject;

    private Vector3 m_AnnieInitialPosition;

    private float m_MovementTimer = 0.0f;
    private bool m_ShouldMoveAnnie = false;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        m_AnnieInitialPosition = m_AnnieObject.transform.position;

        m_TruckDialogData.Add(new DialogData("Mundo: Okay, I think we're safe for now...", "Mundo"));
        m_TruckDialogData.Add(new DialogData("Annie: ...................... *sobs*", "Annie"));
        m_TruckDialogData.Add(new DialogData("Mundo: Annie... I can't imagine how you must feel right now...", "Mundo"));
        m_TruckDialogData.Add(new DialogData("Annie: .......Mundo? Why do bad people exist? Why would anyone kill my parents like that?", "Annie"));
        m_TruckDialogData.Add(new DialogData("Mundo: I don't know Annie. It sounds like the empire of Zion was responsible, & they've caused the destruction of many cities & villages.", "Mundo"));
        m_TruckDialogData.Add(new DialogData("Annie: But why?", "Annie"));
        m_TruckDialogData.Add(new DialogData("Mundo: I don't know....", "Mundo"));
        m_TruckDialogData.Add(new DialogData("Annie: .............huh? What is that?", "Annie", () => MoveAnnie()));
        m_TruckDialogData.Add(new DialogData("Mundo: It actually looks familiar. We should take it! It might be useful...", "Mundo"));
        m_TruckDialogData.Add(new DialogData("You've gained access to hacking device! Hold right click + drag + unclick when mouse is initially on top of enemy to move the enemy to placed location", "Annie", 
            () => m_HackingDeviceObject.SetActive(false), false));
        m_TruckDialogData.Add(new DialogData("Mundo: I think we're getting close. We'll find a place & people to call our home, I'm sure of it", "Mundo"));
        m_TruckDialogData.Add(new DialogData("Annie: ................", "Annie"));

        m_DialogManager.Show(m_TruckDialogData);
    }

    private void MoveAnnie()
    {
        m_ShouldMoveAnnie = true;
        m_AnnieObject.GetComponent<Animator>().SetTrigger("MoveAnnieInTruck");
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if(m_ShouldMoveAnnie)
        {
            m_MovementTimer += Time.deltaTime;
            m_AnnieObject.transform.position = Vector3.Lerp(m_AnnieInitialPosition, m_HackingDeviceObject.transform.position, m_MovementTimer / 2.0f);
        }

        if(m_MovementTimer >= 2.0f && CanPlayerMove())
        {
            m_LevelLoader.LoadNextLevel();
        }
    }
}
