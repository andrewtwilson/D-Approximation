using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworks
{


	public class Network
	{
		// call relevant classes
		MathClasses.Probability Prob = MathClasses.Probability.Instance;
		MathClasses.NNMath NNMath = new MathClasses.NNMath();
		DataIO.DataExport DE = new DataIO.DataExport();
		DataIO.DataImport DI = new DataIO.DataImport();
		MathClasses.LinearAlgebra LA = new MathClasses.LinearAlgebra();

		// network variables
		int numInputs = 5; // number of network input nodes
		int numHidden = 10; // number of network hidden nodes
		int numOutputs = 3; // number of network output nodes
		double weightInitSTD = 0.5; // STD of normal distribution for weight initialization
		double[,] weightMat1; // input-hidden layer weight matrix
		double[,] weightMat2; // hidden-output layer weight matrix
		double fitness = double.MinValue / 2.0; // fitness if we are doing neuroevolution
		double mseTraining = double.MaxValue / 2.0; // MSE if we are doing backprop
		double mseValidation = double.MaxValue / 2.0; // MSE if we are doing backprop
		double[] vectorFitness; // multiobjective fitness for MOEAs
		bool fitnessAssigned = false; // has fitness been assigned yet?

		// network properties
		public int NumInputs
		{
			get
			{
				return numInputs;
			}
			set
			{
				numInputs = value;
			}
		}
		public int NumHidden
		{
			get
			{
				return numHidden;
			}
			set
			{
				numHidden = value;
			}
		}
		public int NumOutputs
		{
			get
			{
				return numOutputs;
			}
			set
			{
				numOutputs = value;
			}
		}
		public double WeightInitSTD
		{
			get
			{
				return weightInitSTD;
			}
			set
			{
				weightInitSTD = value;
			}
		}
		public double[,] WeightMat1
		{
			get
			{
				return weightMat1;
			}
			set
			{
				weightMat1 = value;
			}
		}
		public double[,] WeightMat2
		{
			get
			{
				return weightMat2;
			}
			set
			{
				weightMat2 = value;
			}
		}
		public double Fitness
		{
			get
			{
				return fitness;
			}
			set
			{
				fitness = value;
			}
		}
		public double MSETraining
		{
			get
			{
				return mseTraining;
			}
			set
			{
				mseTraining = value;
			}
		}
		public double MSEValidation
		{
			get
			{
				return mseValidation;
			}
			set
			{
				mseValidation = value;
			}
		}
		public double[] VectorFitness
		{
			get
			{
				return vectorFitness;
			}
			set
			{
				vectorFitness = value;
			}
		}
		public bool FitnessAssigned
		{
			get
			{
				return fitnessAssigned;
			}
			set
			{
				fitnessAssigned = value;
			}
		}

		// method to create a randomly initialized weight matrix
		double[,] CreateRandomMatrix(int dim1, int dim2)
		{
			// initialize matrix with input dimensions
			double[,] output = new double[dim1, dim2];

			// populate output with random elements
			for (int i = 0; i < dim1; i++)
			{
				for (int j = 0; j < dim2; j++)
				{
					output[i, j] = Prob.Gaussian(WeightInitSTD, 0.0);
				}
			}
			return output;
		}

		// method to initialize network with random weights
		public void InitializeRandomNetwork()
		{
			WeightMat1 = CreateRandomMatrix(NumHidden, NumInputs + 1);
			WeightMat2 = CreateRandomMatrix(NumOutputs, NumHidden + 1);
		}

		// method to compute a forward pass through the network
		// input values are assumed to be scaled between 0 and 1
		public double[] ForwardPass(double[] input)
		{
			// add bias to the input
			double[] inputWithBias = LA.Append(input, 1.0);

			// multiply input by first weight matrix
			double[] hiddenLayerInput = LA.MatByVec(WeightMat1, inputWithBias);

			// take sigmoid of terms
			double[] hiddenLayerOutputNoBias = NNMath.VectorSigmoid(hiddenLayerInput);

			// add bias term
			double[] hiddenLayerOutput = LA.Append(hiddenLayerOutputNoBias, 1.0);

			// multiply by second weight matrix
			double[] outputLayerInput = LA.MatByVec(WeightMat2, hiddenLayerOutput);

			// take sigmoid of terms
			double[] networkOutput = NNMath.VectorSigmoid(outputLayerInput);

			// return network output
			return networkOutput;

		}

		// method to export a full network
		public void ExportNetwork(int i)
		{
			int[] networkTopology = new int[] { NumInputs, NumHidden, NumOutputs };
			double[] networkPerformanceVars = new double[] { Fitness, MSETraining, MSEValidation };
			DE.Export1DIntArray(networkTopology, "networkTopology" + Convert.ToString(i));
			DE.Export1DDoubleArray(networkPerformanceVars, "networkPerfVars" + Convert.ToString(i));
			DE.Export2DArray(WeightMat1, "weightMat1" + Convert.ToString(i));
			DE.Export2DArray(WeightMat2, "weightMat2" + Convert.ToString(i));
		}

		// method to import a full network
		public void ImportNetwork(int i)
		{
			int[] networkTopology = DI.Import1DIntArray("networkTopology" + Convert.ToString(i));
			double[] networkPerformanceVars = DI.Import1DDoubleArray("networkPerfVars" + Convert.ToString(i));
			double[,] importedWeightMat1 = DI.parseCSV("weightMat1" + Convert.ToString(i));
			double[,] importedWeightMat2 = DI.parseCSV("weightMat2" + Convert.ToString(i));
			NumInputs = networkTopology[0];
			NumHidden = networkTopology[1];
			NumOutputs = networkTopology[2];
			Fitness = networkPerformanceVars[0];
			MSETraining = networkPerformanceVars[1];
			MSEValidation = networkPerformanceVars[2];
			WeightMat1 = new double[NumHidden, NumInputs + 1];
			WeightMat1 = importedWeightMat1;
			WeightMat2 = importedWeightMat2;
		}
	}
}
