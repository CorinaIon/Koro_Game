using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quest_Manager : MonoBehaviour
{
    #region Singleton Quest_Manager
    public static Quest_Manager instanta;
    private void Awake()
    {
        instanta = this;
    }
    #endregion

    public List<Quest> questList;
    public GameObject butonulNotif;
    public GameObject fereastraQuest;
    public Text TitluQuest;
    //public Button buttonAccept;
    public GameObject fereastraListaQuesturi;
    public List<Button> buttonList; // in inspector!!
    public int indexQuestVizibil;
    public float timpQuest = 120;
    DialogTrigger_script dialog_quest;

    public bool quest2lansat = false;
    public bool quest3lansat = false;

    private void Start()
    {
        butonulNotif = Dialog_Manager.instanta.butonNotificare;
        questList = new List<Quest>();
        fereastraQuest.SetActive(false);
        fereastraListaQuesturi.SetActive(false);
        dialog_quest = this.GetComponent<DialogTrigger_script>();
        StartCoroutine(AddQuestLaTimp(timpQuest));
    }

    public void AddQuest(int index)
    {
        Quest newQuest = new Quest();
        switch(index)
        {
            case 1:
            {
                string[] s = new string[4];
                s[0] = "The evil was unleashed!";
                s[1] = "Find some weapons to protect yourself!";
                s[2] = "Don't forget! Right Click on an object to pick it!";
                s[3] = "GO!\nYour time is running out!";
                dialog_quest.dialog.nume_personaj = "New Quest!";
                dialog_quest.dialog.sentences = s;
                dialog_quest.TriggerDialog();

                newQuest.eActiva = false; //ca sa fi butonul vizibil
                newQuest.titlu = s[1];
                newQuest.titlu_buton = "Find weapons!";
                string[] s2 = new string[2];
                s2[0] = "Yay! That's a launcher. You can find it in your Inventory!\nPress 'I' to open the inventory.";
                s2[1] = "Click on the inventory slot or press 'G' to equip it!\nRight Click on an enemy to shoot. GoOoO!";
                newQuest.reward_text = s2;
                newQuest.rewardTip = 0;
                newQuest.goal.tipGoal = TIP_QuestGoal.Gather;
                newQuest.goal.nr_required = 1;

                questList.Add(newQuest);
                OpenQuestWindow(questList.Count-1);
                break;
            }
            case 2:
            {
                quest2lansat = true;
                string[] s = new string[1];
                s[0] = "The toxic air is making Dr. Max weaker by the second!\nFind something to eat!";
                dialog_quest.dialog.nume_personaj = "New Quest!";
                dialog_quest.dialog.sentences = s;
                dialog_quest.TriggerDialog();

                newQuest.eActiva = false; //ca sa fi butonul vizibil
                newQuest.titlu = "Find something to eat!";
                newQuest.titlu_buton = "Find food!";
                string[] s2 = new string[1];
                s2[0] = "Yay! That looks delicious!\n As a reward Dr. Max's speed increased!";
                newQuest.reward_text = s2;
                newQuest.rewardTip = 1;
                newQuest.goal.tipGoal = TIP_QuestGoal.Gather;
                newQuest.goal.nr_required = 3;

                questList.Add(newQuest);
                OpenQuestWindow(questList.Count - 1);
                break;
            }
            case 3:
            {
                quest3lansat = true;
                string[] s = new string[2];
                s[0] = "Uhh, that was close!";
                s[1] = "Kill at least 3 opponents!";
                dialog_quest.dialog.nume_personaj = "New Quest!";
                dialog_quest.dialog.sentences = s;
                dialog_quest.TriggerDialog();

                newQuest.eActiva = false; //ca sa fi butonul vizibil
                newQuest.titlu = s[1];
                newQuest.titlu_buton = "Kill opponents!";
                string[] s2 = new string[2];
                s2[0] = "Yay! You are doing great :)";
                s2[1] = "Now find the source of the infestation. Ask the other doctors, they might help you..";
                newQuest.reward_text = s2;
                newQuest.rewardTip = 2;
                newQuest.goal.tipGoal = TIP_QuestGoal.Kill;
                newQuest.goal.nr_required = 3;

                questList.Add(newQuest);
                OpenQuestWindow(questList.Count - 1);
                break;
            }
            case 4:
            {
                newQuest.eActiva = false; //ca sa fi butonul vizibil
                newQuest.titlu = "Find the source of the infestation"; 
                newQuest.titlu_buton = "Find the source!";
                string[] s2 = new string[2];
                s2[0] = "yay";
                s2[1] = "yay";
                newQuest.reward_text = s2;
                newQuest.rewardTip = 3;
                newQuest.goal.tipGoal = TIP_QuestGoal.Reach;
                newQuest.goal.nr_required = 1;

                questList.Add(newQuest);
                OpenQuestWindow(questList.Count - 1);
                break;
            }
            case 5:
                {
                    string[] s = new string[2];
                    s[0] = "You found the source!";
                    s[1] = "But.. There are still people that need to be healed!\nGo and save everyone!";
                    dialog_quest.dialog.nume_personaj = "Congrats!";
                    dialog_quest.dialog.sentences = s;
                    dialog_quest.TriggerDialog();

                    newQuest.eActiva = false; //ca sa fi butonul vizibil
                    newQuest.titlu = "Heal everyone that is sick";
                    newQuest.titlu_buton = "Heal everyone!";
                    string[] s2 = new string[1];
                    s2[0] = "";
                    newQuest.reward_text = s2;
                    newQuest.rewardTip = 3;
                    newQuest.goal.tipGoal = TIP_QuestGoal.Heal;
                    newQuest.goal.nr_required = Targets_Manager.instanta.stats_targets.Count - 1; ;

                    questList.Add(newQuest);
                    OpenQuestWindow(questList.Count - 1);
                    break;
                }
        }

    }

    IEnumerator AddQuestLaTimp(float timp)
    {
        yield return new WaitForSeconds(timp);
        AddQuest(2);
    }
    
    public void OpenQuestWindow(int index)
    {
        indexQuestVizibil = index;
        fereastraListaQuesturi.SetActive(false);//sa se inchida cealalta
        //animatie!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        if (questList[index] != null)
        {
            fereastraQuest.SetActive(true);
            TitluQuest.text = questList[index].titlu;
/*            if (!questList[index].eActiva) //nu a fost acceptata inca
            {
                buttonAccept.GetComponentInChildren<Text>().text = "Accept";
            }
            else
            {
                buttonAccept.GetComponentInChildren<Text>().text = "OK";
            }*/
        }
    }
    public void CloseQuestWindow()
    {
        indexQuestVizibil = -1;
        fereastraQuest.SetActive(false);
        //animatie!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    }


    public void OnAcceptPressed()
    {
        questList[indexQuestVizibil].eActiva = true;
        CloseQuestWindow();
    }

    public void OnRejectPressed()
    {
        questList.RemoveAt(indexQuestVizibil);
        CloseQuestWindow();
    }

    public void OnOpenQueListPressed()
    {
        fereastraListaQuesturi.SetActive(!fereastraListaQuesturi.activeSelf);
        fereastraQuest.SetActive(false); //cealalta saa se inchida
                                         //sa fac asta dinamic, nu asa..

        UpdateQuestWindow();

    }

    public void UpdateQuestWindow()
    {
        if (fereastraListaQuesturi.activeSelf == true)
        {
            int poz = 0;
            foreach (Quest q in questList)
            {
                if (poz < buttonList.Count)
                {
                    string textul = q.titlu_buton + " [" + q.goal.nr_curent + "/" + q.goal.nr_required + "]";
                    buttonList[poz].GetComponentInChildren<Text>().text = textul;
                    buttonList[poz].gameObject.SetActive(true);
                    poz++;
                }
                else
                {
                    return;//nu mai sunt butoane de facut vizibile
                }
            }
            while (poz < buttonList.Count)
            {
                buttonList[poz].gameObject.SetActive(false);
                poz++;
            }
        }
    }

    public void QuestFinalizat()
    {
        fereastraListaQuesturi.SetActive(false);
        fereastraQuest.SetActive(false);
        //o cautam mai intai, pentru ca nu aveam cum sa trimit din QuestGoal
        for (int i = 0; i < questList.Count; i++)
        {
            if(questList[i].goal.finalizat)
            {
                dialog_quest.dialog.nume_personaj = "Quest \"" + questList[i].titlu_buton + "\" completed";
                dialog_quest.dialog.sentences = questList[i].reward_text;

                int tip = questList[i].rewardTip;
                questList.RemoveAt(i);
                switch (tip)
                {
                    case 0:
                        {
                            break;
                        }
                    case 1:
                        {
                            Player_script.instanta.agent.speed += 5f;
                            break;
                        }
                    case 2:
                        {
                            StartCoroutine(GameManager_script.instanta.LevelProgres());
                            break;
                        }
                }
                dialog_quest.TriggerDialog();
            }
        }
    }

    public Quest FindQuestTitlu(string titlu)
    {
        foreach (Quest q in questList)
        {
            if(q.titlu_buton==titlu)
            {
                return q;
            }
        }
        return null;
    }

    public void KoroMort()
    {
        if (quest3lansat)
        {
            Quest q = FindQuestTitlu("Kill opponents!");
            if (q != null)
            {
                q.goal.Add_nr_curent(1);
            }
        }
        else //prima oara cand omoara unul
        {
            StartCoroutine(GameManager_script.instanta.LevelProgres());
        }
    }
}
