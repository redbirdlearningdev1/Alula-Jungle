using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CoinReplacer : MonoBehaviour
{
    public Image image;
    public Sprite turnedCoin;
    public Rigidbody2D rb;
    public GameObject flatCoin;
    public float flatCoinDuration;

    private bool isOn = true;

    void OnCollisionEnter2D(Collision2D col)
    {
        // turn off on first collision
        if (!isOn)
            return;
        isOn = false;

        StartCoroutine(ReplaceCoinRoutine());
    }   

    private IEnumerator ReplaceCoinRoutine()
    {
        // swap this sprite
        image.sprite = turnedCoin;

        yield return new WaitForSeconds(0.15f);

        // spawn flat coin at same velocity and position
        GameObject coin = Instantiate(flatCoin, this.transform.position, this.transform.rotation, this.transform.parent);
        coin.GetComponent<Rigidbody2D>().velocity = rb.velocity;
        coin.GetComponent<DeleteParticle>().Delete(flatCoinDuration);

        // delete this object
        Destroy(this.gameObject);
    }
}
