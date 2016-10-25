using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains
{
    public class MultiRoverDomain
    {
        int num_POI;
        int num_rovers;
        int TIMESTEPS;

        bool DO_LOCAL;
        bool DO_GLOBAL;
        bool DO_DIFFERENCE;

        int MIN_OBS_DIST;
        int MAX_OBS_DIST;

        public List<Rover> fidos = new List<Rover>();
        List<Landmark> landmarks = new List<Landmark>();

        //List<List<double>> local_rewards = new List<List<double>>();
        List<List<double>> global_rewards = new List<List<double>>();
        List<List<double>> difference_rewards = new List<List<double>>();

        Random rand = new Random();

        DataIO.DataExport DE = new DataIO.DataExport();
        DataIO.DataImport DI = new DataIO.DataImport();

        public List<List<double>> Global_rewards
        {
            get
            {
                return global_rewards;
            }

            set
            {
                global_rewards = value;
            }
        }

        public List<List<double>> Difference_rewards
        {
            get
            {
                return difference_rewards;
            }

            set
            {
                difference_rewards = value;
            }
        }

        public MultiRoverDomain(int num_rovers, int num_POI, int TIMESTEPS, bool DO_L, bool DO_G, bool DO_D, bool G_R, bool G_P, int min_o_d, int max_o_d)
        {
            this.num_rovers = num_rovers;
            this.num_POI = num_POI;
            this.TIMESTEPS = TIMESTEPS;
            MIN_OBS_DIST = min_o_d;
            MAX_OBS_DIST = max_o_d;
            DO_LOCAL = DO_L;
            DO_GLOBAL = DO_G;
            DO_DIFFERENCE = DO_D;

            // set up rovers
            if (G_R)
            {
                Rover r;
                for (int i = 0; i < num_rovers; i++)
                {
                    int x = rand.Next(1, 500);
                    int y = rand.Next(1, 500);
                    int head = 0;
                    fidos.Add(r = new Rover(x, y, head, this.num_rovers, this.num_POI, MIN_OBS_DIST));
                }

                write_rovers_to_file();
            }

            else
            {
                double [,] rov_pos = read_rovers_from_file();
                Rover r;
                for (int i = 0; i < num_rovers; i++)
                {
                    int head = 0;
                    fidos.Add(r = new Rover(rov_pos[0,i], rov_pos[1,i], head, this.num_rovers, this.num_POI, MIN_OBS_DIST));
                }
            }

            // set up POIs
            if (G_P)
            {
                Landmark l;
                for (int i = 0; i < num_POI; i++)
                {
                    int x = rand.Next(1, 500);
                    int y = rand.Next(1, 500);
                    int value = rand.Next(20, 40);
                    landmarks.Add(l = new Landmark(x, y, value, this.num_rovers, MIN_OBS_DIST, MAX_OBS_DIST));
                }

                write_POIs_to_file();
            }

            else
            {
                List<double[]> POI_pos = read_POIs_from_file();
                Landmark l;
                for (int i = 0; i < num_POI; i++)
                    landmarks.Add(l = new Landmark(POI_pos[0][i], POI_pos[1][i], POI_pos[2][i], this.num_rovers, MIN_OBS_DIST, MAX_OBS_DIST));
            }

            for (int i = 0; i < num_rovers; i++)
            {
                Global_rewards.Add(new List<double>());
                Difference_rewards.Add(new List<double>());
            }
        }

        public void get_and_use_neuroevo_policies(List<List<NeuralNetworks.Network>> population, int EVOPOP)
        {
            //Console.WriteLine("GET AND USE NEUROEVO POLICIES");
            //swaps the indeces in "Selected" list for each fido
            //for creating teams of policies
            for (int i = 0; i < num_rovers; i++)
            {
                fidos[i].set_up_selected(EVOPOP);
            }

            for (int r = 0; r < num_rovers; r++)
            {
                int SWAPS = 0;
                for (int i = 0; i < SWAPS; i++)
                {
                    int p1 = rand.Next(0,500000) % EVOPOP;
                    int p2 = rand.Next(0,500000) % EVOPOP;
                    int holder;

                    holder = fidos[r].Selected[p1];
                    fidos[r].Selected[p1] = fidos[r].Selected[p2];
                    fidos[r].Selected[p2] = holder;
                }
            }
            //end swap policies


            for (int ev = 0; ev < EVOPOP; ev++)
            {
                for (int k = 0; k < num_rovers; k++)
                {
                    fidos[k].replace();
                    fidos[k].clear_rewards();
                }

                for (int t = 0; t < TIMESTEPS; t++)
                {
                    /// SENSE
                    //perform rover move/score policies
                    for (int r = 0; r < num_rovers; r++)
                    {
                        fidos[r].Heading = 0;
                        fidos[r].sense(landmarks, fidos);
                    }

                    /// DECIDE
                    for (int r = 0; r < num_rovers; r++)
                    {
                        fidos[r].decide(ev, population[r]);
                    }

                    /// MOVE
                    for (int r = 0; r < num_rovers; r++)
                    {
                        fidos[r].act();
                    }

                    /// REACT
                    react(fidos, landmarks);
                }

                collect(fidos, landmarks, ev, population); // End of episode cleanup.
            } // END OF EVOPOP LOOP
        }



        void react(List<Rover> fidos, List<Landmark> landmarks)
        {
            /// Find distance from each POI to each rover.
            for (int i = 0; i < num_POI; i++)
            {
                landmarks[i].find_dist_to_all_rovers(fidos);
            }
            /// Find Local Rewards
            if (DO_LOCAL)
            {
                calculate_locals(fidos, landmarks);
            }
            /// Find Global Rewards
            if (DO_LOCAL || DO_GLOBAL || DO_DIFFERENCE)
            {
                //globalflag = true;
                calculate_globals(fidos, landmarks); /// Always done so we can evaluate team performance.
            }
            /// Find Difference Rewards
            if (DO_DIFFERENCE)
            {
                calculate_differences(fidos, landmarks);
            }
            /// Assign Rewards to Rovers (See Collect)
        }

        void calculate_locals(List<Rover> fidos, List<Landmark> landmarks)
        {
            /// Each rover calculates its observation values for each POI, disregarding all other rovers.
            for (int i = 0; i < num_rovers; i++)
            {
                for (int p = 0; p < num_POI; p++)
                {
                    int l_reward = (int)landmarks[p].calc_observation_value(landmarks[p].Distances[i]);
                    fidos[i].Local_chunks.Add(l_reward);
                }
            }
        }

        void calculate_globals(List<Rover> fidos, List<Landmark> landmarks)
        {
            /// Each POI gives the value of the closest observation to ALL rovers.
            for (int p = 0; p < num_POI; p++)
            {
                int closest = landmarks[p].find_kth_closest_rover(0, fidos);
                double reward = landmarks[p].calc_observation_value(landmarks[p].Distances[closest]);
                //Console.WriteLine(reward.ToString());

                for (int r = 0; r < num_rovers; r++)
                {
                    fidos[r].Global_chunks.Add(reward);
                }
            }
        }

        void calculate_differences(List<Rover> fidos, List<Landmark> landmarks)
        {
            /// Calculate Difference Rewards
            for (int p = 0; p < num_POI; p++)
            {
                int closest = landmarks[p].find_kth_closest_rover(0, fidos);
                int second_closest = landmarks[p].find_kth_closest_rover_not_i(0, closest, fidos);
                /// Globals
                double g_reward = landmarks[p].calc_observation_value(landmarks[p].Distances[closest]);
                //Console.WriteLine(g_reward.ToString());
                /// Counterfactuals
                double c_reward = landmarks[p].calc_observation_value(landmarks[p].Distances[second_closest]);
                //Console.WriteLine(c_reward.ToString());
                //Console.WriteLine();
                /// Push Back Difference
                fidos[closest].Difference_chunks.Add(g_reward - c_reward);
            }
        }

        void collect(List<Rover> fidos, List<Landmark> landmarks, int ev, List<List<NeuralNetworks.Network>> population)
        {
            for (int r = 0; r < num_rovers; r++)
            {
                int thisone = fidos[r].Selected[ev];
                if (DO_LOCAL)
                {
                    double localvalue = fidos[r].Local_chunks.Sum();

                    population[r][thisone].Fitness = localvalue;
                    //local_rewards[r].Add(localvalue);
                }

                if (DO_LOCAL || DO_GLOBAL || DO_DIFFERENCE)
                {
                    double globalvalue = fidos[r].Global_chunks.Sum();
                    //Console.WriteLine(globalvalue.ToString());
                    Global_rewards[r].Add(globalvalue);
                }

                if (DO_GLOBAL)
                {
                    double globalvalue = fidos[r].Global_chunks.Sum();
                    population[r][thisone].Fitness = globalvalue;
                }

                if (DO_DIFFERENCE)
                {
                    double differencevalue = fidos[r].Difference_chunks.Sum();

                    population[r][thisone].Fitness = differencevalue;
                    //Difference_rewards[r].Add(differencevalue);
                }
            }
        }

        void write_rovers_to_file()
        {
            List<List<double>> pos = new List<List<double>>();
            pos.Add(new List<double>());
            pos.Add(new List<double>());
            for (int r = 0; r < num_rovers; r++)
            {
                pos[0].Add(fidos[r].X);
                pos[1].Add(fidos[r].Y);
            }

            double[] posx = pos[0].ToArray();
            double[] posy = pos[1].ToArray();
            double[][] positions_j ={ posx, posy };
            double[,] positions_m = To2D(positions_j);
            DE.Export2DArray(positions_m, "rovers_pos");
        }

        void write_POIs_to_file()
        {
            List<List<double>> pos = new List<List<double>>();
            pos.Add(new List<double>());
            pos.Add(new List<double>());
            pos.Add(new List<double>());
            for (int l = 0; l < num_POI; l++)
            {
                pos[0].Add(landmarks[l].X);
                pos[1].Add(landmarks[l].Y);
                pos[2].Add(landmarks[l].POI_value1);
            }

            double[] posx = pos[0].ToArray();
            double[] posy = pos[1].ToArray();
            double[] posv = pos[2].ToArray();

            List<double[]> positions = new List<double[]>() { posx, posy, posv };

            DE.ExportData(positions, "POIs_pos");
        }

        double[,] read_rovers_from_file()
        {
            double[,] positions_m = DI.parseCSV("rovers_pos");
            return positions_m;
        }

        List<double[]> read_POIs_from_file()
        {
            List<double[]> positions = DI.parseCSVToList("POIs_pos");
            return positions;
        }

        // Method to convert jagged array to multidimensional array
        static T[,] To2D<T>(T[][] source)
        {
            try
            {
                int FirstDim = source.Length;
                int SecondDim = source.GroupBy(row => row.Length).Single().Key; // throws InvalidOperationException if source is not rectangular

                var result = new T[FirstDim, SecondDim];
                for (int i = 0; i < FirstDim; ++i)
                    for (int j = 0; j < SecondDim; ++j)
                        result[i, j] = source[i][j];

                return result;
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("The given jagged array is not rectangular.");
            }
        }
    }
}
