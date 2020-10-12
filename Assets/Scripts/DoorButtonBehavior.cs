using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButtonBehavior : MonoBehaviour
{
    private const string ANNIE_TAG = "annie";

    private Animator m_DoorAnimator;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isCollidingWithAnnie = collision.CompareTag(ANNIE_TAG);
        
        if(isCollidingWithAnnie)
        {
            m_DoorAnimator.SetBool("isTriggered", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        bool isCollidingWithAnnie = collision.CompareTag(ANNIE_TAG);

        if (isCollidingWithAnnie)
        {
            m_DoorAnimator.SetBool("isTriggered", false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_DoorAnimator = gameObject.transform.parent.transform.GetChild(0).gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
