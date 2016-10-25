using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathClasses
{
	public class NNMath
	{
		// method to compute sigmoid activation function
		public double Sigmoid(double x)
		{
			return 1.0 / (1.0 + Math.Exp(-1.0 * x));
		}

		// method to take sigmoid of every element of a vector (array)
		public double[] VectorSigmoid(double[] vec)
		{
			// find dimensions of vector
			int dim = vec.GetLength(0);

			// initialize output vector
			double[] output = new double[dim];

			// populate output vector
			for (int i = 0; i < dim; i++)
			{
				output[i] = Sigmoid(vec[i]);
			}
			return output;
		}
	}
}
