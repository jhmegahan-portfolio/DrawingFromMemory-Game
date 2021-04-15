using UnityEngine;

public class CrayonBrown : Crayon {
    protected override CrayonColors.Color CrayonColor { get { return CrayonColors.Color.BROWN; } }
    protected override GameObject CrayonObject { get { return this.gameObject; } }
}