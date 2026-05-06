public class NNManager
{
public NeuralNetwork mainStateNetwork = new NeuralNetwork(new int[5]
{7168,3584,512,64,1}); // one input per piece per square per team, hidden layer in
hopes it identifies black team negativity, hidden layer size of board, hidden layer 5x
smaller,output
public activationType activation { get; set; }
public costType cost { get; set; }
public NNManager(int[] NNcomposure, activationType activation, costType cost)
{
mainStateNetwork = new NeuralNetwork(NNcomposure);
this.activation = activation;
this.cost = cost;
if (!ActivationAndCostAreCompatible())
{
throw new NeuralNetworkInstantiationFailed("The cost and activation
functions were invalid");
}
}
private bool ActivationAndCostAreCompatible()
{
if (cost == costType.crossEntropy)
{
if (activation == activationType.sigmoid || activation ==
activationType.hardSigmoid)
{
return true;
}
}
else if(cost == costType.tanhCustom)
{
if(activation == activationType.tanh)
{
mainStateNetwork.LoadNNCurrentState();
return true;
}
}
else
{
return true;
}
return false;
}
public float[] FeedForward(float[] inputs)
{
return mainStateNetwork.runNetworkTanh(inputs); // as i know i will jsut be
using this combination of activation and cost easier to implement than many if
statements
}
private void TrainingNNWithData()
{
float[][][] tData = GetTData();
float[][] TinputsWhole = tData[0];
float[][] ToutputsWhole = tData[1];
float[][] Tinputs = new float[100][];
float[][] Toutputs = new float[100][];
mainStateNetwork.LoadNNCurrentState();
for (int i = 0; i < TinputsWhole.Length/100; i+=100)
{
for (int j = 0; j < 100; j++)
{
Tinputs[j] = TinputsWhole[i + j];
Toutputs[j] = ToutputsWhole[i + j];
}
mainStateNetwork.TrainWithBackPropAndTanhWithCustomCostAndL2Regularisation(Tinputs,
Toutputs);
mainStateNetwork.SaveNNCurrentState();
}
}
private float[][][] GetTData()
{
string filePath = "Assets/Training Data/T.txt";
string allData = File.ReadAllText(filePath);
float[][][] tData = new float[2][][];
float[] data = new float[7168];
float[] output = new float[100000];
int current = 0;
for (int i = 0; i < allData.Length / 7169; i += 7169) //7 pieces per square
per team + one output
{
for (int j = 0; j < 7168; j++)
{
data[j] = Convert.ToInt32(allData[i + j]);
}
output[0] = Convert.ToInt32(allData[i + 7169]);
tData[0][current] = data;
tData[1][current] = output;
}
return tData;
}
}