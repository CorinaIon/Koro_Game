
// sa ii pun delay pana devine reinteractionabil ca sa nu il poata lasa jos in timp ce inca fac animatia de lift playerul.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Obiect_script : Obiect_script
{//copy paste la mare parte din codul Koro

    public float delayHeal = 2f; //nu e instant + ca trebuie playerul sa faca animatia in timpul asta
    public float delayReInteractionabil = 2f; //dupa ce e atacat nu se va mai putea interacctiona cu el 2 secunde
    public int heal_ul; //PUBLIC doar ca sa il vad
    //private Player_script p;
    private NPC_script npc_s;
    public float unghiAcceptatInteractiune = 25f;
    public float razaHereInteract = 1f;
    //public float razaStopUnPic = 3f; //l-am inlocuit cu raza_pick_departare in clasa parinte. Sa sterg linia asta de tot..
    public float timpStopUnPic = 3f;
    public bool are_mesaj = false; //trebuie setata daca vreau sa dialogheze + adaug scriptul
    public bool inHeal = false;
    public bool este_cazut = false;  // doar daca este pe jos, nu si in tarc
    public bool este_carat = false; 
    public bool inTarc = false;
    public float distHeal = 0.65f;
    public GameObject masaLibera;
    Transform centruTarc;
    public GameObject prefabPotiune;
    GameObject potiune;
    public bool mesajAfiseaza = false;

    public override void Start()
    {
        //p = Player_script.instanta;
        base.Start();
        npc_s = GetComponent<NPC_script>();
        Dialog_Manager.instanta.onDialogClosed += PotiContinua;
        centruTarc = GameManager_script.instanta.centruTarc;
    }

    public override void Interactioneaza() //asta era functia apelata atunci cand Playerul da click dreapta pe un obiect interactionabil. 
                                           //In cazul asta, cand da click pe Koro, il va ataca (cu arma).
    {
        p.okInteract = false;

        //base.Interactioneaza();
        Debug.Log("interactioneaza din clasa NPC_O_s");
        // o data ce a dat click pe el, il va urmari pe player peste tot.
        //? Daca playerul da click din nou sunt 2 variante: Daca are si potiune in mana ii va da Heal. Daca nu are sa ii dau stop follow??  
        //? Daca intre timp playerul a dat click pe altceva si apoi revine la player, il va urmari din nou
        npc_s.isInteracting = true;
        //verificam daca vrea sa ii ofere din potiune. aka Eu sunt focusul.
        if (are_mesaj)
        {
            stop_interactiune = true;
            this.GetComponentInChildren<DialogTrigger_script>().TriggerDialog();
            //isInteracting ramane true pana cand playerul apasa pe Close de la mesaje. Va fi apelat un callback si se va seta la loc isInteracting false. Se va relua update-ul npc.
            mesajAfiseaza = true;
        }
        else if (este_cazut)
        {
            StartCoroutine(Carat());
        }
        else if (este_carat)
        {
            StartCoroutine(LasatJos());
        }
        else if (p.animator.GetBool("tine_arma") && p.obiect_pt_ATAC.GetComponentInChildren<ItemPickup_script>().item.name == "Potiune vindecatoare")
        {
            heal_ul = ((Echipament_script)p.obiect_pt_ATAC.GetComponentInChildren<ItemPickup_script>().item).damage;
            stop_interactiune = true; // sa nu mai interactioneze pana nu termin asta
            StartCoroutine(Heal());
        }
        else
        {
            //ce urmeaeza a fost mutat mai jos oricum..
            //altele 
            /*
            if(Vector3.Distance(p.transform.position, transform.position) < razaStopUnPic)
            {
                if(!trimis_mesajul)
                {
                    this.GetComponentInChildren<DialogTrigger_script>().TriggerDialog();
                    //isInteracting ramane true pana cand playerul apasa pe Close de la mesaje. Va fi apelat un callback si se va seta la loc isInteracting false. Se va relua update-ul npc.
                }
                else
                {
                    StartCoroutine(StopUnPic());
                }
                    
            }
            else*/
            StartCoroutine(StopUnPic());
            //npc_s.isInteracting = false;
        }
        p.okInteract = false;  // trebuie sa dea din nou click pentru a interactiona.
    }

    public override void Interactioneaza_Dep()
    {
        //base.Interactioneaza_Dep();
        Debug.Log("interactioneaza DEP din clasa NPC_O_s");

        if (este_cazut)
        {
            StartCoroutine(Carat());
            return;
        }
        if (inTarc)
            return;
        npc_s.isInteracting = true;
        if (are_mesaj)
        {
            stop_interactiune = true;
            this.GetComponentInChildren<DialogTrigger_script>().TriggerDialog();
            //isInteracting ramane true pana cand playerul apasa pe Close de la mesaje. Va fi apelat un callback si se va seta la loc isInteracting false. Se va relua update-ul npc.
            mesajAfiseaza = true;
        }
        else
        {
            StartCoroutine(StopUnPic());
        }
        p.okInteract = false;
    }

    IEnumerator Heal()
    {
        //trateaza ambele cazuri: NPC sanatos/bolnav in tarc.
        inHeal = true;
        Debug.Log("Corutina HEAL START");
        p.VinoLangaNPC(razaHereInteract);


        if (!inTarc)
        {
            while (Vector3.Angle(transform.forward, p.transform.position - transform.position) > unghiAcceptatInteractiune)
            {
                //sa ii pun conditie de iesire..!!!!!!!!!!!!!!!!! ca la Carat

                yield return null; //sa astepte pana se intoarce playerul cu fata la el
                //in update-ul din NPC_script el face FacePlayer cat timp are isInteracting true)
            }
        }
        else
        {
            p.VinoLangaNPCCazut(transform.right * distHeal);
            p.validOffset = 1;
            while (p.agent.velocity.magnitude > 0.1f || Vector3.Angle(transform.right, p.transform.position - transform.position) > unghiAcceptatInteractiune)
            {
                // daca inca se misca agentul sau nu este pozitionat pe directia corecta
                // verificam daca inca doreste sa interactioneze cu NPC-ul
                if (p.focus == null || p.focus.gameObject != this.gameObject)
                {
                    //Debug.Log("w NULL");
                    break;
                }
                else
                {
                    //Debug.Log("w waiting" + Vector3.Angle(transform.right, p.transform.position - transform.position));
                    // verificam daca sta ~nemiscat. 
                    if (p.agent.velocity.magnitude < 0.1f)
                    {
                        //Debug.Log(p.agent.velocity.magnitude);
                        // Il rotim incet astfel incat sa fie orientat corect
                        Debug.Log("NU AR TREBUI SA FIE O PROBLEMAAAA");
                        p.FaceTarget();
                    }
                }
                yield return null; //sa astepte pana se intoarce playerul cu fata la el
                                   //in update-ul din NPC_script el face FacePlayer cat timp are isInteracting true)
            }
            p.validOffset = 0;
        }

        



        if (p.focus == null || p.focus.gameObject != this.gameObject || (!p.animator.GetBool("tine_arma") || p.obiect_pt_ATAC.GetComponentInChildren<ItemPickup_script>().item.name != "Potiune vindecatoare"))
        { //intre timp playerul s-a razgandit :'(
            //Initial faacusem asta in update-ul din npc_s, dar nu avea sens sa se opreasca din intoarcere pana sa ajunga sa vada ca nu mai are potiunea in mana. e mai ok sa se intoarca, isi da seama ca nu mai vrea sa il videce si se opreste atunci.
            //sa rescriu altfel asta. poate cu Defocus in clasa parinte Object_script
            npc_s.isInteracting = false;
            stop_interactiune = false; // poate interactiona din nou
            Player_script.instanta.okInteract = false; //trebuie sa dea click din nou playerul.
            yield return 0;
        }
        p.animator.SetBool("nu_anima_damage", true); //ca sa nu faca animatia Damage in loc de cea de melee. Pentru ca aia e din AnyState
        p.animator.SetTrigger("actiune_melee"); //playerul se animeaeza
        Player_script.instanta.RemoveFocus();
        // INSERT COD PENTRU ANIMATIA LUI NPC AICI
        
        yield return new WaitForSeconds(delayHeal); //astept pana dau viata++
        if(inTarc)
        {
            npc_s.animator.SetTrigger("primeste_potiune_tarc");
            //!!!Sa instantiez o potiune in mana lui NPC. si sa pun delay-uri!!!!!!!!!!!!!!!
            potiune = Instantiate(prefabPotiune);
            GetComponent<ShooterMechanics_script>().weapon = potiune.transform;

            npc_s.timpUltimaSchimbare = Time.time;
        }
        Echipare_Manager.instanta.DistrugeEchipament((int)TipEchipament.ATAC, "Potiune vindecatoare"); //dispare potiunea din mana lui
        if (inTarc)
        {
            yield return new WaitForSeconds(delayHeal); // inca o data pentru ca animatia de primeste_potiune dureaza si ar sari de pe masa imediat.....
        }
        this.GetComponent<Stats_script>().AdaugaViata(heal_ul);
        p.animator.SetBool("nu_anima_damage", false);

        if(potiune!=null)
        {
            yield return new WaitForSeconds(1);
            GetComponent<ShooterMechanics_script>().weapon = null;
            Destroy(potiune);
        }

        npc_s.isInteracting = false; //nu mai am nevoie de player. 
        yield return new WaitForSeconds(delayReInteractionabil); //mai astept un pic pana sa interactioneze din nou cei doi
        stop_interactiune = false; // poate interactiona din nou
        Player_script.instanta.okInteract = false; //trebuie sa dea click din nou playerul.
        inHeal = false;
    }

    IEnumerator StopUnPic()
    {
        //stop_interactiune = true; //fara asta pentru ca altfel nu ar mai accepta potiunea
        Debug.Log("Corutina stop un pic");
        yield return new WaitForSeconds(timpStopUnPic);
        if (!inHeal) //fusese deranjat degeaba. Nu va mai raspunde pentru x secunde
        {
            npc_s.isInteracting = false; //nu mai am nevoie de player. 
            stop_interactiune = true;
            yield return new WaitForSeconds(delayReInteractionabil); //mai astept un pic pana sa interactioneze din nou cei doi
            stop_interactiune = false; // poate interactiona din nou
        }
        Player_script.instanta.okInteract = false; //trebuie sa dea click din nou playerul.
    }

    void PotiContinua()
    {
        if (are_mesaj && mesajAfiseaza) // fara if-ul asta ar fi functia de la toti NPC-ii, cu mesaj, fara, indiferent daca ii fusese citit sau nu inca.
        {
            are_mesaj = false;
            mesajAfiseaza = false;
            StartCoroutine(StopCitireMesaj());
            if (GameManager_script.instanta.progres_level == 0)
                StartCoroutine(GameManager_script.instanta.LevelProgres());
            p.RemoveFocus();
        }
    }
    IEnumerator StopCitireMesaj() //isi va relua miscarea
    {
        npc_s.isInteracting = false; //nu mai am nevoie de player.
        yield return new WaitForSeconds(delayReInteractionabil); //mai astept un pic pana sa interactioneze din nou cei doi
        stop_interactiune = false; // poate interactiona din nou
        Player_script.instanta.okInteract = false; //trebuie sa dea click din nou playerul.
    }

    IEnumerator Carat()
    {
        stop_interactiune = true;
        this.GetComponent<Stats_script>().nuModificaViata = true;
        Debug.Log("Corutina Carat start");
        /*p.VinoLangaNPC(razaHereInteract);
        Debug.Log(Vector3.Angle(transform.right, p.transform.position - transform.position));
        while (Vector3.Angle(transform.right, p.transform.position - transform.position) > unghiAcceptatInteractiune)
        {
            if (p.focus == null || p.focus.gameObject != this.gameObject)
            {
                Debug.Log("w NULL");
                break;
            }
            else
                Debug.Log("w waiting" + Vector3.Angle(transform.right, p.transform.position - transform.position));
            yield return null; //sa astepte pana se intoarce playerul cu fata la el
            //in update-ul din NPC_script el face FacePlayer cat timp are isInteracting true)
        }*/
        
        p.VinoLangaNPCCazut(transform.right * 0.5f);
        p.validOffset = 1;
        while (p.agent.velocity.magnitude > 0.1f || Vector3.Angle(transform.right, p.transform.position - transform.position) > 40 + unghiAcceptatInteractiune)
        {
            // daca inca se misca agentul sau nu este pozitionat pe directia corecta
            // verificam daca inca doreste sa interactioneze cu NPC-ul
            if (p.focus == null || p.focus.gameObject != this.gameObject)
            {
                //Debug.Log("w NULL");
                break;
            }
            else
            {
                //Debug.Log("w waiting" + Vector3.Angle(transform.right, p.transform.position - transform.position));
                // verificam daca sta ~nemiscat. 
                if (p.agent.velocity.magnitude < 0.1f)
                {
                    //Debug.Log(p.agent.velocity.magnitude);
                    // Il rotim incet astfel incat sa fie orientat corect
                    Debug.Log("NU AR TREBUI SA FIE O PROBLEMAAAA");
                    p.FaceTarget();
                }
            }
            yield return null; //sa astepte pana se intoarce playerul cu fata la el
            //in update-ul din NPC_script el face FacePlayer cat timp are isInteracting true)
        }
        Debug.Log("out while");
        if (p.focus == null || p.focus.gameObject != this.gameObject)
        { //intre timp playerul s-a razgandit :'(
            //Initial faacusem asta in update-ul din npc_s, dar nu avea sens sa se opreasca din intoarcere pana sa ajunga sa vada ca nu mai are potiunea in mana. e mai ok sa se intoarca, isi da seama ca nu mai vrea sa il videce si se opreste atunci.
            //sa rescriu altfel asta. poate cu Defocus in clasa parinte Object_script
            this.GetComponent<Stats_script>().nuModificaViata = false;
            npc_s.isInteracting = false;
            stop_interactiune = false; // poate interactiona din nou
            Player_script.instanta.okInteract = false; //trebuie sa dea click din nou playerul.
            Debug.Log("in if urmeaza return 0"); 
            yield break;
        }

        Debug.Log("good. urmeaza sa setam *cara*");
        //p.animator.SetBool("nu_anima_damage", true); //ca sa nu faca animatia Damage in loc de cea de melee. Pentru ca aia e din AnyState
        p.animator.SetBool("cara", true);
        yield return new WaitForSeconds(1); //cat dureaza animatia pana ridica
        este_cazut = false; 
        este_carat = true;
        this.GetComponentInChildren<Animator>().SetBool("este_carat", este_carat);
        stop_interactiune = false;
        Player_script.instanta.okInteract = false;
        Player_script.instanta.RemoveFocus();
        p.validOffset = 0;
    }

    IEnumerator LasatJos()
    {
        Debug.Log("Lasat jos start. Suntem in poz " + transform.position + " si player " + p.transform.position);
        stop_interactiune = true;
        //este_carat = false;
        //este_cazut = true; // conteaza ordinea, nu trebuie sa prinda un ciclu in while cand nu are niciuna setata.
        

        if (EsteInTarc() && (masaLibera=GameManager_script.instanta.esteMasaLibera()))  // nu credeam ca o sa scriu asa ceva vreodata intentionat..
        {
            Debug.Log("Asteptam sa ajunga la masa!!");
            // nu ii mai scade din viata pentru ca nu mai este in lista de targets
            // ca sa ii creasca viata, playerul trebuie sa ii ofere potiuni.
            p.agent.stoppingDistance = 0.01f; // un numar oarecare mic pe care il verific mai jos in cod
            p.VinoLangaMasa(masaLibera.transform);

            while (Vector3.Distance(p.transform.position, masaLibera.transform.position) > 0.3f)
            {
                if (p.agent.stoppingDistance != 0.01f) // daca intre timp a apasat pe altceva
                {
                    Debug.Log("corutina return 0");
                    yield return 0; //sa iasa complet. Inseamna ca a dat click pe altceva si s-a razgandit
                }
                yield return null; //asteptam sa ajunga la masa
            }
            yield return new WaitForSeconds(0.5f);
            p.transform.rotation = masaLibera.transform.rotation; //mda.. srry
            yield return new WaitForSeconds(0.5f);
            Debug.Log("Ocupam");
            este_carat = false;
            inTarc = true;
            this.GetComponentInChildren<Animator>().SetBool("inTarc", inTarc);
            masaLibera.tag = "Untagged"; // adica ocupata
        }
        else
        {
            este_carat = false;
            inTarc = false;
            este_cazut = true; //false; DE CE AR FI FOST FALSE??
            this.GetComponentInChildren<Animator>().SetBool("inTarc", inTarc);
            this.GetComponent<Stats_script>().nuModificaViata = false;
        }
        this.GetComponentInChildren<Animator>().SetBool("este_carat", este_carat);
        yield return new WaitForSeconds(0.1f);
        //npc_s.prima = true; //decomenteaz-o pentru stabilire  
        npc_s.doAnimatieLasatJos = true;
        p.animator.SetBool("cara", false);
        //p.animator.SetBool("nu_anima_damage", false);
        
        Player_script.instanta.RemoveFocus();
        npc_s.isInteracting = false;
        yield return new WaitForSeconds(delayReInteractionabil);
        stop_interactiune = false;
        Player_script.instanta.okInteract = false;
        //decomenteaz-o pentru stabilire npc_s.doAnimatieLasatJos = false; //sau in alta parte..
    }

    bool EsteInTarc()
    {
        if((centruTarc.position - this.transform.position).magnitude <= GameManager_script.instanta.razaTarc)
        {
            return true;
        }
        return false;
    }

}

