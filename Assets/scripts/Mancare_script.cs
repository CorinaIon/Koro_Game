using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Mancare")]
public class Mancare_script : Items_script
{
    public int yamm;
    public TipMancare tip;
    public override void Use()
    {
        base.Use();
        Player_script.instanta.stats_player.AdaugaViata(yamm);
        Player_script.instanta.animator.SetTrigger("a_primit_mancare");
        RemoveFromInventory();
    }
}

public enum TipMancare {CIUPERCA_MOV, CIUPERCA_GRI, POTIUNE }