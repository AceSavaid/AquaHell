using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Manager : MonoBehaviour
{
    public GameObject noMenu;

    [Header("Item Management")]
    public List<GameObject> items;
    bool spawnItems = true;
    public GameObject spawnArea;

    [Header("Enemy Management")]
    public List<GameObject> enemies;
    bool spawnEnemies = true;
    float enemyTimer = 0;

    [Header("GameOver")]
    bool gameOver = false;
    public GameObject gameOverScreen;
    public TextMeshProUGUI gTimeText;
    public TextMeshProUGUI gscoreText;
    public TextMeshProUGUI gBestTimeText;
    public TextMeshProUGUI gHiScoreText;
    public TextMeshProUGUI gpointsText;



    [Header("Pause")]
    public GameObject pauseMenu;

    [Header("Shop")]
    public GameObject upgradesMenu;
    public TextMeshProUGUI spointsText;
    int points = 0;

    const int maxUpgrade = 5;
    int healthUpgrade = 0;
    int speedUpgrade = 0;
    int defenseUpgrade = 0;
    int damageUpgrade = 0;
    int itemSpawnSpeedUpgrade = 0;
    int reloadSpeedUpgrade = 0;
    List<int> costs = new List<int>() {20, 50, 100, 250, 500, 0};

    [Header("Important Variables and Timers")]
    float gametimer = 0f;

    int currentScore;
    int highScore = 0;
    float currentTime;
    float bestTime = 0.00f;

    float stimer = 0;
    float spawntime = 5.0f;

    Player _p;

    [Header("UI")]
    public Button healthbutton;
    public TMP_Text healthbuttonText;
    public Button speedbutton;
    public TMP_Text speedbuttonText;
    public Button defensebutton;
    public TMP_Text defensebuttonText;
    public Button damagebutton;
    public TMP_Text damagebuttonText;
    public Button reloadbutton;
    public TMP_Text reloadbuttonText;
    public Button itembutton;
    public TMP_Text itembuttonText;

    [Header("Sound Effects")]
    public AudioClip buttonSelect;
    public AudioClip buttonDecline;
    public AudioClip heatSound;
    public AudioClip waterSpawnSound;
    public AudioClip EnemySpawnSound;



    // Start is called before the first frame update
    void Start()
    {
        _p = FindObjectOfType<Player>();

        if (PlayerPrefs.HasKey("HighScore"))
        {
            highScore = PlayerPrefs.GetInt("HighScore");
        }

        if (PlayerPrefs.HasKey("BestTime"))
        {
            bestTime = PlayerPrefs.GetFloat("BestTime");
        }

        if (PlayerPrefs.HasKey("Currency"))
        {
            points = PlayerPrefs.GetInt("Currency");
        }

        if (PlayerPrefs.HasKey("HealthUp"))
        {
            healthUpgrade = PlayerPrefs.GetInt("HealthUp");
            _p._healthmultiplier = healthUpgrade;
        }

        if (PlayerPrefs.HasKey("SpeedhUp"))
        {
            speedUpgrade = PlayerPrefs.GetInt("SpeedUp");
            _p._speedMultiplyer = speedUpgrade;
        }

        if (PlayerPrefs.HasKey("DefenseUp"))
        {
            defenseUpgrade = PlayerPrefs.GetInt("DefenseUp");
            _p._defenseMultiplier = damageUpgrade;
        }

        if (PlayerPrefs.HasKey("DamageUp"))
        {
            damageUpgrade = PlayerPrefs.GetInt("DamageUp");
            _p._bDamageMultiplier = damageUpgrade;
        }

        if (PlayerPrefs.HasKey("ReloadUp"))
        {
            reloadSpeedUpgrade = PlayerPrefs.GetInt("ReloadUp");
            _p._reloadSpeedMulitiplier = reloadSpeedUpgrade;
        }

        if (PlayerPrefs.HasKey("ItemUp"))
        {
            itemSpawnSpeedUpgrade = PlayerPrefs.GetInt("ItemUp");
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            UpdateShopUI();
            spawnItems = false;
            spawnEnemies = false;
            Time.timeScale = 0;

        }
        else
        {
            if (_p.notPaused)
            {
                gametimer += Time.deltaTime;
                stimer += Time.deltaTime;
                enemyTimer += Time.deltaTime;

                if (spawnItems && stimer > spawntime - (itemSpawnSpeedUpgrade * 0.25))
                {
                    SpawnItem();
                    stimer = 0f;
                }

                if (spawnEnemies && enemyTimer > 10)
                {
                    SpawnEnemies();
                    enemyTimer = 0f;
                }


                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGame();
                }

            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    ResumeGame();
                }
            }

        }
    }

    void SpawnItem()
    {
        Vector3 spawnPos = new Vector3(Random.Range(-14, 14), Random.Range(-14, 14), 0);
        GameObject spawnItem = Instantiate(items[0], this.transform.position + spawnPos, Quaternion.identity);
        PlayEffects(waterSpawnSound);
    }

    void SpawnEnemies()
    {
        Vector3 spawnPos = new Vector3(Random.Range(-14, 14), Random.Range(-14, 14), 0);
        GameObject spawnItem = Instantiate(enemies[Random.Range(0, enemies.Count)], spawnArea.transform.position + spawnPos, Quaternion.identity);
        PlayEffects(EnemySpawnSound);
    }

    public void PauseGame()
    {
        _p.notPaused = false;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        _p.notPaused = true;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
        currentScore = 0;
        currentTime = 0;
        _p._score = 0;
        _p._time = 0;
    }

    public void ExitShop()
    {
        gpointsText.text = "Tokens: " + points;
        upgradesMenu.SetActive(false);
    }

    public void GameOver()
    {
        _p.notPaused = false;
        spawnItems = false;
        spawnEnemies = false;

        currentScore = _p._score;
        currentTime = _p._time;
        GameEndCalculations();
        gTimeText.text = "Time: " + (int)currentTime;
        Debug.Log("Current Score" + currentScore);
        gscoreText.text = ("Score: " + currentScore);
        gBestTimeText.text = "Best Time: " + (int)bestTime;
        gHiScoreText.text = "HiScore: " + (int)highScore;
        gpointsText.text = "Tokens: " + points;

        gameOverScreen.SetActive(true);
        UpdateShopUI();
    }

    void GameEndCalculations()
    {
        //makes new save states
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        if (currentTime > bestTime)
        {
            bestTime = currentTime;
            PlayerPrefs.SetFloat("BestTime", bestTime);
        }
        points += currentScore + (int)(currentTime/10);
        PlayerPrefs.SetInt("Currency", points);
    }

    //Shop Functions
    void UpdateShopUI()
    {
        spointsText.text = "Tokens: " + points;

        //button texts
        healthbuttonText.text = "Health Upgrade " + healthUpgrade + "/ " + maxUpgrade + ": " + costs[healthUpgrade] ;
        speedbuttonText.text = "Speed Upgrade " + speedUpgrade + "/ " + maxUpgrade + ": " + costs[speedUpgrade] ;
        defensebuttonText.text = "Defense Upgrade " + defenseUpgrade + "/ " + maxUpgrade + ": " + costs[defenseUpgrade] ;
        damagebuttonText.text = "Damage Upgrade " + damageUpgrade + "/ " + maxUpgrade + ": " + costs[damageUpgrade] ;
        reloadbuttonText.text = "Reload Speed Upgrade " + reloadSpeedUpgrade + "/ " + maxUpgrade + ": " + costs[reloadSpeedUpgrade] ;
        itembuttonText.text = "Spawn Rate Upgrade " + itemSpawnSpeedUpgrade + "/ " + maxUpgrade + ": " + costs[itemSpawnSpeedUpgrade] ;

        
        UpdateButtonColour(healthbutton, healthUpgrade);
        UpdateButtonColour(speedbutton, speedUpgrade);
        UpdateButtonColour(defensebutton, defenseUpgrade);
        UpdateButtonColour(damagebutton, damageUpgrade);
        UpdateButtonColour(reloadbutton, reloadSpeedUpgrade);
        UpdateButtonColour(itembutton, itemSpawnSpeedUpgrade);

        /*if (points < costs[healthUpgrade])
        {
            healthbutton.GetComponent<Image>().color = new Color(141, 34, 86);
            Debug.Log("Cant buy button");
        }
        else
        {
            healthbutton.GetComponent<Image>().color = new Color(35, 156, 86);
            Debug.Log("Can buy button");

        }

        if (points < costs[speedUpgrade])
        {
            speedbutton.GetComponent<Image>().color = new Color(141, 34, 86);
            Debug.Log("Cant buy button");
        }
        else
        {
            speedbutton.GetComponent<Image>().color = new Color(35, 156, 86);
            Debug.Log("Can buy button");

        }

        if (points < costs[defenseUpgrade])
        {
            defensebutton.GetComponent<Image>().color = new Color(141, 34, 86);
            Debug.Log("Cant buy button");
        }
        else
        {
            defensebutton.GetComponent<Image>().color = new Color(35, 156, 86);
            Debug.Log("Can buy button");

        }

        if (points < costs[damageUpgrade])
        {
            damagebutton.GetComponent<Image>().color = new Color(141, 34, 86);
            Debug.Log("Cant buy button");
        }
        else
        {
            damagebutton.GetComponent<Image>().color = new Color(35, 156, 86);
            Debug.Log("Can buy button");

        }

        if (points < costs[reloadSpeedUpgrade])
        {
            reloadbutton.GetComponent<Image>().color = new Color(141, 34, 86);
            Debug.Log("Cant buy button");
        }
        else
        {
            reloadbutton.GetComponent<Image>().color = new Color(35, 156, 86);
            Debug.Log("Can buy button");

        }

        if (points < costs[itemSpawnSpeedUpgrade])
        {
            itembutton.GetComponent<Image>().color = new Color(141, 34, 86);
            Debug.Log("Cant buy button");
        }
        else
        {
            itembutton.GetComponent<Image>().color = new Color(35, 156, 86);
            Debug.Log("Can buy button");

        }*/
    }

    //couldnt get to it to work

    void UpdateButtonColour(Button updateButton, int dependantVariable)
    {
        if (dependantVariable == maxUpgrade)
        {
            updateButton.GetComponent<Image>().color = Color.white;
        }
        else if (points < costs[dependantVariable])
        {
            //updateButton.GetComponent<Image>().color = Color.red;
            updateButton.GetComponent<Image>().color = new Color32(141, 34, 86, 255);
            Debug.Log("Cant buy button");
        }
        else
        {
            //updateButton.GetComponent<Image>().color = Color.green;
            updateButton.GetComponent<Image>().color = new Color32 (35, 156, 86, 255);
            Debug.Log("Can buy button");

        }
    }


    public void HealthUpgrades()
    {
        if (BuyUpgrades(healthUpgrade, healthbutton))
        {
            healthUpgrade++;

            PlayerPrefs.SetInt("HealthUp", healthUpgrade);
            _p._healthmultiplier++;
        }

    }

    public void SpeedhUpgrades()
    {
        if (BuyUpgrades(speedUpgrade, speedbutton))
        {
            speedUpgrade++;

            PlayerPrefs.SetInt("SpeedUp", speedUpgrade);
            _p._speedMultiplyer++;
        }

    }

    public void DefenceUpgrades()
    {
        if (BuyUpgrades(defenseUpgrade, defensebutton))
        {
            defenseUpgrade++;

            PlayerPrefs.SetInt("DefenseUp", defenseUpgrade);
            _p._defenseMultiplier++;
        }

    }

    public void GunDamageUpgrades()
    {
        if (BuyUpgrades(damageUpgrade, damagebutton))
        {
            damageUpgrade++;

            PlayerPrefs.SetInt("DamageUp", damageUpgrade);
            _p._bDamageMultiplier++;
        }

    }

    public void ReloadUpgrades()
    {
        if (BuyUpgrades(reloadSpeedUpgrade, reloadbutton))
        {
            reloadSpeedUpgrade++;

            PlayerPrefs.SetInt("ReloadUp", reloadSpeedUpgrade);
            _p._reloadSpeedMulitiplier++;
        }

    }

    public void ItemSpeedUpgrades()
    {
        if (BuyUpgrades(itemSpawnSpeedUpgrade, itembutton))
        {
            itemSpawnSpeedUpgrade++;
            PlayerPrefs.SetInt("ItemUp", itemSpawnSpeedUpgrade);
        }

    }

    bool BuyUpgrades(int level, Button button)
    {
        if (level < maxUpgrade)
        {
            if (EnoughCurrency(level))
            {
                points -= costs[level];
                level++;
                PlayerPrefs.SetInt("Currency", points);

                UpdateShopUI();

                PlayEffects(buttonSelect);
                return true;
            }
            else
            {
                PlayEffects(buttonDecline);
                UpdateShopUI();
                return false;
            }
        }
        else
        {
            button.interactable = false;
            UpdateShopUI();
            return false;
        }
    }

    bool EnoughCurrency(int upgradelevel)
    {
        if (points >= costs[upgradelevel])
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    void PlayEffects(AudioClip soundFX)
    {
        if (soundFX)
        {
            AudioSource.PlayClipAtPoint(soundFX, new Vector3(0, 0, 0));
        }
    }

    public void ClearSaves()
    {
        PlayerPrefs.DeleteAll();
    }
}
