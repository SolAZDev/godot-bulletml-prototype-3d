using Godot;
using Godot.Collections;

public partial class BMLEmitter: Node3D{
    [Export] public string BulletMLFile;
    [Export] public Node3D playerInstance;
    [Export] public bool UseXY = false;
    [Export] public Dictionary<string, PackedScene> bullets = new Dictionary<string, PackedScene> {
        ["default"] = null
    };

}