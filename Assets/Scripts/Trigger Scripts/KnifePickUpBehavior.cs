using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KnifePickUpBehavior : MonoBehaviour
{
    private float m_PickUpInteractTimer = 2.0f;
    private bool m_HasTriggered = false;
    public GameObject m_InteractText;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !m_HasTriggered)
        {
            m_HasTriggered = true;
            Destroy(gameObject.GetComponent<SpriteRenderer>());
            MundoMovement.s_NumKnifesLeft++;
            StartCoroutine("ShowPickUpKnifeScripts");
        }
    }

    IEnumerator ShowPickUpKnifeScripts() {

        TextMeshProUGUI textMesh = m_InteractText.GetComponent<TextMeshProUGUI>();
        string prevString = textMesh.text;
        m_InteractText.SetActive(true);
        textMesh.text = "You picked up a knife!";
        yield return new WaitForSeconds(m_PickUpInteractTimer);
        m_InteractText.SetActive(false);
        textMesh.text = prevString;
        Destroy(gameObject);
    }
}
