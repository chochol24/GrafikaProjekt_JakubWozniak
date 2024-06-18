using Godot;
using System;

public partial class EnemySpawner : Node2D
{
	[Signal]
	public delegate void ChangeTimeEventHandler(int time);

	[Export]
	Godot.Collections.Array<SpawnInfo> spawns = new Godot.Collections.Array<SpawnInfo>();

	public Player player;
	public int time = 0;

	public override void _Ready()
	{
		player = (Player)GetTree().GetFirstNodeInGroup("player");
		Connect("ChangeTime", new Callable(player, "ChangeTime"));
	}


	public override void _Process(double delta)
	{
	}


	private void OnTimerTimeout()
	{
		time++;
		var enemySpawns = spawns;
		foreach (var i in enemySpawns)
		{
			if (time >= i.timeStart && time <= i.timeEnd)
			{
				if (i.spawnDelayCounter < i.enemySpawnDelay)
				{
					i.spawnDelayCounter++;
				}
				else
				{
					i.spawnDelayCounter = 0;
					PackedScene newEnemy = (PackedScene)i.enemy;
					var counter = 0;
					while (counter < i.enemyNumber)
					{
						Node2D enemySpawn = (Node2D)newEnemy.Instantiate();
						enemySpawn.GlobalPosition = GetRandomPosition();
						AddChild(enemySpawn);
						counter++;
					}
				}
			}
		}
		EmitSignal(SignalName.ChangeTime, time);
	}

	public Vector2 GetRandomPosition()
	{
		RandomNumberGenerator random = new RandomNumberGenerator();
		Vector2 v = GetViewportRect().Size * random.RandfRange(1.1f, 1.4f);
		Vector2 topLeft = new Vector2(player.GlobalPosition.X - v.X / 2, player.GlobalPosition.Y - v.Y / 2);
		Vector2 topRight = new Vector2(player.GlobalPosition.X + v.X / 2, player.GlobalPosition.Y - v.Y / 2);
		Vector2 bottomLeft = new Vector2(player.GlobalPosition.X - v.X / 2, player.GlobalPosition.Y + v.Y / 2);
		Vector2 bottomRight = new Vector2(player.GlobalPosition.X + v.X / 2, player.GlobalPosition.Y + v.Y / 2);
		Random random1 = new Random();
		string[] positionsSide = { "up", "down", "right", "left" };
		string positionSide = positionsSide[random1.Next(positionsSide.Length)];

		Vector2 spawnPos1 = Vector2.Zero;
		Vector2 spawnPos2 = Vector2.Zero;

		switch (positionSide)
		{
			case "up":
				spawnPos1 = topLeft;
				spawnPos2 = topRight;
				break;
			case "down":
				spawnPos1 = bottomLeft;
				spawnPos2 = bottomRight;
				break;
			case "right":
				spawnPos1 = topRight;
				spawnPos2 = bottomRight;
				break;
			case "left":
				spawnPos1 = topLeft;
				spawnPos2 = bottomLeft;
				break;
		}

		float x = random.RandfRange(spawnPos1.X, spawnPos2.X);
		float y = random.RandfRange(spawnPos1.Y, spawnPos2.Y);
		return new Vector2(x, y);

	}
}



