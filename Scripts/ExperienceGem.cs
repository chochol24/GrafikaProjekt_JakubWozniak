using Godot;
using System;
using System.Drawing;

public partial class ExperienceGem : Area2D
{
	[Export]
	public int experience = 1;

	public Texture2D gemGreen = (Texture2D)ResourceLoader.Load("res://Textures/GreenGem.png");
	public Texture2D gemBlue = (Texture2D)ResourceLoader.Load("res://Textures/BlueGem.png");
	public Texture2D gemRed = (Texture2D)ResourceLoader.Load("res://Textures/RedGem.png");

	public Player target;
	public float speed = -1.0f;

	public Sprite2D sprite;
	public CollisionShape2D collision;
	public AudioStreamPlayer sound;

	public Timer pulsTimer;
	public PointLight2D light;

	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite2D");
		collision = GetNode<CollisionShape2D>("CollisionShape2D");
		sound = GetNode<AudioStreamPlayer>("CollectSound");
		pulsTimer = GetNode<Timer>("PulsatingTimer");
		light = GetNode<PointLight2D>("PointLight2D");

		if (experience < 5)
			return;
		else if (experience < 25)
			sprite.Texture = gemBlue;
		else
			sprite.Texture = gemRed;
	}


	public override void _PhysicsProcess(double delta)
	{
		if (target != null)
		{
			GlobalPosition = GlobalPosition.MoveToward(target.GlobalPosition, speed);
			speed += 2 * (float)delta;
		}
	}

	public int Collect()
	{
		sound.Play();
		collision.CallDeferred(CollisionShape2D.MethodName.SetDisabled, true);
		sprite.Visible = false;
		return experience;
	}

	private void OnCollectSoundFinished()
	{
		QueueFree();
	}

	private void OnPulsatingTimerTimeout()
	{
		if (light.Energy == 2.5)
		{
			light.Energy -= -1;
		}
		else light.Energy += -1;
		pulsTimer.Start();
	}


}




