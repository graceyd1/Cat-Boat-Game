//Most of this is the default CharacterBody2D code
using Godot;
using System;

public partial class GroundPlayer : Player
{
	public new float Speed = 150.0f;
	public const float JumpVelocity = -250.0f;
	public bool Climbing{get; set;} = false;

	public override void _Ready() {
		base._Ready();
		base.Speed = 150;
		base.Gravity = 0.002F;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!Climbing) {
			Vector2 velocity = Velocity;

			// Add the gravity.
			if (!IsOnFloor())
			{
				velocity += GetGravity() * (float)delta;
			}

			// Handle Jump.
			if (Input.IsActionJustPressed("move_up") && IsOnFloor())
			{
				velocity.Y = JumpVelocity;
			}

			// Get the input direction and handle the movement/deceleration.
			// As good practice, you should replace UI actions with custom gameplay actions.
			Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
			if (direction != Vector2.Zero)
			{
				velocity.X = direction.X * Speed;
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			}

			Velocity = velocity;
			MoveAndSlide();
		}
		else {
			var dir = Vector2.Zero;
			if (Input.IsActionPressed("move_up")) {
				dir.Y -= 1;
			}
			if (Input.IsActionPressed("move_down")) {
				dir.Y += 1;
			}
			if (Input.IsActionPressed("move_right")) {
				dir.X += 1;
				Climbing = false;
			}
			if (Input.IsActionPressed("move_left")) {
				dir.X -= 1;
				Climbing = false;
			}
			var animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
			if (dir.Length() > 0) {
				dir = dir.Normalized() * Speed;
				
				animatedSprite.Play();
			}
			else {
				animatedSprite.Stop();
			}
			Velocity = dir;
			MoveAndSlide();
		}
	}
}
