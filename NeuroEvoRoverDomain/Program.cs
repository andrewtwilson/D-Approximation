using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroEvoRoverDomain
{
    class Program
    {
        static void Main(string[] args)
        {
            const int num_rovers = 20;
            const int num_POI = 40;

            bool GENERATE_ROVERS = false; // if FALSE, read in from file
            bool GENERATE_POIS = false;   // if FALSE, read in from file

            int TIMESTEPS = 100;

            int MIN_OBS_DIST = 1;
            int MAX_OBS_DIST = 3;

            bool DO_L = false;
            bool DO_G = false;
            bool DO_D = true;

            int EVOPOP = 100;

            int GENERATIONS = 100;

            DataIO.DataExport DE = new DataIO.DataExport();

            //setup rover domain
            Domains.MultiRoverDomain RoverDomain = new Domains.MultiRoverDomain(num_rovers, num_POI, TIMESTEPS, DO_L, DO_G, DO_D, GENERATE_ROVERS, GENERATE_POIS, MIN_OBS_DIST, MAX_OBS_DIST);

            //setup neuroevo policies
            NeuralNetworks.NeuroEvo Evo = new NeuralNetworks.NeuroEvo(num_rovers, EVOPOP);

            

            for (int gen = 0; gen < GENERATIONS; gen++)
            {
                Console.WriteLine("GEN = " + gen.ToString());
                //give rovers policies and use/score the policies
                RoverDomain.get_and_use_neuroevo_policies(Evo.Population, EVOPOP);

                //evolve policies and end generation
                Evo.EndGeneration();
            }

            if (DO_G)
                DE.Export1DDoubleArray(RoverDomain.Global_rewards[0].ToArray(), "global");
            if (DO_D)
                DE.Export1DDoubleArray(RoverDomain.Global_rewards[0].ToArray(), "difference");
        }
        
    }
}
