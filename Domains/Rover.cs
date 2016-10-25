using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains
{
    public class Rover
    {
        double pi = 3.141529;
        int QUADRANTS = 4;
        int XMIN = 0;
        int XMAX = 500;
        int YMIN = 0;
        int YMAX = 500;

        int MIN_OBS_DIST;

        int num_POI;
        int num_rovers;

        int num = 1;

        double heading;
        double x;
        double y;
        double xstart;
        double ystart;
        double headingstart;
        double xdot;
        double ydot;
        int ID;
        List<double> rover_state;
        List<double> POI_state;

        List<int> selected = new List<int>();

        double local_reward;

        List<double> local_chunks = new List<double>();
        List<double> global_chunks = new List<double>();
        List<double> difference_chunks = new List<double>();

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

        public List<int> Selected
        {
            get
            {
                return selected;
            }

            set
            {
                selected = value;
            }
        }

        public List<double> Local_chunks
        {
            get
            {
                return local_chunks;
            }

            set
            {
                local_chunks = value;
            }
        }

        public List<double> Global_chunks
        {
            get
            {
                return global_chunks;
            }

            set
            {
                global_chunks = value;
            }
        }

        public List<double> Difference_chunks
        {
            get
            {
                return difference_chunks;
            }

            set
            {
                difference_chunks = value;
            }
        }

        public double Heading
        {
            get
            {
                return heading;
            }

            set
            {
                heading = value;
            }
        }

        void angle_resolve(double angle)
        {
            while (angle > 2 * pi)
            {
                angle -= 2 * pi;
            }
            while (angle < 0)
            {
                angle += 2 * pi;
            }
        }

        void angle_resolve_pmpi(double angle)
        {
            while (angle > pi)
            {
                angle -= 2 * pi;
            }
            while (angle < -pi)
            {
                angle += 2 * pi;
            }
        }

        void xresolve()
        {
            while (X < XMIN)
            {
                X = XMIN + 1;
            }
            while (X > XMAX)
            {
                X = XMAX - 1;
            }
        }

        void yresolve()
        {
            while (Y < YMIN)
            {
                Y = YMIN + 1;
            }
            while (Y > YMAX)
            {
                Y = YMAX - 1;
            }
        }

        public void replace()
        {
            /// resets the rovers to starting position after each policy is implemented
            Heading = headingstart;
            X = xstart;
            Y = ystart;
            xdot = 0;
            ydot = 0;
            local_reward = 0;
        }

        public void clear_rewards()
        {
            Local_chunks.Clear();
            Global_chunks.Clear();
            Difference_chunks.Clear();
        }

        void reset()
        {
            /// clears the rover's information, for easier debugging.
            Heading = 0;
            X = 0;
            Y = 0;
            xdot = 0;
            ydot = 0;
            local_reward = 0;
        }

        void move()
        {
            X += xdot;
            Y += ydot;
            xresolve();
            yresolve();
            Heading = Math.Atan2(ydot, xdot);
        }

        public Rover(double xspot, double yspot, double head, int num_rovers, int num_POI, int min_o_d)
        {
            this.num_rovers = num_rovers;
            this.num_POI = num_POI;
            MIN_OBS_DIST = min_o_d;

            /// places this rover in the world with the specified x,y,theta.
            ID = num;
            num++;
            X = xspot;
            Y = yspot;
            xstart = xspot;
            ystart = yspot;
            headingstart = head;
            Heading = head;
            xresolve();
            yresolve();
            angle_resolve(Heading);

            rover_state = new List<double>();
            POI_state = new List<double>();

            if (X > XMIN && Y > YMIN && X < XMAX && Y < YMAX)
            {
                //return 0;
            }
            else
            {
                Console.WriteLine("rover::place error");
                //return 1;
            }
        }

        int basic_sensor(double roverx, double rovery, double rover_heading, double tarx, double tary)
        {

            double dx;
            double dy;

            dx = tarx - roverx;
            dy = tary - rovery;

            // heading to target with respect to robot frame
            double tarheading;
            tarheading = Math.Atan2(dy, dx);

            double del_heading;
            del_heading = tarheading - rover_heading;
            angle_resolve_pmpi(del_heading);

            //cout << "del_heading: " << del_heading << endl;

            double nw = pi / 4;
            double ne = -pi / 4;
            double sw = 3 * pi / 4;
            double se = -3 * pi / 4;

            //cout << "Deltas (x,y) : " << dx << "\t" << dy << endl;

            if (del_heading < nw && del_heading > ne)
            {
                /// object is "ahead" of the robot.
                //cout << "Ahead" << endl;
                return 0;
            }
            if (del_heading >= nw && del_heading < sw)
            {
                //cout << "Left" << endl;
                /// object is "left" of the robot
                return 1;
            }
            if (del_heading <= ne && del_heading > se)
            {
                //cout << "Right" << endl;
                ///object is "right" of the robot
                return 2;
            }
            if (del_heading <= se || del_heading >= sw)
            {
                //cout << "Behind" << endl;
                ///object is "behind" the robot
                return 3;
            }

            else
            {
                Console.WriteLine("problems in basic_sensor;");
                return 66;
            }
        }

        double strength_sensor(double value, double tarx, double tary)
        {
            double numerator;
            double denominator;
            double strength;

            numerator = value;
            double delx = X - tarx;
            double dely = Y - tary;
            denominator = Math.Max(Math.Sqrt(delx * delx + dely * dely), MIN_OBS_DIST);

            strength = numerator / denominator;

            return strength;
        }
        
        void full_POI_sensor(List<Landmark> POIs)
        {
            int quadrant;
            for (int i = 0; i < QUADRANTS; i++)
            {
                POI_state.Add(0);
            }
            for (int i = 0; i < num_POI; i++)
            {
                quadrant = basic_sensor(X, Y, Heading, POIs[i].X, POIs[i].Y);
                double value = POIs[i].POI_value1;
                double tarx = POIs[i].X;
                double tary = POIs[i].Y;
                double str = strength_sensor(value, tarx, tary);

                POI_state[quadrant] += str;
            }
        }

        void full_rover_sensor(List<Rover> fidos)
        {
            int quadrant;
            for (int i = 0; i < QUADRANTS; i++)
            {
                rover_state.Add(0);
            }
            for (int r = 0; r < num_rovers; r++)
            {
                if (r == ID)
                {
                    continue;
                }
                quadrant = basic_sensor(X, Y, Heading, fidos[r].X, fidos[r].Y);
                double value = 1;
                double tarx = fidos[r].X;
                double tary = fidos[r].Y;
                double str = strength_sensor(value, tarx, tary);

                rover_state[quadrant] += str;
            }
        }

        public void set_up_selected(int evopop)
        {
            for (int i = 0; i < evopop; i++)
            {
                Selected.Add(i);
            }
        }

        public void sense(List<Landmark> POIs, List<Rover> fidos)
        {
            Heading = 0;
            //if (TELEPORTATION == 0)
            //{
            //cout << "sensing " << r << endl;
            full_POI_sensor(POIs);
            full_rover_sensor(fidos);
                //cout << "end sensing " << r << endl;
            //}
            //if (TELEPORTATION == 1)
            //{
                /// stateless.
            //}
        }
        
        public void decide(int ev, List<NeuralNetworks.Network> pop)
        {
            double[] inp = new double[8];

            for (int i = 0; i < QUADRANTS; i++)
            {
                inp[i] = POI_state[i];
            }
            for (int i = 0; i < QUADRANTS; i++)
            {
                inp[4+i] = rover_state[i];
            }

            POI_state.Clear();
            rover_state.Clear();

            double[] output = pop[selected[ev]].ForwardPass(inp);

            xdot = output[0];
            ydot = output[1];
        }

        public void act()
        {
            move();
        }
    }
}
