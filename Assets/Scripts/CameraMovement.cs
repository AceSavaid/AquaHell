using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        if (!player)
        {
            player = FindObjectOfType<Player>().gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {
            this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, this.transform.position.z);
        }
        

    }
}
