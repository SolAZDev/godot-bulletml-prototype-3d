using BulletMLLib.SharedProject;
using Godot;
public partial class BMLBullet : Bullet
{
    [Export] public Shape3D shape;
    public BMLEmitter emitter;
    public Node3D bulletNode;
    /// <summary>
    /// Lifetime in frames
    /// </summary>
    public int lifetime = int.MaxValue;
    public int damage = 1;
    Vector2 pos;
    bool vanished = false;
    public override float X
    {
        get => pos.X;
        set
        {
            pos.X = value;
            bool xy = false;
            if (emitter != null) { xy = emitter.UseXY; }
            GlobalPosition = new Vector3(pos.X, xy ? pos.Y : GlobalPosition.Y, xy ? GlobalPosition.Z : pos.Y);
        }
    }
    public override float Y
    {
        get => pos.Y;
        set
        {
            pos.Y = value;
            bool xy = false;
            if (emitter != null) { xy = emitter.UseXY; }
            GlobalPosition = new Vector3(pos.X, xy ? pos.Y : GlobalPosition.Y, xy ? GlobalPosition.Z : pos.Y);

        }
    }

    public bool Vanished
    {
        get => vanished;
        set
        {
            vanished = value;
            Visible = !value;
            ProcessMode = value ? Node.ProcessModeEnum.Inherit : Node.ProcessModeEnum.Disabled;
        }
    }

    public override void PostUpdate() {
        if(Vanished) 
        lifetime--;
        if(lifetime<=0) Vanished=true;
     }
}