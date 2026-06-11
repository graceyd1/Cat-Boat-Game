using Godot;
using System;
using System.Net;

public partial class TubeCoral : Node2D
{
	//create a signal for when the player should get pulled
	//with the velocity of the pull
	[Signal]
	public delegate void PullEventHandler(Vector2 velocity);

	//signal for when the player should stop getting pulled
	[Signal]
	public delegate void UnpullEventHandler();

	//possible directions
	public enum Direction
	{
		UP, DOWN, LEFT, RIGHT
	}

	//(change in gui) the direction of the tube coral's pull
	[Export]
	public Direction PullDirection {get; set;} = Direction.DOWN;

	//how strong the pull is
	[Export]
	public float Strength {get; set;} = 0.9f;
   
	private bool tubesOn = true;
   
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Godot.Timer>("TubeSwitchTimer").Start();
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	//when a body (the player) enters the AOE, emit a signal
	//with the velocity the player should be pulled in
	private void OnAOEBodyEntered(CharacterBody2D body)
	{
		//do we need to check if the body is the player? idk
		//add later - multiple directions of corals
		if (tubesOn) {
			Vector2 velocity = Vector2.Zero;
			switch (PullDirection)
			{
				case Direction.DOWN:
					velocity.Y = Strength;
					break;
				case Direction.RIGHT:
					velocity.X = Strength;
					break;
				case Direction.LEFT:
					velocity.X = -Strength;
					break;
				default:
					velocity.Y = -Strength;
					break;
			}
			
			EmitSignal(SignalName.Pull, velocity);
		}
	}

	private void OnAOEBodyExited(CharacterBody2D body)
	{
		//do we need to check if the body is the player? idk
		EmitSignal(SignalName.Unpull);
	}
   
	private void OnTimerEnd() {
		//if on, switch to off, if off, switch to on
		tubesOn = !tubesOn;

		//animation
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		if (tubesOn)
		{
			animatedSprite2D.Animation = "on";
		}
		else
		{
		   animatedSprite2D.Animation = "off";
		}

		Area2D area = GetNode<Area2D>("AOE");
		var insideAOE = area.GetOverlappingBodies();

		//if there is a player in the AOE when it turns on/off, trigger player entering/exiting
		if (insideAOE.Count > 0)
		{
			if (tubesOn)
			{
				OnAOEBodyEntered((CharacterBody2D) insideAOE[0]);
			}
			else
			{
				OnAOEBodyExited((CharacterBody2D) insideAOE[0]);
			}
		}
	}
}
