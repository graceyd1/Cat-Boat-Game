using Godot;
using System;

public partial class Flashlight : PointLight2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = true;
		Enabled = false;
		toggleCollision();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (GlobalScript.Inventory.Contains("Flashlight")) {
			if (Input.IsActionJustPressed("toggle_flashlight"))
			{
				Enabled = !Enabled;
				toggleCollision();
			}

			if (Enabled)
			{
				var mousePos = GetGlobalMousePosition();
				var characterPos = GlobalPosition;

				//get the angle from the character to the mouse
				var angle = Math.Atan2(mousePos.Y - characterPos.Y, mousePos.X - characterPos.X);

				Rotation = (float) angle;
			}
		}
	}

	private void toggleCollision()
	{
		var collisionShape = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");
		collisionShape.Disabled = !collisionShape.Disabled; 
	}
}
