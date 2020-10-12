using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryBehavior : MonoBehaviour
{
    [SerializeField]
    private GameObject m_InteractPopUpTextObject;

    private static GameObject s_PopUpTextObject;

    private void Start()
    {
        if(m_InteractPopUpTextObject)
        {
            s_PopUpTextObject = m_InteractPopUpTextObject;
        }
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
