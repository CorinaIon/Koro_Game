using UnityEngine;

[CreateAssetMenu(fileName ="New Item", menuName ="Inventory/Item")]
public class Items_script : ScriptableObject
{
    new public string name = "New Item"; //a pus new ca sa faca override la variabila name care exista deja
    public Sprite imagine = null; // icon pentru inventar
    public GameObject prefab_drop;
    public Vector3 height_drop; //sorry. trebuie pentru fiecare. s-ar putea calcula in functie de collider si centrul sau, ca acolo e de fapt. dar masca are pivotul aiurea si nu ar merge. mai rapid hardcodat. Chiar si asa, scriptul asta nu este propriu zis pe niciun obiect, la fel si copiii?

    public virtual void Use()
    {
        Debug.Log("Folosim obiectul " + name);
        //currency/crafting/potion/echipate
    }
    public void RemoveFromInventory()
    {
        Inventory_script.instanta.Remove(this);
    }
}
