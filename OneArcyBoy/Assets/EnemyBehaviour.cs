using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public float moveSpeed;
    public float baseMoveSpeed;
    public float buffedMoveSpeed;

    public Rigidbody2D rb;
    public SpriteRenderer spr;
    public Color baseColor;
    public Vector2 movement;

    public bool meleeAttackReady = true;
    public int meleeAttackCooldown;
    public int damage = 15;
    public int health;
    public int maxHealth;
    public int pointsWorth;
    public float[] anglesToCheck;
    public bool stopMovingAndStartDying=false;
    public float timeToDie=1.5f;
    public GameObject deathExplosion;
    public GameObject damageNumberPrefab;
    public GameObject testInstantiate;
    public LayerMask layerMask;
    public float raycastLength;
    public int updateMovementTimer;
    public float lastWorkingAngle;
    

    public GameObject target;
    //public GameObject testInstantiate;

    Vector3 targetPos;

    // Update is called once per frame

    private void Awake(){
        target = GameObject.FindGameObjectWithTag("Player");
        maxHealth = health = 100;
        updateMovementTimer = 0; //30 + Random.Range(-15,15);
        anglesToCheck = new float[] {10f, -10f, 20f, -20f, 30f, -30.0f};
         anglesToCheck = new float[91];
        int indexHelp = 0;
        for(int i = -45; i < 46; i++){
            anglesToCheck[indexHelp] = i;
            indexHelp++;
        } 

    }
    void Update()
    {
        if(stopMovingAndStartDying){
            return;
        }
        if (health <= 0){
            GameManager.getInstance().getPoints(pointsWorth);
            startDying();
        }

        if (target != null && updateMovementTimer <= 0){

            //updateMovementTimer = 30;
            updateMovementTimer = 0;
            Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
            movement = targetRb.position - rb.position;
            movement.Normalize();
            /* if(checkDirectApproach(targetRb,true)){
                setMoveTowardsPoint(targetRb.position);
                Debug.DrawLine(rb.position, targetRb.position, Color.green);
            } else{
                movement = Vector2.zero;
                Debug.DrawLine(rb.position, targetRb.position, Color.red);
                //float angle = calculateWorkingAngleWithCircleCast(anglesToCheck, targetRb, true);
                //setMoveTowardsPoint(RotatePointAroundPivot(target.transform.position, rb.position, new Vector3(0,0,angle)));
            } */
        }

        //Color Setting stuff
        spr.color = Color.Lerp(baseColor,Color.green,(moveSpeed - baseMoveSpeed) / (buffedMoveSpeed - baseMoveSpeed));
    }

    public bool checkDirectApproach(Rigidbody2D targetRb, bool debug = false){
        Vector2 direction = targetRb.position - rb.position;
        RaycastHit2D hit = Physics2D.CircleCast(rb.position, 0.01f, direction, raycastLength,layerMask);
        return (!hit || hit.transform.gameObject.tag == "Player");              //Return true if there's no hit, or if player was directly hit, false otherwise
    
    }

    public void startDying(){
        if(PlayerBehaviour.getInstance().ARCexplosion){
                GameObject explode = Instantiate(deathExplosion,transform.position, transform.rotation);
                explode.SetActive(true);
            }
            AudioSource[] popSounds = gameObject.GetComponents<AudioSource>();
            float r = Random.Range(0.0f,1.0f);
            if(r < 0.5f){
                popSounds[0].Play();
            } else {
                popSounds[1].Play();
            }
            stopMovingAndStartDying = true;
            BoxCollider2D col = gameObject.GetComponent<BoxCollider2D>();
            if (col != null){
                Destroy(col);
            }
            SpriteRenderer spr = gameObject.GetComponent<SpriteRenderer>();
            if (spr != null){
                Destroy(spr);
            }
            Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
            if (rb != null){
                Destroy(rb);
            }
            gameObject.tag = "Untagged";
            Destroy(gameObject,timeToDie);
    }


    public float calculateWorkingAngleWithCircleCast(float[] angles, Rigidbody2D targetRb,bool debug = false){
        float totalOfAnglesThatWork = 0f;
        int amountOfAnglesThatWork = 0;
        for (int i = 0; i < angles.Length; i++){
                    //float angle = angles[i] + Random.Range(-5f,5f);
                    float angle = angles[i];
                    Vector2 direction = ((Vector2) RotatePointAroundPivot(targetRb.position, rb.position, new Vector3(0,0,angle))) - rb.position;
                    RaycastHit2D hit = Physics2D.CircleCast(rb.position, 0.8f, direction, raycastLength,layerMask);

                    if(debug){Debug.DrawRay(rb.position, direction, Color.red);}

                    if (hit && hit.transform.gameObject.tag == "Wall"){

                        if(debug){
                            Debug.Log("Wall hit with angle: " + angle + ", trying next angle.");
                            //Instantiate(testInstantiate,hit.point,transform.rotation);
                            }


                        continue;
                    } else{
                        if(debug){
                            Debug.Log("Found working angle: " + angle);
                            Debug.DrawRay(rb.position, direction, Color.green);
                        }
                        //return angle;
                        totalOfAnglesThatWork += angle;
                        amountOfAnglesThatWork++;
                    }
                }
        if(amountOfAnglesThatWork == 0){
            if(lastWorkingAngle > 0){   //Wenn man vor Wand steht, links oder rechts laufen
                return 90f;
            } else {
                return -90f;
            };
        }

        float finalAngle = totalOfAnglesThatWork / amountOfAnglesThatWork;
        lastWorkingAngle = finalAngle;

        return finalAngle;     //calculate average of all working angles
    }

    public void setMoveTowardsPoint(Vector2 target){
                    Vector2 direction = target - rb.position;
                    direction.Normalize();
                    movement = direction;
    }

    void FixedUpdate(){

        if(stopMovingAndStartDying){
            return;
        }

        //Movement
        Debug.DrawLine(transform.position, rb.position + movement, Color.blue, 0.05f);
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        if (meleeAttackCooldown > 0){
                meleeAttackCooldown -= 1;
            } else if(meleeAttackCooldown <= 0){
                meleeAttackReady = true;
            }
        
        updateMovementTimer -= 1;
        /* transform.position = Vector3.MoveTowards(rb.position, targetPos, moveSpeed); */
        checkBuffs();
    }

    
    void OnCollisionStay2D(Collision2D col){
        //Debug.Log("Test lul");
        if(col.gameObject.tag == "Player" && meleeAttackReady){
            GameObject other = col.gameObject;
            PlayerBehaviour pl = other.GetComponent<PlayerBehaviour>();
            pl.takeDamage(damage);
            meleeAttackReady = false;
            meleeAttackCooldown = 60;
        }
    }

    public void setMaxHealth(int newMaxHealth){
        maxHealth = newMaxHealth;
    }

    public void heal(int amount){
        if ((health + amount) >= maxHealth){
            health = maxHealth;
        } else {
            health += amount;
        }

    }
    public void takeDamage(int amount){
        StatCount.increaseTotalDamageDealt(amount);
        DamageNumberController.create(damageNumberPrefab, transform, amount);
        health -= amount;
    }

    public void exch(float[] a, int x, int y){
        float tmp = a[x];
        a[x] = a[y];
        a[y] = tmp;

    }

public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
   Vector3 dir = point - pivot; // get point direction relative to pivot
   dir = Quaternion.Euler(angles) * dir; // rotate it
   point = dir + pivot; // calculate rotated point
   return point; // return it
 }

 public void receiveSpeedBuff(){
     moveSpeed = buffedMoveSpeed;
     spr.color = Color.green;
 }
 public void checkBuffs(){
     if (moveSpeed > baseMoveSpeed){
         moveSpeed -= 0.01f;
         if (moveSpeed < baseMoveSpeed){moveSpeed = baseMoveSpeed;}
     }
 }

}
