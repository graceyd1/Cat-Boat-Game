using Godot;
using System;
using System.Collections.Generic;

public partial class DevMode : CanvasLayer
{
	[Signal]
	public delegate void ChangeHPEventHandler(int hp);
	
	private Dictionary<string, Action<string>> commands;
	private int state = 0;
	private Action<string> command;
	private ConfirmationDialog window;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		window = GetNode<ConfirmationDialog>("Popup");
		window.Hide();
		commands = new Dictionary<string, Action<string>> {
			{"room", ChangeRoom},
			{"add", AddToInventory},
			{"sethp", SetHP},
			{"setquest", SetQuest},
			{"print", DebugPrint},
			{"files", ManageFiles}
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("dev_mode")) {
			if (window.Visible == false) {
				window.PopupCentered();
				GetNode<LineEdit>("Popup/CommandBox").Clear();
				window.Show();
			}
		}
		// if (window.Visible == true)
		// {
		// 	if (Input.IsActionPressed("enter"))
		// 	{
		// 		OnConfirmed();
		// 	}
		// }
	}
	
	
	public void OnConfirmed() {
		string cm = GetNode<LineEdit>("Popup/CommandBox").Text;
		if (state == 0) {
			//if TryGetValue returns true, it will return the associated method from dictionary
			if (commands.TryGetValue(cm, out Action<string> Method)) {
				GetNode<LineEdit>("Popup/CommandBox").Clear();
				window.Show();
				state = 1;
				command = Method;
			}
			else {
				GD.Print("Command not found");
			}
		}
		else {
			command.Invoke(cm);
			state = 0;
		}
		
	}
	
	private void ChangeRoom(string input) {
		GetTree().ChangeSceneToFile("res://Scenes/" + input + ".tscn");
	}
	
	private void AddToInventory(string input) {
		GlobalScript.Inventory.Add(input);
	}
	
	private void SetHP(string input) {
		if (int.TryParse(input, out int result)) {
			EmitSignal(SignalName.ChangeHP, result);
		}
	}
	
	private void SetQuest(string input) {
		if (int.TryParse(input, out int result)) {
			GlobalScript.QuestNum = result;
		}
	}
	
	private void DebugPrint(string input) {
		if (input == "questnum") {
			GD.Print(GlobalScript.QuestNum);
		}
		//if input contains "quest", parse the number after it, print the associated quest
		if (input.Contains("quest") && input.Length == 6) {
			//start index, length
			string numStr = input.Substring(5, 1);
			if (int.TryParse(numStr, out int num)) {
				if (GlobalScript.MainQuests.Count > num) {
					GD.Print(GlobalScript.MainQuests[num]);
				}
				else {
					GD.Print("Quest " + num + " does not exist.");
				}
			}
		}
	}
	
	private void ManageFiles(string input) {
		if (input == "load") {
			GlobalScript.LoadGame();
			GetTree().ChangeSceneToFile("res://Scenes/" + GlobalScript.CurrentRoom + ".tscn"); //needs to eventually be handled by another method
		}
		else {
			GlobalScript.SaveGame();
		}
	}
}
