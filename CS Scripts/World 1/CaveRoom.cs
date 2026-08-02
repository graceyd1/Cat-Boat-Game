using Godot;
using System;
using System.Threading.Tasks;

public partial class CaveRoom : Node2D
{
	private bool transitioning = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var bridge = GetNode<Sprite2D>("Bridge");
		if (GlobalScript.CQ("short") == "ExploreOcean")
		{
			bridge.Position = new Vector2(0, 300);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override async void _Process(double delta)
	{
		if (!transitioning)
		{
			await NextRoomCheck();
		}
	}

	private async Task NextRoomCheck() {
		var player = GetNode<CharacterBody2D>("GroundPlayer");
		var GlobalSceneChange = GetNode<GlobalSceneChange>("/root/GlobalSceneChange");
		Vector2 pos = player.Position;
		if (pos.X < -8) {
			transitioning = true;
			await GlobalSceneChange.ChangeRoom(new Vector2(270, 51), "enter_cave_room", true);
		}

	}

	//enter parva's house
	private async void OnEnterRoom(String roomName)
	{
		var player = GetNode<CharacterBody2D>("GroundPlayer");
		var GlobalSceneChange = GetNode<GlobalSceneChange>("/root/GlobalSceneChange");
		transitioning = true;

		await GlobalSceneChange.ChangeRoom(new Vector2(20, 140), "parva_house", true);
	}
}
