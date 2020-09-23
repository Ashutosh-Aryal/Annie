using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyBehavior : MonoBehaviour
{ 
    private const int RIGHT_CLICK = 1;
    private const int MAX_ENEMY_COUNT = 100;

    [SerializeField] private Transform[] m_Waypoints;

    private int m_WaypointIndex = 0;
    private int m_RandValue;

    private Rigidbody2D myRigidbody2D;

    private static HashSet<int> s_AssignedEnemyNumbers = new HashSet<int>();

    //private void OnMouseOver()
    //{
    //    if (Input.GetMouseButtonDown(RIGHT_CLICK))
    //    {
    //        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        AnnieBehavior.SetRightClickStartPosition(gameObject.name);
    //    }
    //}

    // Start is called before the first frame update
    void Start()
    {
        int rand = 0;

        do {
            rand = Random.Range(0, MAX_ENEMY_COUNT);
        } while (s_AssignedEnemyNumbers.Contains(rand));

        s_AssignedEnemyNumbers.Add(rand);
        m_RandValue = rand;
        gameObject.name += rand;

        myRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
    }

    private void OnDestroy()
    {
        s_AssignedEnemyNumbers.Remove(m_RandValue);
    }

    // Update is called once per frame
    void Update()
    {
        
        // TODO: Check if there is a sound location to go to
        // If not, check what the closest waypoint is from current location and go towards it
        // and update the current index to reflect being at that waypoint
    }
}
