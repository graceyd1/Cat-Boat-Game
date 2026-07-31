using Godot;
using System;
using System.Threading.Tasks;

public partial class SubmarineShop : Node2D
{
	private bool transitioning = false;
	Node2D playerTextNode;
	Node2D azucatTextNode;
	private int EnterNum; //since upwall, enters Area2D twice when jump

	private bool Secret1;
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		Secret1 = false;
		playerTextNode = GetNode<Node2D>("GroundPlayer/TextBox");
		azucatTextNode = GetNode<Node2D>("Azucat/TextBox");
		var player = GetNode<GroundPlayer>("GroundPlayer");
		var azucat = GetNode<Sprite2D>("Azucat");
		var sprite = GetNode<AnimatedSprite2D>("GroundPlayer/AnimatedSprite2D");

		if (player.Position.X > azucat.Position.X) {
			azucat.FlipH = true;
			sprite.Animation = "sit_left";
		}
		else {
			azucat.FlipH = false;
			sprite.Animation = "sit_right";
		}
		StartDialogue(player);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override async void _Process(double delta)
	{
		if (!transitioning) {
			
			await NextRoomCheck();
		}
	}
	public async void OnEnterWorkLedge(Node2D body) {
		if (body is Player dash) {
			if (playerTextNode is TextBox pText && azucatTextNode is TextBox aText) {
				if (dash.Position.Y < 120) { //prevents glitch where it triggers when entering room
					if (EnterNum == 0) {
						EnterNum++;
					}
					else {
						dash.InputEnabled = false;
						await aText.ShowText("HEY! Get down from there! Mechanics only!");
						dash.Position = new Vector2(dash.Position.X, 132);
						dash.InputEnabled = true;
						EnterNum = 0;
					}
				}
			}
		}
	}
	
	public async void StartDialogue(Node2D player) {
		if (playerTextNode is TextBox pText && azucatTextNode is TextBox aText) {
			//Quest 0: find mechanic to fix ship
			if (GlobalScript.CQ("short") == "MeetAzucat") {
				if (player is Player p)
				{
					p.SetDisableMovement(true);
					/*var sprite = GetNode<AnimatedSprite2D>("GroundPlayer/AnimatedSprite2D");
					sprite.Animation = "sit_right";*/
				}

				await pText.ShowText("What is my ship doing on top of your roof??"); //fumi misty edit this stuff pls if you want
				await aText.ShowText("Oh sorry. I thought it looked cool, so I patched it up and put it there."); 
				await aText.ShowText("Didn't know it was yours."); 
				await pText.ShowText("How did you-- My ship just crashed! I need it to go back to the surface...");
				await aText.ShowText("How ‘bout let’s make a deal. You get some boba milk tea for me, and I’ll see what I can do ‘bout getting you a new boat.");
				await aText.ShowText("And make sure to get it with tapioca pearls! You can't go without the tapioca!");
				await aText.ShowText("Don’t worry, it’s pretty easy to get. Best deal you’ll get ‘round here.");
				await pText.ShowText("Where is...here?");
				await aText.ShowText("You're in Bubbly Town, our lively little town under the ocean! I'm the mechanic here. The name's Azucat.");
		
				string choice = await pText.Ask("Accept deal?\n1. Yes \n2. No");
				if (choice == "2") {
					await pText.ShowText("I don't know if I can trust you.");
					await aText.ShowText("I’m the only one who can make boats around here, so it’s not like you have a choice.");
					await pText.ShowText("...Fine.");
				}
				else {
					await pText.ShowText("Call it a deal!");
					await aText.ShowText("Pleasure doing business with you.");
				}

				//GetNode<Control>("EnterLabel").Hide();

				//make player go through entire dialogue if they exited the shop in the middle of it
				GlobalScript.QuestNum++; //next quest: 1. visit boba shop

				if (player is Player p2)
				{
					p2.SetDisableMovement(false);
				}
			}
			//should make method for this
			else if (GlobalScript.Inventory.IndexOf(GlobalScript.CQ("short")) > GlobalScript.Inventory.IndexOf("MeetAzucat")) {
				await aText.ShowText("I trust that you're working on getting that boba for me?");
			}
		}
	}

	private void OnSecretButton1Pressed()
	{
		if (!Secret1)
		{
			Secret1 = true;
		}
	}

	private void OnSecretButton2Pressed()
	{
		if (Secret1)
		{
			GetNode<AnimationPlayer>("Secret/AnimationPlayer").Play("laser");
			GlobalScript.Azulcat = true;
			Secret1 = false;
		}
	}

	private async Task NextRoomCheck() {
		var player = GetNode<CharacterBody2D>("GroundPlayer");
		var FaderNode = GetNode<CanvasLayer>("/root/Fader");
		var GlobalSceneChange = GetNode<GlobalSceneChange>("/root/GlobalSceneChange");
		Vector2 pos = player.Position;

		if (pos.X < 5) {
			transitioning = true;
			if (FaderNode is Fader fader) {
				await fader.FadeIn(.7f);
			}
			await GlobalSceneChange.ChangeRoom(new Vector2(325, 525), "underwater_town", false);
		}
		if (pos.X > 315)
		{
			transitioning = true;
			if (FaderNode is Fader fader) {
				await fader.FadeIn(.7f);
			}
			await GlobalSceneChange.ChangeRoom(new Vector2(330, 525), "underwater_town", true);
		}
	}
	
	public async void OnExitRoom(string doorName) {
		var FaderNode = GetNode<CanvasLayer>("/root/Fader");
		var GlobalScene = GetNode<GlobalSceneChange>("/root/GlobalSceneChange");
		transitioning = true;
		if (FaderNode is Fader fader) {
			await fader.FadeIn(.7f);
		}
		
		if (doorName == "ShopExit")
		{
			await GlobalScene.ChangeRoom(new Vector2(231, 517), "underwater_town", true);
		}
	}
}
