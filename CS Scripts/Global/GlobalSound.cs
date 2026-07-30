using Godot;
using System;

public partial class GlobalSound : Node
{
	private AudioStreamPlayer SoundPlayer;
	private AudioStreamPlayer MusicPlayer;

	private AudioStream hurt, collect_coin, bounce;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SoundPlayer = GetNode<AudioStreamPlayer>("SoundEffectPlayer");
		MusicPlayer = GetNode<AudioStreamPlayer>("MusicPlayer");

		hurt = ResourceLoader.Load<AudioStream>("res://assets/sound_effects/hurt.wav");
		collect_coin = ResourceLoader.Load<AudioStream>("res://assets/sound_effects/collect_coin.wav");
		bounce = ResourceLoader.Load<AudioStream>("res://assets/sound_effects/bounce.wav");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// Plays the specified sound.
	/// </summary>
	/// <param name="sound">The name of the sound (probably the name of the file without .wav)</param>
	public void PlaySound(String sound)
	{
		var stream = (AudioStream) (GodotObject) Get(sound);
		if (stream != null)
		{
			SoundPlayer.Stream = stream;
			SoundPlayer.Play();
		}
	}
}
