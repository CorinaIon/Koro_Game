using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemPickup_script : Obiect_script
{
    public Items_script item;
    public float timpAnimatiePickUp = 2.5f;
    /*
    private void Start()
    {
        base.Start();
    }*/
    public override void Interactioneaza()
    {
        //base.Interactioneaza();
        PickUp();
    }

    void PickUp()
    {
        
        //Debug.Log(this.name + " aka " + item.name + " va fi colectat");

        StartCoroutine(StopPlayerWhenPicking());


        Player_script.instanta.animator.SetTrigger("pick_smth");
        Player_script.instanta.stop_imd = true;

        if (GameManager_script.instanta.progres_level==1 && item.name == "Launcher")
        {
            Quest q = Quest_Manager.instanta.FindQuestTitlu("Find weapons!");
            if (q != null)
            {
                q.goal.Add_nr_curent(1);
            }
        }

        if (Quest_Manager.instanta.quest2lansat && (item.name == "Ciuperca Gri" || item.name == "Ciuperca Magica"))
        {
            Quest q = Quest_Manager.instanta.FindQuestTitlu("Find food!");
            if (q != null)
            {
                q.goal.Add_nr_curent(1);
            }
        }


        //adaug in inventar
        //in loc sa faca cu FindObjectOfType<Inventory_script>, a facut un singleton de tip Inventory_script
        bool a_fost_adaugat = Inventory_script.instanta.Add(item);
        if (a_fost_adaugat)
        {
            this.enabled = false;
            //Debug.Log("comanda destroy");
            Destroy(gameObject,timpAnimatiePickUp);
        }
        //ar fi trebuit si defocusat obiectul, dar  distrus oricum, nu mai conteaza.
        //La fel si stop_interactiune = true;
    }

    
    
    IEnumerator StopPlayerWhenPicking()
    {
        // fara corutina asta, playerul s-ar misca la un nou click in loc sa realizeze animatia de pick pe loc. 
        p.stopComplet = true;
        //Debug.Log("Asteptam " + timpAnimatiePickUp);
        yield return new WaitForSeconds(timpAnimatiePickUp - 0.5f); //am mai scazut pentru ca altfel facea Destroy si nu se mai executa corutina.. aparent..
        p.stopComplet = false;
    }
    
}
