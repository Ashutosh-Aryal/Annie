using System.Collections.Generic;
using Pathfinding;
using TMPro;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public AudioClip EnemyGotKilled;
    private AudioSource audioSource { get { return GetComponent<AudioSource>(); } }

    private const int RIGHT_CLICK = 1;
    private const int MAX_ENEMY_COUNT = 100;
    private const int OBSTACLE_LAYER = 1 << 8;
    private const int TRIGGER_VISION_OBSTACLE_LAYER = 1 << 11;
    private const float MAX_DISTANCE_FROM_TARGET = 1.5f;
    private const float MAX_SEE_DISTANCE = 15.0f;
    private const float MAX_SEEN_TIMER = 2.0f;
    private const float BLUE_HUE = 1.0f / 3.0f;
    private const string PLAYER_TAG = "Player";
    
    [SerializeField] private GameObject m_WaypointsContainer;
    [SerializeField] private GameObject m_GameOverMenu;
    [SerializeField] private GameObject m_DialogueObject;
    [SerializeField] private GameObject m_KnifePrefab;

    private enum AnimationState
    {
        Normal, 
        Distracted, 
        Spotted, 
        Chasing,
        Dead
    };

    private AnimationState m_AnimationState = AnimationState.Normal;

    private int m_WaypointIndex = 0;
    private int m_RandValue;
    private float m_LookingTimer = 0.0f;
    private float m_SeenTimer = 0.0f;
    private float m_ResetSeenTimer = 0.0f;

    private Rigidbody2D myRigidbody2D;
    private AIDestinationSetter myDestinationSetter;
    private GameObject myVisionCone;
    private Animator myAnimator;
    private MyDialogBase myDialogBase;

    private List<GameObject> m_Waypoints = new List<GameObject>();
    private Transform m_SoundLocation = null;

    private bool m_ShouldResetWaypointIndex = false;
    private bool m_DoesSeePlayer = false;
    private bool m_ShouldDecrementSeenTimer = false;
    private bool m_IsDead = false;

    private static HashSet<int> s_AssignedEnemyNumbers = new HashSet<int>();
    private static GameObject s_PlayerObject = null;
    private static GameObject s_KnifePrefab = null;
    private static int s_PlayerLayerMask = 0;
    
    public static bool s_HasPlayerLost = false;
    public static GameObject s_EndGameMenu = null;

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(RIGHT_CLICK) && MundoMovement.se_MundoState == MundoMovement.MundoState.CanPutDownAnnie)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            AnnieBehavior.SetRightClickStartPosition(gameObject.name);
        }
    }

    public void SetSoundLocation(Transform newTransform)
    {
        m_SoundLocation = newTransform;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<AudioSource>();
        audioSource.clip = EnemyGotKilled;
        audioSource.playOnAwake = false;

        if (s_PlayerObject == null)
        {
            s_PlayerObject = GameObject.FindGameObjectsWithTag(PLAYER_TAG)[0];
            s_PlayerLayerMask = (1 << s_PlayerObject.layer);
        }

        if(m_GameOverMenu != null)
        {
            s_EndGameMenu = m_GameOverMenu;
            s_EndGameMenu.SetActive(false);
        }

        if(m_KnifePrefab != null)
        {
            s_KnifePrefab = m_KnifePrefab;
        }

        if(m_DialogueObject != null)
        {
            myDialogBase = m_DialogueObject.GetComponent<MyDialogBase>();
        }

        myVisionCone = gameObject.transform.GetChild(0).gameObject;

        int rand = 0;

        do {
            rand = Random.Range(0, MAX_ENEMY_COUNT);
        } while (s_AssignedEnemyNumbers.Contains(rand));

        s_AssignedEnemyNumbers.Add(rand);
        m_RandValue = rand;
        gameObject.name += rand;

        myAnimator = gameObject.GetComponent<Animator>();
        myRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        myDestinationSetter = gameObject.GetComponent<AIDestinationSetter>();

        int numWaypoints = m_WaypointsContainer.transform.childCount;

        for(int x = 0; x < numWaypoints; x++)
        {
            m_Waypoints.Add(m_WaypointsContainer.transform.GetChild(x).gameObject);
        }
    }

    private Vector3 m_DeathLocation = Vector3.zero;

    public void Kill()
    {
        MetricManager.s_NumKnifes++;
        MundoMovement.s_NumKnifesLeft--;
        m_AnimationState = AnimationState.Dead;
        m_IsDead = true;
        audioSource.PlayOneShot(EnemyGotKilled);
        myVisionCone.SetActive(false);
        myDestinationSetter.target = null;
        m_DeathLocation = gameObject.transform.position;

        if (s_KnifePrefab && Random.Range(0.0f, 1.0f) <= 0.4f) {
            GameObject knife = Instantiate(s_KnifePrefab, gameObject.transform.position, Quaternion.identity);
            knife.GetComponent<KnifePickUpBehavior>().m_InteractText = m_DialogueObject.transform.GetChild(3).gameObject;
        }
        
        Destroy(myDestinationSetter);
        Destroy(gameObject.GetComponent<BoxCollider2D>());
        UpdateAnimation();

        Destroy(gameObject.GetComponent<AIPath>());
    }

    private void OnDestroy()
    {
        s_AssignedEnemyNumbers.Remove(m_RandValue);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsDead) {
            gameObject.transform.position = m_DeathLocation; return;
        } else if (s_HasPlayerLost || CheckWinStateBehavior.s_PlayerDidWin) {
            myDestinationSetter.target = null;
            m_GameOverMenu.SetActive(true);
            MetricManager.s_NumDeaths++; return;
        } else if(!myDialogBase.CanPlayerMove()) {
            myDestinationSetter.target = null; return;
        }

        CreateVisionCone();

        if (m_DoesSeePlayer) {
            myDestinationSetter.target = s_PlayerObject.transform; m_SoundLocation = null; m_ShouldResetWaypointIndex = true;
            m_AnimationState = AnimationState.Chasing;
        } else if (m_LookingTimer > 0.0f) {
            return;
        } else if (null != m_SoundLocation) {
            HandleSoundDestinationCase(); return;
        } else {
            HandleDestinationSetting();
        }

        UpdateAnimation();
    }

    private void UpdateVisionConeStatus()
    {
        bool prevDoesSeePlayer = m_DoesSeePlayer;

        m_DoesSeePlayer = DoesEnemySeePlayer();

        if (m_DoesSeePlayer) {
            m_SeenTimer += Time.fixedDeltaTime; m_ShouldDecrementSeenTimer = false;
        }
        else {

            if (prevDoesSeePlayer) {
                m_ResetSeenTimer = 5.0f - Time.fixedDeltaTime;
            }
            else if (m_ResetSeenTimer > 0.0f) {

                m_ResetSeenTimer -= Time.fixedDeltaTime;

                if (m_ResetSeenTimer <= 0.0f) {
                    m_ShouldDecrementSeenTimer = true;
                }

            } else if (m_ShouldDecrementSeenTimer) {
                m_SeenTimer -= Time.fixedDeltaTime;
            }

            if (prevDoesSeePlayer)
            {
                // TODO: Handle case in which player suddenly disappears from sight
                // IDEA: Have a variable that holds last known location along with last known velocity
                // and checks (using Raycast) around that direction to see if the player can be seen in that direction
            }
        }

        if (m_SeenTimer > MAX_SEEN_TIMER) {
            m_SeenTimer = MAX_SEEN_TIMER;
        }

        Color newColor = GetColorFromTime();

        if (newColor == Color.red) {
            s_HasPlayerLost = true;
            m_GameOverMenu.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "You Lose!";
        }

        myVisionCone.GetComponent<MeshRenderer>().material.color = newColor;
    }

    private bool m_HasSeenDeadEnemy = false;
    private void CreateVisionCone() {

        Mesh visionCone = new Mesh();
        
        myVisionCone.GetComponent<MeshFilter>().mesh = visionCone;

        float FOV = 60.0f;

        float angle = prevAngle;

        if (myDestinationSetter.ai.velocity.magnitude != 0.0f) {
            angle = GetAngleFromDirection();
            prevAngle = angle;
        }

        angle += 30.0f;
        float MAX_ANGLE = angle - 60.0f;

        Vector3[] vertices = new Vector3[(int) FOV + 2];
        int[] triangles = new int[(int) FOV * 3];

        vertices[0] = Vector3.zero;

        int vertexIndex = 1;
        int triangleIndex = 0;

        for(; angle >=  MAX_ANGLE; angle -= 1.0f) {

            float rayAngleInRads = angle * Mathf.Deg2Rad;
            Vector3 directionVector = new Vector3(Mathf.Cos(rayAngleInRads), Mathf.Sin(rayAngleInRads));

            Physics2D.queriesHitTriggers = true;

            var raycast2DInfo = Physics2D.Raycast(gameObject.transform.position, 
                directionVector, MAX_SEE_DISTANCE, OBSTACLE_LAYER | TRIGGER_VISION_OBSTACLE_LAYER);

            Vector3 worldPosition;
            if(raycast2DInfo.collider == null) {
                worldPosition = gameObject.transform.position + directionVector * MAX_SEE_DISTANCE;
            } else {
                worldPosition = raycast2DInfo.point; 
            }

            Physics2D.queriesHitTriggers = true;

            int myLayer = gameObject.layer;

            gameObject.layer = 0;

            RaycastHit2D enemyHitInformation = Physics2D.Raycast(gameObject.transform.position,
                directionVector, MAX_SEE_DISTANCE, 1 << myLayer | OBSTACLE_LAYER | TRIGGER_VISION_OBSTACLE_LAYER);

            if (enemyHitInformation.collider != null && enemyHitInformation.collider.GetComponent<EnemyBehavior>() != null) {

                if (enemyHitInformation.collider.GetComponent<EnemyBehavior>().m_IsDead && !m_HasSeenDeadEnemy) {
                    gameObject.GetComponent<AIPath>().maxSpeed *= 1.25f;
                    m_HasSeenDeadEnemy = true;
                }
            }

            gameObject.layer = myLayer;

            // transform point from world space to vision cone's local space
            Vector3 vertex = myVisionCone.transform.InverseTransformPoint(worldPosition);
            vertices[vertexIndex] = vertex;

            if (vertexIndex > 1) {
                triangles[triangleIndex] = 0; // draw triangle from center of enemy
                triangles[triangleIndex + 1] = vertexIndex - 1; // to last index
                triangles[triangleIndex + 2] = vertexIndex; // to current index (which must be > 0)

                triangleIndex += 3; 
            }

            vertexIndex++;
        }

        visionCone.vertices = vertices;
        visionCone.triangles = triangles;
        
    }

    private bool DoesEnemySeePlayer()
    {
        Vector3 distanceToPlayerVector = s_PlayerObject.transform.position - gameObject.transform.position;
        bool isPlayerTooFarFromEnemy = distanceToPlayerVector.magnitude > MAX_SEE_DISTANCE;

        if (isPlayerTooFarFromEnemy) {
            return false;
        }

        float movementDirectionAngle = prevAngle;

        if (myDestinationSetter.ai.velocity.magnitude != 0.0f)
        {
            movementDirectionAngle = GetAngleFromDirection();
            prevAngle = movementDirectionAngle;
        }

        for (float rayAngle = movementDirectionAngle - 30.0f; rayAngle <= movementDirectionAngle + 30.0f; rayAngle += 1.0f) {

            float rayAngleInRads = rayAngle * Mathf.Deg2Rad;
            Vector3 directionVector = new Vector3(Mathf.Cos(rayAngleInRads), Mathf.Sin(rayAngleInRads));

            RaycastHit2D hitInformation = Physics2D.Raycast(gameObject.transform.position, 
                directionVector, MAX_SEE_DISTANCE, s_PlayerLayerMask | OBSTACLE_LAYER | TRIGGER_VISION_OBSTACLE_LAYER);

            if (hitInformation.collider == null)
            {
                continue;
            }

            bool rayCastIntersectsWithRandomTrigger = hitInformation.collider.isTrigger && hitInformation.collider.gameObject.layer != TRIGGER_VISION_OBSTACLE_LAYER;
            if (rayCastIntersectsWithRandomTrigger)
            {
                Physics2D.queriesHitTriggers = false;
                hitInformation = Physics2D.Raycast(gameObject.transform.position,
                    directionVector, MAX_SEE_DISTANCE, s_PlayerLayerMask | OBSTACLE_LAYER);
                Physics2D.queriesHitTriggers = true;
                if(hitInformation.collider == null) { continue; }
            }

            bool collidesWithPlayer = hitInformation.collider.gameObject.CompareTag(PLAYER_TAG);

            if (collidesWithPlayer) {
                Vector3 finalPos = gameObject.transform.position + directionVector * MAX_SEE_DISTANCE;
                Debug.DrawLine(gameObject.transform.position, finalPos, Color.black, 1.0f, false);
                return true;
            }
        }

        return false;
    }

    private void HandleSoundDestinationCase()
    {
        bool hasTargetDestinationAlreadyBeenSetToSound = myDestinationSetter.target == m_SoundLocation;
        Vector3 distanceFromSoundVector = gameObject.transform.position - m_SoundLocation.position;
        bool isWithinEnoughDistanceToSound = distanceFromSoundVector.magnitude <= MAX_DISTANCE_FROM_TARGET;

        if (!hasTargetDestinationAlreadyBeenSetToSound)
        {
            myDestinationSetter.target = m_SoundLocation;
            m_AnimationState = AnimationState.Distracted;
        }
        else if (hasTargetDestinationAlreadyBeenSetToSound && isWithinEnoughDistanceToSound)
        {
            PlayLookingAnimation();
            m_SoundLocation = null;
            m_ShouldResetWaypointIndex = true;
        }

        UpdateAnimation();
    }

    private float GetAngleFromDirection()
    {
        Vector3 movementDirection = myDestinationSetter.ai.velocity.normalized;
        return Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;
    }

    private void HandleDestinationSetting() {

        m_AnimationState = AnimationState.Normal;

        if(m_ShouldResetWaypointIndex) {

            float smallestDistanceFound = 99999.0f;
            int index = 0;
            foreach(GameObject waypoint in m_Waypoints) {

                Vector3 distanceVector = gameObject.transform.position - waypoint.transform.position;
                
                if(distanceVector.magnitude < smallestDistanceFound) {
                    m_WaypointIndex = index;
                    smallestDistanceFound = distanceVector.magnitude;
                }

                ++index;
            }

            m_ShouldResetWaypointIndex = false;
        }

        Vector3 distanceToWaypointVector = gameObject.transform.position - m_Waypoints[m_WaypointIndex].transform.position;
        bool shouldUpdateWaypointIndex = distanceToWaypointVector.magnitude <= MAX_DISTANCE_FROM_TARGET;

        if(shouldUpdateWaypointIndex) {
            m_WaypointIndex = (m_WaypointIndex + 1) % m_Waypoints.Count;
        }
           
        myDestinationSetter.target = m_Waypoints[m_WaypointIndex].transform;
    }

    private const float FOURTY_FIVE_DEGREES = 45.0f;
    private float prevAngle = 0.0f;

    private const float IDLE_ANIMATOR_SPEED = 0.1f;
    private const float MOVING_ANIMATOR_SPEED = 1.0f;

    private Vector3 prevPosition;

    private GameObject myGameObject { 
        get 
            { return this.gameObject; 
        } 
    }

    private void UpdateAnimation() {

        Vector2 currentVelocity = new Vector2(myDestinationSetter.ai.velocity.x, myDestinationSetter.ai.velocity.y);

        bool isMoving = prevPosition != myDestinationSetter.ai.position || currentVelocity.sqrMagnitude != 0.0f;

        float angle, speed;
        if (isMoving) {
            speed = MOVING_ANIMATOR_SPEED;
            prevAngle = GetAngleFromDirection();
        } else {
            speed = IDLE_ANIMATOR_SPEED;
        }

        angle = prevAngle;
        prevPosition = myDestinationSetter.ai.position;

        myAnimator.SetBool("isDead", m_AnimationState == AnimationState.Dead);
        myAnimator.SetBool("isSpotted", m_AnimationState == AnimationState.Spotted);
        myAnimator.SetBool("isDistracted", m_AnimationState == AnimationState.Distracted);
        myAnimator.SetBool("isChasing", m_AnimationState == AnimationState.Chasing);

        myAnimator.SetFloat("horizontal", 0.0f);
        myAnimator.SetFloat("vertical", 0.0f);

        bool isFacingLeft = angle >= (FOURTY_FIVE_DEGREES * 3.0f) || angle <= (FOURTY_FIVE_DEGREES * -3.0f);
        bool isFacingUp = (angle >= FOURTY_FIVE_DEGREES) && (angle < FOURTY_FIVE_DEGREES * 3.0f);
        bool isFacingDown = (angle <= -FOURTY_FIVE_DEGREES) && (angle > -FOURTY_FIVE_DEGREES * 3.0f);
        bool isFacingRight = angle < FOURTY_FIVE_DEGREES && angle > -FOURTY_FIVE_DEGREES;

        if (isFacingLeft) {
            myAnimator.SetFloat("horizontal", -speed);  
        } else if(isFacingRight) {
            myAnimator.SetFloat("horizontal", speed);
        } else if(isFacingUp) {
            myAnimator.SetFloat("vertical", speed);
        } else if(isFacingDown) {
            myAnimator.SetFloat("vertical", -speed);
        } 
     }

    private void FixedUpdate()
    {
        if (m_IsDead || !myDialogBase.CanPlayerMove()) {
            return;
        } else if(m_LookingTimer > 0.0f) {
            m_LookingTimer -= Time.fixedDeltaTime;
        }

        UpdateVisionConeStatus();
    }

    private Color GetColorFromTime()
    {
        if(m_HasSeenDeadEnemy && m_SeenTimer < MAX_SEEN_TIMER / 2.0f) {
            m_SeenTimer = MAX_SEEN_TIMER / 2.0f;
        }

        float ratio = Mathf.Clamp(m_SeenTimer / MAX_SEEN_TIMER, 0.0f, 1.0f);

        float hue = (1.0f - ratio) * BLUE_HUE;

        Color newColor = Color.HSVToRGB(hue, 1.0f, 1.0f);

        return newColor;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && m_DoesSeePlayer)
        {
            s_HasPlayerLost = true;
            MetricManager.s_NumDeaths++;
            m_GameOverMenu.SetActive(true);
            m_GameOverMenu.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "You Lose!";
        }
    }

    private void PlayLookingAnimation()
    {
        m_LookingTimer = 1.0f;

        // TODO: Add looking for sound animation trigger here
    }
}
