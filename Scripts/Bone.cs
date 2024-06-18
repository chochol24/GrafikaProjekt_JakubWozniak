using Godot;
using System;

public partial class Bone : Area2D
{
	[Signal]
	public delegate void RemoveFromArrayEventHandler(Bone b);
	public int level = 1;
	public int hp = 3;
	public float speed = 100.0f;
	public float damage = 10.0f;
	public int knockback = 100;
	public float attackSize = 2.0f;

	public Vector2 target = Vector2.Zero;
	public Vector2 angle = Vector2.Zero;

	public Player player;
	public override void _Ready()
	{
		player = (Player)GetTree().GetFirstNodeInGroup("player");
		angle = GlobalPosition.DirectionTo(target);
		Rotation = angle.Angle() + Mathf.DegToRad(135);
		switch (level)
		{
			case 1:
				hp = 3;
				speed = 100.0f;
				damage = 10.0f;
				knockback = 100;
				attackSize = 2.0f;
				break;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Position += angle * speed * (float)delta;
	}

	public void EnemyHit(int charge = 1)
	{
		hp -= charge;
		if (hp <= 0)
		{
			EmitSignal(SignalName.RemoveFromArray, this);
			QueueFree();
		}
			
	}

	private void OnTimerTimeout()
	{
		QueueFree();
	}

}


