using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectingObjects : MonoBehaviour
{
    // Start is called before the first frame update
    public Trigger ts;
    // Update is called once per frame
    void Update()
    {


        if (Input.GetMouseButtonUp(1))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null)
            {
                Debug.Log(hit.collider.gameObject.name);
                //WaitForTime wait = gameObject.AddComponent<WaitForTime>();
                hit.collider.gameObject.SetActive(false);
                //if (ts.isTriggered == false)
                //{
                //    //ts.activeIt();

                //}

            }
        }
        //    if (ts.isTriggered == true)
        //    {
        //        WaitForTime wait = new WaitForTime();
        //        ts.deactiveIt();
        //    }
        //}
    }
}
public class WaitForTime : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(wait());
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(4);
    }
}