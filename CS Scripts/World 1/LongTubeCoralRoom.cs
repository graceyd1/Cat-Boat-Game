using Godot;
using System;
using System.Threading.Tasks;

public partial class LongTubeCoralRoom : Node2D
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
			await NextRoomCheck();
		}
	}
	
	private async Task NextRoomCheck() {
		Vector2 pos = GetNode<CharacterBody2D>("UnderwaterPlayer").Position;
		var GlobalSceneChange = GetNode<GlobalSceneChange>("/root/GlobalSceneChange");
		if (pos.Y > 188) {
			transitioning = true;
			await GlobalSceneChange.ChangeRoom(new Vector2(595, 20), "fish_room", false);
		}
		if (pos.X < -8) {
			transitioning = true;
			await GlobalSceneChange.ChangeRoom(new Vector2(720, 135), "jellyfish_room", false);
		}
	}
}
