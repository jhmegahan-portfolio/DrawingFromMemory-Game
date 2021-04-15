using UnityEngine;

public class CrayonBlue : Crayon {
    protected override CrayonColors.Color CrayonColor { get { return CrayonColors.Color.BLUE; } }
    protected override GameObject CrayonObject { get { return this.gameObject; } }
}