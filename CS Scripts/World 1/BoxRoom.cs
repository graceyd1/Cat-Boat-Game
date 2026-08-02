using Godot;
using System;
using System.Threading.Tasks;

public partial class BoxRoom : Node2D
{
	private bool transitioning = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override async void _Process(double delta)
	{
		if (!transitioning) {
			await nextRoomCheck();
		}
	}
	private async Task nextRoomCheck() {
		Vector2 pos = GetNode<CharacterBody2D>("UnderwaterPlayer").Position;
		var GlobalSceneChange = GetNode<GlobalSceneChange>("/root/GlobalSceneChange");
		if (pos.X < -8) {
			transitioning = true;
			await GlobalSceneChange.ChangeRoom(new Vector2(478, 499), "underwater_town", false);
		}
		if (pos.X > 508)
		{
			transitioning = true;
			await GlobalSceneChange.ChangeRoom(new Vector2(20, 127), "fish_room", false);
		}
	}
}
