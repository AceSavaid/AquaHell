using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    Rigidbody2D _rb;

    public int _speed;
    public int _speedMultiplyer = 0;

    [Header("Health")]
    int _health = 0;
    int _maxHealth = 100;
    public Slider _healthBar;
    public Image heatSignature;
    //multipliers
    public int _healthmultiplier = 0;
    public int _defenseMultiplier = 0;
    

    [Header("Scoring")]
    public bool notPaused = true;

    public int _score = 0;
    public TextMeshProUGUI _scoreText;

    public float _time = 0.00f;
    public TextMeshProUGUI timerText;

    [Header("TicDamage")]
    float _heattimer;
    float _timerTic = 1;
    int _heatValue = 5;

    [Header("Attack")]
    public GameObject bullet;
    public float bulletSpeed = 50;
    public int bHealthUsage = 10;
    public int bulletDamage = 10;

    bool canShoot = true;
    float shootTimer = 0;

    public int _bDamageMultiplier = 0;
    public int _reloadSpeedMulitiplier = 0;



    [Header("SFX")]
    public AudioClip shootsound;
    public AudioClip hurtsound;
    public AudioClip deathsound;
    public GameObject deathParticles;



    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _maxHealth = 100 + (_healthmultiplier * 10);
        _health = _maxHealth;
        _healthBar.maxValue = _maxHealth;
        _healthBar.value = _health;

        _scoreText.text = "Score: " + _score.ToString();

        timerText.text = "Time: " + ((int)_time).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        _heattimer += Time.deltaTime;
        if (_heattimer > _timerTic)
        {
            RemoveHealth(_heatValue);
            _heattimer = 0;
        }

        Calculations();
        UpdateUI();

        if (!canShoot)
        {
            shootTimer += Time.deltaTime;
            if (shootTimer >= 1 -(_reloadSpeedMulitiplier * .1))
            {
                canShoot = true;
                shootTimer = 0f;
            }
        }
        if (Input.GetMouseButton(0))
        {
            Shoot();
        }
    }

    void Movement()
    {
        Vector2 MDirection = new Vector2(0.0f, 0.0f);

        if (Input.GetKey(KeyCode.W))
        {
            MDirection += new Vector2(0.0f, 1.0f);
        }
        if (Input.GetKey(KeyCode.S))
        {
            MDirection += new Vector2(0.0f, -1.0f);
        }

        if (Input.GetKey(KeyCode.A))
        {
            MDirection += new Vector2(-1.0f, 0.0f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            MDirection += new Vector2(1.0f, 0.0f);
        }

        _rb.velocity = MDirection * (_speed + (_speedMultiplyer* 1));
    }

    void Die()
    {
        PlayEffects(deathsound, deathParticles);
        FindObjectOfType<Manager>().GameOver();
        this.gameObject.SetActive(false);
        Destroy(this.gameObject,20);
        
    }

    void Calculations()
    {
        if (notPaused)
        {
            _time += Time.deltaTime;
            
        }
        
    }

    void UpdateUI()
    {
        _healthBar.value = _health;

        _scoreText.text = "Score: " + _score.ToString();

        timerText.text = "Time: " + ((int)_time).ToString();

        //adds heat affects if HP is lower than half
        if (_health < _maxHealth / 2)
        {
            heatSignature.CrossFadeAlpha(1, 2, true);
        }
        else
        {
            heatSignature.CrossFadeAlpha(0, 2, true);
        }
        
    }

    void Shoot()
    {
        if (canShoot)
        {
            PlayEffects(shootsound, null);
            RemoveHealth(bHealthUsage);

            Vector3 mposition = Input.mousePosition;
            mposition = Camera.main.ScreenToWorldPoint(mposition);

            Vector3 bulletdirection = (mposition - transform.position).normalized;
            GameObject spawnBullet = Instantiate(bullet, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
            spawnBullet.transform.rotation = transform.rotation;

            spawnBullet.GetComponent<Rigidbody2D>().AddForce(bulletdirection * bulletSpeed);

            canShoot = false;
        }
        
    }

    public void AddHealth(int amount)
    {
        _health += amount;

        //error handling
        if (_health > _maxHealth)
        {
            _health = _maxHealth;
        }
    }
    
    public void RemoveHealth(int amount)
    {
        _health -= amount;
        PlayEffects(hurtsound, null);
        if (_health <= 0)
        {
            _health = 0;
            Die();
        }
    }

    void PlayEffects(AudioClip soundFX, GameObject particlesFX)
    {
        if (soundFX)
        {
            AudioSource.PlayClipAtPoint(soundFX, new Vector3(0, 0, 0));
        }

        if (particlesFX)
        {
            GameObject particles = Instantiate(particlesFX);
            Destroy(particles, 3);
        }
    }
}
