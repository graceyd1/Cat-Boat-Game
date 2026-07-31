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

	private InteractArea InteractArea;

	private Vector2 StartPos;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		StartPos = Position;
		Visible = true;
		idx = 0;
		InteractArea = GetNodeOrNull<InteractArea>("InteractArea");
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Visible)
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

		if (InteractArea != null && InteractArea.GetOverlappingBodies().Count > 0)
		{
			InteractArea.ResetLabelPos();
		}
	}

	private void OnTimerTimeout()
	{
		if (Visible)
		{
			idx ++;
			if (idx > 3)
			{
				idx = 0;
			}
		}
	}

	public void Disable()
	{
		Visible = false;
		GetNode<Timer>("Timer").Stop();
		GetNode<CollisionShape2D>("Hitbox/HitboxShape").Disabled = true;
		var area = GetNodeOrNull<InteractArea>("InteractArea");
		if (area != null)
		{
			area.Interactable(false);
		}
		
	}

	public void Enable()
	{
		idx = 0;
		Position = StartPos;
		Visible = true;
		GetNode<Timer>("Timer").Start();
		GetNode<CollisionShape2D>("Hitbox/HitboxShape").Disabled = false;
		var interactArea = GetNodeOrNull<Area2D>("InteractArea");
		if (interactArea != null && interactArea is InteractArea area)
		{
			area.Interactable(true);
		}
	}
}
