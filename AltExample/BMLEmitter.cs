using System.Linq;
using BulletMLLib.SharedProject;
using Godot;
using Godot.Collections;

public partial class BMLEmitter: Node3D{
	[Export] public string BulletMLFile;
	[Export]public bool AutoFire=false;
	[Export] public Node3D playerInstance;
	[Export] public bool UseXY = false;
	[Export] public Dictionary<string, PackedScene> bullets = new Dictionary<string, PackedScene> {
		["default"] = null
	};
	BMLBullet topBullet;
	BulletPattern pattern;

	public override void _Ready(){
		pattern = new BulletPattern();
		pattern.ParseXML(BulletMLFile);
		if(AutoFire) StartShooting();
	}
	public PackedScene GetReferencedBullet(string name){
		GD.Print(bullets.Keys, name);
		PackedScene returnable =   bullets.ContainsKey(name)?bullets[name]:bullets.First(b=>b.Value!=null).Value;
		return returnable;
	}

	public void StartShooting(){
		BMLBulletManager manager = BMLBulletManager.GetInstance();
		manager.AddToQueue(this,bullets["default"], "default", true);
		topBullet = (BMLBullet)BMLBulletManager.GetInstance().CreateTopBullet();
		topBullet.InitTopNode(pattern.RootNode);
	}

}
