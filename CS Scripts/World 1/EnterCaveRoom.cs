using Godot;
using System;
using System.Threading.Tasks;

public partial class EnterCaveRoom : Node2D
{
	private bool transitioning = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var label = GetNode<Label>("Label");
		if (GlobalScript.Inventory.Contains("flashlight"))
		{
			label.Show();
		}
		else
		{
			label.Hide();
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
		if (pos.X > 328) {
			transitioning = true;
			await GlobalSceneChange.ChangeRoom(new Vector2(10, 90), "cave_room", true);
		}
		else if (pos.Y > 140) {
			transitioning = true;
			//fall behind water
			GetNode<Player>("GroundPlayer").ZIndex = -5;
			await GlobalSceneChange.ChangeRoom(new Vector2(160, 20), "tall_tube_coral_room", false);
		}
	}
}
