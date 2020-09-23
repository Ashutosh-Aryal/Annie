using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;

public class AnnieBehavior : MonoBehaviour
{
    private const int LEFT_CLICK = 0;
    private const int RIGHT_CLICK = 1;
    private const int MIDDLE_CLICK = 2;

    private const float ARROW_SIZE_SCALAR = 3.0f;

    private static GameObject s_AttachedEnemyObject = null;
    private static bool sb_HasOverlayAppeared = false;

    private static GameObject lastCreatedArrow = null;

    [SerializeField] GameObject arrowPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(RIGHT_CLICK)) {
            DestroyArrow(true);
        } else if(s_AttachedEnemyObject != null) { 
            CreateArrow();
        }
    }

    private void CreateArrow()
    {
        Vector3 startMousePos = s_AttachedEnemyObject.transform.position;
        startMousePos.z = 0.0f;

        Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentMousePos.z = 0.0f;

        Vector3 arrowDirection = currentMousePos - startMousePos;
        float centerDistance = arrowDirection.magnitude / 2.0f;
        arrowDirection.Normalize();

        DestroyArrow(false);

        lastCreatedArrow = Instantiate(arrowPrefab, startMousePos + arrowDirection * centerDistance, Quaternion.identity);

        float angle = Mathf.Atan2(arrowDirection.y, arrowDirection.x) * Mathf.Rad2Deg;
        lastCreatedArrow.transform.eulerAngles = new Vector3(0.0f, 0.0f, angle);
        
        SpriteRenderer arrowSpriteRenderer = lastCreatedArrow.GetComponent<SpriteRenderer>();
        arrowSpriteRenderer.size = new Vector2(centerDistance * ARROW_SIZE_SCALAR, arrowSpriteRenderer.size.y);
    }

    private void DestroyArrow(bool bResetStartPos)
    {
        if (lastCreatedArrow != null)
        {
            Destroy(lastCreatedArrow);
            lastCreatedArrow = null;

            if(bResetStartPos)
            {
                PlaceSound();
                s_AttachedEnemyObject = null;
            }
        }
    }
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(RIGHT_CLICK))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            AnnieBehavior.SetRightClickStartPosition(gameObject.name);
        }
    }
    public static void SetRightClickStartPosition(string enemyName)
    {
        s_AttachedEnemyObject = GameObject.Find(enemyName);
    }

    private void PlaceSound()
    {
        // TODO: Place a sound and have that become the new target for the enemy type
    }
}
