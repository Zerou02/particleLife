using Godot;
public partial class BackgroundTileOverlay : Control
{
	HSlider hSlider;
	BackgroundTile backgroundTile;
	Label label;
	Button closeBtn;

	double value = 0;
	[Signal]
	public delegate void onCloseEventHandler(double val);
	public override void _Ready()
	{
		hSlider = GetNode<HSlider>("HSlider");
		backgroundTile = GetNode<BackgroundTile>("BackgroundTile");
		label = GetNode<Label>("Label");
		closeBtn = GetNode<Button>("CloseBtn");
		setFontSize();
		init(0.4);
		hSlider.ValueChanged += (val) => init(val);
		closeBtn.Pressed += () => { EmitSignal(SignalName.onClose, this.value); QueueFree(); };
	}

	public void init(double value)
	{
		this.value = value;
		var str = value.ToString();
		label.Text = "";
		for (int i = 0; i < 5; i++)
		{
			if (i >= str.Length)
			{
				break;
			}
			label.Text += str[i];
		}
		backgroundTile.setValue(value);
	}
	void setFontSize()
	{
		var baseSize = 16;
		var scale = label.Size / label.CustomMinimumSize;
		label.LabelSettings.FontSize = (int)(baseSize * scale.Y);
	}
	public override void _Process(double delta)
	{

	}
}
