using System.Collections.Generic;
using System.Linq;
using BulletMLLib.SharedProject;
using Godot;

public partial class BMLBulletManager:Node3D, IBulletManager{
	public static BMLBulletManager instance;
	[Export] public Node3D playerRef;
	public BMLBulletManager(){
		instance = this; 
	}
	Dictionary<PackedScene, List<BMLBullet>> bulletSets;
	Dictionary<PackedScene, List<BMLBullet>> topBullets;
	Queue<BulletRequest> bulletQueue;
	Queue<BulletRequest> topBulletQueue;
	
	[Export]public float SpaceSpeed {
		get { return speedScale; }
		set {
			speedScale=value;
			foreach(var set in bulletSets){
				set.Value.ForEach(bullet=>bullet.Scale=speedScale);
			}
			foreach(var set in topBullets){
				set.Value.ForEach(bullet=>bullet.Scale=speedScale);
			}
		}
	}
	float speedScale=1;
	[Export]public float TimeSpeed {
		get { return timeSpeed; }
		set {
			timeSpeed=value;
			foreach(var set in bulletSets){
				set.Value.ForEach(bullet=>bullet.TimeSpeed=timeSpeed);
			}
			foreach(var set in topBullets){
				set.Value.ForEach(bullet=>bullet.TimeSpeed=timeSpeed);
			}
		}
	}
	float timeSpeed = 1;

	public override void _Ready(){
		instance = this; 
		bulletQueue = new Queue<BulletRequest>();
		topBulletQueue = new Queue<BulletRequest>();
		bulletSets = new Dictionary<PackedScene, List<BMLBullet>>();
		topBullets = new Dictionary<PackedScene, List<BMLBullet>>();
		GameManager.GameDifficulty = () => 1.0f;
	}
	public static BMLBulletManager GetInstance(){
		if(BMLBulletManager.instance!=null){ return BMLBulletManager.instance;}
		else { return new BMLBulletManager(); }
	}

	public override void _Process(double delta){
		GD.Print(topBullets.Count);
		foreach(var set in bulletSets){
			set.Value.ForEach(bullet => {
				bullet.Update();
				// TODO: PhysicsServer check
			});
		}
		foreach(var set in topBullets){
			set.Value.ForEach(bullet=>{
				bullet.Update();
			});
		}
	}

	// We bending the spec with this one boyos
	public void AddToQueue(BMLEmitter emitter, PackedScene refScene, string name, bool top=false) {
		GD.Print("Adding Request from "+emitter.Name);
		(top?topBulletQueue:bulletQueue).Enqueue(new BulletRequest(emitter,refScene,name));
		GD.Print("Total in Queue: "+(top?topBulletQueue:bulletQueue).Count);
	}

	public IBullet CreateBullet()
	{
		// This will probaly have a race condition
		var bulletToSpawn = bulletQueue.Dequeue();
		var bullet = GetBulletNode(bulletToSpawn.Emitter,bulletToSpawn.BulletScene,bulletToSpawn.Name);
		bullet.GlobalPosition = bulletToSpawn.Emitter.GlobalPosition;
		(bullet as Bullet).Scale=speedScale;
		(bullet as Bullet).TimeSpeed=timeSpeed;
		return bullet as IBullet;
	}

	public IBullet CreateTopBullet()
	{
		// This will probaly have a race condition
		var bulletToSpawn = topBulletQueue.Dequeue();
		var bullet = GetBulletNode(bulletToSpawn.Emitter,bulletToSpawn.BulletScene,bulletToSpawn.Name, true);
		bullet.GlobalPosition = bulletToSpawn.Emitter.GlobalPosition;
		return bullet as IBullet;
	}

	public Vector2 PlayerPosition(IBullet targettedBullet)
	{
		BMLBullet bul = targettedBullet as BMLBullet;
		return new Vector2(playerRef.GlobalPosition.X, bul.emitter.UseXY?playerRef.GlobalPosition.Y:playerRef.GlobalPosition.Z);
	}

	public void RemoveBullet(IBullet deadBullet) => (deadBullet as BMLBullet).Vanished=true;

	public double Tier()
	{
		return 0; //TODO, really, how do we calculate this?
	}

	public Node3D GetBulletNode(BMLEmitter emitter, PackedScene bulletRef, string bulletName="", bool topBullet=false){
		//This will probably cause trouble
		PackedScene sceneRef = bulletRef!=null?bulletRef:emitter.GetReferencedBullet(bulletName);
		List<BMLBullet> refferableSet = null;
		(topBullet?topBullets:bulletSets).TryGetValue(sceneRef, out refferableSet);
		BMLBullet bullet = null;
		bool newBullet=false;
		bool newEntry=false;
		if(refferableSet!=null){
			var bulletAvailable = refferableSet.Find(bl=>bl.Visible==false && bl.ProcessMode==Node.ProcessModeEnum.Disabled);
			if (bulletAvailable!=null) bullet = bulletAvailable;
			else newBullet=true; 
		} else {
			refferableSet = new List<BMLBullet>();
			newEntry = true;
			newBullet = true; //No Entry? Probably No Bullet Too.
		}
		if (newBullet) {
			var scene = ResourceLoader.Load<PackedScene>(sceneRef.ResourcePath);
			sceneRef = scene;
			GD.Print(sceneRef);
			bullet = scene.Instantiate() as BMLBullet;
			this.AddChild(bullet);
			bullet.MyBulletManager = this;
			bullet.emitter = emitter;
			refferableSet.Add(bullet);
		}
		if(newEntry){ (topBullet?topBullets:bulletSets).Add(sceneRef, refferableSet); }
		return bullet;
	}

	void DisableAllBulletSets(){ foreach(var set in bulletSets.Keys) { DisableBulletSet(set); } }
	void DisableBulletSet(PackedScene setName) => bulletSets[setName].ForEach(b=>b.Vanished=true);
	void DeleteAllBulletSet(){ foreach(var set in bulletSets.Keys){ DeleteBulletSet(set); } }
	void DeleteBulletSet(PackedScene setName){
		bulletSets[setName].ForEach(b=>b.QueueFree());
		bulletSets.Remove(setName);
	}
	void ClearAll(){
		
	}
	void ColissionCheck(BMLBullet bullet){
		
	}

	void Process(float delta){
		foreach(var set in bulletSets){
			set.Value.ForEach(b=>{
				b.Update();
			});
		}
	}

}
