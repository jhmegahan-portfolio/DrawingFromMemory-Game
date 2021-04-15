using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DirectorAccess : MonoBehaviour {
    public static Director app { get { return Director.Instance; } }
}

public class Director : Singleton<Director> {
    protected Director() { }

    public ButtonController buttonController { get; private set; }
    public BrushController brushController { get; private set; }

    private void Awake() {
        brushController = GetComponent<BrushController>();
        buttonController = GetComponent<ButtonController>();
        
        DontDestroyOnLoad(gameObject);
    }
}