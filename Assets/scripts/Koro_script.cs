using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//public class Koro_script : MonoBehaviour
public class Koro_script : MonoBehaviour
{
    public float razaAtac = 10f;
    //vectorul de targets se acceseaza prin " Targets_Manager.instanta.targets " care e de tip List<Transform>
    //private Transform playerul;
    protected Transform target_curent;
    private Transform target_gasit;  // puteam sa nu fac si cu linia asta, ci direct cu targeet_curent, da nu mi-l mai arata frumos in Inspector. Arata mereu null, pentru ca il fac eu null in update inainte de a cauta.
    protected NavMeshAgent agentul;
    public int damage = 1;
    public float timpIntreAtacuri = 2.0f; //aici: o data la 2 secunde ia 1 din viata targetului curent.
    protected float timpUltimulAtac;
    private float timpStartInDir;
    public float timpSchimbaDir = 10;
    private Transform tf_boxPatrulaNext;
    private float maxDistRandom = 100; //cea pentru cautarea punctului de patrulare.
    protected Animator animator;
    Vector3 centruTarcPos;
    private float razaTarc;

    protected virtual void Start()
    {
        agentul = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        //playerul = Player_script.instanta.transform;
        timpUltimulAtac = -timpIntreAtacuri; // ca sa poata incepe printr-un atac la secunda 0.
        timpStartInDir = -Mathf.Infinity;
        tf_boxPatrulaNext = transform.Find("BoxPatrulaNext").transform;
        centruTarcPos = GameManager_script.instanta.centruTarc.position;
        razaTarc = GameManager_script.instanta.razaTarc + 1;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, razaAtac);
    }

    protected virtual void Update()
    {
        float distanceMin = razaAtac;
        target_gasit = null;
        //foreach (Transform target in Targets_Manager.instanta.targets) //nu cu foreach daca vectorul se modifica intr-o corutina. La fel ca in GameManager, ar da eroare.
        for (int i = 0; i < Targets_Manager.instanta.targets.Count; i++)
        {
            Transform target = Targets_Manager.instanta.targets[i].transform;
            float dist = Vector3.Distance(target.position, transform.position);
            if (dist <= razaAtac && dist <= distanceMin && !esteInTarc(target))
            {
                distanceMin = dist;
                target_gasit = target; //targetul curent va fi cel care este cel mai aproape din raza de actiune. Daca intre timp a iesit din raza, nu il va mai urmari. 
            }
        }
        if (target_gasit != null) //a gasit un target in lista
        {
            target_curent = target_gasit;
            UrmaresteTargetul();
            //Debug.Log("Il urmareste. Avea distMin " + distanceMin + " iar stopping era " + agentul.stoppingDistance);
            if (distanceMin <= agentul.stoppingDistance)
            {
                AtacaTarget();
            }
            timpStartInDir = -Mathf.Infinity; //ca sa poata incepe patrularea in orice directie
        }
        else // nu a gasit niciun target
        {
            //Debug.Log("nu am gasit niciun traget, distanta fusese " + distanceMin);
            //target_curent nu s-a modificat intre timp, a ramas cel de la ultima patrulare, bun
            Patruleaza();
        }
        AnimatieDeplasare();
    }

    private void UrmaresteTargetul()
    {
        agentul.SetDestination(target_curent.position);
        //Nu are nevoie de FaceTarget, yay. 
    }

    private bool esteInTarc(Transform t)
    {
        return Vector3.Distance(t.position, centruTarcPos) <= razaTarc;
    }
    private void Patruleaza()
    {
        if (esteInTarc(transform))
        {
            timpStartInDir = 0;  // sa schimbe sensul imediat
        }
        if (Time.time >= timpStartInDir + timpSchimbaDir)
        {
            timpStartInDir = Time.time;
            tf_boxPatrulaNext.position = RandomNavSphere(transform.position, maxDistRandom, -1);
            target_curent = tf_boxPatrulaNext;
            UrmaresteTargetul();
        }
    }

    protected virtual void AnimatieDeplasare()
    {
        float speed = agentul.velocity.magnitude / agentul.speed;  //viteza curenta/viteza_max da o val intre 0 si 1 
        animator.SetFloat("viteza", speed);
    }

    public /*static*/ Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        if(Vector3.Distance(navHit.position, GameManager_script.instanta.centruTarc.position) < GameManager_script.instanta.razaTarc + 1 )
        {
            navHit.position += new Vector3(razaTarc * 2, 0, 0); //nu e nevoie de fapt, nu? E de ajuns ca il pun sa schimbe sensul atunci..
        }
        return navHit.position;
    }

    protected virtual void AtacaTarget() // in tutorial o punea in Stats practic..
    {
        //Mda.. Aici a  fost momentul cand am realizat ca toate obiectele vii din scena (player, npc, koro) trebuie sa aiba o clasa comuna care sa aiba functia TakeDamage, pentru ca nu vreau sa fac if(vad de ce tip e obiectul pe care vreau sa il atac) Getcomponent<de tipul ala> si apoi takedamage.
        //Deci trebuie o clasa comuna. si sa faca viata-=damage acolo. Viata, nu player.viata. Pentru ca si acolo e aceeasi problema, nu ar fi rapid sa determin daca e (player_script) player.viata sau npc.viata. Decaaaat daca as avea un alt vector cu targets de ceva tip universal si as face si camp special pentru script cu asta..
        //deci tot ca in tutorial e cel mai bine, cu clasa de Stats care are variabila viata, eh.. nu voiam asa.. 
        //Debug.Log("koro ataca target");
        if (Time.time >= timpUltimulAtac + timpIntreAtacuri) //pot ataca, a trecut destul timp
        {
            target_curent.gameObject.GetComponent<Stats_script>().TakeDamage(damage);
            timpUltimulAtac = Time.time;
        }
        //bara vizibila cand ataca (cerinta pjv) De sters pentru irva.
        StartCoroutine(this.GetComponent<Stats_script>().Corutina_activ_bar());
    }

    public void SetFinalDestination()
    {
        agentul.stoppingDistance = 1;
        agentul.SetDestination(Player_script.instanta.transform.position);
        this.GetComponent<Rigidbody>().isKinematic = false;
    }
}
