using Godot;
using System;
using System.Threading.Tasks;

public partial class BobaShop : Node2D
{
	private TextBox dashT;
	private TextBox catssavaT;
	private AnimatedSprite2D csAnimation;
	private GroundPlayer player;
	private AnimationPlayer AniPlayer;
	private Sprite2D Pearl;
	private const int MAX_STORY_NUM = 2;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		dashT = GetNode<TextBox>("GroundPlayer/TextBox");
		catssavaT = GetNode<TextBox>("Catssava/TextBox");
		csAnimation = GetNode<AnimatedSprite2D>("Catssava");
		AniPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		player = GetNode<GroundPlayer>("GroundPlayer");
		Pearl = GetNode<Sprite2D>("Pearl");
		Pearl.Hide();
		csAnimation.FlipH = true;
		dashT.SetLabel("Dash");
		catssavaT.SetLabel("Catssava");
		dashT.Known(true);
		catssavaT.Known(true); 
		StartDialogue();
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
		player.InputEnabled = false;
		//Quest == Visit the boba shop and ask for brown sugar boba
		if (GlobalScript.CQ("short") == "MeetCatssava") {
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
		}
		//already finished quest 1: Visit the boba shop and ask for brown sugar boba
		else if (GlobalScript.CatssavaStoryNum == 0 && GlobalScript.NumPearls > 0) {
			var choice = await dashT.Ask("Should I present a pearl to Catssava?\n1. Yes\n2. No");
			if (choice == "1") {
				GlobalScript.NumPearls--;
				if (GlobalScript.NumPearls == 0) {
					GlobalScript.Inventory.Remove("Pearl");
				}
				await dashT.ShowText("I found a pearl while exploring the ocean! I thought you might be interested in it.");
				await catssavaT.ShowText("M-Me? Why that's very kind of you to think of me! May I see it? What flavor is it?");
				Pearl.Show();
				AniPlayer.Play("give_pearl");
				await ToSignal(AniPlayer, AnimationPlayer.SignalName.AnimationFinished);
				await catssavaT.ShowText("Dash...that's...not a tapioca pearl...");
				await catssavaT.ShowText("But it's beautiful. I love it.");
				await dashT.ShowText("Then you should have it.");
				await catssavaT.ShowText("Really? Wow, thank you so much Dash!");
				await catssavaT.ShowText("I feel like I've thanked you so many times recently.");
				if (GlobalScript.IsAfterQuest("GetBoat")) {
					await catssavaT.ShowText("You really are a true hero.");
					choice = await dashT.Ask("1. The townspeople are the true heroes 2. I should accept the praise");
					if (choice == "1") {
						await dashT.ShowText("You and the other cats of Bubbly Town are the true heroes.");
						await dashT.ShowText("I would never have gotten my new boat if not for you and Azucat.");
						await dashT.ShowText("And I even have to admit...Olive was a big help in teaching me to grow vines");
						await catssavaT.ShowText("You've been such a big help that I've nearly forgotten how new you are to the town.");
						await catssavaT.ShowText("Come back if you ever want to hear some town stories!");
						await catssavaT.ShowText("*winks* Even about Olive!");
						await dashT.ShowText("Well, there's no resisting that! Thanks, Catssava!");
					}
					else {
						await dashT.ShowText("There were times it was frightening, but it was also one of the greatest adventures I've had!");
						await dashT.ShowText("Coming from a sailor cat, that's saying a lot!");
						await dashT.ShowText("I'm just glad to have been able to help the cats of Bubbly Town on top of getting a new boat!");
						await catssavaT.ShowText("You'll probably be busy off on your other adventures then, but make sure to stop by!");
						await catssavaT.ShowText("I'd love to hang out and tell you some town stories!");
						await dashT.ShowText("I'd love that. Thanks, Catssava!");
					}
				}
				else {
					await catssavaT.ShowText("You can always drop by and I can tell you some town stories!");
					await dashT.ShowText("I can't sit around too much if I'm going to find your boba! But I'll definitely drop by afterwards!");
				}
				GlobalScript.CatssavaStoryNum++;
			}
		}
		else if (GlobalScript.CatssavaStoryNum > 0 && GlobalScript.CatssavaStoryNum % 1 == 0 && GlobalScript.IsAfterQuest("GetBoat")) {
			if (GlobalScript.CatssavaStoryNum <= MAX_STORY_NUM) {
				var choice = await catssavaT.Ask("Would you like to hear a story?\n1. Yes\n2. No");
				if (choice == "1") {
					await dashT.ShowText("Sure!");
					await TellStory();
				}
				else {
					await dashT.ShowText("Maybe another time!");
				}
			}	
		}
		else if (GlobalScript.IsAfterQuest("MeetCatssava"))
		{
			//maybe we can have a list of dialogue and pick a random one
			await catssavaT.ShowText("Oh, thank you so very much for helping me, Dash!");
		}
		player.InputEnabled = true;
	}
	
	public async Task TellStory() {
		switch (GlobalScript.CatssavaStoryNum) {
			case 1: await PirateStory(); break;
			case 2: await CatnipStory(); break;
		}
	}
	
	private async Task CatnipStory() {
		await catssavaT.ShowText("It was a nice day. Sunlight filtered through the waters. No one should by crying behind their shop counter, but there I was.");
		await catssavaT.ShowText("It was because of Olive. She was bitter towards me that day because a customer asked why she wasn't as nice as I was.");
		await catssavaT.ShowText("I decided I didn't want to mope on such a fine day. I would go and fix things.");
		await dashT.ShowText("Woah. You went to talk things out with Olive?");
		await catssavaT.ShowText("No. I went to find something that would cheer her up. I went on a mission to get more customers for her.");
		await catssavaT.ShowText("I needed to find her a plant she didn't already have. One that many cats would want.");
		await catssavaT.ShowText("So I went to the surface.");
		await dashT.ShowText("No way! You would do all that for Olive?");
		await catssavaT.ShowText("I suppose I wanted her to like me. After going to the surface, I found the most amazing plant I've ever smelled before.");
		await dashT.ShowText("Catnip?");
		await catssavaT.ShowText("None of us knew what it was, but most of our cats were absolutely enthralled with it. I gave it to Olive to sell in her shop.");
		await dashT.ShowText("She must have loved you for that!");
		await catssavaT.ShowText("Well, she was pleased at first...until her customers blamed her when the plants molded and rotted away.");
		await catssavaT.ShowText("She still thinks I purposely tried to ruin her business so I could steal her customers.");
		var choice = await dashT.Ask("1. I should try and fix the misunderstanding 2. Olive is Olive. She won't ever change.");
		if (choice == "1") {
			await dashT.ShowText("That's unfair. Olive never knew you were actually trying to help her.");
			await dashT.ShowText("I'll talk to her for you.");
			//half done, must go to Olive for other half
			GlobalScript.CatssavaStoryNum += 0.5;
		}	
		else {
			await dashT.ShowText("That's unfair, but I'm not surprised given that it's Olive.");
			await catssavaT.ShowText("Yes well, it's alright. I'm learning to not chase after approval.");
			await dashT.ShowText("Catssava, you're an amazing cat. True friends will accept you as you are!");
			await catssavaT.ShowText("You're right, Dash. I hope you consider me as your true friend.");
			GlobalScript.CatssavaStoryNum += 1;
		}
	}
	
	private async Task PirateStory() {
		await catssavaT.ShowText("I've heard you're off to have adventures on your new boat!");
		await catssavaT.ShowText("There is something I'm concerned about, though.");
		await catssavaT.ShowText("Once when I went to the surface to pick up my boba shipment, I was suddenly knocked out!");
		await catssavaT.ShowText("All I saw was a bright flashing sword.");
		await catssavaT.ShowText("When I came too, I'd been robbed of all my lychee popping boba!");
		await catssavaT.ShowText("There are strange dangers on the surface, Dash. Just make sure to be careful, ok?");
		await catssavaT.ShowText("Some of the cats might not be as nice as the cats here at Bubbly Town.");
		await dashT.ShowText("Don't worry, Catssava! I've faced Parva. I can handle lychee popping boba thieves!");
		GlobalScript.CatssavaStoryNum++;
	}
}
