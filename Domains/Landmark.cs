using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains
{
    public class Landmark
    {
        int MIN_OBS_DIST;
        int MAX_OBS_DIST;

        int num_rovers;

        double POI_value;
        double POI_value_start;
        double x;
        double y;
        List<double> distances;
        List<double> distances_k;
        List<double> distances_k_not_i;

        public double X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }
        }

        public double Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }

        public double POI_value1
        {
            get
            {
                return POI_value;
            }

            set
            {
                POI_value = value;
            }
        }

        public List<double> Distances
        {
            get
            {
                return distances;
            }

            set
            {
                distances = value;
            }
        }

        public List<double> Distances_k
        {
            get
            {
                return distances_k;
            }

            set
            {
                distances_k = value;
            }
        }

        public List<double> Distances_k_not_i
        {
            get
            {
                return distances_k_not_i;
            }

            set
            {
                distances_k_not_i = value;
            }
        }

        public Landmark(double xpos, double ypos, double value, int num_rovers, int min_o_d, int max_o_d)
        {
            this.num_rovers = num_rovers;

            X = xpos;
            Y = ypos;
            POI_value1 = value;
            POI_value_start = POI_value1;
            MIN_OBS_DIST = min_o_d;
            MAX_OBS_DIST = max_o_d;

            Distances = new List<double>();
            Distances_k = new List<double>();
            Distances_k_not_i = new List<double>();
        }

        void reset()
        {
            POI_value1 = POI_value_start;
        }
        
        public int find_kth_closest_rover(int k, List<Rover> fidos)
        {
            int closest = 0;
            double closest_distance;
            Distances_k = new List<double>();
            for (int b = 0; b < num_rovers; b++)
            {
                double delx, dely;
                delx = fidos[b].X - X;
                dely = fidos[b].Y - Y;
                double dis = Math.Sqrt(delx * delx + dely * dely);
                Distances_k.Add(dis);
                //Console.WriteLine(dis.ToString());
            }
            Distances_k.Sort();
            //Distances_k = Distances_k.OrderByDescending(d => d).ToList();
            closest_distance = Distances_k[k];
            for (int b = 0; b < num_rovers; b++)
            {
                double delx, dely;
                delx = fidos[b].X - X;
                dely = fidos[b].Y - Y;
                double dis = Math.Sqrt(delx * delx + dely * dely);
                if (dis == closest_distance)
                {
                    closest = b;
                    break;
                }
            }
            Distances_k.Clear();
            return closest;
        }

        public int find_kth_closest_rover_not_i(int k, int i, List<Rover> fidos)
        {
            int closest = 0;
            double closest_distance;
            Distances_k_not_i = new List<double>();
            for (int b = 0; b < num_rovers; b++)
            {
                if (b == i) { continue; }
                double delx, dely;
                delx = fidos[b].X - X;
                dely = fidos[b].Y - Y;
                double dis = Math.Sqrt(delx * delx + dely * dely);
                Distances_k_not_i.Add(dis);
            }
            Distances_k_not_i.Sort();
            closest_distance = Distances_k_not_i[k];
            for (int b = 0; b < num_rovers; b++)
            {
                if (b == i) { continue; }
                double delx, dely;
                delx = fidos[b].X - X;
                dely = fidos[b].Y - Y;
                double dis = Math.Sqrt(delx * delx + dely * dely);
                if (dis == closest_distance)
                {
                    closest = b;
                    break;
                }
            }
            Distances_k_not_i.Clear();
            return closest;
        }

        double find_dist_to_rover(int rvr, List<Rover> fidos)
        {
            double delx, dely;
            delx = fidos[rvr].X - X;
            dely = fidos[rvr].Y - Y;
            double dis = Math.Sqrt(delx * delx + dely * dely);

            return dis;
        }

        public void find_dist_to_all_rovers(List<Rover> fidos)
        {
            Distances.Clear();
            for (int i = 0; i < num_rovers; i++)
            {
                Distances.Add(find_dist_to_rover(i, fidos));
            }
        }

        public double calc_observation_value(double d)
        {
            double val;
            d = Math.Max(d, MIN_OBS_DIST);
            if (d > MAX_OBS_DIST)
            {
                return 0;
            }
            val = POI_value1 / d;
            return val;
        }
        
    }
}
