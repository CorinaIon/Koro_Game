using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class GameManager_script : MonoBehaviour
{
    #region Singleton GameManager
    public static GameManager_script instanta;
    private void Awake()
    {
        instanta = this;
    }
    #endregion

    public GameObject NPC_parinte;
    public GameObject koro_group_parinte;
    public GameObject NPC_doctor_1;
    public List<GameObject> koroList; //sunt cei trei initiali
    public GameObject prefab_Koro;
    public Transform poz_start_koro;
    public float timpScadeViataAutomat = 20;
    public float timpAddKoroAutomat = 60;

    public int progres_level = 0;
    public Targets_Manager targets_Manager;

    public Transform centruTarc;
    public float razaTarc = 10f;
    public List<GameObject> tableList; //din inspector
    public bool solved = false;
    private float pragViata;  // este 60
    private float timpSchimbaAnimatia = 10;
    public bool stopKoroutine = true;
    public void Start()
    {
        NPC_parinte.SetActive(false);
        foreach (GameObject koro in koroList)
        {
            koro.SetActive(false);
        }
        //NPC_doctor_1.GetComponent<NPC_script>().isInteracting = true; // ca sa stea pe loc si sa faca FacePlayer();
        targets_Manager = Targets_Manager.instanta;
        StartCoroutine(ScadeViataTuturor());
        pragViata = Player_script.instanta.stats_player.pragBolnav;
    }

    public void ReLoadActiveScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReLoadMainScene()
    {
        SceneManager.LoadScene("main_scena_din_pachet");
    }

    IEnumerator ScadeViataTuturor()
    {
        yield return new WaitForSeconds(timpScadeViataAutomat);
        //for(Stats_script s in targets_Manager.stats_targets) //va da eroarea InvalidOperationException: Collection was modified; enumeration operation may not execute.
        for (int i = 0; i < targets_Manager.stats_targets.Count; i++) 
        {
            targets_Manager.stats_targets[i].TakeDamage(1);
        }
        StartCoroutine(ScadeViataTuturor());
    }

    public IEnumerator AddKoroAutomat()
    {   
        yield return new WaitForSeconds(timpAddKoroAutomat);
        if (stopKoroutine)
        {
            yield break;
        }
        timpAddKoroAutomat += 20; // sa apara din ce in ce mai incet
        var x = Instantiate(prefab_Koro, poz_start_koro.position, poz_start_koro.rotation);
        x.transform.parent = poz_start_koro;
        //+++sound
        StartCoroutine(AddKoroAutomat());
    }
 
    public void KoroVisible(int index)
    {
        koroList[index].SetActive(true);
        //+++sound
    }

    public void Generare_NPCs_si_1koro()
    {
        NPC_parinte.SetActive(true);

    }

    public IEnumerator LevelProgres()
    {
        progres_level++;
        yield return new WaitForSeconds(1);//ca sa nu porneasca imediat quest-ul
        switch (progres_level)
        {
            case 1:
                {
                    //1: daca a citit mesajul de la doctor_1
                    KoroVisible(0);
                    NPC_parinte.SetActive(true); //ii face pe toti vizibili
                    Quest_Manager.instanta.AddQuest(1);
                    break;

                }
            case 2:
                {
                    for (int i = 1; i < koroList.Count; i++)
                    {
                        KoroVisible(i);
                    }
                    Quest_Manager.instanta.AddQuest(3);
                    break;
                }
            case 3:
                {
                    Quest_Manager.instanta.AddQuest(4);
                    koro_group_parinte.SetActive(true);
                    stopKoroutine = false;
                    StartCoroutine(AddKoroAutomat());
                    break;
                }
        }
    }

    public GameObject esteMasaLibera()
    {

        foreach (GameObject t in tableList)
        {
            if (t.tag == "Liber")
            {
                Debug.Log("am gasit una libera");
                return t;
            }
        }
        return null;
    }

    public bool CheckFinish()
    {
        if(!solved)
        {
            return false;
        }
        foreach(Stats_script stats in Targets_Manager.instanta.stats_targets)
        {
            if (stats.viata < pragViata)
            {
                return false;
            }
        }
        Debug.Log("HNY");

        DialogTrigger_script d = this.GetComponent<DialogTrigger_script>();
        string[] s = new string[1];
        s[0] = "You won!";
        d.dialog.nume_personaj = "Congratulations!!!!";
        d.dialog.sentences = s;
        d.TriggerDialog();
        //dar nu dau end game, poate se mai distreaza..
        //ar trebui sa aiba exit si continu
        //sa se porneasca meniul.
        return true;
    }

    public void EndGame()
    {
        Debug.Log("Game over!");
        GameObject.Find("shooter_GameManager").GetComponent<AudioSource>().enabled = true;
        GameObject.Find("shooter_GameManager").GetComponent<AudioSource>().loop = true;
        Player_script.instanta.animator.SetTrigger("gameOver");
        Player_script.instanta.GetComponent<Player_script>().enabled = false;
        Player_script.instanta.GetComponent<NavMeshAgent>().enabled = false;
        Player_script.instanta.GetComponent<Stats_script>().nuModificaViata = true;
        if (stopKoroutine) //merge si in cazul in care nu fusese pornita niciodata.. 
        {
            Debug.Log("era true");
            stopKoroutine = false; // sa reporneasca >:)
            StartCoroutine(AddKoroAutomat());
        }
        foreach (Koro_script koro in Transform.FindObjectsOfType<Koro_script>())
        {
            koro.gameObject.GetComponent<Koro_script>().SetFinalDestination();
            koro.gameObject.GetComponent<Koro_script>().enabled = false;
        }
        koro_group_parinte.SetActive(true);
        //animatie speciala cu zombie daca descoperise existenta lor
        ZombieCalling();
        StartCoroutine(TransformAllZombie());
        //afisare ecran de GameOver
        //afisare info pentru restart
    }

    void ZombieCalling()
    {
        foreach (Zombie_script zombie in Transform.FindObjectsOfType<Zombie_script>())
        {
            StartCoroutine(SchimbaAnimatiaDans(zombie.gameObject.GetComponentInChildren<Animator>()));
            zombie.gameObject.GetComponent<Zombie_script>().SetFinalDestination();
            zombie.gameObject.GetComponent<Zombie_script>().enabled = false;
        }
    }

    IEnumerator TransformAllZombie()
    {
        // toti ceilalti se vor transforma in zombie
        for (int i = 0; i < Targets_Manager.instanta.stats_targets.Count; i++)
        {
            if (Targets_Manager.instanta.stats_targets[i].viata > 1) //adica nu playerul
            {
                Targets_Manager.instanta.stats_targets[i].viata = 30;
                Targets_Manager.instanta.stats_targets[i].TakeDamage(3); //viata 27, bolnav
                yield return new WaitForSeconds(3);
                Targets_Manager.instanta.stats_targets[i].TakeDamage(3);
                yield return new WaitForSeconds(1);

            }
        }
        ZombieCalling();
    }

    IEnumerator SchimbaAnimatiaDans(Animator animator)
    {
        animator.SetInteger("danceIndex", Random.Range(0, 5));
        animator.SetTrigger("dance");
        yield return new WaitForSeconds(timpSchimbaAnimatia);
        StartCoroutine(SchimbaAnimatiaDans(animator));
    }

    public void PlayPauseMusic()
    {
        bool loop = !GameObject.Find("shooter_GameManager").GetComponent<AudioSource>().loop;
        if (loop)
        {
            GameObject.Find("shooter_GameManager").GetComponent<AudioSource>().Play();
        }
        GameObject.Find("shooter_GameManager").GetComponent<AudioSource>().loop = loop;
        
    }
}
