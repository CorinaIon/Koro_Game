using UnityEngine;

public class Echipare_Manager : MonoBehaviour
{
    #region Singleton Echipare_Manager
    public static Echipare_Manager instanta;
    private void Awake()
    {
        instanta = this;
    }
    #endregion

    Echipament_script[] echipament_curent;
    GameObject[] echipamente_instante;
    Inventory_script inventar;

    public GameObject playerul;

    public delegate void OnEquipmentChanged(Echipament_script newItem, int poz);
    public OnEquipmentChanged onEquipmentChangedCallback;
    
    //public Transform rootbonespine2;
    public GameObject masca_player;
    private Color culoare_default_masca;

    private void Start()
    {
        inventar = Inventory_script.instanta;
        int nrTipuriEchipamente = System.Enum.GetNames(typeof(TipEchipament)).Length; // sunt 2: launcher si protectia
        echipament_curent = new Echipament_script[nrTipuriEchipamente]; 
        echipamente_instante = new GameObject[nrTipuriEchipamente];
        culoare_default_masca = new Color(231.0f / 255, 231.0f / 255, 231.0f / 255, 1.0f);
    } 

    public void Echipare(Echipament_script newEchip)
    {
        int slotIndex = (int) newEchip.tip; //pe ce pozitie e in enum
        //0 pentru arma, 1 pentru protectie, 2 etc
        Dezechipare(slotIndex);
        //liniile astea se fac si in Dezechipare
        /*Echipament_script echipament_vechi = echipament_curent[slotIndex];
        //daca aveam mai multe tipuri de arme: 
        if(echipament_curent[slotIndex]!=null)
        {
            inventar.Add(echipament_vechi);
        }*/

        echipament_curent[slotIndex] = newEchip;

        if (echipament_curent[slotIndex].tip == TipEchipament.ATAC)
        {
            echipamente_instante[slotIndex] = Instantiate(newEchip.prefabul);
            playerul.GetComponent<ShooterMechanics_script>().weapon = echipamente_instante[slotIndex].transform;
            playerul.GetComponentInChildren<Animator>().SetBool("tine_arma", true);
            if (onEquipmentChangedCallback != null) //sunt functii pe care sa le anunt cu callback: ActualizareObiectATAC din Player_script
            {
                onEquipmentChangedCallback.Invoke(newEchip, slotIndex);
            }
            Player_script.instanta.muzzle = echipamente_instante[slotIndex].GetComponentInChildren<ParticleSystem>();
        }
        else //if (echipament_curent[slotIndex].tip == TipEchipament.PROTECTIE)
        {
            /* las echipamente_instante[slotIndex] null, nu instantiez prefab pentru masca.. 
             * Am incercat si nu a mers nici cu prefab simplu ca la arma (nu statea fix pe fata) si 
             * nici nu skinnedMshRenderer pentru ca era in permanenta mai jos cu 0.1f fata de locul unde voiam sa fie lipita..
            playerul.GetComponent<ShooterMechanics_script>().masca = echipamente_instante[slotIndex].transform;
            echipamente_instante[slotIndex].GetComponentInChildren<SkinnedMeshRenderer>().rootBone = rootbonespine2;//playerul.GetComponentInChildren<SkinnedMeshRenderer>().rootBone;
            echipamente_instante[slotIndex].GetComponentInChildren<SkinnedMeshRenderer>().bones = playerul.GetComponentInChildren<SkinnedMeshRenderer>().bones;
            * Am setat direct culoarea pe masca pe care o avea pe fata de la inceput.
            */
            //masca_player.GetComponent<Material>().color = Color.black; // nu se poate folosi Material, nu are asa ceva pe el. 
            masca_player.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
            masca_player.GetComponent<Renderer>().material.SetFloat("_Glossiness", 0.3f); // pentru Smoothness
        }
    }

    public void Dezechipare(int slotIndex)
    {
        if(echipament_curent[slotIndex]!=null) //poarta ceva acum
        {
            Echipament_script echipament_vechi = echipament_curent[slotIndex];
            inventar.Add(echipament_vechi);

            if (echipament_curent[slotIndex].tip == TipEchipament.ATAC)
            {
                Destroy(echipamente_instante[slotIndex]); //distrug instanta prefabului cu arma/potiunea din mana omului
                playerul.GetComponent<ShooterMechanics_script>().weapon = null;
                playerul.GetComponentInChildren<Animator>().SetBool("tine_arma", false);
                if (onEquipmentChangedCallback != null) //sunt functii pe care sa le anunt cu callback: ActualizareObiectATAC din Player_script
                {
                    onEquipmentChangedCallback.Invoke(null, slotIndex);
                }
            }
            else
            {
                //playerul.GetComponent<ShooterMechanics_script>().masca = null;
                masca_player.GetComponent<Renderer>().material.SetColor("_Color", culoare_default_masca);
                //sau asa: masca_player.GetComponent<Renderer>().material.color = culoare_default_masca;
                masca_player.GetComponent<Renderer>().material.SetFloat("_Glossiness", 0);
            }
            echipament_curent[slotIndex] = null;
        }
    }

    public void DezechipareTotala()
    {
        for (int i = 0; i < echipament_curent.Length; i++)
        {
            Dezechipare(i);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
            DezechipareTotala();
    }

    public GameObject Get_GO_poz(int slotIndex)
    {
        return echipamente_instante[slotIndex];
    }

    public void DistrugeEchipament(int slotIndex, string nume) //ca dezechipare, dar nu mai apare in inventar dupa
    {
        if (echipament_curent[slotIndex] != null && echipament_curent[slotIndex].name == nume) //poarta ceva acolo
        {
            if (echipament_curent[slotIndex].tip == TipEchipament.ATAC)
            {
                Destroy(echipamente_instante[slotIndex]); //distrug instanta prefabului cu arma/potiunea din mana omului
                playerul.GetComponent<ShooterMechanics_script>().weapon = null;
                playerul.GetComponentInChildren<Animator>().SetBool("tine_arma", false);
            }
            else
            {
                masca_player.GetComponent<Renderer>().material.SetColor("_Color", culoare_default_masca);
                masca_player.GetComponent<Renderer>().material.SetFloat("_Glossiness", 0);
            }
            echipament_curent[slotIndex] = null;
        }
    }
    
}
