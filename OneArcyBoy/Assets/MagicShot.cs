using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicShot : MonoBehaviour
{
    public static GameObject createNewShot(GameObject magicShotPrefab, Transform tr, int damage, Vector2 targetPos){
        GameObject g = Instantiate(magicShotPrefab,tr.position,tr.rotation);
        MagicShot m = g.GetComponent<MagicShot>();
        m.setup(damage,targetPos);
        return g;
    }

    public int damage;
    public int shotSpeed;
    public Vector2 targetPosition;
    private bool setupFinished=false;
    public Vector2 direction;
    public Rigidbody2D rb;

    public void setup(int dmg,Vector2 targetPos){
        this.damage = dmg;
        direction = targetPos - rb.position;
        direction.Normalize();
        setupFinished = true;
    }

    IEnumerator Start(){
        while(!setupFinished){
            yield return new WaitForSeconds(0.05f);
        }
        
    }
    void OnTriggerEnter2D(Collider2D col){
        if(col.gameObject.tag == "Player"){
            PlayerBehaviour pl = col.gameObject.GetComponent<PlayerBehaviour>();
            pl.takeDamage(damage);
        } else if(col.gameObject.tag == "Enemy"){
            EnemyBehaviour e = col.gameObject.GetComponent<EnemyBehaviour>();
            e.receiveSpeedBuff();
            return;
        } else {Destroy(gameObject);}
        Destroy(gameObject);
    }
    void FixedUpdate(){
        rb.MovePosition(rb.position + direction * shotSpeed * Time.fixedDeltaTime);

    }
}
