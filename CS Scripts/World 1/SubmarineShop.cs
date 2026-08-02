using Godot;
using System;
using System.Threading.Tasks;

public partial class SubmarineShop : Node2D
{
	private bool transitioning = false;
	TextBox pText;
	TextBox aText;
	private Player p;
	private Sprite2D azucat;
	private const int MAX_STORY_NUM = 2;
	private int EnterNum; //since upwall, enters Area2D twice when jump
	private Sprite2D Pearl;
	private bool DialogueTimeout;

	private bool Secret1;
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		Secret1 = false;
		pText = GetNode<TextBox>("GroundPlayer/TextBox");
		aText = GetNode<TextBox>("Azucat/TextBox");
		p = GetNode<GroundPlayer>("GroundPlayer");
		azucat = GetNode<Sprite2D>("Azucat");
		Pearl = GetNode<Sprite2D>("Pearl");
		Pearl.Hide();
		pText.SetLabel("Dash");
		aText.SetLabel("Azucat");
		pText.Known(true);
		aText.Known(GlobalScript.IsAfterQuest("MeetAzucat"));
		var sprite = GetNode<AnimatedSprite2D>("GroundPlayer/AnimatedSprite2D");

		if (p.Position.X > azucat.Position.X) {
			azucat.FlipH = true;
			sprite.Animation = "sit_left";
		}
		else {
			azucat.FlipH = false;
			sprite.Animation = "sit_right";
		}
		StartDialogue();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override async void _Process(double delta)
	{
		if (!transitioning) {
			
			await NextRoomCheck();
		}
	}
	public override void _Input(InputEvent @event) {
		var exit = GetNode<Area2D>("Door/ShopExit");
		if (@event.IsActionPressed("enter")) {
			if (!exit.OverlapsBody(p)) {
				if (DialogueTimeout) {
					StartDialogue();
				}
			}
		}
	}
	public async void OnEnterWorkLedge(Node2D body) {
		if (body is Player dash) {
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
	
	public async void StartDialogue() {
		DialogueTimeout = false;
		p.SetDisableMovement(true);
		//Quest 0: find mechanic to fix ship
		if (GlobalScript.CQ("short") == "MeetAzucat") {
			await aText.ShowText("Hello there! Welcome to Bubbly Town, our lively little town under the ocean!");
			await aText.ShowText("I'm the mechanic here. Name's Azucat. What's your name?");
			aText.Known(true);
			var choice = await pText.Ask("1. Let's skip the formalities and cut to the point\n2. Better to make a good impression!");
			if (choice == "1") {
				await pText.ShowText("Dash. But more importantly, what is my ship doing on top of your roof??"); //fumi misty edit this stuff pls if you want
				await aText.ShowText("Oh sorry. I thought it looked cool, so I patched it up and put it there."); 
				await aText.ShowText("Didn't know it was yours."); 
				await pText.ShowText("How did you-- My ship just crashed! I need it to go back to the surface...");
			}
			else {
				await pText.ShowText("I'm Dash. It's nice to meet you! Cool submarines you've got back there.");
				await aText.ShowText("Yes, they're great for cats who hate swimming! Makes it safer to explore the ocean too.");
				await aText.ShowText("They're 10,000 coins each. You interested?");
				await pText.ShowText("My ship just crashed, and I don't have much left.");
				await pText.ShowText("And I meant to ask, if that was my ship I saw on your roof?");
				await aText.ShowText("Oh sorry. I thought it looked cool, so I patched it up and put it there."); 
				await aText.ShowText("Didn't know it was yours."); 
				await pText.ShowText("I really need it back to return to the surface...I don't have much to exchange though.");
			}
			await aText.ShowText("How ‘bout let’s make a deal. You get some boba milk tea for me, and I’ll see what I can do ‘bout getting you a new boat.");
			await aText.ShowText("And make sure to get it with tapioca pearls! You can't go without the tapioca!");
			await aText.ShowText("Don’t worry, there's a boba shop nearby. This is the best deal you’ll get ‘round here.");
			choice = await pText.Ask("Accept deal?\n1. Yes \n2. No");
			if (choice == "2") {
				await pText.ShowText("I don't know if I can trust you.");
				await aText.ShowText("I’m the only one who can make boats around here, so it’s not like you have a choice.");
				await pText.ShowText("...Fine.");
			}
			else {
				await pText.ShowText("Call it a deal!");
				await aText.ShowText("Pleasure doing business with you.");
			}
			GlobalScript.QuestNum++; //next quest: 1. visit boba shop
		}
		else if (GlobalScript.AzucatStoryNum == 0 && GlobalScript.NumPearls > 0) {
			var choice = await pText.Ask("Should I present a pearl to Azucat?\n1. Yes\n2. No");
			if (choice == "1") {
				GlobalScript.NumPearls--;
				if (GlobalScript.NumPearls == 0) {
					GlobalScript.Inventory.Remove("Pearl");
				}
				await pText.ShowText("I found a little something during my adventures and wanted to give it to you!");
				var ani = GetNode<AnimationPlayer>("%AnimationPlayer");
				Pearl.Show();
				if (p.Position.X > azucat.Position.X) {
					ani.Play("pearl_roll_left");
				}
				else {
					ani.Play("pearl_roll_right");
				}
				await ToSignal(ani, AnimationPlayer.SignalName.AnimationFinished);
				await aText.ShowText("Wow, a pearl. These are quite rare. What would you like in exchange?");
				if (GlobalScript.IsAfterQuest("GetBoat")) {
					await pText.ShowText("You've already been a big help in getting me a new boat!");
					await pText.ShowText("But I wouldn't mind hearing a few stories!");
					await aText.ShowText("Of course mate, I'm free anytime you want to stop by!");
				}
				else {
					await pText.ShowText("You've already offered me a new boat, and that's all I really need!");
					await pText.ShowText("But I wouldn't mind hearing a few stories once I find the boba!");
					await aText.ShowText("Alrighty, make sure to stop by afterwards then!");
				}
				GlobalScript.AzucatStoryNum++;
			}
		}
		else if (GlobalScript.IsAfterQuest("GetBoat") && GlobalScript.AzucatStoryNum > 0 && GlobalScript.AzucatStoryNum <= MAX_STORY_NUM && GlobalScript.AzucatStoryNum % 1 == 0) {
			var choice = await aText.Ask("Would you like to hear a story?\n1. Yes\n2. No");
			if (choice == "1") {
				await pText.ShowText("Sure!");
				await TellStory();
			}
			else {
				await pText.ShowText("Maybe another time!");
			}
		}
		else if (GlobalScript.IsAfterQuest("MeetAzucat") && GlobalScript.IsBeforeQuest("GetBoat")) {
			await aText.ShowText("I trust that you're working on getting that boba for me?");
		}
		else if (GlobalScript.IsAfterQuest("GetBoat")) {
			await aText.ShowText("How's that new boat of yours holding up?");
			await pText.ShowText("Just fine, thanks!");
			await aText.ShowText("Impressive, that stand-off of yours with the seabunny.");
		}
		p.SetDisableMovement(false);
		await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
		DialogueTimeout = true;
	}
	
	private async Task TellStory() {
		switch (GlobalScript.AzucatStoryNum) {
			case 1: await MeetingStory(); break;
			case 2: await OtterStory(); break;
		}
	}
	
	private async Task MeetingStory() {
		await aText.ShowText("Let's see...ah yes. This isn't a story I tell often...");
		await aText.ShowText("It started when a certain cat walked into my shop. Brilliant cat, really.");
		await aText.ShowText("In the brains, that is.");
		await aText.ShowText("I asked this cat if he was looking to buy a submarine.");
		await aText.ShowText("He responded no, he wanted to know how it worked.");
		await aText.ShowText("We bonded over mechanical and scientific talks. Even hung out at Catssava's boba shop.");
		await aText.ShowText("I asked if he wanted to move into the town, and he considered it.");
		await aText.ShowText("But in the end, we grew apart. The cat was a loner, and after a final disagreement, he never came back to see me.");
		var choice = await pText.Ask("1. Did you regret it?\n2. Do you wish to see him again?");
		if (choice == "1") {
			await pText.ShowText("Did you regret it afterwards? Do you wish you two could make up?");
			await aText.ShowText("I'm afraid it isn't possible. I failed to see what kind of cat he really was at first.");
			await aText.ShowText("Selfish and stubborn. Never lets any cat rely on him and never relies on any cat.");
		}
		else {
			await pText.ShowText("Do you wish you could see him again? Maybe you could look for him.");
			await aText.ShowText("No, he isn't the same cat anymore. That cat that walked into my shop full of curiosity is gone.");
			await aText.ShowText("There's no way to bring him back.");
		}
		GlobalScript.AzucatStoryNum++;
	}
	
	private async Task OtterStory() {
		await aText.ShowText("Once there was a pair of sea otters that visited our town.");
		await aText.ShowText("Carsava nearly freaked out and tried to chase them away. We have a town rule: felines only.");
		await aText.ShowText("It was Catssava that saved them, that sweet cat. Pointed out that otters have whiskers.");
		await pText.ShowText("That's nice of her, but that doesn't make them felines...right?");
		await aText.ShowText("Well, at least it calmed Carsava down. The otters told us that they were having trouble finding clams to crack their mussels.");
		await aText.ShowText("We didn't have any clams, but Catssava showed them to her shop.");
		await aText.ShowText("When they came out, their arms were laden with boba pearls. They said it was the best thing they've ever tasted before!");
		await aText.ShowText("I offered them a submarine...but they weren't as interested.");
		await pText.ShowText("Otters probably enjoy swimming freely in the water, much more than cats. That's probably why!");
		await aText.ShowText("Yea, I suppose that could be it.");
		GlobalScript.AzucatStoryNum++;
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
		var GlobalSceneChange = GetNode<GlobalSceneChange>("/root/GlobalSceneChange");
		Vector2 pos = player.Position;

		if (pos.X > 330)
		{
			transitioning = true;
			await GlobalSceneChange.ChangeRoom(new Vector2(330, 525), "underwater_town", true);
		}
	}
	
	public async void OnExitRoom(string doorName) {
		var GlobalScene = GetNode<GlobalSceneChange>("/root/GlobalSceneChange");
		transitioning = true;
		if (doorName == "ShopExit")
		{
			await GlobalScene.ChangeRoom(new Vector2(231, 517), "underwater_town", true);
		}
	}
}
