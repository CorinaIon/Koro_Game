using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Echipament")]
public class Echipament_script : Items_script
{
    public int damage;
    public TipEchipament tip;
    public GameObject prefabul;
    public override void Use()
    {
        base.Use();
        //echipare
        Echipare_Manager.instanta.Echipare(this);
        //sterg din inventar
        RemoveFromInventory();
    }

}

public enum TipEchipament { ATAC, PROTECTIE}
