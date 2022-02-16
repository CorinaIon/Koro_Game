using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Bill_Obiect_script : Obiect_script
{
    public float delayReInteractionabil = 2f; //dupa ce e atacat nu se va mai putea interacctiona cu el 2 secunde
    public float unghiAcceptatInteractiune = 25f;
    public float razaHereInteract = 1f;

    public override void Start()
    {
        base.Start();
    }

    public override void Interactioneaza()
    {
        Debug.Log("interactioneaza din clasa NPC_O_s");
        this.GetComponentInChildren<DialogTrigger_script>().TriggerDialog();
        p.okInteract = false;
    }

    public override void Interactioneaza_Dep()
    {
        Interactioneaza();
    }
}

