using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public bool eActiva;
    public string titlu;
    public string titlu_buton;
    //public string description;
    public string[] reward_text;
    public int rewardTip;
    public QuestGoal goal = new QuestGoal(); //chiar nu merge fara..

    /* 0 - reward sub forma de text ;)
     * 1 - viteza lui creste
     * 2 - porneste imediat quest nou
     * 3 - nimic sau la final de joc
     * */
/*
    public Quest()
    {
        eActiva = false;
        titlu = "";
        reward_text = "";
        rewardTip = 0;
    }
    
    public Quest(bool e, string t, string d, string r)
    {
        eActiva = e;
        titlu = t;
        description = d;
        reward = r;
    }*/

    public void afisare()
    {
        Debug.Log(eActiva + titlu);
    }

}
