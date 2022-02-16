using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger_script : MonoBehaviour
{
    public Dialog dialog;

    public void TriggerDialog()
    {
        Dialog_Manager.instanta.StartDialog(dialog);
    }

}
