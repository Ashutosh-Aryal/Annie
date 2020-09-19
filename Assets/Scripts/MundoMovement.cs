using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

public class MundoMovement : MonoBehaviour {    
    enum AnimationType
    {
        Idle,
        MovingRight, 
        MovingLeft, 
        MovingUp,
        MovingDown,
        StartAttacking,
        StopAttacking
    }

    private enum DirectionFacing
    {
        North, East, South, West
    }

    private const KeyCode UP_KEY = KeyCode.W;
    private const KeyCode LEFT_KEY = KeyCode.A;
    private const KeyCode RIGHT_KEY = KeyCode.D;
    private const KeyCode DOWN_KEY = KeyCode.S;
    private const KeyCode INTERACT_WITH_ANNIE_KEY = KeyCode.F;
    private const KeyCode ATTACK_KEY = KeyCode.Space;

    private const float MAX_INTERACT_DISTANCE = 1.0f;

    private static AnimationType se_AnimationType = AnimationType.Idle;
    private static DirectionFacing se_DirectionFacing = DirectionFacing.East;
    private static bool sb_IsHoldingAnnie = true;
    private static bool sb_CanInteract = true;

    private static List<GameObject> sL_AvailableEnemiesToAttack = new List<GameObject>();

    private static Rigidbody2D myRigidbody;
    private static Animator myAnimator;
    private static SpriteRenderer myRenderer;

    [SerializeField] GameObject m_AnnieObject;
    [SerializeField] GameObject m_EnemyPrefab;

    // Start is called before the first frame update
    void Start()
    {
        myRenderer = gameObject.GetComponent<SpriteRenderer>();
        myAnimator = gameObject.GetComponent<Animator>();
        myRigidbody = gameObject.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isCollisionEnemy = collision.gameObject.CompareTag(m_EnemyPrefab.tag);
        bool isCollisionAnnie = collision.gameObject.CompareTag(m_AnnieObject.tag);

        if(isCollisionEnemy)
        {
            sL_AvailableEnemiesToAttack.Add(collision.gameObject);
        } else if(isCollisionAnnie)
        {

        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckInteract();
        UpdateMovement();
        UpdateAnimation();
    }

    private void CheckInteract()
    {
        if(sb_IsHoldingAnnie) {
            sb_CanInteract = true; return;
        }

        Vector3 mundoPosition = gameObject.transform.position;
        Vector3 anniePosition = m_AnnieObject.transform.position;
        Vector3 distanceVector = mundoPosition - anniePosition;

        sb_CanInteract = distanceVector.magnitude <= MAX_INTERACT_DISTANCE; // TODO: Change this so its based on BoxCollider2D instead
    }

    private void UpdateMovement()
    {
        bool didPressUp = Input.GetKey(UP_KEY);
        bool didPressDown = Input.GetKey(DOWN_KEY);
        bool didPressLeft = Input.GetKey(LEFT_KEY);
        bool didPressRight = Input.GetKey(RIGHT_KEY);
        bool didPressInteract = Input.GetKey(INTERACT_WITH_ANNIE_KEY);
        bool didPressAttack = Input.GetKeyDown(ATTACK_KEY);

        bool shouldAttack = !sb_IsHoldingAnnie && didPressAttack;
        if(shouldAttack)
        {
            OnAttack();
            return;
        }


        bool shouldInteract = sb_CanInteract && didPressInteract;

        if(shouldInteract && !shouldAttack)
        {
            if(sb_IsHoldingAnnie)
            {
                PutDownAnnie();
            } else
            {
                PickUpAnnie();
            }
        }
    }

    private void OnAttack()
    {


        se_AnimationType = AnimationType.StartAttacking;
    }

    private void PutDownAnnie()
    {
        sb_IsHoldingAnnie = false;
        m_AnnieObject.transform.position = gameObject.transform.position;
        m_AnnieObject.GetComponent<SpriteRenderer>().enabled = true;
    } 

    private void PickUpAnnie()
    {
        sb_IsHoldingAnnie = true;
        m_AnnieObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void UpdateAnimation()
    {
        /*
         * 8 Idle Animations/Sprites
         * 8 Movement Animations (one with Annie & one without)
         * 4 Attacking Animations
         */

        myAnimator.SetBool("isCarryingAnnie", sb_IsHoldingAnnie);

        switch (se_AnimationType)
        {
            case AnimationType.Idle:
                myAnimator.SetFloat("vertical", 0.0f);
                myAnimator.SetFloat("horizontal", 0.0f);
                break;

            case AnimationType.StartAttacking:
                myAnimator.SetFloat("vertical", 0.0f);
                myAnimator.SetFloat("horizontal", 0.0f);
                myAnimator.SetBool("isAttacking", true);
                break;

            case AnimationType.StopAttacking:
                myAnimator.SetFloat("vertical", 0.0f);
                myAnimator.SetFloat("horizontal", 0.0f);
                myAnimator.SetBool("isAttacking", false);
                break;

            case AnimationType.MovingLeft:
                myAnimator.SetFloat("vertical", 0.0f);
                myAnimator.SetFloat("horizontal", -1.0f);
                se_DirectionFacing = DirectionFacing.West;
                break;

            case AnimationType.MovingRight:
                myAnimator.SetFloat("vertical", 0.0f);
                myAnimator.SetFloat("horizontal", 1.0f);
                se_DirectionFacing = DirectionFacing.East;
                break;

            case AnimationType.MovingUp:
                myAnimator.SetFloat("vertical", 1.0f);
                myAnimator.SetFloat("horizontal", 0.0f);
                se_DirectionFacing = DirectionFacing.North;
                break;

            case AnimationType.MovingDown:
                myAnimator.SetFloat("vertical", -1.0f);
                myAnimator.SetFloat("horizontal", 0.0f);
                se_DirectionFacing = DirectionFacing.North;
                break;
        }
    }
}
