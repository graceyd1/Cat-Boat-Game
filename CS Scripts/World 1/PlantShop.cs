using Godot;
using System;
using System.Threading.Tasks;

public partial class PlantShop : Node2D
{
	private TextBox dText;
	private TextBox oText;
	private GroundPlayer classPlayer;
	public override void _Ready()
	{
		dText = GetNode<TextBox>("GroundPlayer/TextBox");
		oText = GetNode<TextBox>("Olive/TextBox");
		classPlayer = GetNode<GroundPlayer>("GroundPlayer");
		
		if (!GlobalScript.Inventory.Contains("Flashlight"))
		{
			FirstShopDialogue(GlobalScript.OliveVisitNum);
		}
		else
		{
			if (GlobalScript.CatssavaStoryNum == 2.5) {
				ClearMisunderstanding();
				GlobalScript.CatssavaStoryNum += 0.5;
			}
		}
		dText.SetLabel("Dash");
		oText.SetLabel("Olive");
		dText.Known(true);
		oText.Known(GlobalScript.OliveVisitNum > 0);

		GetNode<Hitbox>("Olive/Laser/Hitbox").SetDisabled(true);
		GetNode<CollisionShape2D>("Olive/Flashlight/Area2D/CollisionShape2D").Disabled = true;
	}
	
	private async void ClearMisunderstanding() {
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
	
	
	private async void FirstShopDialogue(int VisitNum) {
		classPlayer.InputEnabled = false;
		classPlayer.GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation = "sit_left";
		//delete the placeholder stuff and write the dialogue
		//who are you? you're new here? welcome to my shop?
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
			FlashlightShop();
		}
		else if (VisitNum == 1) {
			await oText.ShowText("Well, well, well. Look who decided to come back.");
			await oText.ShowText("Since you've already wasted my time, I won't bother giving you a second demonstration.");
			await oText.ShowText("If you didn't pay attention the first time, that's on you!");
			FlashlightShop();
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
				default: await oText.ShowText("It's your " + VisitNum + "th time here! Jeez, what do you want?!");
						break;
			}
			FlashlightShop();
		}
		GlobalScript.OliveVisitNum++;
	}

	private async void FlashlightShop()
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
				//eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
				///edit this one too?
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
				classPlayer.InputEnabled = true;
				await ToSignal(laser, AnimatedSprite2D.SignalName.AnimationFinished);

				laser.Animation = "laser";
				laser.Play();

				var player = GetNode<Player>("GroundPlayer");
				await ToSignal(player, Player.SignalName.Respawned);
				var GlobalScene = GetNode<GlobalSceneChange>("/root/GlobalSceneChange");
				await GlobalScene.ChangeRoom(new Vector2(40, 321), "underwater_town", true);
				// laser.Hide();
				// laserHitbox.SetDisabled(true);
			}
		}
		classPlayer.InputEnabled = true;
	}



	private async void OnExitRoom() {
		var FaderNode = GetNode<CanvasLayer>("/root/Fader");
		var GlobalScene = GetNode<GlobalSceneChange>("/root/GlobalSceneChange");
		if (FaderNode is Fader fader) {
			await fader.FadeIn(.7f);
		}
		
		await GlobalScene.ChangeRoom(new Vector2(40, 321), "underwater_town", true);
	}
}
