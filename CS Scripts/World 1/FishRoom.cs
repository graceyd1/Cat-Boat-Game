using Godot;
using System;
using System.Threading.Tasks;

public partial class FishRoom : Node2D
{
	[Export]
	public int numCollectableFish {get; set;} = 1;
	private bool transitioning = false;
	private bool playingMinigame = false;
	private TextBox pText;
	private TextBox iText;
	private int fishCollected;
	private Godot.Collections.Array<Node> CollectableFish;
	private Node2D Clam;
	private int originalHp;
	private bool playerDied;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Label>("UnderwaterPlayer/MinigameTime").Visible = false;
		pText = GetNode<TextBox>("UnderwaterPlayer/TextBox");
		iText = GetNode<TextBox>("IceCreamAreaSprite/InteractArea/TextBox");
		pText.SetLabel("Dash");
		iText.SetLabel("Ice Cream");
		pText.Known(true);
		iText.Known(true);
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
		var player = GetNode<Player>("UnderwaterPlayer");
		player.SetDisableControl(true);
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
		player.SetDisableControl(false);
	}

	private void StartMinigame()
	{
		playerDied = false;
		playingMinigame = true;
		fishCollected = 0;
		var player = GetNode<Player>("UnderwaterPlayer");
		originalHp = player.GetHP();
		player.SetHP(2);

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
		var player = GetNode<Player>("UnderwaterPlayer");
		player.Position = new Vector2(535, 172);
		player.SetDisableControl(true);
		GetNode<Control>("InteractLabel").Hide();
		if (fishCollected >= CollectableFish.Count)
		{
			await iText.ShowText("game over! you got all the fish!");
		}
		else
		{
			await iText.ShowText("game over!");
		}
		await iText.ShowText("you got : " + fishCollected + " fish");
		await iText.ShowText("you get: " + fishCollected + " coins");
		GlobalScript.Coins += fishCollected;
		
		if (fishCollected > GlobalScript.FishGameHighScore)
		{
			GetNode<GlobalSound>("/root/GlobalSound").PlaySound("high_score");
			await iText.ShowText("that's a new high score!");
			GlobalScript.FishGameHighScore = fishCollected;
			UpdateHighScore();
		}

		if (fishCollected >= CollectableFish.Count 
			&& !GlobalScript.ClamsCollected[0]
			&& player.GetHP() > 1 && !playerDied)
		{
			await iText.ShowText("perfect game! congratulations! you unlocked an extra prize!");
			Clam.Show();
			Clam.GetNode<InteractArea>("InteractArea").Interactable(true);
		}
		GetNode<Label>("UnderwaterPlayer/MinigameTime").Hide();
		
		iText.EnableInteractArea();
		playingMinigame = false;
		player.SetHP(originalHp);
		player.SetDisableControl(false);

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
	private async void OnPlayerDied()
	{
		if (playingMinigame)
		{
			// var FaderNode = GetNode<CanvasLayer>("/root/Fader");
			// if (FaderNode is Fader fader) {
			// 	fader.Hide();
			// }
			var timer = GetNode<MinigameTime>("UnderwaterPlayer/MinigameTime");
			timer.EndGame();
			playerDied = true;
			var player = GetNode<Player>("UnderwaterPlayer");
			await player.RespawnOverride(new Vector2(535, 172));
			EndMinigame();
		}
	}

	private void UpdateHighScore()
	{
		GetNode<Label>("%HighScore").Text = "" + GlobalScript.FishGameHighScore;
	}
	
	private async Task NextRoomCheck() {
		Vector2 pos = GetNode<CharacterBody2D>("UnderwaterPlayer").Position;
		var GlobalSceneChange = GetNode<GlobalSceneChange>("/root/GlobalSceneChange");
		if (pos.X < -7) {
			transitioning = true;
			await GlobalSceneChange.ChangeRoom(new Vector2(480, 140), "box_room", false);
		}
		else if (pos.Y < -8)
		{
			transitioning = true;
			await GlobalSceneChange.ChangeRoom(new Vector2(290, 135), "long_tube_coral_room", false);
		}
		else if (pos.X > 648)
		{
			transitioning = true;
			await GlobalSceneChange.ChangeRoom(new Vector2(20, 863), "geyser_room", true);
		}
	}
}
