using Godot;

public partial class State : Node, IState
{

    public State parent = null;
    public StateMachine stateMachine;

    [Export] private NodePath stateMachinePath;

    public override async void _Ready()
    {
        stateMachine = GetNode<StateMachine>(stateMachinePath);
        await ToSignal(Owner, "ready");
        if (!GetParent().IsInGroup("stateMachine"))
        {
            parent = GetParent<State>();
        }
    }

    public virtual void UnhandledInput(InputEvent @event)
    {
        
    }

    public virtual void Process(double delta)
    {
        
    }

    public virtual void PhysicsProcess(double delta)
    {
        
    }

    public virtual void Enter(Godot.Collections.Dictionary msg)
    {
        
    }
    public virtual void Enter()
    {
        Enter(new Godot.Collections.Dictionary());
    }

    public virtual void Exit()
    {

    }

    public Node GetStateMachine(Node node)
    {
        if (node != null && !node.IsInGroup("stateMachine"))
        {
            GetStateMachine(node.GetParent());
        }
        return node;
    }

}