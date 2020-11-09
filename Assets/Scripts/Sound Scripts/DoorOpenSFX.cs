using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenSFX : MonoBehaviour
{
    public AudioClip DoorOpen;
    private AudioSource audioSource { get { return GetComponent<AudioSource>(); } }
    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<AudioSource>();
        audioSource.clip = DoorOpen;
        audioSource.playOnAwake = false;
        //AddListener(() => PlaySound());
    }

    // Update is called once per frame
    void PlaySound()
    {
        audioSource.PlayOneShot(DoorOpen);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("annie"))
        {
            PlaySound();
        }
    }
}
