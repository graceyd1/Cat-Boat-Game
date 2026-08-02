using Godot;
using System;
using System.Threading.Tasks;

public partial class TallTubeCoralRoom : Node2D
{
	private bool transitioning = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
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
		var player = GetNode<CharacterBody2D>("UnderwaterPlayer");
		var GlobalSceneChange = GetNode<GlobalSceneChange>("/root/GlobalSceneChange");
		Vector2 pos = player.Position;
		if (pos.Y < -8) {
			transitioning = true;
			await GlobalSceneChange.ChangeRoom(new Vector2(124, 115), "enter_cave_room", true);
		}
		else if (pos.X > 328) {
			transitioning = true;
			await GlobalSceneChange.ChangeRoom(new Vector2(20, 84), "geyser_room", true);
		}
		else if (pos.X < -8)
		{
			transitioning = true;
			await GlobalSceneChange.ChangeRoom(new Vector2(480, 126), "vine_room", false);
		}
	}
}
