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
	public int damage = 1;
	Vector2 pos;
	bool vanished = false;
	 Vector3 startPos = Vector3.Zero;
	public override float X
	{
		get => pos.X;
		set
		{
			pos.X = value;
			bool xy = false;
			if (emitter != null) { xy = emitter.UseXY; }
			// GlobalPosition = new Vector3(pos.X, xy ? pos.Y : GlobalPosition.Y, xy ? GlobalPosition.Z : pos.Y);
			GlobalPosition = new Vector3(startPos.X+pos.X, xy ? startPos.Y+pos.Y : GlobalPosition.Y, xy ? GlobalPosition.Z :startPos.Z+pos.Y);
			// GD.Print("X"+value+" GlobalP:"+GlobalPosition+" Nanme:"+Name);
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
			GlobalPosition = new Vector3(startPos.X+pos.X, xy ? startPos.Y+pos.Y : GlobalPosition.Y, xy ? GlobalPosition.Z :startPos.Z+pos.Y);
			// GlobalPosition = new Vector3(pos.X, xy ? pos.Y : GlobalPosition.Y, xy ? GlobalPosition.Z : pos.Y);
			// GD.Print("Y"+value+" GlobalP:"+GlobalPosition+" Nanme:"+Name); 

		}
	}

	[Export] public bool Vanished
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
		if(Vanished)  Lifetime--;
		if(Lifetime<=0) Vanished=true;
	 }

	public void SetStartPos(Vector3 pos) => startPos=pos;
}
