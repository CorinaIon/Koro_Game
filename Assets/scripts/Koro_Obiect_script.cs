using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Koro_Obiect_script : Obiect_script
{
    public float delayDamage = 0.5f; //nu e instanta impuscatura, nu vrem sa ii ia direct din viata.
    public float delayReInteractionabil = 2f; //dupa ce e atacat nu se va mai putea interacctiona cu el 2 secunde
    public int damage_ul = 34;
    private int damage_close;
    public float razaHereInteract = 3f;
    //private Player_script p;
    public float unghiAcceptatInteractiune = 20f;

    public override void Start()
    {
        //p = Player_script.instanta;
        base.Start();
    }
    public override void Interactioneaza() //asta era functia apelata atunci cand Playerul da click dreapta pe un obiect interactionabil. 
                                           //In cazul asta, cand da click pe Koro, il va ataca (cu arma).
    {
        base.Interactioneaza();
        Debug.Log("interactioneaza din clasa Koro");

        //Debug.Log(" --name: " + p.obiect_pt_ATAC.GetComponentInChildren<ItemPickup_script>().name + " --item: " + p.obiect_pt_ATAC.GetComponentInChildren<ItemPickup_script>().item + " --item name: " + p.obiect_pt_ATAC.GetComponentInChildren<ItemPickup_script>().item.name);
        // --name: Weapon_Launcher --item: Echip_range (Echipament_script) --item name: Launcher
        //--name: FLAKON05_mana_modified_by_MeMeMee (2) --item: Echip_Potiune (Echipament_script) --item name: Potiune vindecatoare

        if (p.animator.GetBool("tine_arma") && p.obiect_pt_ATAC.GetComponentInChildren<ItemPickup_script>().item.name == "Launcher")
        {
            //animatie atac player
            p.animator.SetBool("nu_anima_damage", true); //ca sa nu faca animatia Damage in loc de cea de range. Pentru ca aia e din AnyState
            p.animator.SetTrigger("ataca_range");
            StartCoroutine(IaDamage()); // un delay pana ii ia damage-ul, nu direct: this.GetComponent<Stats_script>().TakeDamage(34); + cooldown
        }
        //special pentru pjv, sa aiba si range si melee
        if (p.animator.GetBool("tine_arma") && p.obiect_pt_ATAC.GetComponentInChildren<ItemPickup_script>().item.name == "Potiune vindecatoare")
        {
            Debug.Log("al doilea if");
            damage_close = ((Echipament_script)p.obiect_pt_ATAC.GetComponentInChildren<ItemPickup_script>().item).damage;
            p.animator.SetBool("nu_anima_damage", true); 
            StartCoroutine(IaDamage_pjv());
        }

        Debug.Log("facut ok false");
        p.okInteract = false; // trebuie sa dea din nou click pentru a interactiona.
    }

    public override void Interactioneaza_Dep()
    {
        base.Interactioneaza_Dep();
        Debug.Log("interactioneaza DEP din clasa NPC_O_s");

        if (p.animator.GetBool("tine_arma") && p.obiect_pt_ATAC.GetComponentInChildren<ItemPickup_script>().item.name == "Launcher")
        {
            //animatie atac player
            p.animator.SetBool("nu_anima_damage", true); //ca sa nu faca animatia Damage in loc de cea de range. Pentru ca aia e din AnyState
            p.animator.SetTrigger("ataca_range");
            StartCoroutine(IaDamage()); // un delay pana ii ia damage-ul, nu direct: this.GetComponent<Stats_script>().TakeDamage(34); + cooldown
        }
        Debug.Log("facut ok false");
        p.okInteract = false; // trebuie sa dea din nou click pentru a interactiona.
    }

    IEnumerator IaDamage()
    {
        stop_interactiune = true;
        Player_script.instanta.muzzle.Play();
        yield return new WaitForSeconds(delayDamage); 
        this.GetComponent<Stats_script>().TakeDamage(damage_ul);
        p.animator.SetBool("nu_anima_damage", false);
        //Debug.Log("este ora " + Time.time);
        yield return new WaitForSeconds(delayReInteractionabil);
        //Debug.Log("este " + Time.time + " au trecut " + delayReInteractionabil + " secunde");
        stop_interactiune = false; // poate interactiona din nou
        Player_script.instanta.okInteract = false; //trebuie sa dea click din nou playerul. (se facuse la loc true in timp ce koro se refacea).
    }

    IEnumerator IaDamage_pjv() //copy paste npc_obiect script
    {
        Debug.Log("In ia damage pjv");
        stop_interactiune = true; // sa nu mai interactioneze pana nu termin asta
        p.VinoLangaNPC(razaHereInteract); //sa vina playerul mai aproape de target, adica Koro (nu npc, dar asa se cheama functia.. sa o redenumesc eventual..)
            Debug.Log("i-a zis sa vina la dist. " + razaHereInteract);
        if ((transform.position - p.transform.position).magnitude > razaHereInteract) //mai asteapta o secunda. #HC
            yield return new WaitForSeconds(1);//cat sa vina playerul mai aproape
        if (p.focus == null || p.focus.gameObject != this.gameObject || (!p.animator.GetBool("tine_arma") || p.obiect_pt_ATAC.GetComponentInChildren<ItemPickup_script>().item.name != "Potiune vindecatoare"))
        { //intre timp playerul s-a razgandit :'(
            //sa rescriu altfel asta. poate cu Defocus in clasa parinte Object_script
            stop_interactiune = false; // poate interactiona din nou
            Player_script.instanta.okInteract = false; //trebuie sa dea click din nou playerul.
            yield return 0;
        }

        p.animator.SetBool("nu_anima_damage", true); //ca sa nu faca animatia Damage in loc de cea de melee. Pentru ca aia e din AnyState
        p.animator.SetTrigger("actiune_melee"); //playerul se animeaeza
        // INSERT COD PENTRU ANIMATIA LUI koro AICI
        yield return new WaitForSeconds(delayDamage);
        Echipare_Manager.instanta.DistrugeEchipament((int)TipEchipament.ATAC, "Potiune vindecatoare"); //dispare potiunea din mana lui
        this.GetComponent<Stats_script>().TakeDamage(damage_ul);
        p.animator.SetBool("nu_anima_damage", false);
        Player_script.instanta.RemoveFocus();
        yield return new WaitForSeconds(delayReInteractionabil); //mai astept un pic pana sa interactioneze din nou cei doi
        stop_interactiune = false; // poate interactiona din nou
        Player_script.instanta.okInteract = false; //trebuie sa dea click din nou playerul.
    }
}
