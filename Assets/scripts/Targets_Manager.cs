using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targets_Manager : MonoBehaviour
{
    #region Singleton Targets_Manager
    public static Targets_Manager instanta;
    private void Awake()
    {
        instanta = this;
    }
    #endregion

    public List<Transform> targets; //targetul poate sa fie Playerul sau un NPC. Dar pe player il voi verifica altfel cred.. Sa mai modific comentariul asta si sa decid odataaaa. Cred ca pe player il voi verifica direct in Player_script.cs
    public List<Stats_script> stats_targets; 
    void Start()
    {
        // targets = new List<Transform>();  // aparent chiar nu trebuie si linia asta, strica tot.. :'(
        targets.Add(Player_script.instanta.transform);
        stats_targets.Add(Player_script.instanta.stats_player);
    }

    //prin functia asta NPC-ii se adauga singuri dupa ce s-au creat.
    public void AddTargetAndStats(Transform newTarget, Stats_script newStats)
    {
        targets.Add(newTarget);
        stats_targets.Add(newStats);
    }

    public void AddTarget(Transform newTarget)
    {
        targets.Add(newTarget);
    }

    public void RemoveTarget(Transform tf)
    {
        targets.Remove(tf);
        //il mai las pe stats_targets ca sa ii scada automat viata in continuare
    }

    public void RemoveStats(Stats_script stat)
    {
        stats_targets.Remove(stat);
    }


}
