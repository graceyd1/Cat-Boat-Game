using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

public partial class GlobalSceneChange : Node //=changed this to a node from node2d hope that's ok
{
	public static Player currPlayer;
	
	[Signal]
	public delegate void SceneReadyEventHandler();

	public static List<string> NoPlayerRooms = new List<string>
	{
		"WorldEndScreen"
	};
	
	public static List<string> UnderwaterRooms = new List<string> {
		"FirstRoom",
		"UnderwaterTown",
		"BoxRoom",
		"FishRoom",
		"LongTubeCoralRoom",
		"JellyfishRoom",
		"VineRoom",
		"SecretRoom1",
		"TallTubeCoralRoom",
		"GeyserRoom",
	};
	
	public static List<string> GroundRooms = new List<string> {
		"SubmarineShop",
		"BobaShop",
		"PlantShop",
		"EnterCaveRoom",
		"CaveRoom",
		"ParvaHouse",
		"EnterSeaBunnyRoom",
		"SeaBunnyRoom",
		"TreasureRoom"
	};
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetTree().NodeAdded += checkNode;
		if (UnderwaterRooms.Contains(GetTree().CurrentScene.Name)) {
			currPlayer = GetTree().CurrentScene.GetNode<Player>("UnderwaterPlayer");
		}
		else if (GroundRooms.Contains(GetTree().CurrentScene.Name)) {
			currPlayer = GetTree().CurrentScene.GetNode<Player>("GroundPlayer");
		}
		else if (!NoPlayerRooms.Contains(GetTree().CurrentScene.Name))
		{
			GD.Print("Room not found in list: " + GetTree().CurrentScene.Name);
		}
	}
	
	public void checkNode(Node n) {
		if (n is Player || (n == GetTree().CurrentScene && NoPlayerRooms.Contains(GetTree().CurrentScene.Name))) {
			EmitSignal(SignalName.SceneReady);
		}
	}

	
	public async Task ChangeRoom(Vector2 pos, String room, bool right) {
		GetTree().ChangeSceneToFile(
			"res://Scenes/World " + GlobalScript.WorldNum + "/" + room + ".tscn");
		GlobalScript.CurrentRoom = room;
		await ToSignal(this, GlobalSceneChange.SignalName.SceneReady);
		string roomName = GetTree().CurrentScene.Name;
		GD.Print("HELLO");
		GD.Print(NoPlayerRooms.Contains(roomName));
		if (!NoPlayerRooms.Contains(roomName))
		{
			if (UnderwaterRooms.Contains(roomName)) {
				currPlayer = GetTree().CurrentScene.GetNode<Player>("UnderwaterPlayer");
			}
			else if (GroundRooms.Contains(roomName)) {
				currPlayer = GetTree().CurrentScene.GetNode<Player>("GroundPlayer");
			}
			else {
				GD.Print("Room not found in list");
			}
			if (currPlayer == null) {
				GD.Print("ERROR getting player at GlobalSceneChange");
			}
			//there's probably a better fix, but if you put (0, 0) in parameter it spawns player in defualt pos
			if (pos != Vector2.Zero) {
				currPlayer.Position = pos;
			}
			if (right) {
				currPlayer.FacingRight = true;
			}
			else {
				currPlayer.FacingRight = false;
			}
			currPlayer.SetCameraDrag(roomName);
		}
		
		var FaderNode = GetNode<CanvasLayer>("/root/Fader");
		if (FaderNode is Fader fade) {
			await fade.FadeOut(1.5f);
		}
	}
	
}
