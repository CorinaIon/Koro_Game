using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestGoal
{
    public TIP_QuestGoal tipGoal;
    public int nr_required;
    public int nr_curent = 0;
    public bool finalizat = false;

    public bool eIndeplinit()
    {
        return (nr_curent >= nr_required);
    }

    public void Add_nr_curent(int nr)
    {
        nr_curent += nr;
        Quest_Manager.instanta.UpdateQuestWindow();
        if(eIndeplinit())
        {
            finalizat = true;
            Quest_Manager.instanta.QuestFinalizat();
        }
    }



}

public enum TIP_QuestGoal { Kill, Gather, Reach, Heal}