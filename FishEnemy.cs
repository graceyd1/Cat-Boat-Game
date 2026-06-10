using Godot;
using System;

public partial class FishEnemy : Node2D
{
	[Export]
	public int Speed{get; set;} = 60;

	[Export]
	public Boolean isOrange {get; set;} = true;

	private static int[] Velocities = {1, 0, -1, 0};

	private int idx;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		idx = 0;
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 velocity = Vector2.Zero;
		velocity.X = Velocities[idx];
		velocity *= Speed;

		Position += velocity * (float)delta;

		//animation (set isOrange in the Inspector)
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		if (isOrange)
		{
			animatedSprite2D.Animation = "orange";
		}
		else
		{
			animatedSprite2D.Animation = "pink";
		}

		if (velocity.X < 0)
		{
			animatedSprite2D.FlipH = true;
			animatedSprite2D.Play();
		}
		else if (velocity.X > 0)
		{
			animatedSprite2D.FlipH = false;
			animatedSprite2D.Play();
		}
		else
		{
			animatedSprite2D.Stop();
			//animatedSprite2D.FlipH = !animatedSprite2D.FlipH;
		}
	}

	private void OnTimerTimeout()
	{
		idx ++;
		if (idx > 3)
		{
			idx = 0;
		}
	}
}
