using UnityEngine;

public interface ICommand
{
    void Execute(GameActor gameActor);
}

public class AttackCommand : ICommand
{
    public void Execute(GameActor gameActor)
    {
        gameActor.Attack();
    }
}

public class UpAttackCommand : ICommand
{
    public void Execute(GameActor gameActor)
    {
        gameActor.UpAttack();
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

public class SprintCommand : ICommand
{
    public void Execute(GameActor gameActor)
    {
        gameActor.Sprint();
    }
}


