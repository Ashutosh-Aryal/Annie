using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;

public class BatteryBehavior : MonoBehaviour
{
    [SerializeField]
    private GameObject m_InteractPopUpTextObject;

    public static GameObject s_PopUpTextObject;
    public static List<DialogData> s_DialogueOnFirstPickUp = new List<DialogData>();
    public static bool s_IsFirstPickedUpBattery = true;

    private void Start()
    {
        if(m_InteractPopUpTextObject)
        {
            s_PopUpTextObject = m_InteractPopUpTextObject;
        }

        s_DialogueOnFirstPickUp.Clear();

        s_DialogueOnFirstPickUp.Add(new DialogData("Annie: Huh? What's this?", "Annie"));

        s_DialogueOnFirstPickUp.Add(new DialogData("Mundo: It looks like a battery AND it looks important.", "Mundo"));

        s_DialogueOnFirstPickUp.Add(new DialogData("Mundo: Hey! It's probably for that reactor! Let's keep an eye out for more!", "Mundo"));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        MundoMovement.s_BatteryToPickUpObject = gameObject;
        s_PopUpTextObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        MundoMovement.s_BatteryToPickUpObject = null;
        s_PopUpTextObject.SetActive(false);
    }
}
