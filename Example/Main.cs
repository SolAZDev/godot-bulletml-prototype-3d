using System.Collections.Generic;
using System.IO;
using BulletMLLib.SharedProject;
using Godot;

namespace bulletmltemplate;

public partial class Main : Node3D
{
	[Export]
	private PackedScene playerScene;

	[Export]
	private PackedScene bulletScene;

	[Export]
	public Node3D playerInstance; // TODO: PlayerManager
	[Export] public Label label;

	// TODO: GameManager/Globals
	// public static float ViewportWidth => Instance.GetViewportRect().Size.X;
	// public static float ViewportHeight => Instance.GetViewportRect().Size.Y;
	public static Main Instance { get; private set; }

	private readonly List<BulletPattern> myPatterns = new();

	private MoveManager moveManager;

	private int currentPattern;
	private Mover topLevelBullet;

	public Main()
	{
		Instance = this;
	}

	private Vector2 GetPlayerPosition()
	{
		return new Vector2(playerInstance.Position.X, playerInstance.Position.Z);
	}

	public override void _Ready()
	{
		base._Ready();

		moveManager = new(GetPlayerPosition, bulletScene);

		GameManager.GameDifficulty = () => 1.0f;

		foreach (var source in Directory.GetFiles("./samples", "*.xml"))
		{
			// load the pattern
			var pattern = new BulletPattern();
			pattern.ParseXML(source);
			myPatterns.Add(pattern);
		}

		AddBullet();

		// Add a dummy player sprite
		// var scene = ResourceLoader.Load<PackedScene>(playerScene.ResourcePath);
		// if (scene.Instantiate() is not Sprite2D player)
		// 	return;
		// playerInstance = player;
		// player.Position = new(GetViewportRect().Size.X / 2f, GetViewportRect().Size.Y - 100f);
		// AddChild(player);
	}

	public override void _Process(double bigDelta)
	{
		var delta = (float)bigDelta;
		base._Process(delta);

		// var label = GetNode<Label>("Control/VBoxContainer/PlayerPositionLabel");
		// label.Text = $"Player: ({playerInstance.Position.X}, {playerInstance.Position.Y})";

		if (Input.IsActionJustPressed("ui_select"))
		{
			currentPattern++;

			AddBullet();

			return;
		}

		moveManager.Update(delta);

		// TODO: physics?

		moveManager.PostUpdate();
	}

	private void AddBullet()
	{
		label.Text = $"Pattern: {myPatterns[currentPattern % myPatterns.Count].Filename}";
		label.Show();

		// clear out all the bullets
		foreach (var child in GetChildren())
		{
			if (child is Player) 
				continue; // HACK: the player

			// TODO: object pooling
			(child as Node3D)?.QueueFree();
		}

		moveManager.Clear();

		// add a new bullet in the center of the screen (ish)
		topLevelBullet = (Mover)moveManager.CreateTopBullet();
		topLevelBullet.Position = new Vector2(Position.X, Position.Z);
		// topLevelBullet.Position = new(
		// 	GetViewportRect().Size.X / 2f,
		// 	GetViewportRect().Size.Y / 2f - 100f
		// );
		topLevelBullet.InitTopNode(myPatterns[currentPattern % myPatterns.Count].RootNode);
	}
}
