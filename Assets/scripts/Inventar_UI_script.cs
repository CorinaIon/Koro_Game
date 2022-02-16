using UnityEngine;

public class Inventar_UI_script : MonoBehaviour
{
    Inventory_script inventar;
    public Transform parinteItems;
    InventorySlot_script[] slots;
    public GameObject inventar_panel;
    void Start()
    {
        inventar = Inventory_script.instanta; // ca sa nu faca de fiecare data .instanta pentru a lua obiectul
        inventar.onItemChangedCallback += UpdateUI; //functia UpdateUI se va apela de fiecare data cand se modifica un Item in inventar

        slots = parinteItems.GetComponentsInChildren<InventorySlot_script>();
        //Debug.Log("Start. slots are " + slots.Length);

        inventar_panel.SetActive(false);
    }

    void Update()
    {
        if(Input.GetButtonDown("Inventar"))
        {
            inventar_panel.SetActive(!inventar_panel.activeSelf);
        }


        //foarte urat ca le-am pus aici, stiu
        if (Input.GetKeyDown(KeyCode.F3))
        {
            GameManager_script.instanta.ReLoadMainScene();
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            GameManager_script.instanta.ReLoadActiveScene();
        }
    }

    void UpdateUI()
    {
        //Debug.Log("Se va updata inventarul . . . ");
        //inventar.items[i] e cel cu mai putine elemente
        for (int i = 0; i < slots.Length; i++)
        {
            if(i<inventar.items.Count)
            {
                slots[i].AddItem(inventar.items[i]);
            }
            else
            {
                slots[i].StergeItem();
            }
        }
    }

    public void GetPentruPlayer(string obiectCerut)
    {
        foreach (InventorySlot_script i in slots)
        {
            if (i.item.name == obiectCerut)
            {
                i.item.Use();
                return;
            }
        }
    }
}
