using UnityEngine;

public class CrayonPurple : Crayon {
    protected override CrayonColors.Color CrayonColor { get { return CrayonColors.Color.PURPLE; } }
    protected override GameObject CrayonObject { get { return this.gameObject; } }
}