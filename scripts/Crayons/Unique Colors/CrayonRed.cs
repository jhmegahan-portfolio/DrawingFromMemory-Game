using UnityEngine;

public class CrayonRed : Crayon {
    protected override CrayonColors.Color CrayonColor { get { return CrayonColors.Color.RED; } }
    protected override GameObject CrayonObject { get { return this.gameObject; } }
}