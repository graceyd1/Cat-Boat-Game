using Godot;
using System;
using System.Data.SqlTypes;

public partial class Player : CharacterBody2D
{
	[Signal]
	public delegate void HitEventHandler(int hp);

	[Signal]
	public delegate void DiedEventHandler();

	[Signal]
	public delegate void RespawnedEventHandler();

	public int Speed{get; set;}
	
	public float Gravity{get; set;}
	
	public float Mass = 4.54f; //in kg

	public int hp;

	public bool respawning;
	
	public bool respawnFadingIn;

	//if true, player can't get hit
	public bool invulnerable;

	//used in cutscenes (implemented in the GroundPlayer and UnderwaterPlayer classes)
	private bool disableMovement;

	//player flashing animation (when hit)
	public bool Flash{get; set;}
	
	//determines player sitting position
	public bool FacingRight{get; set;}

	//stores velocity modifiers such as wind/tube coral pull
	public Vector2 VelocityModifier{get; set;}
	
	public bool InputEnabled{get;set;} = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		respawning = false;
		//ScreenSize = GetViewportRect().Size;
		VelocityModifier = Vector2.Zero;
		hp = 2; //2;
		invulnerable = false;
		Flash = false;
		
		var devMode = GetNode<DevMode>("/root/DevMode");
		//connect signal
		devMode.ChangeHP += DevChangeHP;
		//gravity = Gravity.Underwater; //todo - change to update based on the player's room
	}
	
	
	public void OnPromptUser(TextBox box, string prompt) {
		GetNode<TextEnterLabel>("Camera2D/TextEnterLabel").FadeIn(box, prompt);
	}
	
	
	private void DevChangeHP(int newHP) {
		hp = newHP;
		GD.Print(hp);
		GD.Print(newHP);
	}
	
	//player enteres hitbox
	private void OnHurtboxAreaEntered(Node2D area)
	{
		if (GetNode<Godot.Timer>("HurtTimer").TimeLeft == 0) //bc OnTimerTimeout isn't working
		{
			invulnerable = false;
		}
		if (!invulnerable)
		{
			GetHit();
		}
	}

	//get hit
	private void GetHit()
	{
		hp --;
		Flash = true;
		var hurtTimer = GetNode<Godot.Timer>("HurtTimer");
		hurtTimer.Start();

		if (hp <= 0 || 
		GetParent().Name == "CaveRoom" ||
		GetParent().Name == "LongTubeCoralRoom" ||
		GetParent().Name == "TallTubeCoralRoom") // making you respawn after hit in cave room
		{
			respawning = true;
			EmitSignal(SignalName.Died);
			Respawn();
			hp = 2;
		}

		//i-frames	
		invulnerable = true;
		EmitSignal(SignalName.Hit, hp);
	}
	
	//when invulnerablility ends
	//I don't know why but this method doesn't ever run for me

	private void OnHurtTimerTimeout()
	{
		GD.Print("Timeout");///
		invulnerable = false;
		Flash = false;

		var insideHurtbox =  GetNode<Area2D>("Hurtbox").GetOverlappingBodies();

		//if player is in hitbox when invulnerability ends
		if (insideHurtbox.Count > 0)
		{
			GetHit();
		}
	}
	
	//we need to make a list
	public async void Respawn() {
		respawning = true;
		respawnFadingIn = true;
		invulnerable = true;
		SetDisableMovement(true);
		var fader = GetNode<CanvasLayer>("/root/Fader");
		if (fader is Fader transition) {
			await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
			await transition.FadeIn(1.0f);
			
			Vector2 respawnPoint = Vector2.Zero;
			var room = GetParent().Name;
			if (room == "EnterCaveRoom") {
				//coorinates might change if room coordinates change
				respawnPoint = new Vector2(113, 122);
			}
			else if (room == "TubesArea") {
				respawnPoint = new Vector2(45, 123);
			} 
			else if (room == "FirstRoom" || room == "BoxRoom") {
				respawnPoint = new Vector2(10, 140);
			}
			else if (room == "SeabunnyBossRoom") {
				respawnPoint = new Vector2(100, 144);
			}
			else if (room == "UnderwaterTown") {
				respawnPoint = new Vector2(10, 518);
			}
			else if (room == "BoxRoom")
			{
				respawnPoint = new Vector2(10, 140);
			}
			else if (room == "FishRoom")
			{
				respawnPoint = new Vector2(35, 122);
			}
			else if (room == "LongTubeCoralRoom")
			{
				respawnPoint = new Vector2(276, 138);
			}
			else if (room == "JellyfishRoom")
			{
				respawnPoint = new Vector2(690, 138);
			}
			else if (room == "VineRoom")
			{
				respawnPoint = new Vector2(163, 126);
			}
			else if (room == "TallTubeCoralRoom")
			{
				respawnPoint = new Vector2(160, 557);
			}
			else if (room == "CaveRoom")
			{
				respawnPoint = new Vector2(25, 90);
			}
			else if (room == "SeaBunnyRoom")
			{
				respawnPoint = new Vector2(50, 232);
			}
			else
			{
				respawnPoint = new Vector2(20, 90);//default
			}

			GlobalPosition = respawnPoint;
			respawnFadingIn = false;
			FacingRight = true;
			await transition.FadeOut(1.0f);
		}
		invulnerable = false;
		respawning = false;
		disableMovement = false;
		EmitSignal(SignalName.Respawned);
	}
	public void SetVelocityModifier(Vector2 vel)
	{
		VelocityModifier = vel;
	}

	//removes player movement control without setting velocity to zero (for bouncing)
	public void SetDisableControl(Boolean disable)
	{
		disableMovement = disable;
	}

	//removes player movement control and sets velocity to zero (for cutscenes)
	public void SetDisableMovement(Boolean disable)
	{
		disableMovement = disable;
		if (disable)
		{
			Velocity = Vector2.Zero;

		}
	}
	public bool MovementIsDisabled()
	{
		return disableMovement;
	}
	
	public void GetItem(string item)
	{
		GetNode<AnimatedSprite2D>("Camera2D/Controls/ItemIcon").Animation = item;
		GetNode<AnimationPlayer>("Camera2D/Controls/AnimationPlayer").Play("item_get");
		if (!GlobalScript.Inventory.Contains(item)) {
			GlobalScript.AddItem(item);
		}
	}
	
	public void SetCameraDrag(string room) {
		var cam = GetNode<Camera2D>("Camera2D");
		if (room == "TallTubeCoralRoom") {
			GD.Print("HI");
			cam.DragVerticalEnabled = true;
			cam.SetDragMargin(Side.Top, 0.5f);
			cam.SetDragMargin(Side.Bottom, 0.5f);
		}
		else {
			cam.SetDragMargin(Side.Left, 0);
			cam.SetDragMargin(Side.Top, 0);
			cam.SetDragMargin(Side.Bottom, 0);
			cam.SetDragMargin(Side.Right, 0);
		}
	}
}
