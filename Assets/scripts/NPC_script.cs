using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_script : MonoBehaviour
{ 
    private Player_script p;
    private NPC_Obiect_script npc_o_s;
    public bool isInteracting = false;
    public float speedSlerp = 2f;
    NavMeshAgent agentul;
    private float timpStartInDir;
    public float timpSchimbaDir = 10;
    private Transform target_curent; //ar fi mers si doar boxPatrula, dar sper sa aiba si alte target-uri, nu doar miscarea (in viitor)
    private Transform tf_boxPatrulaNext;
    private float maxDistRandom = 100; //cea pentru cautarea punctului de patrulare.
    public Animator animator; //ca sa il pot accesa si din koro_obj_sript
    public float offsetForwardCarat = 0.5f;
    public float offsetRightCarat = 0.3f;
    public float offsetUpCarat = 0.3f;
    public bool doAnimatieLasatJos = false;
    public Vector3 vectorLasatJos;
    public float unghi;
    public bool prima = true; //utila doar pentru stabilire
    Vector3 pos;
    public float timpUltimaSchimbare = 0f;
    public float timpSchimba = 20f;

    void Start()
    {
        Targets_Manager.instanta.AddTargetAndStats(transform, this.GetComponent<Stats_script>()); //se  adauga singur in lista cu targets
        //Debug.Log("target++ this npc");
        p = Player_script.instanta;
        npc_o_s = GetComponent<NPC_Obiect_script>();
        agentul = GetComponent<NavMeshAgent>(); 
        timpStartInDir = -Mathf.Infinity;
        tf_boxPatrulaNext = transform.Find("BoxPatrulaNext").transform;
        animator = GetComponentInChildren<Animator>();
    }
    
    void Update()
    {
        if(npc_o_s.este_carat)
        {
            transform.rotation = p.transform.rotation;
            transform.Rotate(transform.up, 80);
            transform.position = p.transform.position + p.transform.forward * offsetForwardCarat + p.transform.right * offsetRightCarat + p.transform.up * offsetUpCarat;
            return;
        }
        if(doAnimatieLasatJos)
        {
            //liniile comentate au ajutat la stabilirea valorilor.
            //if (prima)
            //{
                pos = transform.position;
                prima = false;
                transform.Rotate(transform.up, unghi);
            //}
            transform.position = pos + transform.right * vectorLasatJos.z + transform.forward * vectorLasatJos.x + transform.up * vectorLasatJos.y; ;
            doAnimatieLasatJos = false; //comenteaz-o pentru stabilire
            return;
        }

        if (npc_o_s.este_cazut)
        {
            return;
        }

        if(npc_o_s.inTarc)
        {
            if(Time.time >= timpUltimaSchimbare + timpSchimba)
            {
                //schimbam animatia NPC-ului din tarc
                animator.SetTrigger("schimba");
                timpUltimaSchimbare = Time.time;
            }
            return;
        }

        if (isInteracting)
        {
            agentul.SetDestination(this.transform.position); //ca sa se opreasca si sa nu mai mearga pana 
            FacePlayer();
            
            timpStartInDir = -Mathf.Infinity; //ca sa poata incepe patrularea in orice directie
        }
        else
        {
            Patruleaza();
        }
        AnimatieDeplasare();
    }
    public void FacePlayer()
    {
        Vector3 direction = (p.transform.position - transform.position).normalized;
        direction = new Vector3(direction.x, 0f, direction.z);
        if (direction != Vector3.zero) //pentru ca aveam Look rotation viewing vector is zero; UnityEngine.Quaternion:LookRotation(UnityEngine.Vector3)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speedSlerp);
        }
    }

    private void UrmaresteTargetul()
    {
        agentul.SetDestination(target_curent.position);
        //Nu are nevoie de FaceTarget, yay. 
    }

    private void Patruleaza()
    {
        if (Time.time >= timpStartInDir + timpSchimbaDir)
        {
            timpStartInDir = Time.time;
            tf_boxPatrulaNext.position = RandomNavSphere(transform.position, maxDistRandom, -1);
            target_curent = tf_boxPatrulaNext;
            UrmaresteTargetul();
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    void AnimatieDeplasare()
    {
        float speed = 0;
        if (agentul.speed != 0)
        {
            speed = agentul.velocity.magnitude / agentul.speed;  //viteza curenta/viteza_max da o val intre 0 si 1 pentru Blend Tree
        }
        animator.SetFloat("speedBlend", speed, .1f, Time.deltaTime); // va dura 1/10 sec sa faca smooth inte doua val
    }

}
