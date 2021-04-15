using UnityEngine;

public abstract class Crayon : DirectorAccess {

    protected abstract CrayonColors.Color CrayonColor { get; }
    protected abstract GameObject CrayonObject { get; }

    public void AssignColor() {
        app.brushController.SetBrushColor(CrayonColor);
        app.buttonController.SetSelected(CrayonObject);
    }
}
