using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obiect_script : MonoBehaviour
{
    public float raza_pick = 2f;
    public float raza_pick_departare = 5f; //ca sa il impuste de la distanta sau sa il opreasca pe om

    private bool este_focus = false;
    private Transform player;
    public bool stop_interactiune = false; // ar putea sa revina false intr-o functie dupa ce isi reincarca puterile, sau redevine pick-able sau orice idk
                                           //in tutorial stop_interactiune credea ca e si un fel de :a fost focusat" adica ii dadea true in OnFocused(). gen, sa nu mai faca animatia de onfocused() just to be safe idk  
                                           // De exemplu pentru Pickup uri e ok sa fac true si apoi niciodata false. Nu mai poti consuma pickupul a doua oara, dar pentru Koro nu e ok.

    // a pus si pozitie pentru unde sa se aseze playerul ca sa interactioneze cu obiectul. Cum ar fi locul unde trebuie sa stea ca sa deschida cufarul
    // public Transform interaction_transform;
    // l-a pus in aproape toate locurile aici in loc de transform si in Player pe linia cu FOllowtarget target = newtarget.TRANSFORM <-- acolo
    public Player_script p;

    public virtual void Start()
    {
        p = Player_script.instanta;
    }

    public virtual void Interactioneaza() 
    {
        //Debug.Log("Interactioneazaaa()");
        if (this.tag == "Finish")
        {
            GameManager_script.instanta.solved = true;
            //StopCoroutine(GameManager_script.instanta.AddKoroAutomat()); //nu merge asa
            GameManager_script.instanta.stopKoroutine = true;
            if (!GameManager_script.instanta.CheckFinish())
            {
                Quest_Manager.instanta.AddQuest(5);
            }
            stop_interactiune = true;
        }
    }
    public virtual void Interactioneaza_Dep()
    {
        //Debug.Log("Interactioneazaaa Deppppp()");
    }

    //public virtual void OnDrawGizmosSelected()
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, raza_pick);
    }

    void Update()
    {
        //if (this.tag == "analiza" /*||this.tag == "NPC" && this.GetComponent<Stats_script>().viata < 30 */)
        //   Debug.Log(este_focus + " " + !stop_interactiune + " " + Player_script.instanta.okInteract);
        if (este_focus && !stop_interactiune && p.okInteract) //okInteract e mereu true, mai putin cand da click pe un koro. Atunci devine false ca sa nu interactioneze incontinuu, dar sa ramana focusat. La un nou click va putea Interactiona din nou
        {
            float dist = Vector3.Distance(player.position, transform.position);
            /*if (this.tag == "analiza" /*||this.tag == "NPC" && this.GetComponent<Stats_script>().viata < 30)
            {
                Debug.Log(dist + "<=" + raza_pick + " " + Player_script.instanta.esteOrientatOk());
                Debug.Log(dist <= raza_pick && Player_script.instanta.esteOrientatOk());
            }*/
            if (dist <= raza_pick && p.esteOrientatOk()) //i-am mai adaugat si sa fie cat de cat orientat inspre target pentru a interactiona, nu cu spatele.. 
            {
                Interactioneaza();
            }
            else if (dist <= raza_pick_departare && p.esteOrientatOk()) //i-am mai adaugat si sa fie cat de cat orientat inspre target pentru a interactiona, nu cu spatele.. 
            {
                Interactioneaza_Dep();
            }
        }
    }

    public void Focusat(Transform playerTransform) // va fi apelata din Player in functia SetFocus
    {
        este_focus = true;
        player = playerTransform;
    }

    public void DeFocusat()
    {
        este_focus = false;
        player = null;
    }


}
