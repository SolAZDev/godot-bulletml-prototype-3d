using BulletMLLib.SharedProject;
using Godot;
public partial class BMLBullet : Bullet
{
    public BMLEmitter emitter;
    public Node3D bulletNode;
    /// <summary>
    /// Lifetime in frames
    /// </summary>
    public int lifetime = int.MaxValue;
    public int damage = 1;
    Vector2 pos;
    bool vanished = false;

    public BMLBullet(IBulletManager manager, BMLEmitter emitter, string bulletName, float lifetime) : base(manager) { 
        this.emitter = emitter;
    }
    public override float X
    {
        get => pos.X;
        set
        {
            pos.X = value;
            bool xy = false;
            if (emitter != null) { xy = emitter.UseXY; }
            bulletNode.GlobalPosition = new Vector3(pos.X, xy ? pos.Y : bulletNode.GlobalPosition.Y, xy ? bulletNode.GlobalPosition.Z : pos.Y);
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
            bulletNode.GlobalPosition = new Vector3(pos.X, xy ? pos.Y : bulletNode.GlobalPosition.Y, xy ? bulletNode.GlobalPosition.Z : pos.Y);

        }
    }

    public bool Vanished
    {
        get => vanished;
        set
        {
            vanished = value;
            bulletNode.Visible = !value;
            bulletNode.ProcessMode = value ? Node.ProcessModeEnum.Inherit : Node.ProcessModeEnum.Disabled;
        }
    }

    public void Init(BMLEmitter emitter, string bulletName, float lifetime)
    {
        this.emitter = emitter;
        var scene = BMLBulletManager.GetInstance().GetBulletNode(emitter, bulletName);
        Vanished=true; 
    }

    public override void PostUpdate() {
        if(Vanished) 
        lifetime--;
        if(lifetime<=0) Vanished=true;
     }
}