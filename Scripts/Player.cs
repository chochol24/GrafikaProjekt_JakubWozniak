using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Player : CharacterBody2D
{
	[Export]
	float maxHp = 100.0f;
	float hp = 100.0f;
	public float speed = 70.0f;
	public int time = 0;

	//GUI
	public TextureProgressBar expBar;
	public Label lblLevel;
	public Panel levelPanel;
	public VBoxContainer upgradeOptions;
	public PackedScene itemOptions;
	public TextureProgressBar healthBar;
	public Label labelTimer;
	public Panel deathPanel;
	public BasicButton menuButton;
	public Label labelResult;
	//Exp
	public int experience = 0;
	public int experienceLevel = 1;
	public int collectedExperience = 0;

	//Attack
	PackedScene bone;
	Timer boneTimer;
	Timer boneAttackTimer;

	//Bone
	public int boneAmmo = 0;
	public int baseBoneAmmo = 3;
	public float boneAttackSpeed = 1.5f;
	public int boneLevel = 1;


	public List<Node2D> enemyClose = new List<Node2D>();

	//Main
	Sprite2D sprite;
	Timer walkTimer;
	private Random random = new Random();

	public override void _Ready()
	{
		bone = (PackedScene)ResourceLoader.Load("res://Scenes/Attacks/Bone.tscn");
		sprite = GetNode<Sprite2D>("Sprite2D");
		walkTimer = GetNode<Timer>("WalkTimer");
		boneTimer = GetNode<Timer>("Attack/BoneTimer");
		boneAttackTimer = GetNode<Timer>("Attack/BoneTimer/BoneAttackTimer");
		lblLevel = GetNode<Label>("GUILayer/GUI/ExperienceBar/LevelLabel");
		expBar = GetNode<TextureProgressBar>("GUILayer/GUI/ExperienceBar");
		upgradeOptions = GetNode<VBoxContainer>("GUILayer/GUI/LevelUp/UpgradeOptions");
		levelPanel = GetNode<Panel>("GUILayer/GUI/LevelUp");
		itemOptions = (PackedScene)ResourceLoader.Load("res://Scenes/Utility/ItemOption.tscn");
		healthBar = GetNode<TextureProgressBar>("GUILayer/GUI/HealthBar");
		labelTimer = GetNode<Label>("GUILayer/GUI/LabelTimer");
		deathPanel = GetNode<Panel>("GUILayer/GUI/DeathPanel");
		labelResult = GetNode<Label>("GUILayer/GUI/DeathPanel/ResultLabel");
		menuButton = GetNode<BasicButton>("GUILayer/GUI/DeathPanel/MenuButton");

		Attack();
		SetExpBar(experience, CalculateExperienceCap());
		OnHurtBoxHurt(0, Vector2.Zero, 0);
	}
	public override void _PhysicsProcess(double delta)
	{
		Movement();
		if (Input.IsKeyPressed(Key.Escape))
		{
			_ = GetTree().ChangeSceneToFile("res://Scenes/Menu.tscn");
		}
		if(time>300)
			Death();
	}

	public void Movement()
	{
		var x = Input.GetActionStrength("right") - Input.GetActionStrength("left");
		var y = Input.GetActionStrength("down") - Input.GetActionStrength("up");
		var move = new Vector2(x, y);
		if (move.X < 0)
			sprite.FlipH = true;
		else
			sprite.FlipH = false;

		if (move != Vector2.Zero)
			if (walkTimer.IsStopped())
			{
				if (sprite.Frame < sprite.Hframes - 1)
					sprite.Frame++;
				else
					sprite.Frame = 0;
				walkTimer.Start();
			}

		Velocity = move.Normalized() * speed;
		MoveAndSlide();
	}


	public void Attack()
	{
		if (boneLevel > 0)
		{
			boneTimer.WaitTime = boneAttackSpeed;
			if (boneTimer.IsStopped())
				boneTimer.Start();
		}
	}
	private void OnHurtBoxHurt(float damage, Vector2 _angle, int _knockback)
	{
		hp -= damage;
		healthBar.MaxValue = maxHp;
		healthBar.Value = hp;
		if (hp <= 0)
			Death();
	}

	private void OnBoneTimerTimeout()
	{
		boneAmmo += baseBoneAmmo;
		boneAttackTimer.Start();
	}


	private void OnBoneAttackTimerTimeout()
	{
		if (boneAmmo > 0)
		{
			Bone boneAttack = (Bone)bone.Instantiate();
			boneAttack.Position = Position;
			boneAttack.target = GetRandomTarget();
			boneAttack.level = boneLevel;
			AddChild(boneAttack);
			boneAmmo--;
			if (boneAmmo > 0)
				boneAttackTimer.Start();
			else
				boneAttackTimer.Stop();

		}
	}

	public Vector2 GetRandomTarget()
	{

		if (enemyClose.Count > 0)
			return enemyClose.ElementAt(random.Next(enemyClose.Count)).GlobalPosition;
		else
			return Vector2.Up;
	}

	private void OnEnemyDetectionAreaBodyEntered(Node2D body)
	{
		if (!enemyClose.Contains(body))
			enemyClose.Add(body);
	}


	private void OnEnemyDetectionAreaBodyExited(Node2D body)
	{
		if (enemyClose.Contains(body))
			enemyClose.Remove(body);
	}

	private void OnGrabAreaAreaEntered(Area2D area)
	{
		if (area.IsInGroup("loot") && area is ExperienceGem e)
			e.target = this;
	}


	private void OnCollectAreaAreaEntered(Area2D area)
	{
		if (area.IsInGroup("loot") && area is ExperienceGem e)
		{
			var gemExp = e.Collect();
			CalculateExperience(gemExp);
		}
	}

	public void CalculateExperience(int gemExp)
	{
		int requiredExp = CalculateExperienceCap();
		collectedExperience += gemExp;
		if (experience + collectedExperience >= requiredExp)
		{
			collectedExperience -= requiredExp - experience;
			experienceLevel++;
			experience = 0;
			requiredExp = CalculateExperienceCap();
			LevelUp();
		}
		else
		{
			experience += collectedExperience;
			collectedExperience = 0;
		}
		SetExpBar(experience, requiredExp);
	}

	public int CalculateExperienceCap()
	{
		int expCap = experienceLevel;
		if (experienceLevel < 20)
			expCap = experienceLevel * 5;
		else if (experienceLevel < 40)
			expCap += 95 * (experienceLevel - 19) * 8;
		else
			expCap = 255 + (experienceLevel - 39) * 12;
		return expCap;
	}

	public void SetExpBar(int value = 1, int maxValue = 100)
	{
		expBar.Value = value;
		expBar.MaxValue = maxValue;
	}

	public void LevelUp()
	{
		lblLevel.Text = "Level: " + experienceLevel.ToString();
		var tween = levelPanel.CreateTween();
		tween.TweenProperty(levelPanel, "position", new Vector2(220, 50), 0.2).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.In);
		tween.Play();
		levelPanel.Visible = true;
		var options = 0;
		var optionsmax = 3;
		while (options < optionsmax)
		{
			var optionChoice = itemOptions.Instantiate();
			upgradeOptions.AddChild(optionChoice);
			options += 1;
		}
		GetTree().Paused = true;
	}

	public void Upgrade(ItemOption item)
	{
		var optionChildren = upgradeOptions.GetChildren();
		foreach (var i in optionChildren)
		{
			i.QueueFree();

		}
		levelPanel.Visible = false;
		levelPanel.Position = new Vector2(800, 50);
		if (experienceLevel % 2 == 0) baseBoneAmmo++;
		GetTree().Paused = false;
		CalculateExperience(0);
	}

	public void ChangeTime(int argTime = 0)
	{
		time = argTime;
		var getM = (int)(time / 60.0);
		var getS = time % 60;
		string min = getM.ToString(), sec = getS.ToString();
		if (getM < 10)
			min = 0 + getM.ToString();
		if (getS < 10)
			sec = 0 + getS.ToString();
		labelTimer.Text = min + ":" + sec;
	}

	public void Death()
	{
		deathPanel.Visible = true;
		GetTree().Paused = true;
		var tween = deathPanel.CreateTween();
		tween.TweenProperty(deathPanel, "position", new Vector2(220, 50), 3.0).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
		tween.Play();
		if (time >= 300)
			labelResult.Text = "You win!!!";
		else
			labelResult.Text = "You lose";
	}

	private void OnMenuButtonClickEnd()
	{
		GetTree().Paused = false;
		var _level = GetTree().ChangeSceneToFile("res://Scenes/Menu.tscn");
	}
}
