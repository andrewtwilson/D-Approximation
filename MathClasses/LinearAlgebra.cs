using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathClasses
{
	public class LinearAlgebra
	{
		// method to compute the dot product of two vectors
		public double DotProduct(double[] vec1, double[] vec2)
		{
			// find dimensions of each vector
			int dim1 = vec1.GetLength(0);
			int dim2 = vec2.GetLength(0);

			// check for dimensional consistency between vectors
			if (dim1 != dim2)
			{
				Console.WriteLine("DotProduct Error: Inconsistent Array Dimensions");
				if (dim1 > dim2)
				{
					dim1 = dim2;
				}
			}

			// compute dot product
			double output = 0.0;
			for (int i = 0; i < dim1; i++)
			{
				output += vec1[i] * vec2[i];
			}
			return output;
		}

		// method normalize a vector such that the sum of all elements is one
		public double[] NormalizeVector(double[] vec)
		{
			double sum = vec.Sum();
			int dim = vec.GetLength(0);
			double[] output = new double[dim];
			for (int i = 0; i < dim; i++)
			{
				output[i] = vec[i] / sum;
			}
			return output;
		}

		// method to perform matrix by vector multiplication
		public double[] MatByVec(double[,] mat, double[] vec)
		{
			// find matrix dimensions
			int matDim1 = mat.GetLength(0);
			int matDim2 = mat.GetLength(1);

			// find vector dimensions
			int vecDim = vec.GetLength(0);

			// check dimensional consistency
			if (matDim2 != vecDim)
			{
				Console.WriteLine("MatByVec Error: Inconsistent Dimensions");
			}

			// perform matrix by vector multiplication
			double[] output = new double[matDim1];

			for (int i=0;i<matDim1;i++)
				//Parallel.For(0, matDim1, i =>
			{
				for (int j = 0; j < matDim2; j++)
				{
					output[i] += mat[i, j] * vec[j];
				}
			}//);
			return output;
		}

		// method to append a value to an array
		public double[] Append(double[] vec, double val)
		{
			// find vector dimensions
			int dim = vec.GetLength(0);

			// initialize output
			double[] output = new double[dim + 1];

			// populate output
			Array.Copy(vec, output, dim);
			output[dim] = val;
			return output;
		}

		// take specific columns from a matrix
		public double[,] TakeColumns(double[,] mat, int[] colIndices)
		{
			// find matrix dimensions
			int dim1 = mat.GetLength(0);
			int dim2 = mat.GetLength(1);

			// initialize output matrix
			int numCols = colIndices.GetLength(0);
			double[,] output = new double[dim1, numCols];

			// populate output matrix
			for (int i = 0; i < dim1; i++)
			{
				for (int j = 0; j < numCols; j++)
				{
					output[i, j] = mat[i, colIndices[j]];
				}
			}
			return output;
		}

		// turn 2D array into a list of arrays
		public List<double[]> ArrayToList(double[,] mat)
		{
			// find matrix dimensions
			int dim1 = mat.GetLength(0);
			int dim2 = mat.GetLength(1);

			// initialize output and temp var
			List<double[]> output = new List<double[]>();
			double[] tempArray;

			// populate output
			for (int i = 0; i < dim1; i++)
			{
				tempArray = new double[dim2];
				for (int j = 0; j < dim2; j++)
				{
					tempArray[j] = mat[i, j];
				}
				output.Add(tempArray);
			}
			return output;
		}

		// join 2 arrays
		public double[] Join(double[] vec1, double[] vec2)
		{
			// find vector dimensions
			int dim1 = vec1.GetLength(0);
			int dim2 = vec2.GetLength(0);

			// initialize output
			double[] output = new double[dim1 + dim2];
			vec1.CopyTo(output, 0);
			vec2.CopyTo(output, dim1);
			return output;
		}

		// take first n rows from a matrix
		public double[,] FirstNRows(double[,] mat, int n)
		{
			// find matrix dimensions
			int dim1 = mat.GetLength(0);
			int dim2 = mat.GetLength(1);

			// initialize output
			double[,] output = new double[n, dim2];

			// populate output
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < dim2; j++)
				{
					output[i, j] = mat[i, j];
				}
			}
			return output;
		}

		// take last n rows from a matrix
		public double[,] LastNRows(double[,] mat, int n)
		{
			// find matrix dimensions
			int dim1 = mat.GetLength(0);
			int dim2 = mat.GetLength(1);

			// initialize output
			double[,] output = new double[n, dim2];

			// populate output
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < dim2; j++)
				{
					output[i, j] = mat[dim1 - n + i, j];
				}
			}
			return output;
		}

		// method to extract a column vector from a matrix
		public double[] GetColumnVec(double[,] matrix, int index)
		{
			// find matrix dimensions
			int dim1 = matrix.GetLength(0);
			int dim2 = matrix.GetLength(1);

			// check dimensional consistency
			if (index + 1 > dim2)
			{
				Console.WriteLine("GetColumnVecError: Requested Index Out of Bounds");
				return null;
			}

			// initialize and populate column vector
			double[] output = new double[dim1];
			for (int i = 0; i < dim1; i++)
			{
				output[i] = matrix[i, index];
			}
			return output;
		}

		// method to subtract one vector from another
		public double[] VecMinusVec(double[] vec1, double[] vec2)
		{
			// find dimensions of vectors
			int dim1 = vec1.GetLength(0);
			int dim2 = vec2.GetLength(0);

			// check dimensional consistency
			if (dim1 != dim2)
			{
				Console.WriteLine("VecMinusVec Error: Inconsistent Dimensions");
				if (dim1 > dim2)
				{
					dim1 = dim2;
				}
			}

			// find output
			double[] output = new double[dim1];
			for (int i=0;i<dim1;i++)
			{
				output[i] = vec1[i] - vec2[i];
			}
			return output;
		}

		// method to take first n elements of an array
		public double[] Take(double[] vec, int dim)
		{
			double[] output = new double[dim];
			for (int i = 0; i < dim; i++)
			{
				output[i] = vec[i];
			}
			return output;
		}
	}
}
