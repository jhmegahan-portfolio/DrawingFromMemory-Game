using UnityEngine;

public class CrayonErase : Crayon {
    protected override CrayonColors.Color CrayonColor { get { return CrayonColors.Color.ERASE; } }
    protected override GameObject CrayonObject { get { return this.gameObject; } }
}