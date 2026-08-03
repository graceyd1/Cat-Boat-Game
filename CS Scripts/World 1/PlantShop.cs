using Godot;
using System;
using System.Threading.Tasks;

public partial class PlantShop : Node2D
{
	private TextBox dText;
	private TextBox oText;
	private GroundPlayer classPlayer;
	private bool DialogueTimeout;
	private Control interactLabel;
	private bool NoMoreDialogue;
	public override void _Ready()
	{
		dText = GetNode<TextBox>("GroundPlayer/TextBox");
		oText = GetNode<TextBox>("Olive/TextBox");
		classPlayer = GetNode<GroundPlayer>("GroundPlayer");
		dText.SetLabel("Dash");
		oText.SetLabel("Olive");
		dText.Known(true);
		oText.Known(GlobalScript.OliveVisitNum > 0);
		GetNode<Control>("DiedScreen").Hide();
		GetNode<Hitbox>("Olive/Laser/Hitbox").SetDisabled(true);
		GetNode<CollisionShape2D>("Olive/Flashlight/Area2D/CollisionShape2D").Disabled = true;
		interactLabel = GetNode<Control>("InteractLabel");
		interactLabel.Show();
		
		StartDialogue();
	}
	
	private async void StartDialogue() {
		DialogueTimeout = false;
		classPlayer.SetDisableMovement(true);
		interactLabel.Hide();
		classPlayer.GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation = "sit_left";
		if (!GlobalScript.Inventory.Contains("Flashlight"))
		{
			await FirstShopDialogue(GlobalScript.OliveVisitNum);
		}
		else if (GlobalScript.CatssavaStoryNum == 2.5)
		{
			await ClearMisunderstanding();
			GlobalScript.CatssavaStoryNum += 0.5;
		}
		NoMoreDialogue = (GlobalScript.Inventory.Contains("Flashlight") && GlobalScript.CatssavaStoryNum != 2.5);
		if (!NoMoreDialogue) {
			interactLabel.Show();
		}
		classPlayer.SetDisableMovement(false);
		await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
		DialogueTimeout = true;
	}
	
	public override void _Input(InputEvent @event) {
		var exit = GetNode<Area2D>("Door/ShopExit");
		if (@event.IsActionPressed("enter")) {
			if (!exit.OverlapsBody(classPlayer)) {
				if (DialogueTimeout) {
					StartDialogue();
				}
			}
		}
	}

	//display interact label above shopkeeper
	private void OnDoorAreaExited(Node2D player)
	{
		interactLabel.Position = new Vector2(78, 102);
		if (!NoMoreDialogue) {
			interactLabel.Show();
		}
	}
	
	private async Task ClearMisunderstanding() {
		await dText.ShowText("Olive -");
		await oText.ShowText("What is it? I will say you seem to be making good use of your flashlight.");
		await oText.ShowText("Now that's not saying much since it's a [i]flashlight[/i]. So I don't think you're quite ready for a plant yet.");
		await dText.ShowText("*mumbles* That's because...it's all you've given me...");
		await oText.ShowText("What was that?");
		await dText.ShowText("Oh nothing. I was actually here to tell you something.");
		await oText.ShowText("Well? Speak up! It had better not be a complaint about the flashlight.");
		await dText.ShowText("It's actually about Catssava.");
		await oText.ShowText("I'm telling you, that cat is not as nice as she seems. She tried to steal my customers before!");
		await dText.ShowText("That's the thing. You think that's what happened, but it's not.");
		await dText.ShowText("Catssava was trying to help your business. She told me so. She actually really cares about how you feel.");
		await oText.ShowText("She RUINED things! I was the talk of the town after everyone's plants ROTTED within days!");
		await dText.ShowText("She didn't know they would mold. Cats sometimes make mistakes. But everyone deserves a second chance.");
		await oText.ShowText("Okay, enough with the wise-talk. Now get out, you're holding up the line.");
		await dText.ShowText("*quietly* I don't see a line...");
	}
	
	
	private async Task FirstShopDialogue(int VisitNum) {
		if (VisitNum == 0) {
			await oText.ShowText("My oh my, a visitor. The last one tried to return their succulents after they withered.");
			await oText.ShowText("SUCCULENTS! No one ever appreciates the careful art of growing plants.");
			await oText.ShowText("They don't ever have the patience, and I doubt you'll be any different.");
			await oText.ShowText("So I won't be selling you any plants.");
			await oText.ShowText("Oh, I've forgotten to introduce myself. My name is 100% italian organic extra virgin olive oil.");
			oText.Known(true);
			await oText.ShowText("You may call me Olive.");
			await dText.ShowText("What can I buy then, if you won't sell me plants?");
			await oText.ShowText("I've got just the thing for you. A flashlight!");
			await dText.ShowText("How do I use it?");
			await oText.ShowText("You can control a flashlight with the mouse.");
			await oText.ShowText("It can also grow vines. Now I am going to demonstrate.");

			GetNode<Node2D>("Olive/Flashlight").Show();
			GetNode<CollisionShape2D>("Olive/Flashlight/Area2D/CollisionShape2D").Disabled = false;
			AnimatedSprite2D vineAnim = GetNode<AnimatedSprite2D>("GrowableVine/AnimatedSprite2D");
			await ToSignal(vineAnim, AnimatedSprite2D.SignalName.AnimationFinished);
			GetNode<Node2D>("Olive/Flashlight").Hide();

			await oText.ShowText("You'd better not be pot-headed enough to mess that up.");
			await FlashlightShop();
		}
		else if (VisitNum == 1) {
			await oText.ShowText("Well, well, well. Look who decided to come back.");
			await oText.ShowText("Since you've already wasted my time, I won't bother giving you a second demonstration.");
			await oText.ShowText("If you didn't pay attention the first time, that's on you!");
			await FlashlightShop();
		}
		else {
			switch (VisitNum) {
				case 2: await oText.ShowText("Come again to gaze at the plants I won't let you buy?");
						break;
				case 3: await oText.ShowText("I can't see why you choose to bother me when there are so many other cats you could bother.");
						break;
				case 4: await oText.ShowText("You must be the most indecisive cat I've met before.");
						break;
				case 5: await oText.ShowText("I don't know why I put up with you.");
						await oText.ShowText("I should put a sign banning all cats with the name of - what's your name?");
						break;
				case 6: await oText.ShowText("How very rude of you to ignore me that last encounter. Now I can't put up that sign.");
						break;
				default: await oText.ShowText("You've been here " + VisitNum + " times! Jeez, what do you want?!");
						break;
			}
			await FlashlightShop();
		}
		GlobalScript.OliveVisitNum++;
	}

	private async Task FlashlightShop()
	{
		await oText.ShowText("Do you want to buy the flashlight? It's 10 coins.");
		string choice = await dText.Ask("1. Buy the flashlight\n2. No thanks\n3. Steal the flashlight");
		if (choice == "1")
		{
			if (GlobalScript.Coins >= 10)
			{
				classPlayer.GetItem("Flashlight");
				GlobalScript.Coins -= 10;
				await oText.ShowText("Here's your flashlight. I've given you the most basic one. It only has one mode.");
				await oText.ShowText("Press F to toggle the flashlight and use the mouse to change it's direction.");
				await oText.ShowText("You can view your inventory in the ESC menu.");
				await dText.ShowText("You really have that little trust in me?");
				await oText.ShowText("Gotten yourself stranded by hitting a [i]rock[/i] I've heard. I don't have high hopes.");
			}
			else
			{
				await oText.ShowText("You don't even have [i]ten[/i] measly coins? What even are you doing in my shop to begin with?");
				await oText.ShowText("Tut-tut, wasting my time I see. What an awful customer, just like the rest of them.");
			}
		}
		else if (choice == "2") {
			if (GlobalScript.OliveVisitNum == 1) {
				await oText.ShowText("Tut-tut, wasting my time I see. What an awful customer, just like the rest of them.");
			}
		}
		else if (choice == "3") 
		{
			int success = GD.RandRange(0, 100); ///idk if I did this right
			if (success >= 99)
			{
				classPlayer.GetItem("Flashlight");
				await dText.ShowText("I stole a flashlight.");
			}
			else
			{
				//eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
				await oText.ShowText("Hey");
				AnimatedSprite2D laser = GetNode<AnimatedSprite2D>("Olive/Laser");
				Hitbox laserHitbox = laser.GetNode<Hitbox>("Hitbox");
				laserHitbox.SetDisabled(false);
				laser.Show();
				laser.Animation = "start_laser";
				laser.Play();
				await ToSignal(laser, AnimatedSprite2D.SignalName.AnimationFinished);

				laser.Animation = "laser";
				laser.Play();

				var player = GetNode<Player>("GroundPlayer");
				player.SetDisableRespawn(true);
				await ToSignal(player, Player.SignalName.Died);
				player.invulnerable = true;
				laser.Hide();
				laser.GetNode<Hitbox>("Hitbox").SetDisabled(true);

				var fader = GetNode<Fader>("/root/Fader");
				await fader.FadeIn(.7f);
				var diedScreen = GetNode<Control>("DiedScreen");
				diedScreen.Show();
				await fader.FadeOut(.7f);

				var button = diedScreen.GetNode<Button>("RespawnButton");
				await ToSignal(button, Button.SignalName.Pressed);
				
				await fader.FadeIn(.7f);
				diedScreen.Hide();
				await fader.FadeOut(.7f);
				player.SetDisableRespawn(false);
				player.invulnerable = false;
			}
		}
	}

	private async void OnExitRoom() {
		var GlobalScene = GetNode<GlobalSceneChange>("/root/GlobalSceneChange");
		
		await GlobalScene.ChangeRoom(new Vector2(40, 321), "underwater_town", true);
	}
}
