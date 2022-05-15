using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{

    public static PlayerBehaviour instance;
    public int health = 100;
    public int healthMAX = 100;
    public float currentARCooldown;
    public float ARCooldown;
    public Slider healthUI;
    public Slider manaUI;
    public int ARCdmg;
    public int ARCmaxhits;
    public int ARCrange; 
    public bool ARCexplosion;
    public bool biggerARCexplosion;
    public int ARCsplits;
    public float ARCAmpUp;
    public bool doubleDamageOnFinalChain;
    public GameObject emptySkill;
    public GameObject damagePopupPrefabTest;

    private AudioManager audioPlayer;

    public static PlayerBehaviour getInstance(){
        return instance;
    }
    private void Awake(){
        if (PlayerBehaviour.getInstance() != null){
            Destroy(gameObject);
        }
        instance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        ARCooldown = 120;                   //default = 120
        currentARCooldown = 0;              //
        ARCmaxhits = 20;                     //default = 3
        ARCdmg = 50;                        //default = 10
        ARCrange = 10;                      //default = 10
        ARCexplosion = false;               //default = false
        biggerARCexplosion = false;         //default = false
        ARCsplits = 1;                      //default = 0 (no splits)
        ARCAmpUp = 1.2f;                    //default = 0.9 (10% damage loss each chain)
        doubleDamageOnFinalChain = false;   //default = false
        audioPlayer = AudioManager.getInstance();
        
    }

    // Update is called once per frame
    void Update()
    {

        if (health <= 0){
            healthUI.value = 0;
            GameManager.getInstance().GameOver();
            Destroy(gameObject);
        }

        if(Input.GetMouseButtonDown(0) && currentARCooldown <= 0) {
            if(!targetsInRange()){return;}
            currentARCooldown = ARCooldown;
            Arc.createNewArc(emptySkill, transform, ARCdmg, ARCrange, ARCmaxhits, ARCsplits, ARCAmpUp, doubleDamageOnFinalChain);
        }

        if(Input.GetMouseButtonDown(1)){
            DamageNumberController.create(damagePopupPrefabTest, transform, Random.Range(0,300));

        }
    }

    void FixedUpdate(){
        healthUI.maxValue = healthMAX;
        healthUI.value = health;
        manaUI.maxValue = ARCooldown;
        manaUI.value = ARCooldown - currentARCooldown;

        if(currentARCooldown > 0){
            currentARCooldown -= 1;
        }
    }

    public void takeDamage(int dmg){
        playRandomHitSound();
        health -= dmg;
        DamageNumberController.createTextPopup(transform, "Aua!");
    }

    public void playRandomHitSound(){
        float r = Random.Range(0.0f,1.0f);
        if (r < 0.2375f){
            audioPlayer.Play("Hit1");
        } else if (r < 0.475f){
            audioPlayer.Play("Hit2");
        } else if (r < 0.7125f){
            audioPlayer.Play("Hit3");
        } else if (r < 0.95){
            audioPlayer.Play("Hit4");
        } else{
            audioPlayer.Play("Ouch");
        }
    }

    public void heal(int amount){
        if(health + amount > healthMAX){
            health = healthMAX;
        } else {
            health += amount;
        }
    }

    public void healFull(){
        health = healthMAX;
    }

    //returns true, if a target is in range
    public bool targetsInRange(){
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Vector3 currentPos = transform.position;
        foreach (GameObject g in enemies){
            if(g == null){continue;}
            Vector3 enemyPos = g.transform.position;
            float currentDistance = Vector3.Distance(currentPos,enemyPos);
            if(currentDistance <= ARCrange){
                return true;
            }
        }
        return false;
    }

}
