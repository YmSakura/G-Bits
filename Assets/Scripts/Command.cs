using UnityEngine;

public interface ICommand
{
    void Execute(GameActor gameActor);
}

public class JumpCommand : ICommand
{
    public void Execute(GameActor gameActor)
    {
        gameActor.Jump();
    }
}

public class AttackCommand : ICommand
{
    public void Execute(GameActor gameActor)
    {
        gameActor.Attack();
    }
}

public class SwitchWeaponCommand : ICommand
{
    public void Execute(GameActor gameActor)
    {
        gameActor.SwitchWeapon();
    }
}

public class SwitchStatusCommand : ICommand
{
    public void Execute(GameActor gameActor)
    {
        gameActor.SwitchStatus();
    }
}