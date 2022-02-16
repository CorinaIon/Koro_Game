using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_script : MonoBehaviour
{
    #region Singleton Inventar oaaauuu
    public static Inventory_script instanta;
    private void Awake()
    {
        if(instanta!=null)
        {
            Debug.LogWarning("More than one instance of Inventar found :((");
        }
        instanta = this;
    }
    #endregion

    public List<Items_script> items = new List<Items_script>();
    public int dimMax = 20;
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;
    public bool Add (Items_script item)
    {
        //el avea o linie cu default item
        if (items.Count < dimMax)
        {
            items.Add(item);
            if(onItemChangedCallback != null)
                onItemChangedCallback.Invoke(); // ca sa se updateze UI.. am in Inventory_UI_script functia UpdadteUI() si linia inventar.onItemChangedCallback += UpdateUI;
            return true;
        }
        else
        {
            Debug.Log("Inventarul este plin!");
            return false;
        }       
    }
    public void Remove(Items_script item)
    {
        items.Remove(item);
        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke(); // ca sa se updateze UI..
    }

    public void PlayerCere(string obiectCerut)
    {
        /*
        Debug.Log("In inventory script avem asaaa:");
        
        for(int i=0; i< items.Count;i++)
        {
            if (items[i] == null)
                Debug.Log("Am gasit ceva null pe pozitia " + i);
            else Debug.Log(items[i].name);
        }*/
            
        foreach (Items_script item in items)
        {
            if (item.name == obiectCerut)
            {
                Transform.FindObjectOfType<Inventar_UI_script>().GetPentruPlayer(obiectCerut);
                //items.Remove(item); //e apelata deja din Items_script
                return;
            }     
        }
        Debug.Log("~~ sunet ca nu exista in inventar un " + obiectCerut + "  ~~");
    }
}
