using Godot;
public class Particle
{
    public Vector3 position;
    public Vector3 velocity;
    public int colour;
    public Particle(Vector3 position, Vector3 velocity, int colour)
    {
        this.position = position;
        this.velocity = velocity;
        this.colour = colour;
    }
}
