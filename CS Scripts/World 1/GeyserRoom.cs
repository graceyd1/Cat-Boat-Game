using Godot;
using System;
using System.Threading.Tasks;

public partial class GeyserRoom : Node2D
{
	private bool transitioning = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//move gates offscreen if already opened
		if (GlobalScript.GeyserOpened)
		{
			GetNode<Node2D>("TopGate").Position = new Vector2(-100, 0);
			GetNode<Node2D>("BottomGate").Position = new Vector2(-200, 0);
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

	private void OnTopGateOpened()
	{
		GetNode<AnimationPlayer>("Geyser/AnimationPlayer").Play("open_bottom_gate");
		GlobalScript.GeyserOpened = true;
	}

	private async Task NextRoomCheck() {
		var player = GetNode<CharacterBody2D>("UnderwaterPlayer");
		var GlobalSceneChange = GetNode<GlobalSceneChange>("/root/GlobalSceneChange");
		Vector2 pos = player.Position;
		if (pos.X < -8) {
			transitioning = true;
			if (pos.Y < 100)
			{
				await GlobalSceneChange.ChangeRoom(new Vector2(300, 72), "tall_tube_coral_room", false);
			}
			else
			{
				await GlobalSceneChange.ChangeRoom(new Vector2(620, 171), "fish_room", false);
			}
		}
	}
}
