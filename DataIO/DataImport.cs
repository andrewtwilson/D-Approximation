using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DataIO
{
	public class DataImport
	{
		// NOTE: all methods import from current directory

		// method to parse a csv into a 2D array
		public double[,] parseCSV(string name)
		{
			string path = Environment.CurrentDirectory.ToString();
			path = System.IO.Directory.GetParent(path).ToString();
			path = System.IO.Directory.GetParent(path).ToString();
			path = System.IO.Directory.GetParent(path).ToString();

			List<string[]> parsedData = new List<string[]>();

			try
			{
				using (StreamReader readFile = new StreamReader(path + "/" + name + ".csv"))
				{
					string line;
					string[] row;

					while ((line = readFile.ReadLine()) != null)
					{
						row = line.Split(',');
						parsedData.Add(row);
					}
				}
			}
			catch (Exception e)
			{

			}

			int dim1 = parsedData.Count;
			int dim2 = parsedData[0].GetLength(0);
			double[,] output = new double[dim1, dim2];
			for (int i = 0; i < dim1; i++)
			{
				for (int j = 0; j < dim2; j++)
				{
					output[i, j] = Convert.ToDouble(parsedData[i][j]);
				}
			}

			return output;
		}

		// method to import a 1D array of integers (from a csv)
		public int[] Import1DIntArray(string name)
		{
			string path = Environment.CurrentDirectory.ToString();
			path = System.IO.Directory.GetParent(path).ToString();
			path = System.IO.Directory.GetParent(path).ToString();
			path = System.IO.Directory.GetParent(path).ToString();

			List<string[]> parsedData = new List<string[]>();

			try
			{
				using (StreamReader readFile = new StreamReader(path + "/" + name + ".csv"))
				{
					string line;
					string[] row;

					while ((line = readFile.ReadLine()) != null)
					{
						row = line.Split(',');
						parsedData.Add(row);
					}
				}
			}
			catch (Exception e)
			{

			}

			int dim = parsedData[0].GetLength(0);
			int[] output = new int[dim];
			for (int i = 0; i < dim; i++)
			{
				output[i] = Convert.ToInt32(parsedData[0][i]);
			}
			return output;
		}

		// method to import a 1D array of doubles (from a csv)
		public double[] Import1DDoubleArray(string name)
		{
			string path = Environment.CurrentDirectory.ToString();
			path = System.IO.Directory.GetParent(path).ToString();
			path = System.IO.Directory.GetParent(path).ToString();
			path = System.IO.Directory.GetParent(path).ToString();

			List<string[]> parsedData = new List<string[]>();

			try
			{
				using (StreamReader readFile = new StreamReader(path + "/" + name + ".csv"))
				{
					string line;
					string[] row;

					while ((line = readFile.ReadLine()) != null)
					{
						row = line.Split(',');
						parsedData.Add(row);
					}
				}
			}
			catch (Exception e)
			{

			}

			int dim = parsedData[0].GetLength(0);
			double[] output = new double[dim];
			for (int i = 0; i < dim; i++)
			{
				output[i] = Convert.ToDouble(parsedData[0][i]);
			}
			return output;
		}

		// method to import a csv into a 2D array, while omiting the first n lines from the csv file
		public double[,] parseCSVWithoutFirstLines(string name, int numLinesToRemove)
		{
			string path = Environment.CurrentDirectory.ToString();
			path = System.IO.Directory.GetParent(path).ToString();
			path = System.IO.Directory.GetParent(path).ToString();
			path = System.IO.Directory.GetParent(path).ToString();

			List<string[]> parsedData = new List<string[]>();

			try
			{
				using (StreamReader readFile = new StreamReader(path + "/" + name + ".csv"))
				{
					string line;
					string[] row;

					while ((line = readFile.ReadLine()) != null)
					{
						row = line.Split(',');
						parsedData.Add(row);
					}
				}
			}
			catch (Exception e)
			{

			}

			int dim1 = parsedData.Count - numLinesToRemove;
			int dim2 = parsedData[0].GetLength(0);
			double[,] output = new double[dim1, dim2];
			for (int i = numLinesToRemove; i < dim1; i++)
			{
				for (int j = 0; j < dim2; j++)
				{
					output[i - numLinesToRemove, j] = Convert.ToDouble(parsedData[i][j]);
				}
			}

			return output;
		}

		// method to import a csv into a list of arrays
		public List<double[]> parseCSVToList(string name)
		{
			double[,] rawData = parseCSV(name);
			int dim1 = rawData.GetLength(0);
			int dim2 = rawData.GetLength(1);
			List<double[]> output = new List<double[]>();
			double[] tempArray;
			for (int i = 0; i < dim1; i++)
			{
				tempArray = new double[dim2];
				for (int j = 0; j < dim2; j++)
				{
					tempArray[j] = rawData[i, j];
				}
				output.Add(tempArray);
			}
			return output;
		}
	}
}
