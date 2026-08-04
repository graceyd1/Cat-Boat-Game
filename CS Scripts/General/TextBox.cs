using Godot;
using System;
using System.Threading.Tasks;

public partial class TextBox : Node2D
{
	[Signal]
	public delegate void ContinueDialogueEventHandler();
	
	[Signal]
	public delegate void ChoiceMadeEventHandler(String choice);
	
	[Signal]
	public delegate void HidePromptEventHandler();
	
	[Signal]
	public delegate void PromptUserEventHandler(TextBox box);
	

	private Boolean showingText;
	private RichTextLabel label;
	private Godot.Timer timer;
	private Boolean asking;
	private bool inactive;
	public static int dialogueNum = 0;
	public string LabelName;
	private bool Unknown;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		dialogueNum = 0;
		showingText = false;
		label = GetNode<RichTextLabel>("%RichTextLabel");
		timer = GetNode<Godot.Timer>("Timer");
		Hide();
	}
	
	public override void _Process(double delta) {
		if (Unknown) {
			GetNode<Label>("%Label").Text = "???";
		}
		else {
			GetNode<Label>("%Label").Text = LabelName;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (showingText && Input.IsActionJustPressed("enter") && timer.IsStopped())
		{
			showingText = false;
			inactive = false;
			Hide();
			dialogueNum++;
			EmitSignal(SignalName.ContinueDialogue);
			EmitSignal(SignalName.HidePrompt);
		}
		else if (asking && Input.IsActionJustPressed("option_1") && timer.IsStopped()) {
			asking = false;
			dialogueNum++;
			Hide();
			EmitSignal(SignalName.ChoiceMade, "1");
			EmitSignal(SignalName.HidePrompt); //to hide <<Number Keys>>
		}
		else if (asking && Input.IsActionJustPressed("option_2") && timer.IsStopped()) {
			asking = false;
			dialogueNum++;
			Hide();
			EmitSignal(SignalName.ChoiceMade, "2");
			EmitSignal(SignalName.HidePrompt);
		}
		else if (asking && Input.IsActionJustPressed("option_3") && timer.IsStopped()) {
			asking = false;
			dialogueNum++;
			Hide();
			EmitSignal(SignalName.ChoiceMade, "3");
			EmitSignal(SignalName.HidePrompt);
		}
	}

	//show text
	public async Task ShowText(String text)
	{
		label.Clear();
		label.AppendText("[font_size=10]" + text + "[/font_size]");
	
		showingText = true;
		Show();
		
		inactive = true;
		InactiveCountdown(dialogueNum, "Enter/Space");
		timer.Start();
		await ToSignal(this, TextBox.SignalName.ContinueDialogue);
	}
	
	//todo - it's broken because idk how to get it to stop if player continues before timeout
	private async void InactiveCountdown(int curNum, string text) {
		await ToSignal(GetTree().CreateTimer(2f), SceneTreeTimer.SignalName.Timeout);
		if (curNum == dialogueNum) {
			EmitSignal(SignalName.PromptUser, this, text);
		}
	}
	
	public async Task<string> Ask(String text, String op1stat = "", String op2stat = "", String op3stat = "") {
		label.Clear();
		label.AppendText("[font_size=10]" + text + "[/font_size]");
		asking = true;
		Show();
		InactiveCountdown(dialogueNum, "Number Keys");
		timer.Start();
		var result =  await ToSignal(this, TextBox.SignalName.ChoiceMade);
		string stat;
		if ((string)result[0] == "1") {
			stat = op1stat;
		}
		else if ((string)result[0] == "2") {
			stat = op2stat;
		}
		else {
			stat = op3stat;
		}
		switch (stat) {
			case "w": GlobalScript.Wisdom += 10; break;
			case "cou": GlobalScript.Courage += 10; break;
			case "l": GlobalScript.Loyalty += 10; break;
			case "com": GlobalScript.Compassion += 10; break;
			case "s": GlobalScript.Stupid += 10; break;
		}
		return (string)result[0];
	}
	
	/// <summary>
	/// If this textbox's parent is an InteractArea, disable that parent
	/// so that pressing enter doesn't restart the conversation
	/// </summary>
	public void DisableInteractArea()
	{
		var parent = GetParent();
		if (parent is InteractArea area)
		{
			area.Interactable(false);
		}
	}

	/// <summary>
	/// If this textbox's parent is an InteractArea, enable that parent
	/// so that pressing enter doesn't restart the conversation
	/// </summary>
	public void EnableInteractArea()
	{
		var parent = GetParent();
		if (parent is InteractArea area)
		{
			area.DelayEnable();
		}
	}
	
	public void Known(bool val) {
		Unknown = !val;
	}
	
	public void SetLabel(string name) {
		LabelName = name;
	}

}
