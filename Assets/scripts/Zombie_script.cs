using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie_script : Koro_script
{
    private float unghiAtac = 30;
    private float speedSlerp = 5;
    public float distAtacValid = 1.3f;
    bool nuIncepe = true;
    float timpStart;
    float delayStart = 4; // ca sa nu porneasca imediat in urmarire, sa astepte pana se face animatia de trezire
    protected override void Start()
    {
        base.Start();
        timpStart = Time.time;
    }

    protected override void Update()
    {
        if(nuIncepe)
        {
            if(timpStart + delayStart <= Time.time)
            {
                nuIncepe = false;
            }
            else
            {
                return;
            }
        }
        // puteam sa fac direct if si apoi ce e mai jos, dar suma e mai costisitoare decat bool
        base.Update();
    }

    protected override void AnimatieDeplasare()
    {
        float speed = 0;
        if (agentul.speed != 0)
        {
            speed = agentul.velocity.magnitude / agentul.speed;
        }
        animator.SetFloat("speedBlend", speed, .1f, Time.deltaTime);
    }    

    protected override void AtacaTarget() // in tutorial o punea in Stats practic..
    {
        if (Time.time >= timpUltimulAtac + timpIntreAtacuri) //pot ataca, a trecut destul timp
        {
            if (Vector3.Angle(transform.forward, target_curent.transform.position - transform.position) < unghiAtac)
            {
                animator.SetTrigger("ataca");
                StartCoroutine(DoDamage());
                timpUltimulAtac = Time.time;
            }
            else
            {
                FaceTarget();
            }
        }
        //bara vizibila cand ataca (cerinta pjv) De sters pentru irva.
        StartCoroutine(this.GetComponent<Stats_script>().Corutina_activ_bar());
    }

    void FaceTarget()
    {
        Vector3 direction = (target_curent.position - transform.position).normalized;
        direction = new Vector3(direction.x, 0f, direction.z);
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speedSlerp);
        }
    }

    IEnumerator DoDamage()
    {
        yield return new WaitForSeconds(1);  // delay pentru animatia de atac.
        // verific daca playerul e inca aproape, ca sa nu ii mai iau damage daca s-a ferit.
        if ((target_curent.position - transform.position).magnitude < distAtacValid)
        {
            target_curent.gameObject.GetComponent<Stats_script>().TakeDamage(damage);
        }
        // else Debug.Log("S-a ferit, yay");
    }
}
