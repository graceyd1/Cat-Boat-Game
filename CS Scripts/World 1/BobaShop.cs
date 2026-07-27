using Godot;
using System;

public partial class BobaShop : Node2D
{
	private TextBox dashT;
	private TextBox catssavaT;
	private AnimatedSprite2D csAnimation;
	private GroundPlayer player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		dashT = GetNode<TextBox>("GroundPlayer/TextBox");
		catssavaT = GetNode<TextBox>("Catssava/TextBox");
		csAnimation = GetNode<AnimatedSprite2D>("Catssava");
		player = GetNode<GroundPlayer>("GroundPlayer");
		csAnimation.FlipH = true;
		StartDialogue();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private async void OnExitRoom() {
		var FaderNode = GetNode<CanvasLayer>("/root/Fader");
		var GlobalScene = GetNode<GlobalSceneChange>("/root/GlobalSceneChange");
		if (FaderNode is Fader fader) {
			await fader.FadeIn(.7f);
		}
		
		await GlobalScene.ChangeRoom(new Vector2(460, 170), "underwater_town", true);
	}
	
	public async void StartDialogue() {
		csAnimation.Animation = "sit";
		//Quest == Visit the boba shop and ask for brown sugar boba
		if (GlobalScript.CQ("short") == "MeetCatssava") {
			player.InputEnabled = false;
			await catssavaT.ShowText("Oh hi there, I’m Catssava, the shopkeeper here. What can I help you with?");
			await dashT.ShowText("I need some milk tea with tapioca boba, that’s all!");
			await catssavaT.ShowText("Oh dear, t-tapioca boba?!");
			await dashT.ShowText("That's right, Azucat told me it was fairly easy to get.");
			await catssavaT.ShowText("I-I-I'm really sorry, dear, but -");
			csAnimation.Animation = "cry";
			await catssavaT.ShowText("*[i]cries[/i]*");
			await dashT.ShowText("What's the matter? Is it something I said?");
			await catssavaT.ShowText("Well — this is embarrassing to say, since this is a boba shop…");
			await catssavaT.ShowText("But I'm all out of tapioca pearls!");
			await dashT.ShowText("Is there...a way to get more?");
			await catssavaT.ShowText("I get monthly shipments from the surface, but it's only the second of the month and it's all vanished!");
			await catssavaT.ShowText("Oh, if only I knew where it all went...");

			var choice = await dashT.Ask("1. I should help Catssava!\n2. Well, I need my boat fixed...");

			await dashT.ShowText("Catssava, let me help you find the tapioca pearls.");
			csAnimation.Animation = "sit";
			await catssavaT.ShowText("Really?! You'd do that for me? I'd be soo grateful- what's your name?");

			if (choice == "1") {
				await dashT.ShowText("My name is Dash. And I'd be happy to help; I can tell you love this shop and it certainly needs some boba!");
			}
			else {
				await dashT.ShowText("My name is Dash. Azucat won't give me a new boat until I get him tapioca, so it's only right for me to do this.");
			}
			await catssavaT.ShowText("In that case, you'll need a pass to leave town. I'll give you mine.");
			player.GetItem("Town Pass");
			GlobalScript.QuestNum++;
			player.InputEnabled = true;
		}
		//already finished quest 1: Visit the boba shop and ask for brown sugar boba
		else if (GlobalScript.IsAfterQuest("MeetCatssava"))
		{
			//maybe we can have a list of dialogue and pick a random one
			await catssavaT.ShowText("Oh, thank you so very much for helping me, Dash!");
		}
	}
}
