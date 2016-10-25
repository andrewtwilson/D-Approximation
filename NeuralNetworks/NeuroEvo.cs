using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworks
{
    public enum FitnessType
    {
        fStatic,
        fDynamic
    }
    public class NeuroEvo
    {
        MathClasses.Probability Prob = MathClasses.Probability.Instance;
        MathClasses.Statistics Stats = new MathClasses.Statistics();
        //Paccet P = new Paccet();

        int num_AGENTS;

        List<List<Network>> population = new List<List<Network>>();
        int popSize = 0;
        int numInputs = 8;
        int numHidden = 10;
        int numOutputs = 2;
        double weightInitSTD = 1.0;
        double mutateSTD = 1.0;
        int numMutationsMatrix1 = 2;
        int numMutationsMatrix2 = 2;
        //IMOEvo domain;
        int epochs = 1000;
        int statRuns = 10;
        FitnessType fitType = FitnessType.fStatic;
        int numObjectives = 1;
        //int numParetoPoints = 100;
        //List<Network> paretoNetworks;

        public List<List<Network>> Population
        {
            get
            {
                return population;
            }
            set
            {
                population = value;
            }
        }
        public int PopSize
        {
            get
            {
                return popSize;
            }
            set
            {
                popSize = value;
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
        public double MutateSTD
        {
            get
            {
                return mutateSTD;
            }
            set
            {
                mutateSTD = value;
            }
        }
        public int NumMutationsMatrix1
        {
            get
            {
                return numMutationsMatrix1;
            }
            set
            {
                numMutationsMatrix1 = value;
            }
        }
        public int NumMutationsMatrix2
        {
            get
            {
                return numMutationsMatrix2;
            }
            set
            {
                numMutationsMatrix2 = value;
            }
        }
        public int Epochs
        {
            get
            {
                return epochs;
            }
            set
            {
                epochs = value;
            }
        }
        public int StatRuns
        {
            get
            {
                return statRuns;
            }
            set
            {
                statRuns = value;
            }
        }
        public FitnessType FitType
        {
            get
            {
                return fitType;
            }
            set
            {
                fitType = value;
            }
        }
        public int NumObjectives
        {
            get
            {
                return numObjectives;
            }
            set
            {
                numObjectives = value;
            }
        }

        // Constructor
        public NeuroEvo(int num_rovers, int evopop)
        {
            num_AGENTS = num_rovers;
            popSize = evopop;

            InitializePopulation();
        }

        // method to randomly initialize population
        void InitializePopulation()
        {
            // List of populations for each agent
            Population = new List<List<Network>>();

            // List of single population
            List<Network> temPop;

            Network tempNetwork;
            for (int i = 0; i < num_AGENTS; i++)
            {
                temPop = new List<Network>();
                for (int j = 0; j < PopSize; j++)
                {
                    tempNetwork = new Network();
                    tempNetwork.NumInputs = NumInputs;
                    tempNetwork.NumHidden = NumHidden;
                    tempNetwork.NumOutputs = NumOutputs;
                    tempNetwork.WeightInitSTD = WeightInitSTD;
                    tempNetwork.InitializeRandomNetwork();
                    temPop.Add(tempNetwork);
                }
                Population.Add(temPop);
            }

        }

        public void EndGeneration()
        {
            ReorderPopulation();

            SelectPopulation();

            MutatePopulation();
        }

        // method to mutate one weight matrix
        double[,] MutateOneMatrix(double[,] matrix, int numMutations)
        {
            // find dimensions of matrix
            int dim1 = matrix.GetLength(0);
            int dim2 = matrix.GetLength(1);

            // create and populate copied weight matrix
            double[,] output = new double[dim1, dim2];
            for (int i = 0; i < dim1; i++)
            {
                for (int j = 0; j < dim2; j++)
                {
                    output[i, j] = matrix[i, j];
                }
            }

            // for each mutation, randomly select one value and mutate it
            int tempDim1;
            int tempDim2;
            for (int i = 0; i < numMutations; i++)
            {
                tempDim1 = Prob.Next(dim1);
                tempDim2 = Prob.Next(dim2);
                output[tempDim1, tempDim2] += Prob.Gaussian(MutateSTD, 0.0);
            }

            // return mutated matrix
            return output;
        }

        // method to create a mutated copy of a neural network
        Network CreateMutatedNetwork(Network inputNetwork)
        {
            // create a new neural network
            Network output = new Network();

            // assign network properties
            output.NumInputs = NumInputs;
            output.NumHidden = NumHidden;
            output.NumOutputs = NumOutputs;
            output.WeightInitSTD = WeightInitSTD;

            // assign network weights as mutated versions of input network's weights
            output.WeightMat1 = MutateOneMatrix(inputNetwork.WeightMat1, NumMutationsMatrix1);
            output.WeightMat2 = MutateOneMatrix(inputNetwork.WeightMat2, NumMutationsMatrix2);

            // return mutated copy of the neural network
            return output;
        }

        // method to mutate entire population (doubles population size)
        void MutatePopulation()
        {
            // for each population member, create a mutated copy and add it to the population
            Network tempNetwork;
            for (int i = 0; i < num_AGENTS; i++)
            {
                for (int j = 0; j < PopSize/2; j++)
                {
                    tempNetwork = new Network();
                    tempNetwork = CreateMutatedNetwork(Population[i][j]);
                    Population[i].Add(tempNetwork);
                }
            }
        }

        // method to reorder population based on fitness
        void ReorderPopulation()
        {
            for (int i = 0; i < num_AGENTS; i++)
            {
                List<Network> newPopulation = Population[i].OrderByDescending(o => o.Fitness).ToList();
                Population[i].Clear();
                Population[i].AddRange(newPopulation);
                newPopulation.Clear();
            }
        }

        // method to select population members to survive
        void SelectPopulation()
        {
            for (int i = 0; i < num_AGENTS/2; i++)
            {
                Population[i].RemoveRange(popSize / 2, popSize / 2);

                /*
                List<Network> tempPop = new List<Network>();
                tempPop.Add(Population[i][0]);
                Population[i].RemoveAt(0);
                int index1;
                int index2;
                int tempVal;

                for (int j = 1; j < PopSize; j++)
                {
                    index1 = Prob.Next(Population[i].Count());
                    index2 = Prob.Next(Population[i].Count());
                    while (index2 == index1)
                    {
                        index2 = Prob.Next(Population[i].Count());
                        Console.WriteLine(Population[i].Count());
                    }
                    if (Population[i][index1].Fitness > Population[i][index2].Fitness)
                    {
                        tempPop.Add(Population[i][index1]);
                    }
                    else
                    {
                        tempPop.Add(Population[i][index2]);
                    }
                    if (index1 > index2)
                    {
                        tempVal = index1;
                        index1 = index2;
                        index2 = tempVal;
                    }
                    Population[i].RemoveAt(index1);
                    Population[i].RemoveAt(index2 - 1);
                }

                Population[i].Clear();
                Population[i].AddRange(tempPop);
                */
            }
        }

        // method to copy a network
        Network CopyNetwork(Network oldNet)
        {
            Network output = new Network();
            output.Fitness = oldNet.Fitness;
            output.FitnessAssigned = oldNet.FitnessAssigned;
            output.MSETraining = oldNet.MSETraining;
            output.MSEValidation = oldNet.MSEValidation;
            output.NumHidden = oldNet.NumHidden;
            output.NumInputs = oldNet.NumInputs;
            output.NumOutputs = oldNet.NumOutputs;
            output.VectorFitness = oldNet.VectorFitness;
            output.WeightInitSTD = oldNet.WeightInitSTD;
            output.WeightMat1 = oldNet.WeightMat1;
            output.WeightMat2 = oldNet.WeightMat2;

            return output;
        }









        


        /*
        // method to run one evolutionary algorithm
        public List<double[]> SingleEA()
		{
			InitializePopulation();
			//P.Init(NumObjectives, NumParetoPoints);
			double[] output = new double[Epochs];
			//List<Network> NDS;
			//List<Network> tempPareto;
			int tempIndex;
			for (int ep = 0; ep < Epochs; ep++)
			{
				if (ep % 1000 == 0)
				{
					Console.WriteLine("Beginning Epoch {0}", ep);
				}
				MutatePopulation();
				AssignFitness();
				ReorderPopulation();
				//*NDS = NonDominatedSet(Population);
				ParetoNetworks.AddRange(NDS);
				tempPareto = new List<Network>();
				tempPareto.AddRange(NonDominatedSet(ParetoNetworks));
				ParetoNetworks.Clear();
				//ParetoNetworks.AddRange(tempPareto);
				ParetoNetworks.AddRange(tempPareto.OrderBy(T => T.VectorFitness[0]).ToList());
				while (ParetoNetworks.Count() > NumParetoPoints)
				{
					tempIndex = Prob.Next(ParetoNetworks.Count() - 2) + 1;
					ParetoNetworks.RemoveAt(tempIndex);
				}
				//networkPFront.OrderBy(T => T[0]).ToList();
				SelectPopulation();
				output[ep] = Population[0].Fitness;
			}
			return P.PFront;
		}
        */
    }
}

