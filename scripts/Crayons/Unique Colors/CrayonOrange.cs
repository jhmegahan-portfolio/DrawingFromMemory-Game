using UnityEngine;

public class CrayonOrange : Crayon {
    protected override CrayonColors.Color CrayonColor { get { return CrayonColors.Color.ORANGE; } }
    protected override GameObject CrayonObject { get { return this.gameObject; } }
}