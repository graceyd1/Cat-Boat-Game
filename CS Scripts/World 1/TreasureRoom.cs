using Godot;
using System;
using System.Threading.Tasks;

public partial class TreasureRoom : Node2D
{
	private bool transitioning = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (GlobalScript.CQ("short") == "ReturnBoba")
		{
			GetBobaCutscene(GetNode<Node2D>("GroundPlayer"));
		}
		else
		{
			GetNode<Node2D>("BOBA").Hide();
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

	public async void GetBobaCutscene(Node2D player)
	{
		var azucat = GetNode<Sprite2D>("Azucat");
		var catssava = GetNode<Sprite2D>("Catssava");
		catssava.Frame = 0;
		azucat.FlipH = true;
		var anim = GetNode<AnimationPlayer>("AnimationPlayer");

		var dashT = player.GetNode<TextBox>("TextBox");
		var catssavaT = catssava.GetNode<TextBox>("TextBox");
		var azucatT = GetNode<TextBox>("Azucat/TextBox");
		dashT.SetLabel("Dash");
		catssavaT.SetLabel("Catssava");
		azucatT.SetLabel("Azucat");
		dashT.Known(true);
		catssavaT.Known(true);
		azucatT.Known(true);

		if (player is Player p)
		{
			p.SetDisableMovement(true);	
		}

		var playerAnim = player.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		playerAnim.Animation = "walk_right";
		playerAnim.Play();
		anim.Play("dash_enters");
		await ToSignal(anim, AnimationPlayer.SignalName.AnimationFinished);
		playerAnim.Animation = "sit_right";

		await dashT.ShowText("Phew... Thank goodness I escaped the sea bunny!");
		await dashT.ShowText("Holy tapioca pearls that's a lot of boba.");

		playerAnim.Animation = "sit_left";

		anim.Play("cats_enter");
		await ToSignal(anim, AnimationPlayer.SignalName.AnimationFinished);
		azucat.FlipH = false;

		//todo: dialogue with catssava and azucat
		//help idk how to write dialogue
		await azucatT.ShowText("There it is!");
		await catssavaT.ShowText("Dash, you actually did it! You found the boba! Now my boba shop will have business again!");
		await dashT.ShowText("Yes - huff - the cat that took me down here admitted to stealing it from your shop.");
		await azucatT.ShowText("How dare that pesky cat steal our town's boba!!");
		await catssavaT.ShowText("What matters is that it's rightfully in our hands now.");
		await dashT.ShowText("Can I give some for Azucat to exchange for my boat?");
		await catssavaT.ShowText("Are you kidding? I would've never gotten it back if not for you. Take as much as you'd like.");
		await azucatT.ShowText("Perfect timing! I've got a brand new boat waiting for you in the other room, Dash!");
		await catssavaT.ShowText("Thank you so much, Dash!");

		//wowee you found the boba blah blah blah azucat has a new ship for dash
		//azucat picks up the entire boba!!!!!!!
		anim.Play("azucat_takes_the_boba");
		await ToSignal(anim, AnimationPlayer.SignalName.AnimationFinished);

		//both cats leave
		anim.Play("cats_leave");
		await ToSignal(anim, AnimationPlayer.SignalName.AnimationFinished);

		if (player is Player p2)
		{
			p2.SetDisableMovement(false);	
		}
		
		GlobalScript.QuestNum ++;
	}

	private async Task NextRoomCheck() {
		var player = GetNode<CharacterBody2D>("GroundPlayer");
		var GlobalSceneChange = GetNode<GlobalSceneChange>("/root/GlobalSceneChange");
		Vector2 pos = player.Position;
		if (pos.X < -8) {
			transitioning = true;
			await GlobalSceneChange.ChangeRoom(new Vector2(620, 126), "sea_bunny_room", false);
		}

	}
}
