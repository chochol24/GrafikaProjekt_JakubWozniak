using Godot;
using System;

public partial class Enemy : CharacterBody2D
{

	[Signal]
	public delegate void RemoveFromArrayEventHandler(Enemy e);

	[Export]
	float speed = 20.0f;
	[Export]
	float hp = 10.0f;
	[Export]
	float knockbackRecovery = 3.5f;
	[Export]
	int experience = 1;
	[Export]
	float damage = 1.0f;

	Vector2 knockback = Vector2.Zero;
	Player player;
	Sprite2D sprite;
	AnimationPlayer animation;
	Node lootBase;
	PackedScene experienceGem = (PackedScene)ResourceLoader.Load("res://Scenes/Objects/ExperienceGem.tscn");
	HitBox hitBox;

	public override void _Ready()
	{
		player = (Player)GetTree().GetFirstNodeInGroup("player");
		sprite = GetNode<Sprite2D>("Sprite2D");
		animation = GetNode<AnimationPlayer>("AnimationPlayer");
		lootBase = GetTree().GetFirstNodeInGroup("loot");
		hitBox = GetNode<HitBox>("HitBox");
		hitBox.damage = damage;
		animation.Play("walk");
	}

	public override void _PhysicsProcess(double delta)
	{
		knockback = knockback.MoveToward(Vector2.Zero, knockbackRecovery);
		var direction = GlobalPosition.DirectionTo(player.GlobalPosition);
		Velocity = direction * speed;
		Velocity += knockback;
		MoveAndSlide();

		if (direction.X < .1)
			sprite.FlipH = true;
		else if (direction.X > -.1)
			sprite.FlipH = false;
	}

	public void Death()
	{
		EmitSignal(SignalName.RemoveFromArray, this);
		ExperienceGem newGem = (ExperienceGem)experienceGem.Instantiate();
		newGem.GlobalPosition = deathPos;
		newGem.experience = experience;
		lootBase.CallDeferred(Area2D.MethodName.AddChild, newGem);
		QueueFree();
	}

	private Vector2 deathPos;

	private void OnHurtBoxHurt(float damage, Vector2 angle, int knockback)
	{
		hp-=damage;
		if(hp<=0)
		{
			deathPos = GlobalPosition;
			Death();
		}
		this.knockback = angle * knockback;
			
	}
}



