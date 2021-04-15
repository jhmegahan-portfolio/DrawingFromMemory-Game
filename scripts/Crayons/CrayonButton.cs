using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrayonButton : MonoBehaviour {
    public Animator anim;
    public bool selected;
    
    void Awake () {
        anim = GetComponent<Animator>();
        selected = false;
	}
}
