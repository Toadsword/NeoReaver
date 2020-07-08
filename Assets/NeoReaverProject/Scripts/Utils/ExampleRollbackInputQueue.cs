using System.Collections;
using System.Collections.Generic;
using Packages.EZRollback.Runtime.Scripts;

public class ExampleRollbackInputQueue : InputQueue
{
    const int numNetworkableActions = 5;
    
    public override void PrepareInput() {
        SetActionsValue();
    }

    private void SetActionsValue() {
        RollbackInputBaseActions actionsValue = new RollbackInputBaseActions(1);
        
        SetBitFromAction(InputActionManager.InputType.LEFT, ref actionsValue);
        SetBitFromAction(InputActionManager.InputType.RIGHT, ref actionsValue);
        SetBitFromAction(InputActionManager.InputType.UP, ref actionsValue);
        SetBitFromAction(InputActionManager.InputType.DOWN, ref actionsValue);
        SetBitFromAction(InputActionManager.InputType.SHOOT, ref actionsValue);

        actionsValue.horizontalValue = InputActionManager.GetAxisSByte(InputActionManager.AxisType.HORIZONTAL);
        actionsValue.verticalValue = InputActionManager.GetAxisSByte(InputActionManager.AxisType.VERTICAL);

        _baseActions.value = actionsValue;
    }

    void SetBitFromAction(InputActionManager.InputType inputType, ref RollbackInputBaseActions actionsValue) {
        actionsValue.SetOrClearBit((int)inputType, InputActionManager.GetInput(inputType));
    }
}
