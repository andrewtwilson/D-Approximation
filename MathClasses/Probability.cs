using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathClasses
{
	public class Probability
	{
		// create private class constructor
		private Probability() { lockObject = new object(); }

		// create singleton to access probability class
		private static Probability instance = null;
		object lockObject;
		public static Probability Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new Probability();
				}
				return instance;
			}
		}

		// call relevant classes
		Random myRand = new Random();


		// method to draw numbers from Gaussian distribution
		// created using Box-Muller transform
		public double Gaussian(double std, double mean)
		{
			lock (lockObject)
			{
				double x = 0.0;
				double y = 0.0;
				double rds = x * x + y * y;
				while (rds == 0.0 || rds > 1)
				{
					x = myRand.NextDouble() * 2 - 1;
					y = myRand.NextDouble() * 2 - 1;
					rds = x * x + y * y;
				}
				double c = Math.Sqrt(-2 * Math.Log(rds) / rds);
				double output = c * x * std + mean;
				return output;
			}
		}

		// method to return random double between 0 and 1
		public double NextDouble()
		{
			lock (lockObject)
			{
				return myRand.NextDouble();
			}
		}

		// method to return random integer up to n (non-inclusive)
		public int Next(int n)
		{
			lock (lockObject)
			{
				return myRand.Next(n);
			}
		}
	}
}
