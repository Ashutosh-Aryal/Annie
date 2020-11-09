using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MundoMovement : MonoBehaviour {    
    private enum AnimationType
    {
        IdleRight,
        IdleLeft, 
        IdleUp,
        IdleDown,
        MovingRight, 
        MovingLeft, 
        MovingUp,
        MovingDown,
        StartAttacking,
        StopAttacking
    }
    private enum MovementDirection
    {
        East,
        West,
        North,
        South,
        Northeast,
        Southeast,
        Southwest,
        Northwest,
        Idle
    }

    public enum MundoState
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
    private const KeyCode INTERACT_WITH_OTHER_OBJECTS_KEY = KeyCode.E;

    [SerializeField] private float MOVEMENT_SPEED = 20.0f;
    private const float IDLE_SPEED = 0.1f;

    private const int NUM_MOVEMENT_DIRECTIONS = 4;

    public static int s_NumHeldBatteries = 0;

    private static AnimationType se_AnimationType = AnimationType.IdleRight;
    private static AnimationType? se_LastValidAnimationType = null;
    private static MovementDirection se_MovementDirection = MovementDirection.Idle;
    private static MovementDirection se_LastRealMovementDirection = MovementDirection.East;
    
    private static List<GameObject> sL_AvailableEnemiesToAttack = new List<GameObject>();

    private static Rigidbody2D myRigidbody;
    private static Animator myAnimator;

    public static MundoState se_MundoState = MundoState.CannotInteractWithAnnie;
    public static GameObject s_BatteryToPickUpObject = null;

    [SerializeField] 
    private GameObject m_AnnieObject;
    
    [SerializeField] 
    private GameObject m_EnemyPrefab;

    [SerializeField]
    private GameObject m_InteractWithAnnieText;

    [SerializeField]
    private GameObject m_DialogueObject;

    [Header("SFX Values")]

    [SerializeField] private AudioSource[] m_StabSound;
    [SerializeField] private AudioSource[] m_MovementSounds;


    private TextMeshProUGUI m_NumKnivesLeftTextGUI;

    private MyDialogBase m_LevelDialogue;

    private bool m_IsInSceneWherePlayerCanMove = true;

    public static int s_NumKnifesLeft = 0;

    private void OnApplicationQuit()
    {
        MetricManager.OnApplicationQuit();
    }

    // Start is called before the first frame update
    void Start()
    {
        BatteryBehavior.s_IsFirstPickedUpBattery = true;
        s_NumHeldBatteries = 0;
        s_BatteryToPickUpObject = null;
        m_LevelDialogue = m_DialogueObject.GetComponent<MyDialogBase>();

        var levelDialogue = m_LevelDialogue as AnniesHouseDialogue;

        if(levelDialogue != null)
        {
            m_IsInSceneWherePlayerCanMove = false;
        }

        myAnimator = gameObject.GetComponent<Animator>();
        myRigidbody = gameObject.GetComponent<Rigidbody2D>();

        sL_AvailableEnemiesToAttack.RemoveRange(0, sL_AvailableEnemiesToAttack.Count);

        m_NumKnivesLeftTextGUI = m_DialogueObject.transform.GetChild(m_DialogueObject.transform.childCount - 1).GetComponent<TextMeshProUGUI>();

        if(!m_AnnieObject.activeInHierarchy)
        {
            se_MundoState = MundoMovement.MundoState.CanPutDownAnnie;
        }
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
            m_InteractWithAnnieText.SetActive(true);
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
            m_InteractWithAnnieText.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(m_NumKnivesLeftTextGUI != null)
        {
            m_NumKnivesLeftTextGUI.text = "Num Knives Left: " + s_NumKnifesLeft;
        }

        if(!m_LevelDialogue.CanPlayerMove() || EnemyBehavior.s_HasPlayerLost || CheckWinStateBehavior.s_PlayerDidWin)
        {
            se_MovementDirection = MundoMovement.MovementDirection.Idle;
            if(myRigidbody.velocity.y < 0.0f)
            {
                se_AnimationType = MundoMovement.AnimationType.IdleDown;
            } else if(myRigidbody.velocity.y > 0.0f)
            {
                se_AnimationType = MundoMovement.AnimationType.IdleUp;
            } else if(myRigidbody.velocity.x > 0.0f)
            {
                se_AnimationType = MundoMovement.AnimationType.IdleRight;
            } else if(myRigidbody.velocity.x < 0.0f)
            {
                se_AnimationType = MundoMovement.AnimationType.IdleLeft;
            }

            myRigidbody.velocity = Vector2.zero;
            se_MovementDirection = MovementDirection.Idle;
            UpdateAnimation();
            return;
        } 

        CheckInput();

        UpdateMovement();
        UpdateAnimation();
    }

    private bool IsPlaying(Animator anim, string stateName) {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            return true;
        else
            return false;
    }

    private void CheckInput()
    {
        bool didPressInteract = Input.GetKeyDown(INTERACT_WITH_OTHER_OBJECTS_KEY);

        if(didPressInteract && s_BatteryToPickUpObject != null)
        {
            Destroy(s_BatteryToPickUpObject);
            s_BatteryToPickUpObject = null;
            s_NumHeldBatteries++;

            if(BatteryBehavior.s_IsFirstPickedUpBattery)
            {
                BatteryBehavior.s_IsFirstPickedUpBattery = false;
                m_LevelDialogue.DisplayDialogue(BatteryBehavior.s_DialogueOnFirstPickUp);
            }

        }

        bool didPressAttack = Input.GetKeyDown(ATTACK_KEY);
        bool UpAttack = Input.GetKeyUp(ATTACK_KEY);

        if(myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Mundo Attack")) {
            UpAttack = UpAttack || !IsPlaying(myAnimator, "Mundo Attack");
        }

        bool isNotHoldingAnnie = se_MundoState != MundoState.CanPutDownAnnie;

        if (s_NumKnifesLeft > 0 && isNotHoldingAnnie && didPressAttack) {

            se_LastValidAnimationType = se_AnimationType;
            OnAttack();
            se_MovementDirection = MovementDirection.Idle; return;

        } else if (isNotHoldingAnnie && UpAttack) {

            if (se_AnimationType != AnimationType.StartAttacking) {
                se_LastValidAnimationType = se_AnimationType;
            }
            se_AnimationType = AnimationType.StopAttacking; return;
        }

        bool didPressInteractWithAnnie = Input.GetKeyDown(INTERACT_WITH_ANNIE_KEY);
        bool shouldInteract = (se_MundoState != MundoState.CannotInteractWithAnnie) && didPressInteractWithAnnie;

        if (shouldInteract) { 
            if (se_MundoState == MundoState.CanPutDownAnnie) {
                PutDownAnnie();
            }
            else {
                PickUpAnnie();
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

            if(se_AnimationType == AnimationType.StopAttacking && se_LastValidAnimationType.HasValue) {
                animationType = se_LastValidAnimationType.Value;
            } else if(se_MovementDirection != MovementDirection.Idle) {
                animationType = (AnimationType)((int)se_LastRealMovementDirection % NUM_MOVEMENT_DIRECTIONS);
            } else
            {
                animationType = se_AnimationType;
            }

            se_MovementDirection = MovementDirection.Idle;

            shouldSwapAnimationType = (int) se_AnimationType >= NUM_MOVEMENT_DIRECTIONS;
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
        GameObject bestKillOption = null;

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

        if (bestKillOption != null)
        {
            se_MovementDirection = md;

            GameObject enemyToDestroy = GameObject.Find(bestKillOption.name);
            sL_AvailableEnemiesToAttack.Remove(enemyToDestroy);
            enemyToDestroy.GetComponent<EnemyBehavior>().Kill();
        }
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
        m_AnnieObject.SetActive(true);
    } 

    private void PickUpAnnie()
    {
        se_MundoState = MundoState.CanPutDownAnnie;
        m_AnnieObject.SetActive(false);
        m_InteractWithAnnieText.SetActive(false);
    }

    private void UpdateMovement()
    {

        if(!m_IsInSceneWherePlayerCanMove)
        {
            return;
        }

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
                myRigidbody.velocity = (Vector2.up + Vector2.left).normalized;
                myRigidbody.velocity *= MOVEMENT_SPEED;
                break;

            case MovementDirection.Southeast:
                myRigidbody.velocity = (Vector2.down + Vector2.right).normalized;
                myRigidbody.velocity *= MOVEMENT_SPEED;
                break;

            case MovementDirection.Southwest:
                myRigidbody.velocity = (Vector2.down + Vector2.left).normalized;
                myRigidbody.velocity *= MOVEMENT_SPEED;
                break;

            case MovementDirection.Idle:
                myRigidbody.velocity = Vector2.zero;
                break;
        }
    } 

    private void UpdateAnimation()
    {
        //myAnimator.SetBool("isCarryingAnnie", se_MundoState == MundoState.CanPutDownAnnie);

        Vector2 movementDirection = Vector2.zero;
        bool? isAttacking = null;

        if (AnimationType.StartAttacking == se_AnimationType) {
            isAttacking = true;
        } else if (AnimationType.StopAttacking == se_AnimationType) {
            isAttacking = false;
        }

        int enumValue;
        if (isAttacking.HasValue && isAttacking.Value)
        {
            enumValue = (int)se_LastValidAnimationType;
        }
        else
        {
            enumValue = (int)se_AnimationType;
        }

        bool isFacingRight = (enumValue % NUM_MOVEMENT_DIRECTIONS) == (int)MovementDirection.East;
        bool isFacingLeft = (enumValue % NUM_MOVEMENT_DIRECTIONS) == (int)MovementDirection.West;
        bool isFacingUp = (enumValue % NUM_MOVEMENT_DIRECTIONS) == (int)MovementDirection.North;
        bool isFacingDown = (enumValue % NUM_MOVEMENT_DIRECTIONS) == (int)MovementDirection.South;

        if (isFacingRight) {
            movementDirection = Vector2.right;
        } else if(isFacingUp) {
            movementDirection = Vector2.up;
        } else if(isFacingDown) {
            movementDirection = Vector2.down;
        } else if(isFacingLeft) {
            movementDirection = Vector2.left;
        }

        if(enumValue < NUM_MOVEMENT_DIRECTIONS) {
            movementDirection *= IDLE_SPEED;
        }
            
        SetAnimatorValues(movementDirection, isAttacking);
    }

    private void SetAnimatorValues(Vector2 direction, bool? bIsAttacking)
    {
        myAnimator.SetFloat("vertical", direction.x);
        myAnimator.SetFloat("horizontal", direction.y);

        if(bIsAttacking.HasValue)
        {
            myAnimator.SetBool("isAttacking", bIsAttacking.Value);
        }


    }
}
