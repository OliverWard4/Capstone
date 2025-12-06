using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableHazard : MonoBehaviour
{

    [SerializeField] private Animator anim;

    public void Start()
    {
        anim.StopPlayback();
    }

    public void DestroyHazard()
    {
        anim.Play("Start");
        Destroy(this.gameObject); 

    }


}
