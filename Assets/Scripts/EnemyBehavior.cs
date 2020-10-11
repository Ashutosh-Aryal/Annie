using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Pathfinding;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyBehavior : MonoBehaviour
{
    private const int RIGHT_CLICK = 1;
    private const int MAX_ENEMY_COUNT = 100;
    private const int OBSTACLE_LAYER = 1 << 8;
    private const float MAX_DISTANCE_FROM_TARGET = 0.4f;
    private const float MAX_SEE_DISTANCE = 50.0f;
    private const float MAX_SEEN_TIMER = 8.0f;
    private const float BLUE_HUE = 120.0f;
    private const string PLAYER_TAG = "Player";

    [SerializeField] private GameObject m_WaypointsContainer;

    private int m_WaypointIndex = 0;
    private int m_RandValue;
    private float m_LookingTimer = 0.0f;
    private float m_SeenTimer = 0.0f;
    private float m_ResetSeenTimer = 0.0f;

    private Rigidbody2D myRigidbody2D;
    private AIDestinationSetter myDestinationSetter;
    private GameObject myVisionCone;

    private List<GameObject> m_Waypoints = new List<GameObject>();
    private Transform m_SoundLocation = null;

    private bool m_ShouldResetWaypointIndex = false;
    private bool m_DoesSeePlayer = false;
    private bool m_ShouldDecrementSeenTimer = false;

    private static HashSet<int> s_AssignedEnemyNumbers = new HashSet<int>();
    private static GameObject s_PlayerObject = null;
    private static int s_PlayerLayerMask = 0;

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
        if(s_PlayerObject == null)
        {
            s_PlayerObject = GameObject.FindGameObjectsWithTag(PLAYER_TAG)[0];
            s_PlayerLayerMask = (1 << s_PlayerObject.layer);
        }

        myVisionCone = gameObject.transform.GetChild(0).gameObject;

        int rand = 0;

        do {
            rand = Random.Range(0, MAX_ENEMY_COUNT);
        } while (s_AssignedEnemyNumbers.Contains(rand));

        s_AssignedEnemyNumbers.Add(rand);
        m_RandValue = rand;
        gameObject.name += rand;

        myRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        myDestinationSetter = gameObject.GetComponent<AIDestinationSetter>();

        int numWaypoints = m_WaypointsContainer.transform.childCount;

        for(int x = 0; x < numWaypoints; x++)
        {
            m_Waypoints.Add(m_WaypointsContainer.transform.GetChild(x).gameObject);
        }
    }

    private void OnDestroy()
    {
        s_AssignedEnemyNumbers.Remove(m_RandValue);
    }

    // Update is called once per frame
    void Update()
    {
        bool prevDoesSeePlayer = m_DoesSeePlayer;

        m_DoesSeePlayer = DoesEnemySeePlayer();

        if (m_DoesSeePlayer) {
            m_SeenTimer += Time.deltaTime; m_ShouldDecrementSeenTimer = false;
        } else {

            if (prevDoesSeePlayer) {
                m_ResetSeenTimer = 5.0f - Time.deltaTime;
            } else if(m_ResetSeenTimer > 0.0f) {
                m_ResetSeenTimer -= Time.deltaTime;

                if(m_ResetSeenTimer <= 0.0f) {
                    m_ShouldDecrementSeenTimer = true;
                }

            } else if(m_ShouldDecrementSeenTimer) {
                m_SeenTimer -= Time.deltaTime;
            }
            
            if (prevDoesSeePlayer)
            {
                // TODO: Handle case in which player suddenly disappears from sight
                // IDEA: Have a variable that holds last known location along with last known velocity
                // and checks (using Raycast) around that direction to see if the player can be seen in that direction
            }
        } 

        if(m_SeenTimer > MAX_SEEN_TIMER) {
            m_SeenTimer = MAX_SEEN_TIMER;
        }

        myVisionCone.GetComponent<MeshRenderer>().material.color = GetColorFromTime();

        CreateVisionCone();

        if (m_DoesSeePlayer) {
            myDestinationSetter.target = s_PlayerObject.transform; m_SoundLocation = null; m_ShouldResetWaypointIndex = true;
        } else if (m_LookingTimer > 0.0f) {
            return;
        } else if (null != m_SoundLocation) {
            HandleSoundDestinationCase(); return;
        } else {
            HandleDestinationSetting();
        }
    }

    private Color GetColorFromTime()
    {
        float ratio = Mathf.Clamp(m_SeenTimer / MAX_SEEN_TIMER, 0.0f, 1.0f);

        float hue = (1.0f - ratio) * BLUE_HUE;
            
        Color newColor = Color.HSVToRGB(hue, 1.0f, 1.0f);

        return newColor;
    }

    private void CreateVisionCone() {

        Mesh visionCone = new Mesh();
        
        myVisionCone.GetComponent<MeshFilter>().mesh = visionCone;

        float FOV = 60.0f;
        float angle = GetAngleFromDirection() + 30.0f;
        float MAX_ANGLE = angle - 60.0f;
        float viewDistance = 5.0f;

        Vector3[] vertices = new Vector3[(int) FOV + 2];
        int[] triangles = new int[(int) FOV * 3];

        vertices[0] = Vector3.zero;

        int vertexIndex = 1;
        int triangleIndex = 0;

        for(; angle >=  MAX_ANGLE; angle -= 1.0f)
        {
            Vector3 vertex;
            float rayAngleInRads = angle * Mathf.Deg2Rad;
            Vector3 directionVector = new Vector3(Mathf.Cos(rayAngleInRads), Mathf.Sin(rayAngleInRads));

            var raycast2DInfo = Physics2D.Raycast(gameObject.transform.position, directionVector, MAX_SEE_DISTANCE, OBSTACLE_LAYER);

            if(raycast2DInfo.collider == null) {
                vertex = vertices[0] + directionVector * viewDistance; 
            } else
            {
                // transform point from world space to vision cone's local space
                vertex = myVisionCone.transform.InverseTransformPoint(raycast2DInfo.point); 
            }

            vertices[vertexIndex] = vertex;

            if (vertexIndex > 1)
            {
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

        float movementDirectionAngle = GetAngleFromDirection();

        for (float rayAngle = movementDirectionAngle - 30.0f; rayAngle <= movementDirectionAngle + 30.0f; rayAngle += 1.0f) {

            float rayAngleInRads = rayAngle * Mathf.Deg2Rad;
            Vector2 directionVector = new Vector3(Mathf.Cos(rayAngleInRads), Mathf.Sin(rayAngleInRads));

            RaycastHit2D hitInformation = Physics2D.Raycast(gameObject.transform.position, 
                directionVector, MAX_SEE_DISTANCE, s_PlayerLayerMask);

            if(hitInformation.collider == null)
            {
                continue;
            }

            bool collidesWithPlayer = hitInformation.collider.gameObject.CompareTag(PLAYER_TAG);

            if (collidesWithPlayer) {
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
        }
        else if (hasTargetDestinationAlreadyBeenSetToSound && isWithinEnoughDistanceToSound)
        {
            PlayLookingAnimation();
            m_SoundLocation = null;
            m_ShouldResetWaypointIndex = true;
        }

    }

    private float GetAngleFromDirection()
    {
        Vector3 movementDirection = myDestinationSetter.ai.velocity.normalized;
        return Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;
    }

    private void HandleDestinationSetting()
    {
        if(m_ShouldResetWaypointIndex)
        {
            float smallestDistanceFound = 99999.0f;
            int index = 0;
            foreach(GameObject waypoint in m_Waypoints)
            {
                Vector3 distanceVector = gameObject.transform.position - waypoint.transform.position;
                if(distanceVector.magnitude < smallestDistanceFound)
                {
                    m_WaypointIndex = index;
                    smallestDistanceFound = distanceVector.magnitude;
                }

                ++index;
            }

            m_ShouldResetWaypointIndex = false;
        }

        Vector3 distanceToWaypointVector = gameObject.transform.position - m_Waypoints[m_WaypointIndex].transform.position;
        bool shouldUpdateWaypointIndex = distanceToWaypointVector.magnitude <= MAX_DISTANCE_FROM_TARGET;

        if(shouldUpdateWaypointIndex)
        {
            m_WaypointIndex = (m_WaypointIndex + 1) % m_Waypoints.Count;
        }
           
        myDestinationSetter.target = m_Waypoints[m_WaypointIndex].transform;
    }

    private void FixedUpdate()
    {
        if(m_LookingTimer > 0.0f)
        {
            m_LookingTimer -= Time.fixedDeltaTime;
        }
        
    }

    private void PlayLookingAnimation()
    {
        m_LookingTimer = 1.0f;

        // TODO: Add looking for sound animation trigger here
    }
}
