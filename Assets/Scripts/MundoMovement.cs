using System.Collections.Generic;
using UnityEngine;

public class MundoMovement : MonoBehaviour {    
    private enum AnimationType
    {
        Idle,
        MovingRight, 
        MovingLeft, 
        MovingUp,
        MovingDown,
        StartAttacking,
        StopAttacking
    }
    private enum MovementDirection
    {
        Idle,
        North,
        Northeast,
        East,
        Southeast,
        South,
        Southwest,
        West,
        Northwest
    }

    private enum MundoState
    {
        CanPutDownAnnie,
        CanPickUpAnnie, 
        CannotInteractWithAnnie
    }

    private const KeyCode UP_KEY = KeyCode.W;
    private const KeyCode LEFT_KEY = KeyCode.A;
    private const KeyCode RIGHT_KEY = KeyCode.D;
    private const KeyCode DOWN_KEY = KeyCode.S;
    private const KeyCode INTERACT_WITH_ANNIE_KEY = KeyCode.F;
    private const KeyCode ATTACK_KEY = KeyCode.Space;

    private const float MOVEMENT_SPEED = 30.0f;

    private static AnimationType se_AnimationType = AnimationType.Idle;
    private static MovementDirection se_MovementDirection = MovementDirection.Idle;
    private static MovementDirection se_LastRealMovementDirection = MovementDirection.East;
    private static MundoState se_MundoState = MundoState.CannotInteractWithAnnie;
    
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

        bool isCollisionEnemy = false;
        if (m_EnemyPrefab != null)
        {
            isCollisionEnemy = collision.gameObject.CompareTag(m_EnemyPrefab.tag);
        }

        bool isCollisionAnnie = collision.gameObject.CompareTag(m_AnnieObject.tag);

        if(isCollisionEnemy)
        {
            sL_AvailableEnemiesToAttack.Add(collision.gameObject);
        } else if(isCollisionAnnie)
        {
            se_MundoState = MundoState.CanPickUpAnnie;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        bool isCollisionEnemy = collision.gameObject.CompareTag(m_EnemyPrefab.tag);
        bool isCollisionAnnie = collision.gameObject.CompareTag(m_AnnieObject.tag);

        if (isCollisionEnemy)
        {
            sL_AvailableEnemiesToAttack.Remove(collision.gameObject);
        }
        else if (isCollisionAnnie && se_MundoState != MundoState.CanPutDownAnnie)
        {
            se_MundoState = MundoState.CannotInteractWithAnnie;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();

        UpdateMovement();
        UpdateAnimation();
    }

    private void CheckInput()
    {
        bool didPressAttack = Input.GetKeyDown(ATTACK_KEY);

        bool shouldAttack = se_MundoState != MundoState.CanPutDownAnnie && didPressAttack;

        if (shouldAttack)
        {
            OnAttack();
            se_MovementDirection = MovementDirection.Idle;
            return;
        }

        bool didPressInteract = Input.GetKeyDown(INTERACT_WITH_ANNIE_KEY);
        bool shouldInteract = (se_MundoState != MundoState.CannotInteractWithAnnie) && didPressInteract;

        if (didPressInteract)
        {
            if (shouldInteract)
            {
                if (se_MundoState == MundoState.CanPutDownAnnie)
                {
                    PutDownAnnie();
                }
                else
                {
                    PickUpAnnie();
                }
            }
        }

        AnimationType animationType;
        bool shouldSwapAnimationType;

        bool didPressUp = Input.GetKey(UP_KEY);
        bool didPressDown = Input.GetKey(DOWN_KEY);
        bool didPressLeft = Input.GetKey(LEFT_KEY);
        bool didPressRight = Input.GetKey(RIGHT_KEY);

        bool isCurrentAnimationUp = se_AnimationType == AnimationType.MovingUp;
        bool isCurrentAnimationDown = se_AnimationType == AnimationType.MovingDown;
        bool isCurrentAnimationRight = se_AnimationType == AnimationType.MovingRight;
        bool isCurrentAnimationLeft = se_AnimationType == AnimationType.MovingLeft;

        if(didPressUp && didPressRight)
        {
            se_MovementDirection = MovementDirection.Northeast;
            animationType = AnimationType.MovingUp;
            shouldSwapAnimationType = !isCurrentAnimationUp && !isCurrentAnimationRight;

        } else if(didPressUp && didPressLeft)
        {
            se_MovementDirection = MovementDirection.Northwest;
            animationType = AnimationType.MovingUp;
            shouldSwapAnimationType = !isCurrentAnimationUp && !isCurrentAnimationLeft;
        } else if(didPressDown && didPressRight)
        {
            se_MovementDirection = MovementDirection.Southeast;
            animationType = AnimationType.MovingDown;
            shouldSwapAnimationType = !isCurrentAnimationDown && !isCurrentAnimationRight;
        } else if(didPressDown && didPressLeft)
        {
            se_MovementDirection = MovementDirection.Southwest;
            animationType = AnimationType.MovingDown;
            shouldSwapAnimationType = !isCurrentAnimationDown && !isCurrentAnimationLeft;
        } else if(didPressUp)
        {
            se_MovementDirection = MovementDirection.North;
            animationType = AnimationType.MovingUp;
            shouldSwapAnimationType = !isCurrentAnimationUp;
        } else if(didPressLeft)
        {
            se_MovementDirection = MovementDirection.West;
            animationType = AnimationType.MovingLeft;
            shouldSwapAnimationType = !isCurrentAnimationLeft;
        } else if(didPressRight)
        {
            se_MovementDirection = MovementDirection.East;
            animationType = AnimationType.MovingRight;
            shouldSwapAnimationType = !isCurrentAnimationRight;
        } else if(didPressDown)
        {
            se_MovementDirection = MovementDirection.South;
            animationType = AnimationType.MovingDown;
            shouldSwapAnimationType = !isCurrentAnimationDown;
        } else
        {
            se_MovementDirection = MovementDirection.Idle;
            animationType = AnimationType.Idle;
            shouldSwapAnimationType = se_AnimationType != AnimationType.Idle;
        }

        if(se_MovementDirection != MovementDirection.Idle)
        {
            se_LastRealMovementDirection = se_MovementDirection;
        }

        if(shouldSwapAnimationType)
        {
            se_AnimationType = animationType;
        }
    }

    private void OnAttack()
    {
        se_AnimationType = AnimationType.StartAttacking;

        MovementDirection md = MovementDirection.Idle;
        float closestDistance = 1000.0f;
        GameObject bestKillOption = new GameObject();

        foreach(GameObject enemy in sL_AvailableEnemiesToAttack) {
            Vector3 enemyLocation = enemy.transform.position;
            Vector3 myLocation = gameObject.transform.position;

            Vector3 distanceVector = enemyLocation - myLocation;
            Vector2 distanceVector2D = new Vector2(distanceVector.x, distanceVector.y);
            float distance = distanceVector2D.magnitude;

            distanceVector2D.Normalize();

            MovementDirection enemyDirectionFromPlayer = GetDirection(distanceVector2D);

            bool isLastFoundMovementDirectionInDirectionOfPlayer = md == se_LastRealMovementDirection;
            bool isCurrentEnemyInDirectionOfPlayer = enemyDirectionFromPlayer == se_LastRealMovementDirection;
            bool isCurrentDistanceLessThanPriorFoundDistance = distance < closestDistance;

            if(md == MovementDirection.Idle || (!isLastFoundMovementDirectionInDirectionOfPlayer && isCurrentEnemyInDirectionOfPlayer)) {
                md = enemyDirectionFromPlayer;
                closestDistance = distance;
                bestKillOption = enemy;
            } else if(isLastFoundMovementDirectionInDirectionOfPlayer && isCurrentEnemyInDirectionOfPlayer && isCurrentDistanceLessThanPriorFoundDistance) {
                closestDistance = distance;
                bestKillOption = enemy;
            } else if(IsLeftArgCloser(enemyDirectionFromPlayer, md) && isCurrentDistanceLessThanPriorFoundDistance)
            {
                md = enemyDirectionFromPlayer;
                closestDistance = distance;
                bestKillOption = enemy;
            }
        }

        Destroy(GameObject.Find(bestKillOption.name));
    }

    private bool IsLeftArgCloser(MovementDirection newDirection, MovementDirection oldDirection)
    {
        return Mathf.Abs(((int)oldDirection - (int)se_LastRealMovementDirection)) > Mathf.Abs((int)newDirection - (int)se_LastRealMovementDirection); 
    }

    private const float DIFFERENCE_RANGE = 0.1f;

    private MovementDirection GetDirection(Vector2 directionVector)
    {
        MovementDirection[] directionChoices = new MovementDirection[3];
        float difference;

        if(directionVector.x > 0.0f && directionVector.y > 0.0f)
        {
            directionChoices[0] = MovementDirection.East;
            directionChoices[1] = MovementDirection.Northeast;
            directionChoices[2] = MovementDirection.North;
            difference = directionVector.x - directionVector.y;
            
        } else if(directionVector.x > 0.0f && directionVector.y < 0.0f)
        {
            directionChoices[0] = MovementDirection.East;
            directionChoices[1] = MovementDirection.Southeast;
            directionChoices[2] = MovementDirection.South;
            difference = directionVector.x - Mathf.Abs(directionVector.y);
        } else if(directionVector.x < 0.0f && directionVector.y > 0.0f)
        {
            directionChoices[0] = MovementDirection.West;
            directionChoices[1] = MovementDirection.Northwest;
            directionChoices[2] = MovementDirection.North;
            difference = Mathf.Abs(directionVector.x) - directionVector.y;
        } else
        {
            directionChoices[0] = MovementDirection.West;
            directionChoices[1] = MovementDirection.Southwest;
            directionChoices[2] = MovementDirection.South;
            difference = Mathf.Abs(directionVector.x) - Mathf.Abs(directionVector.y);
        }

        if(difference >= DIFFERENCE_RANGE)
        {
            return directionChoices[0];
        } else if(difference <= -DIFFERENCE_RANGE)
        {
            return directionChoices[2];
        } else
        {
            return directionChoices[1];
        }
    }

    private void PutDownAnnie()
    {
        se_MundoState = MundoState.CanPickUpAnnie;
        m_AnnieObject.transform.position = gameObject.transform.position;
        //m_AnnieObject.GetComponent<SpriteRenderer>().enabled = true;
        m_AnnieObject.SetActive(true);
    } 

    private void PickUpAnnie()
    {
        se_MundoState = MundoState.CanPutDownAnnie;
        m_AnnieObject.SetActive(false);
        //m_AnnieObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void UpdateMovement()
    {
        switch(se_MovementDirection)
        {
            case MovementDirection.North:
                myRigidbody.velocity = Vector2.up * MOVEMENT_SPEED;
                break;

            case MovementDirection.East:
                myRigidbody.velocity = Vector2.right * MOVEMENT_SPEED;
                break;

            case MovementDirection.South:
                myRigidbody.velocity = Vector2.down * MOVEMENT_SPEED;
                break;

            case MovementDirection.West:
                myRigidbody.velocity = Vector2.left * MOVEMENT_SPEED;
                break;

            case MovementDirection.Northeast:
                myRigidbody.velocity = Vector2.one.normalized * MOVEMENT_SPEED;
                break;

            case MovementDirection.Northwest:
                myRigidbody.velocity = new Vector2(-Vector2.one.x, Vector2.one.y).normalized;
                myRigidbody.velocity *= MOVEMENT_SPEED;
                break;

            case MovementDirection.Southeast:
                myRigidbody.velocity = new Vector2(Vector2.one.x, -Vector2.one.y).normalized;
                myRigidbody.velocity *= MOVEMENT_SPEED;
                break;

            case MovementDirection.Southwest:
                myRigidbody.velocity = new Vector2(-Vector2.one.x, -Vector2.one.y).normalized;
                myRigidbody.velocity *= MOVEMENT_SPEED;
                break;

            case MovementDirection.Idle:
                myRigidbody.velocity = Vector2.zero;
                break;
        }
    } 

    private void UpdateAnimation()
    {
        /*
         * 8 Idle Animations/Sprites
         * 8 Movement Animations (one with Annie & one without)
         * 4 Attacking Animations
         */

        myAnimator.SetBool("isCarryingAnnie", se_MundoState == MundoState.CanPutDownAnnie);

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
                break;

            case AnimationType.MovingRight:
                myAnimator.SetFloat("vertical", 0.0f);
                myAnimator.SetFloat("horizontal", 1.0f);
                break;

            case AnimationType.MovingUp:
                myAnimator.SetFloat("vertical", 1.0f);
                myAnimator.SetFloat("horizontal", 0.0f);
                break;

            case AnimationType.MovingDown:
                myAnimator.SetFloat("vertical", -1.0f);
                myAnimator.SetFloat("horizontal", 0.0f);
                break;
        }
    }
}
