using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepSFX : MonoBehaviour
{
    Rigidbody2D rb;
    AudioSource audioSource;
    bool isMoving = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.x != 0 || rb.velocity.y != 0)
        {
            isMoving = true;
        }
        else
            isMoving = false;

        if (isMoving)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }



    }
}
