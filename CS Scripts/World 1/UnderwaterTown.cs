using Godot;
using System;
using System.Threading.Tasks;

public partial class UnderwaterTown : Node2D
{
	private bool transitioning = false;
	private TextBox carT;
	private TextBox dashT;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		carT = GetNode<TextBox>("Carsava/TextBox");
		dashT = GetNode<TextBox>("UnderwaterPlayer/TextBox");
		GetNode<Walls>("RightExitWall").Enable();
		GetNode<AnimatedSprite2D>("Carsava").Animation = "red";
		carT.SetLabel("Carsava");
		dashT.SetLabel("Dash");
		dashT.Known(true);
		carT.Known(true); //introduces himself first dialogue
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override async void _Process(double delta)
	{
		if (!transitioning) {
			await NextRoomCheck();
		}
	}

	private async void OnIsThatMyBoatTriggerBodyEntered(Node2D player)
	{
		if (GlobalScript.CQ("short") == "GoToTown")
		{
			if (player is Player p)
			{
				p.SetDisableMovement(true);
				var ani = player.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
				if (player.Position.Y >= 510) {
					ani.Animation = "sit-helmet";
				}
				else {
					ani.Animation = "swim-right";
					ani.Play();
				}
				await dashT.ShowText("Hang on a minute...");
		
				var camera = GetNode<Camera2D>("UnderwaterPlayer/Camera2D");
				camera.PositionSmoothingEnabled = true;
				camera.PositionSmoothingSpeed = 5.0f;
				var cameraOriginalPos = camera.GlobalPosition;
				var textBoxOriginalPos = dashT.GlobalPosition;

				camera.GlobalPosition = new Vector2(300, 340);
				dashT.GlobalPosition = new Vector2(300, 360);
				await dashT.ShowText("Is that my boat up there??");

				camera.GlobalPosition = cameraOriginalPos;
				dashT.GlobalPosition = textBoxOriginalPos;
				
				await dashT.ShowText("How in the world did it get there??");
				camera.PositionSmoothingEnabled = false;
				GlobalScript.QuestNum ++;
				p.SetDisableMovement(false);
			}
		}
	}

	private async void PlayerMeetCarsava(Node2D body) {
		if (body is Player p) {
			p.SetDisableControl(true);
			body.GetNode<AnimatedSprite2D>("AnimatedSprite2D").Stop();

			await CarsavaDialogue();

			p.SetDisableControl(false);
			body.GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play();
		}
	}
	
	private async Task CarsavaDialogue() {
		if (GlobalScript.Inventory.Contains("Town Pass")) {
			if (GlobalScript.CarEncNum == 1) {
				await carT.ShowText("You again - I told you -");
				await carT.ShowText("Wait, you got a pass? That's my sister, Catssava's!");
				await carT.ShowText("She must really have faith in you. The ocean's dangers are no joke!");
				await carT.ShowText("Well, go ahead then. And good luck.");
				GlobalScript.CarEncNum++;
			}
			else if (GlobalScript.CarEncNum == 0) {
				await carT.ShowText("I'm Carsava, the town guard. And you must be the new stranger in town.");
				await carT.ShowText("Gotten a pass already, have you? I'm warning you, the waters beyond are dangerous.");
				await carT.ShowText("I'm assuming you're trying to get boba for my sister.");
				await carT.ShowText("Saying you miraculously succeed somehow...she would be very grateful.");
				await carT.ShowText("She's been down ever since her boba supply vanished.");
				GlobalScript.CarEncNum = 2;
			}
			else {
				int path; 
				int max = (GlobalScript.CarEncNum > 6) ? 6 : GlobalScript.CarEncNum;
				if (GlobalScript.Inventory.Contains("Flashlight")) {
					path = Rand(0, max);
				}
				else {
					path = Rand(-1, max);
				}
				switch (path) {
					//hint if you don't have flashlight
					case -1: await carT.ShowText("Don't forget to check out the other shops in town! Our cats are itching for business.");
							break;
					case 0: await carT.ShowText("Catssava has been telling everyone around town about the cat that offered to find her boba.");
							break;
					case 1: if (GlobalScript.IsAfterQuest("GetBoat")) {await carT.ShowText("You've become a town legend.");}
							else {await carT.ShowText("Everyone is asking for updates on how your hunt for boba is going.");}
							break;
					case 2: await carT.ShowText("Just between you and me, you're the bravest cat I've met. Not counting myself, of course.");
							break;
					case 3: await carT.ShowText("I won't tell you I believe in you. What matters is that you believe in yourself.");
							break;
					case 4: await carT.ShowText("I'll admit, I've grown fond of you, adventurer.");
							break;
					case 5: await carT.ShowText("You're always welcome to visit our little town, no matter how far the waves take you in life.");
							break;
					case 6: await carT.ShowText("I know you can handle the toughest waves that come your way.");
							break;
				}
				GlobalScript.CarEncNum++;
			}
			GetNode<AnimatedSprite2D>("Carsava").Animation = "blue";
			GetNode<Walls>("RightExitWall").Disable();
		}
		else {
			if (GlobalScript.CarEncNum == 0) {
				await carT.ShowText("I'm Carsava, the town guard. And you must be the new stranger in town.");
				await carT.ShowText("Unfortunately, I can't let you leave town without a pass.");
				await carT.ShowText("It's too dangerous out there for a normal cat. No reason to leave if you don't have to.");
				GlobalScript.CarEncNum++;
			}
			else {
				await carT.ShowText("Stranger, I don't know why you insist on leaving.");
				await carT.ShowText("Our town will provide all you need.");
			}
		}
	}

	private async Task NextRoomCheck() {
		var player = GetNode<CharacterBody2D>("UnderwaterPlayer");
		var GlobalSceneChange = GetNode<GlobalSceneChange>("/root/GlobalSceneChange");
		Vector2 pos = player.Position;
		if (pos.X > 508) {
			transitioning = true;
			await GlobalSceneChange.ChangeRoom(new Vector2(10, 140), "box_room", true);
		}
		if (pos.X < -8) {
			transitioning = true;
			await GlobalSceneChange.ChangeRoom(new Vector2(447, 140), "first_room", false);
		
		}
	}

	//shop doors:
	public async void OnEnterRoom(String roomName)
	{
		var player = GetNode<CharacterBody2D>("UnderwaterPlayer");
		var GlobalScene = GetNode<GlobalSceneChange>("/root/GlobalSceneChange");
		transitioning = true;
		
		if (roomName == "SubShopDoor")
		{
			await GlobalScene.ChangeRoom(new Vector2(66, 132), "submarine_shop", true);
		}
		else if (roomName == "SubShopDoor2")
		{
			await GlobalScene.ChangeRoom(new Vector2(252, 132), "submarine_shop", false);
		}
		else if (roomName == "BobaShopDoor") {
			await GlobalScene.ChangeRoom(new Vector2(65, 132), "boba_shop", true);
		}
		else if (roomName == "PlantShopDoor") {
			await GlobalScene.ChangeRoom(new Vector2(258, 132), "plant_shop", false);
		}
	}
	
	private static int Rand(int low, int high) {
		var randomizer = new RandomNumberGenerator();
		randomizer.Randomize();
		return randomizer.RandiRange(low, high);
	}
}
