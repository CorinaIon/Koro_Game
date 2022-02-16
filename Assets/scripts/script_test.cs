using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class script_test : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(scadeViata());
    }
    IEnumerator scadeViata()
    {
        yield return new WaitForSeconds(1);
        this.GetComponentInChildren<Animator>().SetBool("esteBolnav", true);
        this.GetComponent<Stats_script>().viata = 30;

    }
}
