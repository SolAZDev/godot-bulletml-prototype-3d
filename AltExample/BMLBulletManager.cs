using System.Collections.Generic;
using System.Linq;
using BulletMLLib.SharedProject;
using Godot;

public partial class BMLBulletManager:Node3D, IBulletManager{
    public static BMLBulletManager instance;
    public Node3D playerRef;
    public BMLBulletManager(){
        instance = this; 
    }
    Dictionary<string, List<BMLBullet>> bullets;
    Queue<KeyValuePair<BMLEmitter, string>> bulletQueue;
    
    public static BMLBulletManager GetInstance(){
        if(BMLBulletManager.instance!=null){ return BMLBulletManager.instance;}
        else { return new BMLBulletManager(); }
    }
    // We bending the spec with this one boyos
    public void AddToQueue(BMLEmitter emitter, string name) =>bulletQueue.Append(new KeyValuePair<BMLEmitter, string>(emitter,name));
    public IBullet CreateBullet()
    {
        // This will probaly have a race condition
        var bulletToSpawn = bulletQueue.First();
        var bullet = GetBulletNode(bulletToSpawn.Key,bulletToSpawn.Value);
        return bullet as IBullet;
    }

    public IBullet CreateTopBullet()
    {
        throw new System.NotImplementedException();
    }

    public Vector2 PlayerPosition(IBullet targettedBullet)
    {
        BMLBullet bul = targettedBullet as BMLBullet;
        return new Vector2(playerRef.GlobalPosition.X, bul.emitter.UseXY?playerRef.GlobalPosition.Y:playerRef.GlobalPosition.Z);
    }

    public void RemoveBullet(IBullet deadBullet)
    {
        (deadBullet as BMLBullet).Vanished=true;
    }

    public double Tier()
    {
        return 0; //TODO, really
    }

    public Node3D GetBulletNode(BMLEmitter emitter, string bulletName="default"){
        //This will probably cause trouble
        List<BMLBullet> bulletSet = bullets[emitter.Name+bulletName];
        BMLBullet bullet = null;
        bool newBullet=false;
        bool newEntry=false;
        if(bulletSet!=null){
            var bulletAvailable = bulletSet.Find(bl=>bl.Visible==false && bl.ProcessMode==Node.ProcessModeEnum.Disabled);
            if (bulletAvailable!=null) {
                bullet = bulletAvailable;
            } else{ newBullet=true; }
        } else {
            bulletSet = new List<BMLBullet>();
            newEntry = true;
            newBullet = true; //No Entry? Probably No Bullet Too.
        }
        if (newBullet) {
            var scene = ResourceLoader.Load<PackedScene>(emitter.bullets[bulletName].ResourcePath);
            //Old Instantiate
            // bullet = scene.Instantiate() as BMLBullet;
            bullet = new BMLBullet(this, emitter, bulletName, Mathf.Inf);

            bulletSet.Add(bullet);
        }
        if(newEntry){ bullets.Add(emitter.Name+bulletName, bulletSet); }
        return bullet;
    }

    void DisableAllBulletSets(){ foreach(var set in bullets.Keys) { DisableBulletSet(set); } }
    void DisableBulletSet(string setName) => bullets[setName].ForEach(b=>b.Vanished=true);
    void DeleteAllBulletSet(){ foreach(var set in bullets.Keys){ DeleteBulletSet(set); } }
    void DeleteBulletSet(string setName){
        bullets[setName].ForEach(b=>b.QueueFree());
        bullets.Remove(setName);
    }
    void ClearAll(){}
    void ColissionCheck(BMLBullet bullet){}
}