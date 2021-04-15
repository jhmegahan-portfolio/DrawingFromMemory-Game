using UnityEngine;

public class CrayonYellow : Crayon {
    protected override CrayonColors.Color CrayonColor { get { return CrayonColors.Color.YELLOW; } }
    protected override GameObject CrayonObject { get { return this.gameObject; } }
}