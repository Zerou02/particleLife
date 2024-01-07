using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Godot;
public class Simulation
{
    /*     public List<int> colours = new List<int>();
        public List<double> positionsX = new List<double>();
        public List<double> positionsY = new List<double>();
        public List<double> positionsZ = new List<double>();
        public List<double> velocitiesX = new List<double>();
        public List<double> velocitiesY = new List<double>();
        public List<double> velocitiesZ = new List<double>(); */
    public List<Particle> particles = new List<Particle>();
    public List<Color> colourVals = new List<Color>() { new Color("red"), new Color("blue"), new Color("green"), new Color("purple"), new Color("orange"), new Color("black") };
    public Config config;
    public Simulation()
    {
        config = new Config();
        initSim();
    }

    void initSim()
    {
        clearSim();
        createRandomColours(config.amountColours);
        addParticle(config.n);
    }

    void createRandomColours(int amount)
    {
        colourVals = new List<Color>();
        var rnd = new Random();
        for (int i = 0; i < amount; i++)
        {
            colourVals.Add(Color.FromHsv((float)rnd.NextDouble(), 1, 1));
        }
        config.matrix = config.makeRandomMatrix(amount, amount);
    }
    void addParticle(int amount)
    {
        var random = new Random();
        for (int i = 0; i < amount; i++)
        {
            var colour = random.Next(config.amountColours);
            Vector3 position = Vector3.Zero;
            position.X = (float)random.NextDouble(); // * 2 - 1;
            position.Y = (float)random.NextDouble(); // * 2 - 1;
            position.Z = (float)random.NextDouble(); // * 2 - 1;
            Vector3 velocity = Vector3.Zero;
            velocity.X = 0;
            velocity.Y = 0;
            velocity.Z = 0;
            particles.Add(new Particle(position, velocity, colour));
        }
    }

    void removeParticles(int amount)
    {
        particles.RemoveRange(particles.Count - amount, amount);
        /*    colours.RemoveRange(colours.Count - amount, amount);
           positionsX.RemoveRange(positionsX.Count - amount, amount);
           positionsY.RemoveRange(positionsY.Count - amount, amount);
           positionsZ.RemoveRange(positionsZ.Count - amount, amount);
           velocitiesX.RemoveRange(velocitiesX.Count - amount, amount);
           velocitiesY.RemoveRange(velocitiesY.Count - amount, amount);
           velocitiesZ.RemoveRange(velocitiesZ.Count - amount, amount);
           GD.Print("COUNT", positionsX.Count); */
        GD.Print("n", config.n);

    }

    void clearSim()
    {
        particles = new List<Particle>();
        /* colours = new List<int>();
        positionsX = new List<double>();
        positionsY = new List<double>();
        positionsZ = new List<double>();
        velocitiesX = new List<double>();
        velocitiesY = new List<double>();
        velocitiesZ = new List<double>(); */
    }

    public void printMatrix()
    {
        foreach (var y in config.matrix)
        {
            var str = "";
            foreach (var x in y)
            {
                str += x + ", ";
            }
            GD.Print(str);
        }
    }

    double force(double r, double a)
    {
        double beta = 0.3;
        if (r < beta)
        {
            return r / beta - 1;
        }
        else if (beta < r && r < 1)
        {
            return a * (1 - Math.Abs(2 * r - 1 - beta) / (1 - beta));
        }
        else
        {
            return 0;
        }
    }
    public void step()
    {
        //update vel
        for (int i = 0; i < config.n; i++)
        {
            double totalForceX = 0;
            double totalForceY = 0;
            double totalForceZ = 0;

            for (int j = 0; j < config.n; j++)
            {
                if (j == i) { continue; }
                //positionsX[j] - positionsX[i];
                //positionsY[j] - positionsY[i];
                //positionsZ[j] - positionsZ[i];
                double rx = particles[j].position.X - particles[i].position.X;
                double ry = particles[j].position.Y - particles[i].position.Y;
                double rz = particles[j].position.Z - particles[i].position.Z;
                double r = Math.Sqrt(rx * rx + ry * ry + rz * rz);
                if (r > 0 && r < config.rMax)
                {
                    double f = force(r / config.rMax, config.matrix[particles[i].colour][particles[j].colour]);
                    //double f = force(r / config.rMax, config.matrix[colours[i]][colours[j]]);
                    totalForceX += rx / r * f;
                    totalForceY += ry / r * f;
                    totalForceZ += rz / r * f;

                }
            }
            totalForceX *= config.rMax;
            totalForceY *= config.rMax;
            totalForceZ *= config.rMax;

            //particles[i].velocity *= (float)config.frictionFactor;
            var newX = particles[i].velocity.X + (float)(totalForceX * config.dt) * (float)config.frictionFactor;
            var newY = particles[i].velocity.Y + (float)(totalForceY * config.dt) * (float)config.frictionFactor;
            var newZ = particles[i].velocity.Z + (float)(totalForceZ * config.dt) * (float)config.frictionFactor;
            particles[i] = new Particle(particles[i].position, new Vector3(newX, newY, newZ), particles[i].colour);
            //.velocity = new Vector3(newX, newY, newZ);
            //velocitiesX[i] *= config.frictionFactor;
            //velocitiesY[i] *= config.frictionFactor;
            //velocitiesZ[i] *= config.frictionFactor;
            //velocitiesX[i] += totalForceX * config.dt;
            //velocitiesY[i] += totalForceY * config.dt;
            //velocitiesZ[i] += totalForceZ * config.dt;
        }
        //update pos
        for (int i = 0; i < config.n; i++)
        {
            var newPosX = particles[i].position.X + (float)(particles[i].velocity.X * config.dt);
            var newPosY = particles[i].position.Y + (float)(particles[i].velocity.Y * config.dt);
            var newPosZ = particles[i].position.Z + (float)(particles[i].velocity.Z * config.dt);
            particles[i] = new Particle(new Vector3(newPosX, newPosY, newPosZ), particles[i].velocity, particles[i].colour);
            //positionsX[i] += velocitiesX[i] * config.dt;
            //positionsY[i] += velocitiesY[i] * config.dt;
            //positionsZ[i] += velocitiesZ[i] * config.dt;

        }
    }

    public void changeRadiusTo(double val)
    {
        this.config.rMax = val;
    }

    public void changeSpeedTo(double val)
    {
        this.config.dt = val;
    }

    public void changeParticleTypesTo(int amount)
    {
        this.config.amountColours = amount;
        initSim();
    }

    public void changeAmountParticlesTo(int amount)
    {
        if (amount > config.n)
        {
            addParticle(amount - config.n);
        }
        else
        {
            removeParticles(config.n - amount);
        }
        config.n = amount;
    }

}