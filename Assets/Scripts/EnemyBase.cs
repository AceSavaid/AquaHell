using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int damage = 10;
    public int health = 10;
    public int speed = 3;
    Rigidbody2D _rb;

    Vector3 _directionPlayer;

    public AudioClip enemyhitSound;
    public AudioClip enemydeathSound;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Chase();
    }

    void Chase()
    {
        Player player = FindObjectOfType<Player>();
        if (player)
        {
            _directionPlayer = player.transform.position - transform.position;
            _rb.velocity = _directionPlayer.normalized * speed;
        }
        else
        {
            Destroy(this.gameObject);
        }
        


    }

    void HurtPlayer()
    {
        FindObjectOfType<Player>().RemoveHealth(damage -(FindObjectOfType<Player>()._defenseMultiplier));
        
    }

    void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        PlayEffects(enemyhitSound);
        if (health <= 0)
        {
            PlayEffects(enemydeathSound);
            Destroy(this.gameObject);
        }
    }

    void PlayEffects(AudioClip soundFX)
    {
        if (soundFX)
        {
            AudioSource.PlayClipAtPoint(soundFX, new Vector3(0, 0, 0));
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            HurtPlayer();
        }
        if (other.gameObject.tag == "Bullet")
        {
            TakeDamage(FindObjectOfType<Player>().bulletDamage+(FindObjectOfType<Player>()._bDamageMultiplier * 1));
        }
        
    }
}
