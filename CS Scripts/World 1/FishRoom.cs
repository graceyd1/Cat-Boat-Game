using Godot;
using System;
using System.Threading.Tasks;

public partial class FishRoom : Node2D
{
	[Export]
	public int numCollectableFish {get; set;} = 1;
	private bool transitioning = false;
	private bool playingMinigame = false;
	private Node2D playerTextNode;
	private Node2D iceCreamTextNode;
	private int fishCollected;
	private Godot.Collections.Array<Node> CollectableFish;
	private Node2D Clam;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Label>("UnderwaterPlayer/MinigameTime").Visible = false;
		playerTextNode = GetNode<Node2D>("UnderwaterPlayer/TextBox");
		iceCreamTextNode = GetNode<Node2D>("IceCreamAreaSprite/InteractArea/TextBox");
		CollectableFish = GetNode<Node2D>("CollectableFish").GetChildren();
		fishCollected = 0;

		Clam = GetNode<Node2D>("Clam0");
		Clam.GetNode<InteractArea>("InteractArea").Interactable(false);
		if (GlobalScript.FishGameHighScore > CollectableFish.Count)
		{
			Clam.Show();
		}
		else
		{
			Clam.Hide();
		}

		//disable collectable fish
		for (int i = 0; i < CollectableFish.Count; i ++)
		{
			if (CollectableFish[i] is FishEnemy fish)
			{
				fish.Disable();
			}
		}

		UpdateHighScore();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override async void _Process(double delta)
	{
		if (!transitioning) {
			await NextRoomCheck();
		}
	}

	//triggered by interact area signal
	private async void StartDialogue()
	{
		if (playerTextNode is TextBox pText && iceCreamTextNode is TextBox iText)
		{
			iText.DisableInteractArea();
			await iText.ShowText("yo! do you want to play a minigame for coins?");
			
			var choice = await pText.Ask("1. Yes \n2. No");
			if ((string)choice == "2") {
				await pText.ShowText("Maybe later.");
				iText.EnableInteractArea();
			}
			else {
				await pText.ShowText("Sure!");
				await iText.ShowText("the goal of the game is to collect as many fish as you can in the time limit.");
				await iText.ShowText("press enter/space near a fish to collect it.");
				await iText.ShowText("the fish hurt you if you get too close, so be careful!");
				StartMinigame();
			}
		}
	}

	private void StartMinigame()
	{
		playingMinigame = true;
		fishCollected = 0;

		//disable regular fish
		var regularFish = GetNode<Node2D>("RegularFish").GetChildren();
		for (int i = 0; i < regularFish.Count; i ++)
		{
			if (regularFish[i] is FishEnemy fish)
			{
				fish.Disable();
			}
		}

		//enable collectable fish
		for (int i = 0; i < CollectableFish.Count; i ++)
		{
			if (CollectableFish[i] is FishEnemy fish)
			{
				fish.Enable();
			}
		}

		//start timer
		var timer = GetNode<Label>("UnderwaterPlayer/MinigameTime");
		if (timer is MinigameTime mTimer)
		{
			mTimer.StartTime();
		}
	}

	private async void EndMinigame()
	{
		GetNode<CharacterBody2D>("UnderwaterPlayer").Position = new Vector2(535, 172);
		if (playerTextNode is TextBox pText && iceCreamTextNode is TextBox iText)
		{
			await iText.ShowText("game over!");
			await iText.ShowText("you got : " + fishCollected + " fish");
			await iText.ShowText("you get: " + fishCollected + " coins");
			GlobalScript.Coins += fishCollected;
			
			if (fishCollected > GlobalScript.FishGameHighScore)
			{
				await iText.ShowText("that's a new high score!");
				GlobalScript.FishGameHighScore = fishCollected;
				UpdateHighScore();
			}

			var player = playerTextNode.GetParent();
			if (fishCollected >= CollectableFish.Count 
				&& !GlobalScript.ClamsCollected[0]
				&& player is Player p && p.hp > 1)
			{
				await iText.ShowText("nice game! congratulations! you unlocked an extra prize!");
				Clam.Show();
				Clam.GetNode<InteractArea>("InteractArea").Interactable(true);
			}
			GetNode<Label>("UnderwaterPlayer/MinigameTime").Hide();
			
			iText.EnableInteractArea();
		}
		
		playingMinigame = false;

		//enable regular fish
		var regularFish = GetNode<Node2D>("RegularFish").GetChildren();
		for (int i = 0; i < regularFish.Count; i ++)
		{
			if (regularFish[i] is FishEnemy fish)
			{
				fish.Enable();
			}
		}

		//disable collectable fish
		for (int i = 0; i < CollectableFish.Count; i ++)
		{
			if (CollectableFish[i] is FishEnemy fish)
			{
				fish.Disable();
			}
		}
	}
	//ways to end minigame
	private async void OnTimesUp()
	{
		EndMinigame();
	}

	//collect fish
	private void OnFishInteract()
	{
		fishCollected ++;
		GD.Print(fishCollected);///
		//end game early if all fish collected
		if (fishCollected >= numCollectableFish)
		{
			var timer = GetNode<Label>("UnderwaterPlayer/MinigameTime");
			if (timer is MinigameTime mTimer)
			{
				mTimer.EndGame();
				EndMinigame();
			}
		}
	}

	//end minigame if player dies
	private void OnPlayerRespawn()
	{
		if (playingMinigame)
		{
			// var FaderNode = GetNode<CanvasLayer>("/root/Fader");
			// if (FaderNode is Fader fader) {
			// 	fader.Hide();
			// }

			var timer = GetNode<Label>("UnderwaterPlayer/MinigameTime");
			if (timer is MinigameTime mTimer)
			{
				mTimer.EndGame();
				EndMinigame();
			}
		}
	}

	private void UpdateHighScore()
	{
		GetNode<Label>("%HighScore").Text = "" + GlobalScript.FishGameHighScore;
	}
	
	private async Task NextRoomCheck() {
		var FaderNode = GetNode<CanvasLayer>("/root/Fader");
		Vector2 pos = GetNode<CharacterBody2D>("UnderwaterPlayer").Position;
		var GlobalSceneChange = GetNode<GlobalSceneChange>("/root/GlobalSceneChange");
		if (pos.X < 5) {
			transitioning = true;
			if (FaderNode is Fader fader) {
				await fader.FadeIn(.7f);
			}
			await GlobalSceneChange.ChangeRoom(new Vector2(480, 140), "box_room", false);
		}
		else if (pos.Y < 5)
		{
			transitioning = true;
			if (FaderNode is Fader fader) {
				await fader.FadeIn(.7f);
			}
			await GlobalSceneChange.ChangeRoom(new Vector2(290, 135), "long_tube_coral_room", false);
		}
		else if (pos.X > 635)
		{
			transitioning = true;
			if (FaderNode is Fader fader) {
				await fader.FadeIn(.7f);
			}
			await GlobalSceneChange.ChangeRoom(new Vector2(20, 863), "geyser_room", true);
		}
	}
}
