using Godot;

public class BulletRequest
{
    public string Name;
    public PackedScene BulletScene;
    public BMLEmitter Emitter;
    public BulletRequest(BMLEmitter emitter, PackedScene bullet, string name)
    {
        this.Name = name;
        this.BulletScene = bullet;
        this.Emitter = emitter;
    }
}