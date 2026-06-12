using Godot;
using System;

public partial class PlayerHealth : AnimatedSprite2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ResetHP();
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition = GetNode<Camera2D>("..").GetScreenCenterPosition() + new Vector2(-130, -70);
	}

	private async void OnPlayerHit(int hp)
	{
		if (hp == 1)
		{
			Animation = "full_hp_hit";
		}
		else
		{
			Animation = "one_hp_hit";
		}

		Play();
		await ToSignal(GetTree().CreateTimer(2.7f), SceneTreeTimer.SignalName.Timeout);
		if (hp == 2) {
			ResetHP();
		}
	}

	private void ResetHP()
	{
		Animation = "full_hp";
	}
}
