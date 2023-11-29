using BulletMLLib.SharedProject;
using Godot;

namespace bulletmltemplate;

/// <summary>
/// Mover - represents a bullet in game, moved by the MoveManager.
/// </summary>
public partial class Mover : Bullet
{
	private bool used;
	private Node ParentNode { get; set; }
	private Node3D BulletNode { get; set; }

	public Mover(IBulletManager myBulletManager)
		: base(myBulletManager) { }

	public override void PostUpdate()
	{
		// something something out of bounds
		// if (X < 0f || X > Main.ViewportWidth || Y < 0f || Y > Main.ViewportHeight)
		// {
		// 	Used = false;
		// }
		if(Main.Instance.GlobalPosition.DistanceSquaredTo(new Vector3(Position.X, BulletNode.Position.Y,Position.Y))>800){
			Used=false;
		}
	}

	public override float X
	{
		get => Position.X;
		set
		{
			var position = Position;
			position.X = value;
			Position = position;

			BulletNode.Position = new Vector3(Position.X, BulletNode.Position.Y,Position.Y);
		}
	}

	public override float Y
	{
		get => Position.Y;
		set
		{
			var position = Position;
			position.Y = value;
			Position = position;
			GD.Print(Position);
			BulletNode.Position = new Vector3(Position.X, BulletNode.Position.Y,Position.Y);
		}
	}

	public Vector2 Position { get; set; }

	public bool Used
	{
		get => used;
		set
		{
			used = value;
			BulletNode.Visible = value;
		}
	}

	public void Init(PackedScene bulletScene)
	{
		ParentNode = Main.Instance;
		var scene = ResourceLoader.Load<PackedScene>(bulletScene.ResourcePath);
		BulletNode = scene.Instantiate() as Node3D;
		ParentNode.AddChild(BulletNode);

		Used = true;
	}
}
