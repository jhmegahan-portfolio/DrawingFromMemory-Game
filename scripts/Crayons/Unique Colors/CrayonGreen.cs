using UnityEngine;

public class CrayonGreen : Crayon {
    protected override CrayonColors.Color CrayonColor { get { return CrayonColors.Color.GREEN; } }
    protected override GameObject CrayonObject { get { return this.gameObject; } }
}