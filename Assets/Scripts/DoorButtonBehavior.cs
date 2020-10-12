using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorButtonBehavior : MonoBehaviour
{
    private const string ANNIE_TAG = "annie";
    private const string MUNDO_TAG = "Player";
    private const int CLOSED_SORTING_ORDER = 1;
    private const int OPEN_SORTING_ORDER = 4;

    private GameObject m_DoorObject;
    private Animator m_DoorAnimator;

    private Vector2 m_DoorBoxColliderOffset;
    private Vector2 m_DoorBoxColliderSize;

    private bool m_AnnieOnButton = false;

    [SerializeField]
    private GameObject m_DropAnnieHereText;

    private static GameObject s_InteractText;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isCollidingWithAnnie = collision.CompareTag(ANNIE_TAG);
        bool isCollidingWithMundo = collision.CompareTag(MUNDO_TAG);
        
        if(isCollidingWithAnnie)
        {
            Destroy(m_DoorObject.GetComponent<BoxCollider2D>());
            m_DoorObject.GetComponent<SpriteRenderer>().sortingOrder = OPEN_SORTING_ORDER;
            m_DoorAnimator.SetBool("isTriggered", true);
            s_InteractText.SetActive(false);
            m_AnnieOnButton = true;
        } else if(isCollidingWithMundo && !m_AnnieOnButton)
        {
            s_InteractText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        bool isCollidingWithAnnie = collision.CompareTag(ANNIE_TAG);
        bool isCollidingWithMundo = collision.CompareTag(MUNDO_TAG);

        if (isCollidingWithAnnie)
        {
            BoxCollider2D readdedBoxCollider = m_DoorObject.AddComponent<BoxCollider2D>();
            readdedBoxCollider.size = m_DoorBoxColliderSize;
            readdedBoxCollider.offset = m_DoorBoxColliderOffset;
            m_DoorObject.GetComponent<SpriteRenderer>().sortingOrder = CLOSED_SORTING_ORDER;
            m_DoorAnimator.SetBool("isTriggered", false);
            s_InteractText.SetActive(true);
        } else if(isCollidingWithMundo)
        {
            s_InteractText.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_DoorObject = gameObject.transform.parent.GetChild(0).gameObject;
        m_DoorAnimator = m_DoorObject.GetComponent<Animator>();

        BoxCollider2D bc = m_DoorObject.GetComponent<BoxCollider2D>();
        m_DoorBoxColliderOffset = bc.offset;
        m_DoorBoxColliderSize = bc.size;

        if(m_DropAnnieHereText)
        {
            s_InteractText = m_DropAnnieHereText;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
