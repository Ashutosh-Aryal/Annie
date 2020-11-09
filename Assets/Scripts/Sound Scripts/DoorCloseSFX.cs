using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCloseSFX : MonoBehaviour
{
    public AudioClip DoorClose;
    private AudioSource audioSource { get { return GetComponent<AudioSource>(); } }
    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<AudioSource>();
        audioSource.clip = DoorClose;
        audioSource.playOnAwake = false;
        //AddListener(() => PlaySound());
    }

    // Update is called once per frame
    void PlaySound()
    {
        audioSource.PlayOneShot(DoorClose);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("annie"))
        {
            PlaySound();
        }
    }
}
