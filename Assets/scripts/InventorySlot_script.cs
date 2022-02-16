using UnityEngine;
using UnityEngine.UI;

public class InventorySlot_script : MonoBehaviour
{
    public Items_script item; //PUBLIC doar ca sa il vad
    public Image imagine;
    public Button removeButton;
    private Vector3 dropDir = new Vector3(0, 0, 0.8f);
    public void AddItem (Items_script newItem)
    {
        item = newItem;

        imagine.sprite = item.imagine;
        imagine.enabled = true;
        imagine.preserveAspect = true;
        removeButton.interactable = true;
        removeButton.image.enabled = true;
    }

    public void StergeItem()
    {
        item = null;
        imagine.sprite = null;
        imagine.enabled = false;
        removeButton.interactable = false;
        removeButton.image.enabled = false;
    }

    public void OnRemoveButton()
    {
        Transform p = Player_script.instanta.transform;   
        Instantiate(item.prefab_drop/*.GetComponent<ItemPickup_script>()*/, p.position + item.height_drop + p.TransformDirection(dropDir),item.prefab_drop.transform.rotation); //neaparat? prefab_drop.GetComponent<ItemPickup_script>() nu doar preab_drop. raspuns: naah. nu e neaparat.
        Debug.Log(item.prefab_drop.name + " dropped cu height-ul " + item.height_drop);
        Inventory_script.instanta.Remove(item);
    }
    
    public void UseItem()
    {
        if(item!=null)
        {
            item.Use();
        }
    }
}

