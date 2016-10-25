using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace DataIO
{
	public class DataExport
	{
		// NOTE: all exporting methods export to the current directory.  All that must be specified is a filename, not an extension

		// method to export a list of arrays, given a file name
		public void ExportData(List<double[]> dataList, string name)
		{
			string path = Environment.CurrentDirectory.ToString();
			path = System.IO.Directory.GetParent(path).ToString();
			path = System.IO.Directory.GetParent(path).ToString();
			path = System.IO.Directory.GetParent(path).ToString();

			string lineOut = "";

			// find dimensions of data list
			int dim1 = dataList.Count();
			int dim2 = dataList[0].GetLength(0);

			// open streamwriter
			StreamWriter writer = new StreamWriter(path + "/" + name + ".csv");

			// write file
			for (int i = 0; i < dim1; i++)
			{
				for (int j = 0; j < dim2 - 1; j++)
				{
					lineOut = lineOut + Convert.ToString(dataList[i][j]) + ",";
				}
				lineOut = lineOut + Convert.ToString(dataList[i][dim2 - 1]);
				writer.WriteLine(Convert.ToString(lineOut));
				lineOut = "";
			}

			// close writer
			writer.Close();
		}

		// method to export a 2D array, given a file name
		public void Export2DArray(double[,] dataList, string name)
		{
			string path = Environment.CurrentDirectory.ToString();
			path = System.IO.Directory.GetParent(path).ToString();
			path = System.IO.Directory.GetParent(path).ToString();
			path = System.IO.Directory.GetParent(path).ToString();

			string lineOut = "";

			// find dimensions of data list
			int dim1 = dataList.GetLength(0);
			int dim2 = dataList.GetLength(1);

			// open streamwriter
			StreamWriter writer = new StreamWriter(path + "/" + name + ".csv");

			// write file
			for (int i = 0; i < dim1; i++)
			{
				for (int j = 0; j < dim2 - 1; j++)
				{
					lineOut = lineOut + Convert.ToString(dataList[i, j]) + ",";
				}
				lineOut = lineOut + Convert.ToString(dataList[i, dim2 - 1]);
				writer.WriteLine(Convert.ToString(lineOut));
				lineOut = "";
			}

			// close writer
			writer.Close();
		}

		// method to export a 1D array of integers, given a file name
		public void Export1DIntArray(int[] dataList, string name)
		{
			string path = Environment.CurrentDirectory.ToString();
			path = System.IO.Directory.GetParent(path).ToString();
			path = System.IO.Directory.GetParent(path).ToString();
			path = System.IO.Directory.GetParent(path).ToString();

			string lineOut = "";

			// find dimensions of data list
			int dim = dataList.GetLength(0);

			// open streamwriter
			StreamWriter writer = new StreamWriter(path + "/" + name + ".csv");

			// write file
			for (int i = 0; i < dim - 1; i++)
			{
				lineOut = lineOut + Convert.ToString(dataList[i]) + ",";
			}
			lineOut += Convert.ToString(dataList[dim - 1]);
			writer.WriteLine(Convert.ToString(lineOut));

			// close writer
			writer.Close();
		}

		// method to export a 1D array of doubles, given a file name
		public void Export1DDoubleArray(double[] dataList, string name)
		{
			string path = Environment.CurrentDirectory.ToString ();
			path = System.IO.Directory.GetParent (path).ToString ();
			path = System.IO.Directory.GetParent (path).ToString ();
			path = System.IO.Directory.GetParent (path).ToString ();

			string lineOut = "";

			// find dimensions of data list
			int dim = dataList.GetLength (0);

			// open streamwriter
			StreamWriter writer = new StreamWriter (path + "/" + name + ".csv");

			// write file
			for (int i = 0; i < dim - 1; i++) {
                lineOut = lineOut + (i + 1).ToString() + "," + Convert.ToString(dataList[i]) + "," + Environment.NewLine ;
			}
            lineOut += (dim - 1).ToString() + "," + Convert.ToString(dataList[dim - 1]);
			writer.WriteLine (Convert.ToString (lineOut));

			// close writer
			writer.Close ();
		}
	}
}

