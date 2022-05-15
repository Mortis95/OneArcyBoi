using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootyEnemy : EnemyBehaviour
{

    public float range;
    public bool currentlyShooting;
    public int shotDelay;
    public int currentShotDelay;
    public AudioSource shotSound;
    public bool inRange;
    public GameObject magicShotPrefab;
    private void Awake(){
        target = GameObject.FindGameObjectWithTag("Player");
        inRange = false;
        meleeAttackReady = false;
        
        anglesToCheck = new float[] {5f,-5f,10f, -10f, 15f,-15f, 20f, -20f, 25f,-25f, 30f, -30.0f};
    }
    void Update(){
        if(stopMovingAndStartDying){
            return;
        }
        if (health <= 0){
            GameManager.getInstance().getPoints(pointsWorth);
            startDying();
        }
        if (target != null && updateMovementTimer <= 0 && !currentlyShooting){
            Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
            bool directApproach = checkDirectApproach(targetRb,false);
            inRange = Vector2.Distance(targetRb.position,rb.position) <= range;

            if(directApproach && inRange){
                startShooting();
            } else if(directApproach){
                setMoveTowardsPoint(targetRb.position);
            } else {
                float angle = calculateWorkingAngleWithCircleCast(anglesToCheck, targetRb, false);
                setMoveTowardsPoint(RotatePointAroundPivot(targetRb.position,rb.position,new Vector3(0,0,angle)));
            }
            
        }
        if(currentShotDelay <= 0 && currentlyShooting){
            currentShotDelay = shotDelay;
            shootAtTarget(target);
            currentlyShooting = false;
        }
    }

     void FixedUpdate(){
        //Movement
        if(stopMovingAndStartDying){
            return;
        }

        if(!currentlyShooting){rb.MovePosition((Vector2) transform.position + movement * moveSpeed * Time.fixedDeltaTime);}

        if (meleeAttackCooldown > 0){
                meleeAttackCooldown -= 1;
            } else if(meleeAttackCooldown <= 0){
                meleeAttackReady = true;
            }
        
        if (currentlyShooting){
            currentShotDelay -= 1;
        }
    }

    public void startShooting(){
        currentlyShooting = true;
        currentShotDelay = shotDelay;
    }

    public void shootAtTarget(GameObject target){
        shotSound.Play();
        if(target != null){
            Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
            MagicShot.createNewShot(magicShotPrefab, transform, damage, targetRb.position);
        }
    }
}
