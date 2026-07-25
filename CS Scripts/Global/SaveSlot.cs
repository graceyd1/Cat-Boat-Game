using Godot;
using System;

public partial class SaveSlot : Node2D
{
	[Export]
	public int SlotNum{get; set;}
	
	private Button EmptyState;
	private Panel FilledState;
	private string path;
	private GlobalSaveResource SlotFileInfo;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		EmptyState = GetNode<Button>("EmptySlotButton");
		FilledState = GetNode<Panel>("FilledSlot");
		path = "user://save_data" + SlotNum + ".tres";
		GetNode<Label>("FilledSlot/SlotName").Text += SlotNum;
		if (FileAccess.FileExists(path)) {
			SlotFileInfo = ResourceLoader.Load<GlobalSaveResource>(path);
			if (GlobalScript.QuestNames.TryGetValue(GlobalScript.MainQuests[SlotFileInfo.QuestNum], out string fullName)) {
				GetNode<Label>("FilledSlot/Quest").Text += fullName;
			}
			GetNode<Label>("FilledSlot/Time").Text += SlotFileInfo.DateSaved;
		}
	}
	public override void _Process(double delta) {
		//ResourceLoader will return true if still in cache after deleted
		if (FileAccess.FileExists(path)) {
			EmptyState.Hide();
			FilledState.Show();
		}
		else {
			EmptyState.Show();
			FilledState.Hide();
		}
	}
	
	public async void OnLoad() {
		GlobalScript.savePath = path;
		GlobalScript.LoadGame();
		var FaderNode = GetNode<CanvasLayer>("/root/Fader");
		var GlobalScene = GetNode<GlobalSceneChange>("/root/GlobalSceneChange");
		GetNode<CanvasLayer>("..").Hide();
		if (FaderNode is Fader fader) {
			await fader.FadeIn(.7f);
		}
		
		await GlobalScene.ChangeRoom(new Vector2(0, 0), GlobalScript.CurrentRoom, true);
	}
	
	public void OnDelete() {
		if (ResourceLoader.Exists(path)) {
			DirAccess.RemoveAbsolute(path);
		}
	}
	
	public void OnNewGame() {
		DateTime time = DateTime.Now;
		GlobalScript.GameData.DateSaved = time.ToString("yyy-MM-dd"); 
		GlobalScript.savePath = path;
		GlobalScript.SaveGame(); //saves the GlobalSaveResource object with new game configurations
		if (Owner is TitleScreen title) {
			title.ShipCrashCutscene();
		}
	}
}
