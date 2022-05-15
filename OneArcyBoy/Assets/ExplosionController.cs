using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{

    private int timeToLive=60;
    private int dmg;
    private bool maxSizeReached=false;
    private float maxScale;
    private float growthRate;

    private void Awake(){
        PlayerBehaviour player = PlayerBehaviour.getInstance();
        dmg = player.ARCdmg;
        growthRate = 0.2f;
        if(player.biggerARCexplosion){
            maxScale = 5f;
        } else {
            maxScale = 2.5f;
        }
        AudioSource[] explosionsounds = gameObject.GetComponents<AudioSource>();
        float r = Random.Range(0.0f,1.0f);
        if (r < 0.5){
            explosionsounds[0].Play();
        } else {
            explosionsounds[1].Play();
        }
    }
    void FixedUpdate()
    {
        if (!maxSizeReached){
            transform.localScale += new Vector3(growthRate, growthRate, 1);
            float currentX = transform.localScale.x;
            float currentY = transform.localScale.y;
            if (currentX >= maxScale || currentY >= maxScale){
                maxSizeReached = true;
            }
        }
        if(timeToLive<=0 && maxSizeReached){
            float currentX = transform.localScale.x;
            if(currentX > 0){
                transform.localScale -= new Vector3(growthRate,growthRate,0);
            } else {
                Destroy(gameObject);
            }
        }
        if(timeToLive <= -60){
            Destroy(gameObject);
        }
        timeToLive -= 1;
    }

    void OnTriggerEnter2D(Collider2D col){
        Debug.Log("Test lul");
        if(col.gameObject.tag == "Enemy"){
            GameObject other = col.gameObject;
            EnemyBehaviour enemyScript = other.GetComponent<EnemyBehaviour>();
            enemyScript.takeDamage(dmg);
        }
    }




}
