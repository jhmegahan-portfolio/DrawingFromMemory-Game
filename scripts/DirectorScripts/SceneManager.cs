using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {
    
    [SerializeField] GameObject Hospital;
    [SerializeField] GameObject Park;

    //Fake Scene Manager, just changes layers currently;

	public void NextScene() {
        if (Hospital.activeSelf) {
            Hospital.SetActive(false);
            Park.SetActive(true);
        } else {
            Park.SetActive(false);
            Hospital.SetActive(true);
        }
    }
}
