using Godot;
using System;

public partial class UI : Control
{
	public Matrix matrix;
	public ConfigMenu configMenu;

	public bool isVisible = true;
	[Signal]
	public delegate void particleAmountChangedEventHandler(int newAmount);
	public override void _Ready()
	{
		matrix = GetNode<Matrix>("Matrix");
		configMenu = GetNode<ConfigMenu>("ConfigMenu");
		configMenu.particleAmountChanged += (val) => EmitSignal(SignalName.particleAmountChanged, val);
	}
	public override void _Process(double delta)
	{
	}

	public void setVisible(bool val)
	{
		isVisible = val;
		matrix.setVisible(val);
		configMenu.setVisible(val);
	}
}
