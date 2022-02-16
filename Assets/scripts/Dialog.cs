using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialog
{
    public string nume_personaj;
    public bool reiaMiscareaLaFinal = false; //se va apela Functia PotiContinua ca NPC-ul sa fuga imediat dupa ce se incheie conversatia.
    public string triggeredBy;
    [TextArea]
    public string[] sentences;
}
