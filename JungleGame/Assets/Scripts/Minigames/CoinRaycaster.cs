using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinRaycaster : MonoBehaviour
{
    public bool isOn = false;

    void Update()
    {
        // return if off, else do thing
        if (!isOn)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            print ("sending raycast");
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null) 
            {
                print ("hit something: " + hit.collider.name);

                if (hit.collider.CompareTag("Coin"))
                {
                    print ("hit coin: " + hit.collider.name);
                }
            }
        }
    }
}
