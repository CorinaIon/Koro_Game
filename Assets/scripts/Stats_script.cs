using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Stats_script : MonoBehaviour
{
    public int viata;
    public int maxViata = 100;
    private Image healthImage;
    private GameObject canvas; //e gameO casa aiba SetActive
    private int stare_bar = 0; // 0 daca e inactiva, >1 daca e activa (cate frame-uri sta utilizatorul cu crosshair-ul pe oponent) 
    public float timpMinBara = 0.5f;
    //tot aici se  actualizeaeza si bara. Personajele isi pierd din viata datorita aerului toxic la x secunde, 
    //nu am vrut sa afisez bara la fiecare modificare, doar cand playerul are mouse-ul peste. //Eventual pot sa adaug un StartCoroutine si in TakeDamage pentru Damage-urile mari. cu timpMin mai mare, nu doar 0.5s,
    Transform camTransform;
    public float pragBolnav = 60;
    public float pragFoarteBolnav = 30;
    public float pragZombie = 26;
    public float viataDupaReinviere = 80;
    private Animator animatorul;
    private NavMeshAgent agentul;
    public GameObject prefab_zombie;
    public float timpDamage = 2.1f;
    public bool prima_oara_Foarte_bolnav = false;
    public bool nuModificaViata = false;
    public GameObject doctorInainteDeZombie;


    void Start()
    {
        viata = maxViata;
        if (this.tag != "Player")
        {
            healthImage = transform.Find("canvas_health/HealthBar/FillImageHealth").GetComponent<Image>();
            canvas = transform.Find("canvas_health/HealthBar").gameObject;
            canvas.SetActive(false);
            camTransform = Player_script.instanta.cam.transform;
        }
        else
        {
            //il iau direct de pe canavas-ul mare
            canvas = GameObject.Find("PlayerHealth");
            healthImage = canvas.transform.Find("FillImageHealth").GetComponent<Image>();
        }
        //if(this.tag != "Koro") //doar momentan, imediat il animez si pe el.
        animatorul = this.GetComponentInChildren<Animator>();
        agentul = this.GetComponentInChildren<NavMeshAgent>();
    }

    public void TakeDamage(int damage)
    {
        if (nuModificaViata)
        {
            return;
        }
        viata -= damage;
        //Debug.Log(transform.name + " pierde " + damage + " din viata");
        healthImage.fillAmount = (float)viata / maxViata;
        //aici s-ar adauga StartCoroutine pentru ceilalti.La player are mereu setactive(true) si nu e nevoie sa actualizez

        if (this.tag != "Koro" && this.tag != "Zombie")
        {
            if (damage >= 2) //damage 1 este atunci cand le scade tuturor viata in timp.
            {
                animatorul.SetTrigger("doAnimatieDamage");
                StartCoroutine(OpresteAgentulCandIaDamage()); 
            }
            if (this.tag == "NPC")
            {
                if (viata < pragZombie)
                {
                    //metamorfoza
                    Transform tf = this.transform;
                    Targets_Manager.instanta.RemoveStats(this);
                    /////Destroy(this.gameObject);
                    gameObject.SetActive(false);
                    GameObject z = Instantiate(prefab_zombie, tf.position, tf.rotation);
                    z.GetComponent<Stats_script>().doctorInainteDeZombie = gameObject;
                    return; //nu o sa mai faca if-ul cu Die
                }
                else if (viata < pragFoarteBolnav)
                {
                    if (prima_oara_Foarte_bolnav)
                    {
                        Targets_Manager.instanta.RemoveTarget(this.transform);
                        //StartCoroutine(SchimbaCollider(true, 3.2f));
                        animatorul.SetTrigger("foarteBolnav");
                        //this.GetComponent<NPC_script>().enabled = false; //nu am mai pus linia pentru ca voiam sa isi modifice pozitia in update dupa player.
                        prima_oara_Foarte_bolnav = false; // ca sa nu mai faca a doua oara
                        this.GetComponent<NavMeshAgent>().enabled = false;
                        this.GetComponent<NPC_Obiect_script>().este_cazut = true;
                    }
                }
                else if (viata < pragBolnav)
                {
                    animatorul.SetBool("esteBolnav", true);
                }
            }
        }
        if (viata <= 0)
        {
            Die();
            //daca vreau sa si afisez viata, trebuie sa pun un viata = 0 inainte
        }
    }
    IEnumerator OpresteAgentulCandIaDamage() //Poate se facea altfel.. Functie pentru player si NPC care opreste agentul pentru cateva secunde ca sa nu se mai deplaseze in timpul animatiei de damage petru ca are SetDestination inca activ din Update.  
    {

        if (this.tag == "NPC")
        {
            //bool b = this.GetComponent<NPC_script>().isInteracting;
            this.GetComponent<NPC_script>().isInteracting = true;
            yield return new WaitForSeconds(timpDamage);
            this.GetComponent<NPC_script>().isInteracting = false; //aici nu ar trebui false, ci ce era inainte.. gen b
        }
        else if (this.tag == "Player")
        {
            Debug.Log("stop complet!!!");
            this.GetComponent<Player_script>().stopComplet = true;
            yield return new WaitForSeconds(timpDamage);
            this.GetComponent<Player_script>().stopComplet = false; //same
        }

        //float viteza_before = agentul.speed;
        //agentul.speed = 0;
        //yield return new WaitForSeconds(2);
        //agentul.speed = viteza_before;



    }

    public void Die()
    {
        Debug.Log(transform.name + " died!");
        //daar vreeau sa fie personalizat, nu asa.. Treebuie derivat din nou, mmsiii..
        //chiar nu mai vreau inca 3 clase derivate din asta doar ca sa fac Player_Stats si Koro_stats si NPC_stats, am prea multe deja.. Sooo taguri <3
        if (gameObject.tag == "Koro")
        {
            animatorul.SetTrigger("a_murit");
            Destroy(gameObject, 3f);
            Quest_Manager.instanta.KoroMort();
        }
        else
        if (gameObject.tag == "Zombie")
        {
            animatorul.SetTrigger("a_murit");
            StartCoroutine(Reinvie());
        }
        else if (gameObject.tag == "NPC")
        {
            Debug.LogError("NPC nu moare niciodata!");
        }
        else if (gameObject.tag == "Player")
        {
            GameManager_script.instanta.EndGame();
        }

    }

    IEnumerator Reinvie()
    {
        if (Player_script.instanta.focus == this)
        {
            Player_script.instanta.RemoveFocus();
        }
        yield return new WaitForSeconds(3);
        doctorInainteDeZombie.transform.position = transform.position;
        Destroy(gameObject);
        doctorInainteDeZombie.SetActive(true);
        doctorInainteDeZombie.GetComponentInChildren<Animator>().SetTrigger("reinvie");
        Targets_Manager.instanta.AddTargetAndStats(doctorInainteDeZombie.transform, doctorInainteDeZombie.GetComponent<Stats_script>());
        doctorInainteDeZombie.GetComponent<Stats_script>().AdaugaViata((int)(viataDupaReinviere - pragZombie + 1)); //ca sa fi mereu la valoarea vDR cand reinvie
        
        //eventual inca un delay ca sa nu inceapa sa mearga direct cu SetDestination-ul
        //adica sa fie uninteractable pentru o vreme.
    }

    public void AdaugaViata(int v)
    {
        //cand manananca Playerul sau cand NPC primeste potiune
        viata += v;
        if (viata > maxViata) viata = maxViata;
        Debug.Log("viata + " + v + " => " + viata);
        healthImage.fillAmount = (float)viata / maxViata;

        //ar fi trebuit sa o umplu treptat..

        //sa pun si un damage>incrementul_ala cand o sa fie  in modul de vindecare ca sa nu fie mereu pe ecran bara
        if (this.tag != "Player")
        {
            //aici as putea sa ii pun si animatie. Playerul are in Mancare_script
            StartCoroutine(Corutina_activ_bar());
        }

        if (this.tag == "NPC" && viata > pragBolnav)
        {
            animatorul.SetBool("esteBolnav", false);  // se va schimba BlendTree-ul sau se va ridica de pe masa din Tarc.            
            StartCoroutine(RedevineSanatosNPC());
            if(v < viataDupaReinviere - pragZombie + 1)
            {
                Targets_Manager.instanta.AddTarget(this.transform);
            }
            prima_oara_Foarte_bolnav = true;
            nuModificaViata = false;
        }
    }

    public IEnumerator Corutina_activ_bar() //functia e apelata cand playerul are mouse-ul pe deasupra personajului. Dispare in timpMinBara secunde. 
    {
        stare_bar++;
        canvas.SetActive(true);
        canvas.transform.LookAt(camTransform.position);

        yield return new WaitForSeconds(timpMinBara);  // sa stea 0.5 secunde vizibila bara dupa ce a trecut cu crosshair pe deasupra
        stare_bar--;
        if (stare_bar == 0)  // daca NU mai sunt si alte corutine active/ adica NU a trecut crosshair din nou pe deasupra intre timp
        {
            canvas.SetActive(false);
        }
    }

    IEnumerator RedevineSanatosNPC()
    {
        yield return new WaitForSeconds(1/*2.5f*/);  // trebuie sa mai astepte pana se termina animatia de ridicare de pe masa, altfel are viteza prea mare de la SetDestination.


        this.GetComponent<NPC_Obiect_script>().inTarc = false;

        if (this.GetComponent<NPC_Obiect_script>().masaLibera != null)
        {
            this.GetComponent<NPC_Obiect_script>().masaLibera.tag = "Liber";
            this.GetComponent<NPC_Obiect_script>().masaLibera = null;
        }

        this.GetComponent<NavMeshAgent>().enabled = true;
        this.GetComponent<NavMeshAgent>().SetDestination(transform.position + transform.forward * 2); // ca sa sara un pic mai la dreapta pentru ca nu e animatia cum voiam
        yield return new WaitForSeconds(1);
        this.GetComponent<NPC_Obiect_script>().este_cazut = false; // se va relua imediat dupa Update-ul
    }

}
