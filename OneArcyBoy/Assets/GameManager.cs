using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public GameObject player;
    private PlayerBehaviour playerScript;
    public GameObject enemy1;
    public int currentLevel;
    public Canvas UI;
    public MenuController menu;
    public GameOverUIControl gameOverUI;
    public Text currentPointsUI;
    public Text enemyCount;
    public int currentPoints;
    public Text menuHead;
    public GameObject chainUpgrade;
    public int chainUpgradeCount = 0;
    private int chainUpgradeCost = 100;
    public GameObject damageUpgrade;
    public int damageUpgradeCount = 0;
    public int damageUpgradeCost = 100;
    public GameObject rangeUpgrade;
    public int rangeUpgradeCount = 0;
    public int rangeUpgradeCost = 200;
    public GameObject explosionUpgrade;
    public int explosionUpgradeCount = 0;
    public GameObject healthUpgrade;
    public int healthUpgradeCount = 0;
    public int healthUpgradeCost = 300;
    public GameObject cooldownReduction;
    public int cooldownReductionCount = 0;
    public int cooldownReductionCost = 500;
    public bool levelInProgress;
    public bool FInChat;


    public static GameManager getInstance(){
        return instance;
    }
    private void Awake(){
        if (instance != null){
            Destroy(gameObject);
        }

        instance = this;
        playerScript = player.GetComponent<PlayerBehaviour>();
        currentLevel = 0;
        levelInProgress = false;
        FInChat = false;
        nextLevel();
    }


    void UpdateUI()
    {
        //UI Updates
        currentPointsUI.text = "Current Points: " + currentPoints;

        Text[] chainTexts = chainUpgrade.GetComponentsInChildren<Text>();
        chainTexts[0].text = "Buy Chain Upgrade " + (chainUpgradeCount + 1);
        chainTexts[1].text = "Arc hits + 1 Target\r\nCost: " + chainUpgradeCost;

        Text[] damageTexts = damageUpgrade.GetComponentsInChildren<Text>();
        damageTexts[0].text = "Buy Damage Upgrade " + (damageUpgradeCount + 1);
        damageTexts[1].text = "Arc deals 10 more damage\r\nCost: " + damageUpgradeCost;

        Text[] rangeTexts = rangeUpgrade.GetComponentsInChildren<Text>();
        rangeTexts[0].text = "Buy Range Upgrade " + (rangeUpgradeCount + 1);
        rangeTexts[1].text = "Arc has 5 more range\r\nCost: " + rangeUpgradeCost;

        Text[] healthTexts = healthUpgrade.GetComponentsInChildren<Text>();
        if(healthUpgradeCount < 2){
            healthTexts[0].text = "Buy Health Upgrade " + (healthUpgradeCount + 1);
            healthTexts[1].text = "You gain +100 Max HP\r\nCost: " + healthUpgradeCost;
        } else {
            healthTexts[0].text = "Buy Full Health";
            healthTexts[1].text = "Your Health is fully restored\r\nCost: " + 500;
        }

        Text[] cooldownTexts = cooldownReduction.GetComponentsInChildren<Text>();
        Button cdButton = cooldownReduction.GetComponentInChildren<Button>();

        if(cooldownReductionCount < 5){
            cooldownTexts[0].text = "Buy Cooldown Reduction " + (cooldownReductionCount + 1);
            cooldownTexts[1].text = "Reduce your current Cooldown by 25%\r\nCost: " + cooldownReductionCost;
        } else if(cooldownReductionCount == 5 && currentLevel >= 50){
            if (cdButton != null){
                cdButton.enabled = true;
            }
            cooldownTexts[0].text = "Buy No more Cooldown";
            cooldownTexts[1].text = "This will break things.\r\nCost: " + 40000;
            cooldownTexts[1].color = new Color(1,0,0,1);
        } else if(cooldownReductionCount == 6){
            if(cdButton != null){
                cdButton.enabled = false;
                cooldownTexts[0].text = "No more Cooldown";
                cooldownTexts[1].text = "Don't say I didn't warn you.";
            }
        } else {
            if(cdButton != null){
                cdButton.enabled = false;
            }
            cooldownTexts[0].text = "Please Stop";
            cooldownTexts[1].text = "You are already too fast!!!";
        }
        /* switch(cooldownReductionCount){
            case 0:
                cooldownTexts[0].text = "Buy Cooldown Reduction " + (cooldownReductionCount + 1);
                cooldownTexts[1].text = "Cast Arc twice as fast\r\nCost: " + cooldownReductionCost;
                break;
            case 1:
                cooldownTexts[0].text = "Buy Cooldown Reduction " + (cooldownReductionCount + 1);
                cooldownTexts[1].text = "Cast Arc quarce as fast\r\nCost: " + cooldownReductionCost;
                break;
            case 2:
                cooldownTexts[0].text = "Buy Cooldown Reduction " + (cooldownReductionCount + 1);
                cooldownTexts[1].text = "Cast Arc octence as fast\r\nCost: " + cooldownReductionCost;
                break;
            case 3:
                cooldownTexts[0].text = "Please stop you are too fast already";
                cooldownTexts[1].text = "Don't break my game qwq" + cooldownReductionCost;
                break;
            case 4:
                cooldownTexts[0].text = "Oh no";
                cooldownTexts[1].text = ">:(";
                Button cdButton = cooldownReduction.GetComponentInChildren<Button>();
                if (cdButton != null){
                    cdButton.enabled = false;
                    Destroy(cdButton);
                }
                break;
            } */

        Text[] explosionTexts = explosionUpgrade.GetComponentsInChildren<Text>();
        if(explosionUpgradeCount == 0){
            explosionTexts[0].text = "???";
            explosionTexts[1].text = "Cost: 1000";
        } else if(explosionUpgradeCount == 1 && currentLevel >= 40){
            explosionTexts[0].text = "Fuck it. Double explosion radius";
            explosionTexts[1].text = "Cost: 20000";
            Button explosionButton = explosionUpgrade.GetComponentInChildren<Button>();
            explosionButton.enabled = true;
        } else {
            explosionTexts[0].text = "Ragnarok";
            explosionTexts[1].text = "";
            Button explosionButton = explosionUpgrade.GetComponentInChildren<Button>();
            explosionButton.enabled = false;
        }
        
    }

    void Update(){
        currentPointsUI.text = "Current Points: " + currentPoints;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemyCount.text = enemies.Length + " Enemies remain";  
        if (enemies.Length == 0){
            levelInProgress = false;
            UpdateUI();
            menuHead.text = "Level " + currentLevel + " finished!";
            if (currentLevel == 69){
                menuHead.text = "nice";
            }
            if(currentLevel == 50){
                menuHead.text = "Perhaps I should've mentioned this game is infinite...";
            }
            if (currentLevel == 100){
                menuHead.text = "Thank you for enjoying my game. There is nothing left to discover anymore. You can stop anytime you want. :)";
            }
            if (currentLevel == 150){
                menuHead.text = "I am serious, there is really nothing to do anymore...!";
            }
            if (currentLevel == 200){
                menuHead.text = "The only reason I am typing these messages is to waste my own time, and in extention, your time as well.";
            }
            if (currentLevel == 250){
                menuHead.text = "There's not even gonna be a good payoff or anything. I'm just gonna get bored typing and then leave you hanging. Don't do this to yourself.";
            }
            if(currentLevel == 300){
                menuHead.text = "Level " + currentLevel + " finished.";
            }
            if (currentLevel == 302){
                menuHead.text = "Oh, come on! I really thought I had you this time... I'm done";
            }
            menu.setVisible(true);
        }

        if(FInChat && Input.GetKeyDown(KeyCode.Escape)){
                Application.Quit();
            }
        if (FInChat && Input.GetKeyDown(KeyCode.R)){
            SceneManager.LoadScene("MainGame");
        }
    }

    public void buyChainUpgrade(){
        if (currentPoints >= chainUpgradeCost){
            currentPoints -= chainUpgradeCost;
            playerScript.ARCmaxhits += 1;
            chainUpgradeCount += 1;
        }
        chainUpgradeCost = 100 * (chainUpgradeCount + 1);
        UpdateUI();
    }

    public void buyDamageUpgrade(){
        if (currentPoints >= damageUpgradeCost){
            currentPoints -= damageUpgradeCost;
            playerScript.ARCdmg += 10;
            damageUpgradeCount += 1;
        }
        damageUpgradeCost = 100 * (damageUpgradeCount * 2 + 1);
        UpdateUI();
    }

    public void buyRangeUpgrade(){
        if (currentPoints >= rangeUpgradeCost){
            currentPoints -= rangeUpgradeCost;
            playerScript.ARCrange += 5;
            rangeUpgradeCount += 1;
        }
        rangeUpgradeCost = 200 * ((rangeUpgradeCount * 2) + 1);
        UpdateUI();
    }

    public void buyHealthUpgrade(){
        //Health Upgrades later turn to full Heals
        if (healthUpgradeCount >= 2){
            if (currentPoints >= 500){
                currentPoints -= 500;
                playerScript.healFull();
            }
        } else {
            if (currentPoints >= healthUpgradeCost){
                currentPoints -= healthUpgradeCost;
                playerScript.healthMAX += 100;
                playerScript.healFull();
                healthUpgradeCount += 1;
                }
        }
        healthUpgradeCost = 300 * ((healthUpgradeCount * 2) + 1);
        UpdateUI();
    }

    public void buyCooldownReduction(){
        if(cooldownReductionCount < 5){
            if (currentPoints >= cooldownReductionCost){
                currentPoints -= cooldownReductionCost;
                //Debug.Log("Kürze PlayerCD!");
                //Debug.Log("Aktuell:" + playerScript.ARCooldown);
                //Debug.Log("Ergebnis der Rechnung: " + playerScript.ARCooldown + " * (3/4) = " + ((float) playerScript.ARCooldown) * (3/4));
                playerScript.ARCooldown *= 0.75f;
                //Debug.Log("Nach Kürzung" + playerScript.ARCooldown);
                cooldownReductionCount += 1;
            }
        } else if(cooldownReductionCount == 5 && currentPoints >= 40000 && currentLevel >= 50){
            currentPoints -= 40000;
            playerScript.ARCooldown = 0;
            cooldownReductionCount += 1;
        }
        cooldownReductionCost = 500 * (cooldownReductionCount * 3 + 1);
        UpdateUI();
    }

    public void buyExplosionUpgrade(){
        if(explosionUpgradeCount == 0){
            if(currentPoints >= 1000){
            currentPoints -= 1000;
            playerScript.ARCexplosion = true;
            explosionUpgradeCount += 1;
            UpdateUI();
            return;
            }
        } else if(explosionUpgradeCount == 1 && currentLevel >= 40){
            if (currentPoints >= 20000){
                currentPoints -= 20000;
                playerScript.biggerARCexplosion = true;
                explosionUpgradeCount += 1;
                UpdateUI();
                return;
            }
        }
        UpdateUI();
        
    }

    public void nextLevel(){
        if(levelInProgress){
            return;
        }
        levelInProgress = true;
        currentLevel += 1;
        menu.setVisible(false);
        for (int i = 0; i < currentLevel * 2; i++){
            Vector3 pos = getRandomVector3(-24f,24f);
            while (Vector3.Distance(pos,player.transform.position) < 5){
                pos = getRandomVector3(-24f,24f);
            }
            spawnEnemy(pos);
            if (i > 50 && currentLevel < 30){
                break;
            }
        }
        playerScript.currentARCooldown = 0;
    }

    public void spawnEnemy(Vector3 position){
        GameObject enemy = Instantiate(enemy1, position,transform.rotation);
        enemy.SetActive(true);
    }

    public void getPoints(int amount){
        StatCount.increasePoints(amount);
        currentPoints += amount;
    }

    public Vector3 getRandomVector3(float min, float max){
        return new Vector3(Random.Range(min,max),Random.Range(min,max),0);
    }

    public void GameOver(){
        FInChat = true;
        string s = "Game Over!" + "\r\n" + "Press R to restart or Esc to exit the game!" + "\r\n\r\n";
        s += StatCount.getStats();
        gameOverUI.setMessage(s);
        gameOverUI.setVisible(true);
    }





}
