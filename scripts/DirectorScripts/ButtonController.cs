
using UnityEngine.UI;
using UnityEngine;

public class ButtonController : DirectorAccess {
    static private GameObject currentColor;
    static private CrayonButton currentColorButton;

    public void Start() {
        currentColor = GameObject.FindGameObjectWithTag("DefaultCrayon");
        Switch(currentColor);
    }

    public void SetSelected(GameObject go) {
        GameObject selectedColor = go;

        if (selectedColor != currentColor) {
            // disable prior selected button
            Switch(currentColor);

            // enable new selected button
            currentColor = selectedColor;
            Switch(currentColor);
        }
    }

    private void Switch(GameObject go) {
        currentColorButton = go.GetComponent<CrayonButton>();
        currentColorButton.selected = !currentColorButton.selected;
        currentColorButton.anim.SetBool("Selected", currentColorButton.selected);
    }
}
