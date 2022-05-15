using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arc : MonoBehaviour
{
    public GameObject instantee;
    private int damage;
    private int currentDamage;
    private int maxTargets;
    private int range;
    private int amountOfSplits;
    private float damageAmpUp;
    private bool doubleDamageOnFinalChain;
    private int timeToLive;
    private GameObject[] targets;
    private bool[] targetsHit;
    private int numOfHits = 0;
    private Vector3 startPos;
    private Vector3 currentTargetPos;
    private AudioManager audioPlayer;

    private int[] targetIndices;
    private LineRenderer lr;
    private bool spellFinished = false;
    private bool setupFinished = false;

    public static GameObject createNewArc(GameObject instantee, Transform posAndRot, int initialDmg, int range, int maxHits, int splits, float ampUp, bool doubleDamageonFinalChain, GameObject[] targets=null, bool[] alreadyHit=null){
        GameObject newArc = Instantiate(instantee, posAndRot.position, posAndRot.rotation);
        Arc newArcScript = newArc.AddComponent<Arc>();
        newArcScript.setupStats(initialDmg, range, maxHits, splits, ampUp, doubleDamageonFinalChain);
        newArcScript.instantee = instantee;
        if (targets != null){
            newArcScript.setTargets(targets, alreadyHit);
        }
        return newArc;
    }

    public void setupStats(int initialDmg, int r, int m, int s, float ampUp, bool doubleDamageOnFinalChain){
        this.damage = initialDmg;
        this.currentDamage = initialDmg;
        this.range = r;
        this.maxTargets = m;
        this.amountOfSplits = s;
        this.damageAmpUp = ampUp;
        this.doubleDamageOnFinalChain = doubleDamageOnFinalChain;

        setupFinished = true;
    }

    public void setTargets(GameObject[] targets, bool[] alreadyHit){
        this.targets = targets;
        this.targetsHit = alreadyHit;
    }
    private void Awake(){
        //Setup damit alles ordentlich funktioniert

        //Setup Gameobject Basics
        gameObject.name = "Arc";
        timeToLive = 300;
        StatCount.increaseTimesArcCasted();
        startPos = transform.position;

        //Setup Audiomanager
        audioPlayer = AudioManager.getInstance();

        //Setup Line-Renderer, damit man auch visuell was sieht
        lr = gameObject.AddComponent<LineRenderer>();
        lr.startColor = lr.endColor = Color.cyan;
        lr.alignment = LineAlignment.View;
        lr.startWidth = lr.endWidth = 0.3f;
        lr.positionCount = 1;
        lr.numCapVertices = 3;
        lr.numCornerVertices = 3;
        lr.SetPosition(0, transform.position);  //Start at Cast Origin
        lr.material = new Material(Shader.Find("Sprites/Default"));
    }

    IEnumerator Start(){
        while(!setupFinished){
            yield return new WaitForSeconds(0.01f);
        }
        if(targets == null){
            setupNewTargets();
        }
        
        


        //Setup Array that keeps track of Indices hit in order, to help with smoother animation
        targetIndices = new int[maxTargets];

        //MainLoop starten
        StartCoroutine(mainLoop());
    }

    private void setupNewTargets(){
        targets = GameObject.FindGameObjectsWithTag("Enemy");   //Gibt alle Enemies in current Szene
            targetsHit = new bool[targets.Length];                  //Create an array um zu merken welcher Gegner bereits gehittet wurde
            for (int i = 0; i < targetsHit.Length; i++){
                targetsHit[i] = false;                              //Am Anfang wurde noch niemand gehittet
                Debug.Log("Target: " + targets[i].name + " Distance: " + Vector3.Distance(transform.position,targets[i].transform.position));
            }
    }
    

    // Update the 0th position to stick to the caster, but only as long as the spell isn't finished.
    // Also update all other positions to stick to their respective enemies
    // Once the spell is finished, stop updating so the spell can properly disappear
    // Also check for NullPointer, as enemies since the hit happened may have died already
    void Update(){
        if (!spellFinished){
            lr.SetPosition(0, startPos);

            for (int i = 0; i < numOfHits; i++){
                GameObject enemyHit = targets[targetIndices[i]];
                if (enemyHit != null){
                    lr.SetPosition(i+1, enemyHit.transform.position);
                }
            }
        }
    }

    void FixedUpdate(){
        //Check if gameobject has been around for too long, if yes something went wrong and has to be manually cleaned
        if(timeToLive <= 0 && !spellFinished){
            spellFinished = true;
            StartCoroutine(cleanUpSpell());
        }
        //If Gameobject is STILL alive, all hope for proper cleanup is lost. Just get rid of it.
        if(timeToLive <= -180){
            Destroy(gameObject);
        }
        timeToLive -= 1;
    }
    

    IEnumerator mainLoop(){
        int n = int.MaxValue;
        while (numOfHits < maxTargets){
            Debug.Log(numOfHits);
            int newIndex = getIndexOfClosestNotYetBeenHitEnemy(); 
            //Wenn newIndex == int.MaxValue, dann haben wir keinen Gegner den wir hitten können
            if(newIndex == int.MaxValue){
                if(doubleDamageOnFinalChain && n != int.MaxValue){
                    hitEnemy(n);
                }
                break;
            }
            n = newIndex;
            if(!exists(targets[n])){continue;}
            currentTargetPos = targets[n].transform.position;
            targetIndices[numOfHits] = n;
            drawLineToEnemy(currentTargetPos);
            if(numOfHits == maxTargets - 1 && doubleDamageOnFinalChain){
                hitEnemy(n);
                hitEnemy(n);
            }else {
                hitEnemy(n);
            }
            moveToEnemy(currentTargetPos);
            numOfHits++;
            currentDamage = (int)((float)currentDamage * damageAmpUp);
            if(amountOfSplits > 0){
                Arc.createNewArc(instantee, targets[n].transform, currentDamage, range, maxTargets-numOfHits,amountOfSplits-1,damageAmpUp,doubleDamageOnFinalChain,targets,targetsHit);
                amountOfSplits--;
            }
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.1f);
        
        spellFinished = true;
        StartCoroutine(cleanUpSpell());
    }

    IEnumerator cleanUpSpell(){
        Debug.Log("Starting cleanup!");
        for (int i = 0; i < numOfHits; i++){
            deleteOldestLine();
            yield return new WaitForSeconds(0.1f);
        }

        Destroy(gameObject);
    
    }

    void drawLineToEnemy(Vector3 targetPos){
        lr.positionCount = lr.positionCount + 1;
        lr.SetPosition(numOfHits+1, targetPos);

        float r = Random.Range(0.0f,1.0f);
        if(r < 0.5f){
            audioPlayer.Play("Spark1");
        } else {
            audioPlayer.Play("Spark2");
        }
    }
    void hitEnemy(int targetIndex){
        targetsHit[targetIndex] = true;
        GameObject enemy = targets[targetIndex];
        if(!exists(enemy)){return;}
        EnemyBehaviour enemyScript = enemy.GetComponent<EnemyBehaviour>();
        if(enemyScript == null){
            Debug.LogWarning("Kettenblitz hat einen Gegner gefunden, der nicht das erforderliche richtige Skript hat!");
            Debug.LogWarning("Alle Gegner mit den 'Enemy'-Tag müssen dieses Skript-Komponent haben. Wenn hier ein Fehler auftritt, bitte Moritz kontaktieren! :)");
            return;
        }
        enemyScript.takeDamage(currentDamage);
    }

    void moveToEnemy(Vector3 targetPos){
        transform.position = targetPos;
    }

    //Removes oldest Line by copying all other lines one step down
    void deleteOldestLine(){

        int newPositionCount = lr.positionCount - 1;
        for (int i = 0; i < newPositionCount; i++){
            lr.SetPosition(i, lr.GetPosition(i+1));
        }

        if(newPositionCount >= 0){lr.positionCount = newPositionCount;};
    }

    private int getIndexOfClosestNotYetBeenHitEnemy(){

        //Need to remember Distance and Index of closest Enemy
        //Wenn wir jemals 'int.MaxValue' Anzahl von Gegnern in unserem target-Array haben, 
        //haben wir ganz andere Probleme als dass dieser Code malfunctioned.
        float closestDistance = Mathf.Infinity;
        int closestIndex = int.MaxValue;

        for (int i = 0; i < targets.Length; i++) {
            //Check if target has already been hit, if yes, immediately continue.
            //Never hit same target twice.
            if (targetsHit[i]){
                continue;
            }
            //Check if current GameObject is closer than previous closest, if yes, remember new closest.
            GameObject g = targets[i];
            if(!exists(g)){continue;}
            float currentDistance = Vector3.Distance(transform.position, g.transform.position);
            if (currentDistance < closestDistance && currentDistance < range){
                closestDistance = currentDistance;
                closestIndex = i;
            }
        }
        return closestIndex;

    }

    private bool exists(GameObject g){
        return g != null;
    }

}
