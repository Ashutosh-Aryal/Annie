using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;

public class ReactorInFactoryBehavior : MonoBehaviour
{
    private const int TOTAL_NUM_BATTERIES = 3;

    private static List<DialogData> s_DialogWithoutBatteries = new List<DialogData>();
    private static List<DialogData> s_DialogWithBatteries = new List<DialogData>();

    public static bool s_PlayerInTrigger = false;

    [SerializeField]
    private GameObject m_DoorObject;

    [SerializeField]
    private GameObject m_DialogueObject;

    private MyDialogBase m_DialogBase;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            BatteryBehavior.s_PopUpTextObject.SetActive(true);
            s_PlayerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            BatteryBehavior.s_PopUpTextObject.SetActive(false);
            s_PlayerInTrigger = false;
        }
    }

    private List<DialogData> GetDialogData(int numBatteriesHeld) {
        if (numBatteriesHeld < TOTAL_NUM_BATTERIES) {
            return s_DialogWithoutBatteries;
        } else {
            Destroy(m_DoorObject);
            return s_DialogWithBatteries;
        }
    }

    private void Start() {
        m_DialogBase = m_DialogueObject.GetComponent<MyDialogBase>();
        s_DialogWithBatteries.Clear();
        s_DialogWithoutBatteries.Clear();

        s_DialogWithoutBatteries.Add(new DialogData("Mundo: Hey look! It's another reactor!", "Mundo"));
        s_DialogWithoutBatteries.Add(new DialogData("Annie: Ooooooh! Do we get to go on another hunt for batteries?", "Annie"));
        s_DialogWithoutBatteries.Add(new DialogData("Mundo: Looks like it!", "Mundo"));
        s_DialogWithoutBatteries.Add(new DialogData("Mundo: It looks like there's three batteries to look for again. Lets be on the look out!", "Mundo"));

        s_DialogWithBatteries.Add(new DialogData("Mundo: We got the door open!", "Mundo"));
        s_DialogWithBatteries.Add(new DialogData("Annie: Hey Mundo?", "Annie"));
        s_DialogWithBatteries.Add(new DialogData("Mundo: Yes...?", "Mundo"));
        s_DialogWithBatteries.Add(new DialogData("Annie: I don't want to go through that door. I feel something strange & it's scary. Do YOU think we should go?", "Annie"));
        s_DialogWithBatteries.Add(new DialogData("Mundo: I don't know. But maybe that person at the front was a sign that theres people like us deeper within.", "Mundo"));
        s_DialogWithBatteries.Add(new DialogData("Mundo: I think it's at least worth exploring. If we don't like it, we'll find a way to get away!", "Mundo"));
        s_DialogWithBatteries.Add(new DialogData("Mundo: Lets just make sure we don't get any unwanted attention.", "Mundo"));
        s_DialogWithBatteries.Add(new DialogData("Annie: Okay...", "Annie"));
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.E) && s_PlayerInTrigger && m_DialogBase.CanPlayerMove()) {
            m_DialogBase.DisplayDialogue(GetDialogData(MundoMovement.s_NumHeldBatteries));
        }
    }
}
