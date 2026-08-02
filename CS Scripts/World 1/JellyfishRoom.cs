using Godot;
using System;
using System.Threading.Tasks;

public partial class JellyfishRoom : Node2D
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

	private async void OnBlueCatInteract()
	{
		var bArea = GetNode<Node2D>("BlueCat/InteractArea");
		var bText = bArea.GetNode<TextBox>("TextBox");
		bText.SetLabel("blue cat");
		bText.Known(true);
		bText.DisableInteractArea();
		await bText.ShowText("I'm blue cat");
		bText.EnableInteractArea();

	}

	private void OnCaveCameraTriggerEntered(Node2D player)
	{
		var camera = player.GetNode<Camera2D>("Camera2D");
		camera.PositionSmoothingEnabled = true;
		camera.PositionSmoothingSpeed = 5.0f;
		if (camera.LimitBottom == 180)
		{
			camera.SetLimit(Side.Bottom, 220);
			camera.GlobalPosition = new Vector2(camera.GlobalPosition.X, 130);
		}
	}

	private void OnCaveCameraTriggerExited(Node2D player)
	{
		var camera = player.GetNode<Camera2D>("Camera2D");
		camera.SetLimit(Side.Bottom, 180);
	}

	/*
	private void OnCaveCameraTrigger2Entered(Node2D player)
	{
		var camera = player.GetNode<Camera2D>("Camera2D");
		if (camera.LimitBottom == 220)
		{
			camera.SetLimit(Side.Bottom, 180);
		}
	} */

	private void OnSecretWallTriggerBodyEntered(Node2D player)
	{
		var wall = GetNode<Sprite2D>("SecretWall");
		if (wall.Visible)
		{
			wall.Hide();
		}
		else
		{
			wall.Show();
		}
	}
	
	private void OnBreakAOEEntered(Node2D player)
	{
		if (player is Player p)
		{
			if (p.MovementIsDisabled())
			{
				var rock = GetNode<Sprite2D>("BreakableRock");
				rock.Position = new Vector2(rock.Position.X, rock.Position.Y - 100);
			}
		}	
	}

	private async Task NextRoomCheck() {
		Vector2 pos = GetNode<CharacterBody2D>("UnderwaterPlayer").Position;
		var GlobalSceneChange = GetNode<GlobalSceneChange>("/root/GlobalSceneChange");
		if (pos.X > 748) {
			transitioning = true;
			await GlobalSceneChange.ChangeRoom(new Vector2(20, 135), "long_tube_coral_room", true);
		}
		else if (pos.Y < -8) {
			transitioning = true;
			await GlobalSceneChange.ChangeRoom(new Vector2(163, 126), "vine_room", true);
		}
	}
}
