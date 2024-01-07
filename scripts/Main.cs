using Godot;
using System.Collections.Generic;

public partial class Main : Node3D
{
	Simulation simulation = new Simulation();

	public List<Ball> balls = new List<Ball>();

	PackedScene ball = GD.Load<PackedScene>("res://scenes/Ball.tscn");
	UI ui;
	Camera camera3D;
	Node3D ballPivot;
	bool uiActive = false;
	bool ballsAvailable = false;
	public override void _Ready()
	{
		ui = GetNode<UI>("UI");
		camera3D = GetNode<Camera>("Camera3D");
		ballPivot = GetNode<Node3D>("BallPivot");
		addBalls();
		ui.particleAmountChanged += (val) => handleParticleAmountChanged(val);
		simulation.printMatrix();
		ui.matrix.setMatrix(simulation.config.matrix, simulation.colourVals);
		ui.configMenu.init(simulation);
		setUIActive(true);
	}

	public void addBalls()
	{
		for (int i = 0; i < simulation.positionsZ.Count; i++)
		{
			var x = ball.Instantiate<Ball>();
			balls.Add(x);
			var l = new StandardMaterial3D();
			l.AlbedoColor = simulation.colourVals[simulation.colours[i]];
			x.SetSurfaceOverrideMaterial(0, l);
			ballPivot.AddChild(x);
		}
		ballsAvailable = true;
	}
	public void setUIActive(bool val)
	{
		this.uiActive = val;
		camera3D.setActive(!val);
		ui.setVisible(val);
		if (uiActive)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		else
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
	}

	void handleParticleAmountChanged(int val)
	{
		ballsAvailable = false;
		foreach (var x in ballPivot.GetChildren())
		{
			x.QueueFree();
		}
		balls = new List<Ball>();
		addBalls();
		ui.matrix.setMatrix(simulation.config.matrix, simulation.colourVals);
	}
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("toggleUI"))
		{
			setUIActive(!uiActive);
		}
		var startTime = Utils.startTimeMeasurement();
		simulation.step();
		Utils.endTimeMeasurement(startTime, "Step duration");
		startTime = Utils.startTimeMeasurement();
		setBallPositions();
		Utils.endTimeMeasurement(startTime, "Render duration");

	}

	public void setBallPositions()
	{
		if (!ballsAvailable)
		{
			return;
		}
		for (int i = 0; i < balls.Count; i++)
		{
			balls[i].Position = Position with { X = (float)simulation.positionsX[i], Y = (float)simulation.positionsY[i], Z = (float)simulation.positionsZ[i], };
		}
	}
}