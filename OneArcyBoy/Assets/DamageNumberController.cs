using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumberController : MonoBehaviour
{

    //Class variables
    public static int colorThreshold = 50;
    public static Color32[] damageColors = new Color32[] {Color.white, Color.yellow, new Color32(245,107,33,255), Color.red, new Color32(245,29,215,255), new Color32(43,18,245,255)};
    public static int minFontSize = 5;
    public static int maxFontSize = 20;
    public static int maxExpectedDamage = 300;
    public static float timeToLive = 1.2f;

    //Object variables
    [Range(0, 300)]
    public int damage;
    private TextMeshPro UIdamage;
    private Transform tr;
    private Rigidbody2D rb;
    private float baseY;
    private Vector3 basePos;
    public bool setupFinished = false;

    public static void create(GameObject prefab, Transform tr, int dmg){
        GameObject g = Instantiate(prefab, tr.position, tr.rotation);
        g.GetComponent<DamageNumberController>().setup(dmg);
        g.SetActive(true);
    }

    public static void createTextPopup(Transform tr, string text){
        GameObject prefab = Resources.Load<GameObject>("DamageNumber");
        GameObject g = Instantiate(prefab, tr.position, tr.rotation);
        g.GetComponent<DamageNumberController>().setupTextpopup(text);
        g.SetActive(true); 
    }

    public void setup(int dmg){
        //--- Setup Damage value
        damage = dmg;

        //--- Find necessary gameObjects ---
        UIdamage = gameObject.GetComponent<TextMeshPro>();
        tr = gameObject.GetComponent<Transform>();
        rb = gameObject.GetComponent<Rigidbody2D>();

        //--- Set Base Height and base Position
        baseY = tr.position.y;
        basePos = tr.position;

        //--- Set Color
        Color32 c = calculateColorFromDamage(damage);
        UIdamage.faceColor = c;

        //--- Set FontSize
        float fontSize = calculateFontSizeFromDamage(damage);
        UIdamage.fontSize = fontSize;

        //--- Set Text
        UIdamage.SetText(damage.ToString());

        Destroy(gameObject, timeToLive);

        setupFinished = true;       //enables Start Method to continue and Update to be called
    }

    public void setupTextpopup(string txt){
        //--- Find necessary gameObjects ---
        UIdamage = gameObject.GetComponent<TextMeshPro>();
        tr = gameObject.GetComponent<Transform>();
        rb = gameObject.GetComponent<Rigidbody2D>();

        //--- Set Base Height and base Position
        baseY = tr.position.y;
        basePos = tr.position;

        //--- Set Color
        //Color32 c = calculateColorFromDamage(damage);
        //UIdamage.faceColor = new Color32(255, 0, 0, 0);

        //--- Set FontSize
        float fontSize = calculateFontSizeFromDamage(300);
        UIdamage.fontSize = fontSize;

        //--- Set Text
        UIdamage.SetText(txt);

        Destroy(gameObject, timeToLive);

        setupFinished = true;       //enables Start Method to continue and Update to be called
    }

    public Color32 calculateColorFromDamage(int dmg){
        float lerpFactor;
        float damagef = (float) damage;
        int firstColorIndex = damage / colorThreshold;                        //How often has the damage surpassed the color treshhold?
        firstColorIndex = Mathf.Min(firstColorIndex,damageColors.Length-2);   //But it must still fit as index in damageColor array
        damagef -= (DamageNumberController.colorThreshold * firstColorIndex); //Subtract the colorThreshold that many times from the float damage, to get the correct lerpFactor
        lerpFactor = damagef / colorThreshold;

        return Color32.Lerp(damageColors[firstColorIndex], damageColors[firstColorIndex+1], lerpFactor);
    }

    public float calculateFontSizeFromDamage(int dmg){
        float growthRate = (float)(maxFontSize - minFontSize) / (float) maxExpectedDamage;
        float growth = (float) dmg * growthRate;
        float fontSize = minFontSize + growth;

        return Mathf.Min(fontSize,maxFontSize);
    }
    
    IEnumerator Start(){
        rb = gameObject.GetComponent<Rigidbody2D>();
        while(!setupFinished){
            yield return new WaitForSeconds(0.1f);
        }

        rb.velocity = new Vector2(Random.Range(-10f,10f), 20);
        Debug.Log(rb.velocity);
    }

    // Update is called once per frame
    void Update()
    {
        //Text Color:
        //UIdamage.faceColor = calculateColorFromDamage(damage);
        //Text Damage
        //UIdamage.SetText(damage.ToString());
        //FontSize
        //float fontSize = calculateFontSizeFromDamage(damage);
        //UIdamage.fontSize = fontSize;

        if(transform.position.y < baseY){
            transform.position = new Vector3(transform.position.x, baseY, transform.position.z);
            //Debug.Log(rb.velocity);
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Abs(rb.velocity.y)); 
            //Debug.Log(rb.velocity);
        }

    }

    /* void FixedUpdate(){
        if(animationDelay <= 0){
            animationDelay = 180;
            transform.position = basePos;
            
        }
        animationDelay -= 1;
    } */
}
