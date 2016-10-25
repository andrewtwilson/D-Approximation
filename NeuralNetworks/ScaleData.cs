using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworks
{
	public class ScaleData
	{
		// object variables
		List<double[]> unscaledTrainingInputs;
		List<double[]> unscaledTrainingOutputs;
		List<double[]> unscaledValidationInputs;
		List<double[]> unscaledValidationOutputs;
		List<double[]> trainingInputs;
		List<double[]> trainingOutputs;
		List<double[]> validationInputs;
		List<double[]> validationOutputs;
		List<double[]> scalingVars;

		public List<double[]> UnscaledTrainingInputs
		{
			get
			{
				return unscaledTrainingInputs;
			}
			set
			{
				unscaledTrainingInputs = value;
			}
		}
		public List<double[]> UnscaledTrainingOutputs
		{
			get
			{
				return unscaledTrainingOutputs;
			}
			set
			{
				unscaledTrainingOutputs = value;
			}
		}
		public List<double[]> UnscaledValidationInputs
		{
			get
			{
				return unscaledValidationInputs;
			}
			set
			{
				unscaledValidationInputs = value;
			}
		}
		public List<double[]> UnscaledValidationOutputs
		{
			get
			{
				return unscaledValidationOutputs;
			}
			set
			{
				unscaledValidationOutputs = value;
			}
		}
		public List<double[]> TrainingInputs
		{
			get
			{
				return trainingInputs;
			}
			set
			{
				trainingInputs = value;
			}
		}
		public List<double[]> TrainingOutputs
		{
			get
			{
				return trainingOutputs;
			}
			set
			{
				trainingOutputs = value;
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
		public List<double[]> ScalingVars
		{
			get
			{
				return scalingVars;
			}
			set
			{
				scalingVars = value;
			}
		}

		// method to find scaling factors
		// note: inputs are of the form (state, actions) and outputs are of the form (state)
		// so, scaling factors for state vars are found using both input and output data
		// scaling factors for actions are found using only input data
		public void FindScalingFactors()
		{
			// initialize scaling vars
			ScalingVars = new List<double[]>();

			// find dimensions of data set
			int numStateVars = unscaledValidationOutputs[0].GetLength(0);
			int numActionVars = unscaledValidationInputs[0].GetLength(0) - unscaledValidationOutputs[0].GetLength(0);

			double minValue;
			double maxValue;
			double[] tempVec;

			// for each state variable, find min and max values
			for (int sv = 0; sv < numStateVars; sv++)
			{
				tempVec = new double[2];
				minValue = unscaledTrainingInputs[0][sv];
				maxValue = unscaledTrainingInputs[0][sv];

				// 1: compare with unscaled training inputs
				for (int i = 0; i < unscaledTrainingInputs.Count(); i++)
				{
					if (minValue > unscaledTrainingInputs[i][sv])
					{
						minValue = unscaledTrainingInputs[i][sv];
					}
					if (maxValue < unscaledTrainingInputs[i][sv])
					{
						maxValue = unscaledTrainingInputs[i][sv];
					}
				}

				// 2: compare with unscaled training outputs
				for (int i = 0; i < unscaledTrainingOutputs.Count(); i++)
				{
					if (minValue > unscaledTrainingOutputs[i][sv])
					{
						minValue = unscaledTrainingOutputs[i][sv];
					}
					if (maxValue < unscaledTrainingOutputs[i][sv])
					{
						maxValue = unscaledTrainingOutputs[i][sv];
					}
				}

				// 3: compare with unscaled validation inputs
				for (int i = 0; i < unscaledValidationInputs.Count(); i++)
				{
					if (minValue > unscaledValidationInputs[i][sv])
					{
						minValue = unscaledValidationInputs[i][sv];
					}
					if (maxValue < unscaledValidationInputs[i][sv])
					{
						maxValue = unscaledValidationInputs[i][sv];
					}
				}

				// 4: compare with unscaled validation outputs
				for (int i = 0; i < unscaledValidationOutputs.Count(); i++)
				{
					if (minValue > unscaledValidationOutputs[i][sv])
					{
						minValue = unscaledValidationOutputs[i][sv];
					}
					if (maxValue < unscaledValidationOutputs[i][sv])
					{
						maxValue = unscaledValidationOutputs[i][sv];
					}
				}
				tempVec[0] = minValue;
				tempVec[1] = maxValue;
				ScalingVars.Add(tempVec);
			}

			// for each action variable, find min and max values
			for (int sv = numStateVars; sv < numStateVars + numActionVars; sv++)
			{
				tempVec = new double[2];
				minValue = unscaledTrainingInputs[0][sv];
				maxValue = unscaledTrainingInputs[0][sv];

				// 1: compare with unscaled training inputs
				for (int i = 0; i < unscaledTrainingInputs.Count(); i++)
				{
					if (minValue > unscaledTrainingInputs[i][sv])
					{
						minValue = unscaledTrainingInputs[i][sv];
					}
					if (maxValue < unscaledTrainingInputs[i][sv])
					{
						maxValue = unscaledTrainingInputs[i][sv];
					}
				}

				// 2: compare with unscaled validation inputs
				for (int i = 0; i < unscaledValidationInputs.Count(); i++)
				{
					if (minValue > unscaledValidationInputs[i][sv])
					{
						minValue = unscaledValidationInputs[i][sv];
					}
					if (maxValue < unscaledValidationInputs[i][sv])
					{
						maxValue = unscaledValidationInputs[i][sv];
					}
				}
				tempVec[0] = minValue;
				tempVec[1] = maxValue;
				ScalingVars.Add(tempVec);
			}
		}

		// method to go from unscaled input to scaled (0-->1) input
		public double[] ScaleInput(double[] inputVec)
		{
			// find number of input vars
			int numVars = inputVec.GetLength(0);

			// initialize output
			double[] output = new double[numVars];
			double x;
			for (int i = 0; i < numVars; i++)
			{
				x = inputVec[i] - ScalingVars[i][0]; // now x is between 0 and (maxValue-minValue)
				x /= (ScalingVars[i][1] - ScalingVars[i][0]); // now x is between 0 and 1
				x *= 0.8;
				x += 0.1; // now x is between 0.1 and 0.9
				output[i] = x;
				if (ScalingVars[i][1] == ScalingVars[i][0])
				{
					output[i] = 0.5;
				}
			}
			return output;
		}

		// method to go from scaled (0-->1) input to unscaled input
		public double[] UnscaleInput(double[] inputVec)
		{
			// find number of input vars
			int numVars = inputVec.GetLength(0);

			// initialize output
			double[] output = new double[numVars];
			double x;
			for (int i = 0; i < numVars; i++)
			{
				x = inputVec[i] - 0.1; // now x is between 0 and 0.8
				x /= 0.8; // now x is between 0 and 1
				x *= (ScalingVars[i][1] - ScalingVars[i][0]); // now x is between 0 and (maxValue-minValue)
				x += ScalingVars[i][0]; // now x is between minvalue and maxvalue
				output[i] = x;
				if (ScalingVars[i][1] == ScalingVars[i][0])
				{
					output[i] = ScalingVars[i][1];
				}
			}
			return output;
		}

		// method to go from unscaled output to scaled (0-->1) output
		public double[] ScaleOutput(double[] inputVec)
		{
			// find number of input vars
			int numVars = inputVec.GetLength(0);

			// initialize output
			double[] output = new double[numVars];
			double x;
			for (int i = 0; i < numVars; i++)
			{
				x = inputVec[i] - ScalingVars[i][0]; // now x is between 0 and (maxValue-minValue)
				x /= (ScalingVars[i][1] - ScalingVars[i][0]); // now x is between 0 and 1
				x *= 0.8 + 0.1; // now x is between 0.1 and 0.9
				output[i] = x;
				if (ScalingVars[i][1] == ScalingVars[i][0])
				{
					output[i] = 0.5;
				}
			}
			return output;
		}

		// method to go from scaled (0-->1) output to unscaled output
		public double[] UnscaleOutput(double[] inputVec)
		{
			// find number of input vars
			int numVars = inputVec.GetLength(0);

			// initialize output
			double[] output = new double[numVars];
			double x;
			for (int i = 0; i < numVars; i++)
			{
				x = inputVec[i] - 0.1; // now x is between 0 and 0.8
				x /= 0.8; // now x is between 0 and 1
				x *= (ScalingVars[i][1] - ScalingVars[i][0]); // now x is between 0 and (maxValue-minValue)
				x += ScalingVars[i][0]; // now x is between minvalue and maxvalue
				output[i] = x;
				if (ScalingVars[i][1]==ScalingVars[i][0])
				{
					output[i] = ScalingVars[i][1];
				}
			}
			return output;
		}

		// method to create scaled training and validation data
		public void ScaleAllData()
		{
			FindScalingFactors();

			// 1: scale training inputs
			TrainingInputs = new List<double[]>();
			for (int i = 0; i < UnscaledTrainingInputs.Count(); i++)
			{
				TrainingInputs.Add(ScaleInput(UnscaledTrainingInputs[i]));
			}

			// 2: scale training outputs
			TrainingOutputs = new List<double[]>();
			for (int i = 0; i < UnscaledTrainingOutputs.Count(); i++)
			{
				TrainingOutputs.Add(ScaleOutput(UnscaledTrainingOutputs[i]));
			}

			// 3: scale validation inputs
			ValidationInputs = new List<double[]>();
			for (int i = 0; i < unscaledValidationInputs.Count(); i++)
			{
				ValidationInputs.Add(ScaleInput(UnscaledValidationInputs[i]));
			}

			// 4: scale validation outputs
			ValidationOutputs = new List<double[]>();
			for (int i = 0; i < UnscaledValidationOutputs.Count(); i++)
			{
				ValidationOutputs.Add(ScaleOutput(UnscaledValidationOutputs[i]));
			}
		}
	}
}
