using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour
{
    public int healthContained = 5;
    public GameObject particles;
    public AudioClip pickupSound;

   
    void PickUp()
    {
        FindObjectOfType<Player>().AddHealth(healthContained);
        FindObjectOfType<Player>()._score += 5;
        Debug.Log("item");

        if (particles)
        {
            Instantiate(particles,transform);
        }
        if (pickupSound)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //collision.gameObject.GetComponent<Player>().AddHealth(healthContained);
        
        PickUp();
        

    }
}
