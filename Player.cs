using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public int Speed {get; set;} = 400;
	
	public Vector2 ScreenSize;

	//stores velocity modifiers such as wind/tube coral pull
	private Vector2 velocityModifier;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//ScreenSize = GetViewportRect().Size;
		velocityModifier = Vector2.Zero;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		var velocity = Vector2.Zero; //(0, 0)
		if (Input.IsActionPressed("move_right")) {
			velocity.X += 1;
		}
		if (Input.IsActionPressed("move_left")) {
			velocity.X -= 1;
		}
		if (Input.IsActionPressed("move_down")) {
			velocity.Y += 1;
		}
		if (Input.IsActionPressed("move_up")) {
			velocity.Y -= 1;
		}

		velocity = velocity.Normalized() + velocityModifier;
	

		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		if (velocity.Length() > 0) {
			//velocity = velocity * Speed;
			 velocity = velocity * Speed;
			//normalizing the velocity makes it so that the tube coral doesn't affect vertical movement
			//todo: fix fast diagonal movement
			
			animatedSprite2D.Play();
		}
		else {
			animatedSprite2D.Stop();
		}

		//Position += velocity * (float)delta;
		MoveAndCollide(velocity * (float)delta); //character2d movement
	}

	private void OnTubeCoralPull(Vector2 tubeVelocity)
	{
		velocityModifier = tubeVelocity;
	}

	//stop pulling the character when it leaves the AOE
	private void OnTubeCoralUnpull()
	{
		velocityModifier = Vector2.Zero;
	}
}
