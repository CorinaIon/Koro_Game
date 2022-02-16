using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubOneTime : Obiect_script
{
    public float timpReincarcare = 5f;
    public override void Interactioneaza()
    {
        StartCoroutine(Deschide());
    }

    IEnumerator Deschide()
    {
        Debug.Log(this.name + "aka cufar va fi deschis ");
        stop_interactiune = true;
        yield return new WaitForSeconds(timpReincarcare);
        stop_interactiune = false;
        Player_script.instanta.okInteract = false; //asta il separa de un cub infinit.
    }
}
