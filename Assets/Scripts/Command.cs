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

public class SwitchCommand : ICommand
{
    public void Execute(GameActor gameActor)
    {
        gameActor.SwitchWeapon();
    }
}
