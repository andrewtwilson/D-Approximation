using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworks
{
	public enum ToggleShuffle
	{
		yes,
		no
	}
	public class BackpropNetwork
	{
		// call relevant classes
		MathClasses.Probability Prob = MathClasses.Probability.Instance;
		MathClasses.LinearAlgebra LA = new MathClasses.LinearAlgebra();
		MathClasses.NNMath NNMath = new MathClasses.NNMath();
		MathClasses.Statistics Stats = new MathClasses.Statistics();
		MathClasses.DataManagement DM = new MathClasses.DataManagement();
		DataIO.DataExport DE = new DataIO.DataExport();

		// network variables
		NeuralNetworks.Network neuralNet; // neural network itself
		NeuralNetworks.Network bestNetwork; // save best network
		int numInputs = 5; // number of network input nodes
		int numHidden = 10; // number of network hidden nodes
		int numOutputs = 3; // number of network output nodes
		double weightInitSTD = 0.5; // STD of normal distribution for weight initialization
		double eta = 0.5; // network learning rate
		int episodes = 1000; // number of training episodes
		double[,] momentumMat1; // momentum terms for hidden-output layer weights
		double[,] momentumMat2; // momentum terms for input-hidden layer weights
		double momentum = 0.25; // momentum term value
		List<double[]> inputs; // training set inputs (always unshuffled)
		List<double[]> outputs; // training set outputs (always unshuffled)
		List<double[]> trainingInputs; // shuffled
		List<double[]> trainingOutputs; // shuffled
		List<double[]> validationInputs; // validation set inputs
		List<double[]> validationOutputs; // validation set outputs
		int numRestarts = 50; // how many times to restart the network training (to avoid local minima)
		List<double[]> mseTrainingList; // keep track of training MSE
		List<double[]> mseValidationList; // keep track of validation MSE
		ToggleShuffle shuffle = ToggleShuffle.yes; // do we shuffle data sets between each episode?
		int numMSEReportingPoints = 100; // number of MSE points to report

		// network properties
		public NeuralNetworks.Network NeuralNet
		{
			get
			{
				return neuralNet;
			}
			set
			{
				neuralNet = value;
			}
		}
		public NeuralNetworks.Network BestNetwork
		{
			get
			{
				return bestNetwork;
			}
			set
			{
				bestNetwork = value;
			}
		}
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
		public double Eta
		{
			get
			{
				return eta;
			}
			set
			{
				eta = value;
			}
		}
		public int Episodes
		{
			get
			{
				return episodes;
			}
			set
			{
				episodes = value;
			}
		}
		public double[,] MomentumMat1
		{
			get
			{
				return momentumMat1;
			}
			set
			{
				momentumMat1 = value;
			}
		}
		public double[,] MomentumMat2
		{
			get
			{
				return momentumMat2;
			}
			set
			{
				momentumMat2 = value;
			}
		}
		public double Momentum
		{
			get
			{
				return momentum;
			}
			set
			{
				momentum = value;
			}
		}
		public List<double[]> Inputs
		{
			get
			{
				return inputs;
			}
			set
			{
				inputs = value;
			}
		}
		public List<double[]> Outputs
		{
			get
			{
				return outputs;
			}
			set
			{
				outputs = value;
			}
		}
		public List<double[]> ValidationInputs
		{
			get
			{
				return validationInputs;
			}
			set
			{
				validationInputs = value;
			}
		}
		public List<double[]> ValidationOutputs
		{
			get
			{
				return validationOutputs;
			}
			set
			{
				validationOutputs = value;
			}
		}
		public int NumRestarts
		{
			get
			{
				return numRestarts;
			}
			set
			{
				numRestarts = value;
			}
		}
		public List<double[]> MSETrainingList
		{
			get
			{
				return mseTrainingList;
			}
			set
			{
				mseTrainingList = value;
			}
		}
		public List<double[]> MSEValidationList
		{
			get
			{
				return mseValidationList;
			}
			set
			{
				mseValidationList = value;
			}
		}
		public ToggleShuffle Shuffle
		{
			get
			{
				return shuffle;
			}
			set
			{
				shuffle = value;
			}
		}
		public int NumMSEReportingPoints
		{
			get
			{
				return numMSEReportingPoints;
			}
			set
			{
				numMSEReportingPoints = value;
			}
		}

		// method to initialize network
		public void InitializeNetwork()
		{
			NeuralNet = new Network();
			trainingInputs = new List<double[]>();
			double[] tempInput;
			for (int i = 0; i < Inputs.Count();i++ )
			{
				tempInput = new double[Inputs[0].GetLength(0)];
				for (int j=0;j<Inputs[0].GetLength(0);j++)
				{
					tempInput[j] = Inputs[i][j];
				}
				trainingInputs.Add(tempInput);
			}
			trainingOutputs = new List<double[]>();
			for (int i = 0; i < Outputs.Count();i++)
			{
				tempInput = new double[Outputs[0].GetLength(0)];
				for (int j=0;j<Outputs[0].GetLength(0);j++)
				{
					tempInput[j] = Outputs[i][j];
				}
				trainingOutputs.Add(tempInput);
			}
			NeuralNet.NumInputs = NumInputs;
			NeuralNet.NumHidden = NumHidden;
			NeuralNet.NumOutputs = NumOutputs;
			NeuralNet.WeightInitSTD = WeightInitSTD;
			NeuralNet.InitializeRandomNetwork();
			MomentumMat1 = new double[NumHidden, NumInputs + 1];
			MomentumMat2 = new double[NumOutputs, NumHidden + 1];
		}

		// method to initialize best network
		public void InitializeBestNetwork()
		{
			BestNetwork = new Network();
			BestNetwork.NumInputs = NumInputs;
			BestNetwork.NumOutputs = NumOutputs;
			BestNetwork.NumHidden = NumHidden;
			BestNetwork.WeightInitSTD = WeightInitSTD;
			BestNetwork.WeightMat1 = NeuralNet.WeightMat1;
			BestNetwork.WeightMat2 = NeuralNet.WeightMat2;
		}

		// method to shuffle data during training
		void ShuffleData()
		{
			int numShuffles = Inputs.Count();
			int inputCount = Inputs[0].GetLength(0);
			int outputCount = Outputs[0].GetLength(0);
			int index1;
			int index2;
			double[] tempInput;
			double[] tempOutput;

			for (int i = 0; i < numShuffles; i++)
			{
				tempInput = new double[inputCount];
				tempOutput = new double[outputCount];
				index1 = Prob.Next(numShuffles);
				index2 = Prob.Next(numShuffles);

				for (int j=0; j<inputCount; j++)
				{
					tempInput[j] = trainingInputs[index1][j];
					trainingInputs[index1][j] = trainingInputs[index2][j];
					trainingInputs[index2][j] = tempInput[j];
				}
				for (int j = 0; j < outputCount; j++)
				{
					tempOutput[j] = trainingOutputs[index1][j];
					trainingOutputs[index1][j] = trainingOutputs[index2][j];
					trainingOutputs[index2][j] = tempOutput[j];
				}

			}
		}

		// method for backpropogation
		// x is the network input, and target is the desired network output
		public void Backprop(double[] x, double[] target)
		{
			// add bias to the input
			double[] inputWithBias = LA.Append(x, 1.0);

			// multiply biased input by first weight matrix
			double[] hiddenLayerInput = LA.MatByVec(NeuralNet.WeightMat1, inputWithBias);

			// take sigmoid of terms
			double[] hiddenLayerOutput = NNMath.VectorSigmoid(hiddenLayerInput);

			// add bias term
			double[] h = LA.Append(hiddenLayerOutput, 1.0);

			// multiply by second weight matrix
			double[] outputLayerInput = LA.MatByVec(NeuralNet.WeightMat2, h);

			// take sigmoid of terms
			double[] output = NNMath.VectorSigmoid(outputLayerInput);

			// find error in network output
			int dim = target.GetLength(0);
			double[] e = new double[dim];
			for (int i=0;i<dim;i++)
			{
				e[i] = target[i] - output[i];
			}

			// find output layer deltas
			double[] outputDeltas = new double[dim];
			for (int i = 0; i < dim; i++)
			{
				outputDeltas[i] = e[i] * output[i] * (1 - output[i]);
			}

			// update second weight matrix
			//Parallel.For(0, NeuralNet.WeightMat2.GetLength(0), i =>
			for (int i = 0; i <NeuralNet.WeightMat2.GetLength(0); i++)
			{
				for (int j = 0; j < NeuralNet.WeightMat2.GetLength(1); j++)
				{
					NeuralNet.WeightMat2[i, j] += Eta * outputDeltas[i] * h[j] + MomentumMat2[i, j] * Momentum;
					MomentumMat2[i, j] = Eta * outputDeltas[i] * h[j];
				}
			}

			// find hidden layer deltas
			double[] hiddenDeltas = new double[NumHidden];
			double tempVal;
			for (int j = 0; j < NumHidden; j++)
			{
				tempVal = 0.0;
				for (int k = 0; k < NumOutputs; k++)
				{
					tempVal += NeuralNet.WeightMat2[k, j] * outputDeltas[k] * h[j] * (1 - h[j]);
				}
				hiddenDeltas[j] = tempVal;
			}

			// update first weight matrix
			for (int i = 0; i < NeuralNet.WeightMat1.GetLength(0); i++)
			{
				for (int j = 0; j < NeuralNet.WeightMat1.GetLength(1); j++)
				{
					NeuralNet.WeightMat1[i, j] += Eta * hiddenDeltas[i] * inputWithBias[j] + MomentumMat1[i, j] * Momentum;
					MomentumMat1[i, j] = Eta * hiddenDeltas[i] * inputWithBias[j];
				}
			}
		}

		// method to find MSE from validation set
		public double MSEValidation()
		{
			int numPoints = ValidationInputs.Count();
			double numPointsD = Convert.ToDouble(numPoints);
			double error = 0.0;
			double singleError;
			double[] networkOutput;
			for (int i=0;i<numPoints;i++)
			{
				singleError = 0.0;
				networkOutput = NeuralNet.ForwardPass(ValidationInputs[i]);
				for (int j = 0; j < networkOutput.GetLength(0); j++)
				{
					singleError += (networkOutput[j] - ValidationOutputs[i][j]) * (networkOutput[j] - ValidationOutputs[i][j]);
				}
				error += singleError;
			}
			error /= numPointsD;
			return Math.Sqrt(error);
		}

		// method to find MSE from training set
		public double MSETraining()
		{
			int numPoints = Inputs.Count();
			double numPointsD = Convert.ToDouble(numPoints);
			double error = 0.0;
			double singleError;
			double[] networkOutput;
			for (int i = 0; i < numPoints; i++)
			{
				singleError = 0.0;
				networkOutput = NeuralNet.ForwardPass(Inputs[i]);
				for (int j = 0; j < networkOutput.GetLength(0); j++)
				{
					singleError += (networkOutput[j] - Outputs[i][j]) * (networkOutput[j] - Outputs[i][j]);
				}
				error += singleError;
			}
			error /= numPointsD;
			return Math.Sqrt(error);
		}

		// method to train network
		public List<double[]> TrainNetwork()
		{
			InitializeNetwork();
			double[] mseDataValidation = new double[Episodes];
			double[] mseDataTraining = new double[Episodes];

			for (int ep=0;ep<Episodes;ep++)
			{
				if (Shuffle == ToggleShuffle.yes)
				{
					ShuffleData();
				}

				for (int dp = 0; dp < Inputs.Count(); dp++)
				{
					Backprop(trainingInputs[dp], trainingOutputs[dp]);
				}

				mseDataValidation[ep] = MSEValidation();
				mseDataTraining[ep] = MSETraining();
				NeuralNet.MSETraining = mseDataTraining[ep];
				NeuralNet.MSEValidation = mseDataValidation[ep];

				if (mseDataValidation[ep] < BestNetwork.MSEValidation)
				{
					BestNetwork.WeightMat1 = NeuralNet.WeightMat1;
					BestNetwork.WeightMat2 = NeuralNet.WeightMat2;
					BestNetwork.MSETraining = NeuralNet.MSETraining;
					BestNetwork.MSEValidation = NeuralNet.MSEValidation;
				}
			}
			List<double[]> output = new List<double[]>();
			output.Add(mseDataTraining);
			output.Add(mseDataValidation);
			return output;
		}

		// method to train network with random restarts
		public void TrainNetworkRandomRestart()
		{
			InitializeNetwork();
			InitializeBestNetwork();
			MSETrainingList = new List<double[]>();
			MSEValidationList = new List<double[]>();
			List<double[]> singleRunData;
			Console.WriteLine("Beginning Network Training with Random Restarts");
			for (int i = 0; i < NumRestarts; i++)
			{
				Console.WriteLine("Beginning Training Run {0}", i + 1);
				singleRunData = TrainNetwork();
				MSETrainingList.Add(singleRunData[0]);
				MSEValidationList.Add(singleRunData[1]);
				Console.WriteLine("Training Run {0} complete, Validation MSE = {1}", i + 1, BestNetwork.MSEValidation);
			}

			//double[,] mseTrainingStats = Stats.RunStats(MSETrainingList);
			//double[,] mseValidationStats = Stats.RunStats(MSEValidationList);
			//double[,] mseTrainingStatsDownsampled = DM.DownSample(mseTrainingStats, NumMSEReportingPoints);
			//double[,] mseValidationStatsDownsampled = DM.DownSample(mseValidationStats, NumMSEReportingPoints);
			//DE.Export2DArray(mseTrainingStatsDownsampled, "MSE_Training");
			//DE.Export2DArray(mseValidationStatsDownsampled, "MSE_Validation");

			//BestNetwork.ExportNetwork();
		}

		// method to export true vs estimated (NN) output for a particular output index
		public void TestNetworkOutputValidation(int inputIndex, int outputIndex)
		{
			double[,] trueOutput = new double[ValidationInputs.Count(), 2];
			double[,] nnOutput = new double[ValidationInputs.Count(), 2];
			double[,] bestNNOutput = new double[ValidationInputs.Count(), 2];

			for (int i=0;i<ValidationInputs.Count();i++)
			{
				trueOutput[i, 0] = ValidationInputs[i][inputIndex];
				trueOutput[i, 1] = ValidationOutputs[i][outputIndex];
				nnOutput[i, 0] = ValidationInputs[i][inputIndex];
				nnOutput[i, 1] = NeuralNet.ForwardPass(ValidationInputs[i])[outputIndex];
				bestNNOutput[i, 0] = ValidationInputs[i][inputIndex];
				bestNNOutput[i, 1] = BestNetwork.ForwardPass(ValidationInputs[i])[outputIndex];
			}
			DE.Export2DArray(trueOutput, "true_data_validation");
			DE.Export2DArray(nnOutput, "nn_output_validation");
			DE.Export2DArray(bestNNOutput, "best_nn_output_validation");
		}

		// method to export true vs estimated (NN) output for a particular output index
		public void TestNetworkOutputValidationTimeDomain(int outputIndex)
		{
			double[,] trueOutput = new double[Inputs.Count()-1, 2];
			double[,] nnOutput = new double[Inputs.Count()-1, 2];
			double[,] bestNNOutput = new double[Inputs.Count()-1, 2];
			double[] tempInput1 = new double[NumInputs];
			double[] tempInput2 = new double[NumInputs];
			double[] tempOutput1;
			double[] tempOutput2;

			for (int i = 0; i < Inputs.Count()-1; i++)
			{
				if (i == 0)
				{
					for (int j = 0; j < NumInputs; j++)
					{
						tempInput1[j] = Inputs[0][j];
						tempInput2[j] = Inputs[0][j];
					}
				}
				trueOutput[i, 0] = Convert.ToDouble(i);
				trueOutput[i, 1] = Outputs[i][outputIndex];
				nnOutput[i, 0] = Convert.ToDouble(i);
				tempOutput1 = NeuralNet.ForwardPass(tempInput1);
				nnOutput[i, 1] = tempOutput1[outputIndex];
				for (int j = 0; j < NumOutputs;j++)
				{
					tempInput1[j] = tempOutput1[j];
				}
				for (int j = NumOutputs; j < NumInputs; j++)
				{
					tempInput1[j] = Inputs[i][j];
				}
				bestNNOutput[i, 0] = Convert.ToDouble(i);
				tempOutput2 = BestNetwork.ForwardPass(tempInput2);
				bestNNOutput[i, 1] = tempOutput2[outputIndex];
				for (int j=0;j<NumOutputs;j++)
				{
					tempInput2[j] = tempOutput2[j];
				}
				for (int j=NumOutputs;j<NumInputs;j++)
				{
					tempInput2[j] = Inputs[i][j];
				}
			}
			DE.Export2DArray(trueOutput, "true_data_validation");
			DE.Export2DArray(nnOutput, "nn_output_validation");
			DE.Export2DArray(bestNNOutput, "best_nn_output_validation");
		}

		// method to export true vs estimated (NN) output for a particular output index
		public void TestNetworkOutputTraining(int inputIndex, int outputIndex)
		{
			double[,] trueOutput = new double[Inputs.Count(), 2];
			double[,] nnOutput = new double[Inputs.Count(), 2];
			double[,] bestNNOutput = new double[Inputs.Count(), 2];

			for (int i = 0; i < Inputs.Count(); i++)
			{
				trueOutput[i, 0] = Inputs[i][inputIndex];
				trueOutput[i, 1] = Outputs[i][outputIndex];
				nnOutput[i, 0] = Inputs[i][inputIndex];
				nnOutput[i, 1] = NeuralNet.ForwardPass(Inputs[i])[outputIndex];
				bestNNOutput[i, 0] = Inputs[i][inputIndex];
				bestNNOutput[i, 1] = BestNetwork.ForwardPass(Inputs[i])[outputIndex];
			}
			DE.Export2DArray(trueOutput, "true_data_training");
			DE.Export2DArray(nnOutput, "nn_output_training");
			DE.Export2DArray(bestNNOutput, "best_nn_output_training");
		}
	}
}
