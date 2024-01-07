using Godot;
using System;

public partial class ConfigMenu : Control
{
	LabledSlider amountParticles;
	LabledSlider speed;
	LabledSlider particleTypes;
	LabledSlider effectiveDistance;

	public Simulation simulation;

	[Signal]
	public delegate void particleAmountChangedEventHandler(int newAmount);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		amountParticles = GetNode<LabledSlider>("AmountParticles");
		speed = GetNode<LabledSlider>("Speed");
		particleTypes = GetNode<LabledSlider>("Particle Types");
		effectiveDistance = GetNode<LabledSlider>("Effective Distance");

		amountParticles.valueChanged += (val) => { EmitSignal(SignalName.particleAmountChanged, val); simulation.changeAmountParticlesTo((int)val); };
		speed.valueChanged += (val) => simulation.changeSpeedTo(val);
		particleTypes.valueChanged += (val) => { simulation.changeParticleTypesTo((int)val); EmitSignal(SignalName.particleAmountChanged, amountParticles.getVal()); };
		effectiveDistance.valueChanged += (val) => simulation.changeRadiusTo(val);
	}

	public void init(Simulation simulation)
	{
		this.simulation = simulation;
	}

	public void setVisible(bool val)
	{
		if (val)
		{
			this.Show();
		}
		else
		{
			this.Hide();
		}
	}
}
