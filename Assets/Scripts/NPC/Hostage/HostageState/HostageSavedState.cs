using UnityEngine;

public class HostageSavedState : State<HostageController>
{

    public override void Enter(HostageController owner)
    {
        owner.NowHasSaved = true;
    }
}
