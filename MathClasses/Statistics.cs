using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathClasses
{
	public class Statistics
	{
		// method to find the mean value of a vector (array)
		public double Mean(double[] vec)
		{
			// find length of vector and convert to double
			int dim = vec.GetLength(0);
			double dimD = Convert.ToDouble(dim);

			// find mean value
			double output = 0.0;
			for (int i = 0; i < dim; i++)
			{
				output += vec[i];
			}
			output /= dimD;
			return output;
		}

		// method to find the mean value of a vector (list)
		public double Mean(List<double> vec)
		{
			// find length of vector and convert to a double
			int dim = vec.Count();
			double dimD = Convert.ToDouble(dim);

			// find mean value
			double output = 0.0;
			for (int i = 0; i < dim; i++)
			{
				output += vec[i];
			}
			output /= dimD;
			return output;
		}

		// method to find standard deviation of a vector (array)
		public double StandardDeviation(double[] vector)
		{
			// find mean
			double mean = Mean(vector);

			// define output
			double output = 0.0;

			// find sum of square error
			for (int i = 0; i < vector.GetLength(0); i++)
			{
				output += (vector[i] - mean) * (vector[i] - mean);
			}

			// mean mean squared error
			output /= Convert.ToDouble(vector.GetLength(0));

			// find standard deviation
			output = Math.Sqrt(output);

			// return output
			return output;
		}

		// method to find standard deviation of a vector (list)
		public double StandardDeviation(List<double> vector)
		{
			// find mean
			double mean = Mean(vector);

			// find size of vector
			int dim = vector.Count();

			// initialize output
			double output = 0.0;

			// find sum of square error
			for (int i = 0; i < dim; i++)
			{
				output += (vector[i] - mean) * (vector[i] - mean);
			}

			// mean squared error
			output /= Convert.ToDouble(dim);

			// standard deviation
			output = Math.Sqrt(output);
			return output;
		}

		// method to find statistical data from many runs
		public double[,] RunStats(List<double[]> statsData)
		{
			// find number of timesteps and statruns
			int timeSteps = statsData[0].GetLength(0);
			int statRuns = statsData.Count();

			// create temporary vector and output vector
			double[,] output = new double[timeSteps, 3];
			double[] tempVec;

			// for each timestep, find data from each stat run and find mean and error in the mean
			for (int ts = 0; ts < timeSteps; ts++)
			{
				tempVec = new double[statRuns];
				for (int sr = 0; sr < statRuns; sr++)
				{
					tempVec[sr] = statsData[sr][ts];
				}
				output[ts, 0] = Convert.ToDouble(ts);
				output[ts, 1] = Mean(tempVec);
				output[ts, 2] = StandardDeviation(tempVec) / Math.Sqrt(Convert.ToDouble(statRuns));
			}

			return output;
		}

		// method to determine the correlation between two vectors
		public double Correlation(double[] vec1, double[] vec2)
		{
			// find vector dimesions
			int dim1 = vec1.GetLength(0);
			int dim2 = vec2.GetLength(0);

			// check dimensional consistency
			if (dim1 != dim2)
			{
				Console.WriteLine("Correlation Error: Inconsistent Vector Dimensions");
				return 0.0;
			}

			// find mean values of both vectors
			double mean1 = Mean(vec1);
			double mean2 = Mean(vec2);

			// find error in mean vectors
			double[] errorVec1 = new double[dim1];
			double[] errorVec2 = new double[dim2];
			for (int i = 0; i < dim1; i++)
			{
				errorVec1[i] = vec1[i] - mean1;
				errorVec2[i] = vec2[i] - mean2;
			}

			// find total of products of error in the mean
			double numerator = 0.0;
			for (int i = 0; i < dim1; i++)
			{
				numerator += errorVec1[i] * errorVec2[i];
			}

			// find total of squared error in the mean of vec1 and vec2
			double denom1 = 0.0;
			double denom2 = 0.0;
			for (int i = 0; i < dim1; i++)
			{
				denom1 += errorVec1[i] * errorVec1[i];
				denom2 += errorVec2[i] * errorVec1[i];
			}

			// return correlation coefficient
			double output;
			if (denom1 * denom2 != 0)
			{
				output = numerator / (denom1 * denom2);
			}
			else
			{
				output = 0.0;
			}
			return output;
		}

		// method to get a column vector from a matrix
		public double[] GetColumnVec(double[,] matrix, int index)
		{
			int dim1 = matrix.GetLength(0);
			int dim2 = matrix.GetLength(1);
			double[] output = new double[dim2];
			for (int i = 0; i < dim2; i++)
			{
				output[i] = matrix[index, i];
			}
			return output;
		}

		// method to determine correlation coefficient matrix for a dataset
		// this method assumes dataset is a double[,], where column vectors each correspond to one varible
		public double[,] CorrelationMatrix(double[,] matrix)
		{
			// find matrix dimensions
			int dim1 = matrix.GetLength(0);
			int dim2 = matrix.GetLength(1);

			// initialize output matrix
			double[,] output = new double[dim2, dim2];
			double[] vec1;
			double[] vec2;
			for (int i = 0; i < dim2; i++)
			{
				vec1 = GetColumnVec(matrix, i);
				for (int j = 0; j < dim2; j++)
				{
					vec2 = GetColumnVec(matrix, j);
					output[i, j] = Correlation(vec1, vec2);
				}
			}
			return output;
		}

		// method to find most highly correlated variables from a correlation matrix
		public double[,] MaxCorrelations(double[,] correlationMat)
		{
			// find matrix dimensions
			int dim1 = correlationMat.GetLength(0);
			int dim2 = correlationMat.GetLength(1);

			// check dimensions
			if (dim1 != dim2)
			{
				Console.WriteLine("MaxCorrelations Error: Inconsistent Matrix Dimensions");
			}

			// initialize output
			double[,] output = new double[dim1, 2];
			int tempIndex;
			double tempVal;
			for (int i = 0; i < dim1; i++)
			{
				if (i == 0)
				{
					tempIndex = 1;
					tempVal = correlationMat[i, 1];
				}
				else
				{
					tempIndex = 0;
					tempVal = correlationMat[i, 0];
				}
				for (int j = 0; j < dim1; j++)
				{
					if (j != i)
					{
						if (Math.Abs(correlationMat[i, j]) > Math.Abs(tempVal))
						{
							tempVal = correlationMat[i, j];
							tempIndex = j;
						}
					}
				}
				output[i, 0] = Convert.ToDouble(i);
				output[i, 1] = tempIndex;
			}
			return output;
		}
	}
}
