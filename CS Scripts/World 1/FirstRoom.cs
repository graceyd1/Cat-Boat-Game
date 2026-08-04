using Godot;
using System;
using System.Threading.Tasks;

public partial class FirstRoom : Node2D
{
	private bool transitioning = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var brokenBoat = GetNode<Sprite2D>("BrokenBoat");
		if (GlobalScript.CQ("short") == "GoToTown")
		{
			brokenBoat.Show();
		}
		else
		{
			brokenBoat.Hide();
		}
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
		if (pos.X > 508) {
			transitioning = true;
			await GlobalSceneChange.ChangeRoom(new Vector2(50, 518), "underwater_town", true);
		}
	}
}
