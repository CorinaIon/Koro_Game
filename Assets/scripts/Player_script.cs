using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

//[RequireComponent(typeof(NavMeshAgent))]
public class Player_script : MonoBehaviour
{
    #region Singleton Player
    public static Player_script instanta;
    private void Awake()
    {
        instanta = this;
    }
    #endregion

    public Camera cam;
    public LayerMask layerClick;
    public float clickRange = 100f;
    
    private Transform target;

    //e in Start initializat
    public Animator animator; //ca sa il pot accesa si din koro_obj_sript

    public NavMeshAgent agent;

    //inutile
    public Obiect_script focus;
    public float speedSlerp = 5f;

    public Stats_script stats_player; //aici are viata, TakeDamage, Die si AdaugaViata

    public GameObject obiect_pt_ATAC;
    public int damage_ATAC;
    private Echipare_Manager echipare_Manager;

    public bool okInteract = true;
    public float angleOk = 40f; //e ok daca e la 40 de grade, poat interactiona cu obiectele.

    Inventory_script inventory_Script;
    public bool stop_imd = false;
    public bool stopComplet= false;
    public ParticleSystem muzzle;
    public Vector3 offset_cazut;
    public int validOffset = 0;
    public GameObject cazutDoctor;

    public Items_script potiuneTrisat;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        stats_player = GetComponent<Stats_script>();
        echipare_Manager = Echipare_Manager.instanta;
        echipare_Manager.onEquipmentChangedCallback += ActualizareObiectATAC;
        inventory_Script = Inventory_script.instanta;
    }

    void IRVA()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(GameManager_script.instanta.LevelProgres());
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(GameManager_script.instanta.LevelProgres());
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            transform.localPosition = new Vector3(-52.0077515f, 8.0774641f, -63.5340118f); //localPosition, nu position simplu
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            transform.localPosition = new Vector3(-67.3926239f, 14.8451786f, 144.360275f);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            transform.localPosition = new Vector3(-62.1851959f, 35.1015396f, 197.518539f);
        }
        

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            transform.localPosition = new Vector3(-227.795425f, 17.808506f, -96.6941833f);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            if (agent.speed == 20)
            {
                agent.speed = 5;
            }
            else
            {
                agent.speed = 20;
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (focus != null && focus.tag == "NPC")
            {
                focus.GetComponent<Stats_script>().TakeDamage(30);
            }
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (focus!=null && focus.tag == "NPC")
            {
                focus.GetComponent<Stats_script>().TakeDamage(3);
            }
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            for (int i = 0; i < Targets_Manager.instanta.stats_targets.Count; i++)
            {
                if (Targets_Manager.instanta.stats_targets[i] != null && Targets_Manager.instanta.stats_targets[i].gameObject.tag != "Player")
                {
                    transform.localPosition = Targets_Manager.instanta.stats_targets[0].gameObject.transform.localPosition;
                    return;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Inventory_script.instanta.Add(potiuneTrisat);
        }

        if(Input.GetKeyDown(KeyCode.Keypad0))
        {
            cazutDoctor.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            stats_player.AdaugaViata(10);
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            stats_player.TakeDamage(10);
        }
    }

    void Update()
    {
        IRVA();

        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);  //creaza o raza printr-un punct de pe ecran
            if (Physics.Raycast(ray, out RaycastHit hit, clickRange, layerClick))
            {
                if (hit.transform.tag == "NPC" || hit.transform.tag == "Koro")
                {
                    Stats_script stats = hit.transform.GetComponentInParent<Stats_script>();
                    StartCoroutine(stats.Corutina_activ_bar()); // daca nu mai trec din nou cu mouse pe deasupra se va face bara invizibila dupa 0.5secunde
                }
            }
        }

        if (EventSystem.current.IsPointerOverGameObject()) //este peste UI
            return;

        if (stopComplet)
            return;

        if (Input.GetMouseButtonDown(0))  //la apasarea click stanga
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);  //creaza o raza printr-un punct de pe ecran
            if (Physics.Raycast(ray, out RaycastHit hit, clickRange, layerClick))
            {
                Debug.Log("hit cu " + hit.collider.name + " " + hit.point);
                Move_catre_punct(hit.point);
                //inutil
                RemoveFocus();
            }
        }

        if (Input.GetMouseButtonDown(1)) // click dreapta
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);  //creaza o raza printr-un punct de pe ecran
            if (Physics.Raycast(ray, out RaycastHit hit, clickRange, layerClick))
            {
                //eu de obicei verificam tag-ul sa fie pickup de ex, dar acum o sa incerc varianta cu null
                Obiect_script obiect = hit.collider.GetComponent<Obiect_script>();
                if (obiect != null)  //in loc sa verific tagul
                {
                    if (obiect_pt_ATAC!=null &&  obiect == /*(Obiect_script)*/ obiect_pt_ATAC.GetComponentInChildren<ItemPickup_script>())
                    {
                        Debug.Log("A dat click pe obiectul din mana. Il dezechipam");
                        echipare_Manager.Dezechipare((int)TipEchipament.ATAC);
                    }
                    else
                    {
                        SetFocus(obiect);
                    }
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            EchipareDezechipareRapida("Potiune vindecatoare");
        }

        if (Input.GetKeyDown(KeyCode.G))
        {

            EchipareDezechipareRapida("Launcher");
        }

        //as putea sa fac si pentru mancare

        /*
        if(target != null)
        {
            agent.SetDestination(target.position);
            FaceTarget();
        }*/

        //FollowTarget();
        AnimatieDeplasare();
    }

    //--- Functii de miscare & focus ------------------------------------

    void Move_catre_punct(Vector3 punct)
    {
        agent.SetDestination(punct); // daca las doar linia asta, fara rotatie se intoarce playerul cu fata la mine mereu, ughhh
    }

    //inutil
    void SetFocus(Obiect_script newfocus)
    {
        if (newfocus != focus) //obiectul pe care s-a dat click este altul decat cel curent
        {
            if (focus != null)
            {
                focus.DeFocusat(); //cel vechi nu va mai fi focus
            }    
            focus = newfocus;
            //nu il pune sa se miste aici pentru ca nu s-ar mai misca si atunci cand obiectul (cu focus) se misca. de exemplu un oponent.
            //hmm, a facut functiile cu Follow, mare branza. La el sunt in alt script si au update-ul lor, mare diferenta... not really
            target = newfocus.transform;
            StartCoroutine(FollowTarget(newfocus));
        }
        newfocus.Focusat(transform);
        okInteract = true;
    }
    //inutil
    public void RemoveFocus()
    {
        if (focus != null)
        {
            focus.DeFocusat();
        }
        focus = null;
        StopFollowTarget();
    }

    IEnumerator FollowTarget(Obiect_script newtarget)
    {
        if (animator.GetBool("tine_arma") && obiect_pt_ATAC.GetComponentInChildren<ItemPickup_script>().item.name == "Launcher")
        {
            agent.stoppingDistance = newtarget.raza_pick_departare;
        }
        else
        {
            agent.stoppingDistance = newtarget.raza_pick;
        }
        target = newtarget.transform;   //sau interaction_transform aka pozitia usii cufarului
                                        //pentru ca nu facea bine rotatia NavMeshAgentul atunci cand era prea aproape de punctul target din SetDestination
        agent.updateRotation = false;
        while (target != null && target==newtarget.transform) //conditia inseamna ca nu s-a modificat target-ul intre timp. Puteam sa folosesc si esteFocus sau ceva similar.
        {
            //aveam asa initial while (target != null ) si agent. si target= in {}
            // Debug.Log("ii voi da stopdistance " + newtarget.raza_pick + " momentan este " + agent.stoppingDistance);
            //da, problema era ca dadea  focus pe cubul infinit, apoi schimbam targetul fara sa se faca null intre timp ca sa iasa din IENUMERATORUL curent.  
            //Debug.Log("facem targetul " + newtarget.name); //asa am vazut ca erau ambele corutine pornite.
            //sol: mutat liniile in afara. Functioneaza pentru ca Corutina pick-upului celuilalt ar modifica target. Ambelee ar face SetDistance catre acelasi punct si apoi target null & stop ambele.
            //daaar, daca as avea 2 cuburi infinite, raman ambele corutine active pana dau click pe pamant. in loc de doar o corutina
            //deci punem si conditie suplimntara in while, nu doar target != null. Am adaugat deci target==newtarget.transform
            agent.SetDestination(target.position + validOffset * offset_cazut);
            FaceTarget();////
            if (stop_imd)
            {
                stop_imd = false;
                Player_script.instanta.agent.SetDestination(transform.position);
                StopFollowTarget();
                yield return 0;
            }
            yield return null; //reia fix de dupa linia asta, nu? adica de la bucla while, nu de la inceputul corutinei, okok.
        
        }
        if(target==null)
            StopFollowTarget();
        //else inseamna ca incheiase while ul pentru ca fusese selectat alt obiect. Nu facem null si 0 proprietatile agentului.
        yield return 0;
    }


    public void StopFollowTarget()
    {
        agent.stoppingDistance = 0f;
        agent.updateRotation = true;
        target = null;
    }

    public void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction = new Vector3(direction.x, 0f, direction.z);
        if (direction != Vector3.zero) //pentru ca aveam Look rotation viewing vector is zero; UnityEngine.Quaternion:LookRotation(UnityEngine.Vector3)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speedSlerp);
        }
    }

    public bool esteOrientatOk() //docs unity <3
    {
        Vector3 targetDir = target.position - transform.position;
        float angle = Vector3.Angle(targetDir, transform.forward);
        if (angle < angleOk || animator.GetBool("cara"))  // ca sa poata sa il lase jos oricand, nu conteaza orientarea.
            return true;
        return false;

    }
    void AnimatieDeplasare()
    {
        float speed = 0;
        if (agent.speed != 0)
        {
            speed = agent.velocity.magnitude / agent.speed;  //viteza curenta/viteza_max da o val intre 0 si 1 pentru Blend Tree
        }
        animator.SetFloat("speedBlend", speed, .1f, Time.deltaTime); // va dura 1/10 sec sa faca smooth inte doua val
    }

    public void ActualizareObiectATAC(Echipament_script newEchipament, int poz)
    {
        if(newEchipament!=null) //a echipat ceva nou de ex: a luat o arma de atac noua in mana
        {
            obiect_pt_ATAC = echipare_Manager.Get_GO_poz(poz);
            damage_ATAC = newEchipament.damage;
        }
        else // a dezechipat
        {
            obiect_pt_ATAC = null;
            damage_ATAC = 0;
        }
    }
   
    public void VinoLangaNPC(float razaHereNPC)
    {
        agent.stoppingDistance = razaHereNPC;
    }

    public void VinoLangaNPCCazut(Vector3 offset)
    {
        offset_cazut = offset;
        // In FollowTarget se va calcula destinatia agentului cu offset-ul dorit, astfel incat sa stea in dreapta NPC-ului pentru a-l ridica
        agent.stoppingDistance = 0;
    }

    public void VinoLangaMasa(Transform tfMasa)
    {
        target = tfMasa;
        agent.SetDestination(tfMasa.position);
    }

    void EchipareDezechipareRapida(string obiectNume) //Potiunee pentru F, Arma pentru G
    {
        if (obiect_pt_ATAC != null) //daca avea deja ceva in mana, o dezechipeaza (arma sau  potiunea)
        {
            if (obiect_pt_ATAC.GetComponentInChildren<ItemPickup_script>().item.name != obiectNume) //are in mana alt tip de obiect. Il inlocuim cu cel dorit din inventar
            {
                //echipare_Manager.Dezechipare((int)TipEchipament.ATAC);
                //il vaa dezechipa si pe cel vechi, nu?
                inventory_Script.PlayerCere(obiectNume);
            }
            else //are in mana echipamentul. Apasa pe tasta, deci vrea sa il dezechipeze
            {
                echipare_Manager.Dezechipare((int)TipEchipament.ATAC);
            }
        }
        else
        {
            inventory_Script.PlayerCere(obiectNume);
        }
    }

}