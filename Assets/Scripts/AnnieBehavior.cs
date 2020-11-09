using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.WSA;

public class AnnieBehavior : MonoBehaviour
{
    public AudioClip HackSound;
    public AudioClip ClickedEnemySound;
    private AudioSource m_AudioSource;

    private const int LEFT_CLICK = 0;
    private const int RIGHT_CLICK = 1;
    private const int MIDDLE_CLICK = 2;

    private const float ARROW_SIZE_SCALAR = 3.0f;

    private static GameObject s_AttachedEnemyObject = null;
    private static bool sb_HasOverlayAppeared = false;

    private static GameObject lastCreatedArrow = null;
    private static AudioSource myAudioSource = null;
    private static AudioClip myClickSound = null;

    [SerializeField] GameObject arrowPrefab;

    // Start is called before the first frame update
    void Start()
    {
        m_AudioSource = gameObject.AddComponent<AudioSource>();
        m_AudioSource.volume = 0.2f;

        myAudioSource = m_AudioSource;
        if(ClickedEnemySound) {
            myClickSound = ClickedEnemySound;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(RIGHT_CLICK))
        {
            DestroyArrow(true);
            
        }
        else if (s_AttachedEnemyObject != null)
        {
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

            if (bResetStartPos)
            {
                PlaceSound();
                s_AttachedEnemyObject = null;
            }
        }
    }
    
    public static void SetRightClickStartPosition(string enemyName)
    {
        myAudioSource.PlayOneShot(myClickSound);
        s_AttachedEnemyObject = GameObject.Find(enemyName);
    }

    private void PlaceSound()
    {
        m_AudioSource.PlayOneShot(HackSound);
        Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentMousePos.z = 0.0f;

        GameObject gameObject = new GameObject();
        gameObject.transform.position = currentMousePos;

        MetricManager.s_NumHacks++;

        s_AttachedEnemyObject.GetComponent<EnemyBehavior>().SetSoundLocation(gameObject.transform);
    }
}