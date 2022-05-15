using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatCount : MonoBehaviour
{
    
    public static StatCount instance;
    private static int totalPoints;
    private static int enemiesKilled;
    private static int timesArcCasted;
    private static int totalDamageDealt;
    private void Awake(){
        if(instance != null){
            Destroy(gameObject);
        }
        instance = this;

        totalPoints = 0;
        enemiesKilled = 0;
        timesArcCasted = 0;
        totalDamageDealt = 0;

    }

    public static void increasePoints(int amount){
        totalPoints += amount;
    }

    public static void increaseEnemiesKilled(){
        enemiesKilled += 1;
    }

    public static void increaseTimesArcCasted(){
        timesArcCasted += 1;
    }

    public static void increaseTotalDamageDealt(int amount){
        totalDamageDealt += amount;
    }

    public static string getStats(){
        string s = "Total Points: "         + "\t" + totalPoints      + "\r\n";
        s       += "Total Enemies killed: " + "\t" + enemiesKilled    + "\r\n";
        s       += "Total Casts of Arc: "   + "\t" + timesArcCasted   + "\r\n";
        s       += "Total Damge Dealt: "    + "\t" + totalDamageDealt + "\r\n";
        return s;
    }
}
