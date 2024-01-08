using Godot;
public class Particle
{
    public Vector3 position;
    public Vector3 velocity;
    public int colour;
    public int id = 0;
    public Particle(Vector3 position, Vector3 velocity, int colour)
    {
        this.position = position;
        this.velocity = velocity;
        this.colour = colour;
        this.id = Utils.getID();
    }
}
