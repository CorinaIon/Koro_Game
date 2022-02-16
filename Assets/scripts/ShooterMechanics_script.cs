using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterMechanics_script : MonoBehaviour
{
    public Transform weapon;
    private Animator animator;
    public Transform righthand;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        //righthand = animator.GetBoneTransform(HumanBodyBones.RightHand); // nu a mers, a trebuit sa pun eu.
    }

    // Update is called once per frame
    void Update()
    {
        if(weapon != null)
        {
            weapon.position = righthand.position;
            weapon.rotation = righthand.rotation;
        }
    }
}
