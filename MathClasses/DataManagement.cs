using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathClasses
{
	public class DataManagement
	{
		// method to take downsample dataset
		// every n elements are averaged
		public double[,] MeanSampleData(double[,] data, int n)
		{
			// find matrix dimensions
			int dim1 = data.GetLength(0);
			int dim2 = data.GetLength(1);
			int numElements = Convert.ToInt32(Math.Floor(Convert.ToDouble(dim1) / Convert.ToDouble(n)));

			List<double[]> tempOutput = new List<double[]>();
			double[] tempArray;

			for (int index = 0; index < numElements; index++)
			{
				tempArray = new double[dim2];
				for (int i = 0; i < n; i++)
				{
					for (int j = 0; j < dim2; j++)
					{
						tempArray[j] += data[index * n + i, j] / Convert.ToDouble(n);
					}
				}
				tempOutput.Add(tempArray);
			}

			if (numElements * n + n < dim1 - 1)
			{
				tempArray = new double[dim2];
				for (int i = numElements * n + n; i < dim1; i++)
				{
					for (int j = 0; j < dim2; j++)
					{
						tempArray[j] += data[i, j] / Convert.ToDouble(n);
					}
				}
				tempOutput.Add(tempArray);
			}

			// find output dimensions
			dim1 = tempOutput.Count();
			dim2 = tempOutput[0].GetLength(0);
			double[,] output = new double[dim1, dim2];
			for (int i = 0; i < dim1; i++)
			{
				for (int j = 0; j < dim2; j++)
				{
					output[i, j] = tempOutput[i][j];
				}
			}
			return output;
		}

		// method to downsample data
		// takes every n points (no averaging)
		public double[,] DownSample(double[,] rawData, int numPoints)
		{
			// find dimensions of raw data set
			int dim1 = rawData.GetLength(0);
			int dim2 = rawData.GetLength(1);

			// initialize new temp output
			List<double[]> tempOutput = new List<double[]>();

			// find iteration value
			int iterValue = Convert.ToInt32(Math.Floor(Convert.ToDouble(dim1) / Convert.ToDouble(numPoints)));

			// populate downsampled matrix
			double[] tempArray;
			for (int i = 0; i < numPoints; i++)
			{
				tempArray = new double[dim2];
				for (int j = 0; j < dim2; j++)
				{
					tempArray[j] = rawData[i * iterValue, j];
				}
				tempOutput.Add(tempArray);
			}

			// make sure endpoint is included
			if (tempOutput[numPoints - 1][0] != rawData[dim1 - 1, 0])
			{
				tempArray = new double[dim2];
				for (int j = 0; j < dim2; j++)
				{
					tempArray[j] = rawData[dim1 - 1, j];
				}
				tempOutput.Add(tempArray);
			}

			// find dimensions of temp array
			int tempDim = tempOutput.Count();

			// populate output matrix and return it
			double[,] output = new double[tempDim, dim2];
			for (int i = 0; i < tempDim; i++)
			{
				for (int j = 0; j < dim2; j++)
				{
					output[i, j] = tempOutput[i][j];
				}
			}
			return output;
		}
	}
}
