using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadNextLevelTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject m_LevelLoaderObject;

    private LevelLoader m_LevelLoader;
    void Start()
    {
        m_LevelLoader = m_LevelLoaderObject.GetComponent<LevelLoader>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            m_LevelLoader.LoadNextLevel();
        }
    }
}
