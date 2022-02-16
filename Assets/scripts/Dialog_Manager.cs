using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Dialog_Manager : MonoBehaviour
{
    #region Singleton Dialog_M
    public static Dialog_Manager instanta;
    private void Awake()
    {
        instanta = this;
    }
    #endregion

    private Queue<string> sentences;
    public Text text_titlu;
    public Text text_continut;
    public Animator animatorul;
    public Text text_buton_jos;
    public bool este_deschis = false;
    //public float vitezaText //e buna cea de la return din corutina.
    public delegate void OnDialogClosed();
    public OnDialogClosed onDialogClosed; //cabutonul sa se inchida
    public GameObject butonNotificare;
    public bool questOpened = false;
    public bool reiaMiscarea;
    public string triggeredBy;

    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialog(Dialog dialog)
    {
        este_deschis = true;
        animatorul.SetBool("deschide_dialog", este_deschis);
        text_titlu.text = dialog.nume_personaj;
        text_buton_jos.text = "CONTINUE";
        reiaMiscarea = dialog.reiaMiscareaLaFinal;
        triggeredBy = dialog.triggeredBy;
        sentences.Clear();
        foreach (string s in dialog.sentences)
        {
            sentences.Enqueue(s);
        }
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            EndDialog();
            return;
        }
        string urm = sentences.Dequeue();
        if (sentences.Count == 0)
        {
            text_buton_jos.text = "CLOSE";
        }
        //text_continut.text = urm; //nu scriem tot textul deodata
        StopAllCoroutines();
        StartCoroutine(ScrieLitera(urm));

    }

    void EndDialog()
    {
        este_deschis = false;
        animatorul.SetBool("deschide_dialog", este_deschis);
        if (reiaMiscarea && onDialogClosed != null)
        {
            onDialogClosed.Invoke(); //adica functia PotiContinua din NPC_O_s daca e cazul
            //aici trebuie pusa altfel de animatie de fapt!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        }
        if(triggeredBy=="Notificare")
        {
            SetButonNotificare(false);
            //aici animatia de mai sus
        }
    }

    IEnumerator ScrieLitera(string sentence)
    {
        text_continut.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            text_continut.text += letter;
            yield return null;
        }
    }

    public void SetButonNotificare(bool stare)
    {
        butonNotificare.SetActive(stare);
    }

}
