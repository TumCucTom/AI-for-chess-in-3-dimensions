using System;
using System.IO;
public class NeuralNetwork

{

//When Comments Refer to the differented cost equations, these can be found in the anaylsis section.

//The cost function used in conjunction with tanh is -0.5 * ( (1-y)*log(1-a) + (1+y)*log(1+a) ) + log(2) //
(cross-entropy-esque)

//this gives derives to functions that are proportional to the error

//namely, dc/dw = x(a-y) and dc/db or dv = a-y



public float[][] NodeValues; //[layer in][number of node]

public float[][] NodeBiases; //[layer in][number of node]

public float[][][] weights; // [layer in][node connected to][node connected from]



public float[][] DesiredValues; // correct values for training

public float[][] BiasNudges; // how much to nudge for cost

public float[][][] weightNudges; // "" ""



private const float ETA = 0.8f; // learning rate

private const float LAMBDA = 0.001f; //l2 regularisation

private const int MINI_BATCH = 100; // mini batch size for epoch based training

private const float SCALE = 0.01f; // leaky reLU constant

private int N;



private char a_type;

private char c_type;




307
private static Random rand = new Random();



public NeuralNetwork(int[] NNcomposure, char activation = 's', char cost = 'm')

{

a_type = activation;

c_type = cost;



N = NNcomposure[0];



// structure format - {num input nodes, num hidden layer 1 nodes, num hidden layer 2 node,..., num
output nodes}

NodeValues = new float[NNcomposure.Length][];

NodeBiases = new float[NNcomposure.Length][];

weights = new float[NNcomposure.Length - 1][][]; //no connection from output layer forwards



DesiredValues = new float[NNcomposure.Length][];

BiasNudges = new float[NNcomposure.Length][];

weightNudges = new float[NNcomposure.Length - 1][][];// "" ""



for (int i = 0; i < NNcomposure.Length; i++) //adding the respective number of nodes for each layer

{

NodeValues[i] = new float[NNcomposure[i]];

NodeBiases[i] = new float[NNcomposure[i]];



DesiredValues[i] = new float[NNcomposure[i]];

BiasNudges[i] = new float[NNcomposure[i]];

}



for (int i = 0; i < NNcomposure.Length - 1; i++) //adding the respective number of weights needed per
layer




308
{

weights[i] = new float[NodeValues[i + 1].Length][]; // nodes to

weightNudges[i] = new float[NodeValues[i + 1].Length][]; //"" ""

for (int j = 0; j < weights[i].Length; j++)

{

weights[i][j] = new float[DesiredValues[i].Length]; //nodes from

weights[i][j] = new float[DesiredValues[i].Length];//"" ""

for (int k = 0; k < weights[i][j].Length; k++)

{

// set every weight in the NN to a random value between 0 and 1

// then multiply by the sqaure root of two over the number of nodes in the layer

// this distribution improves the initial learning of the NN

weights[i][j][k] = (float)(rand.NextDouble()) * MathF.Sqrt(2f / weights[i][j].Length);

}

}

}

}



public float[] runNetwork(float[] inputs)

{

for (int i = 0; i < NodeValues[0].Length; i++) // setting values

{

NodeValues[0][i] = inputs[i];



}



for (int i = 0; i < NodeValues.Length; i++)

{

for (int j = 0; j < NodeValues[i].Length; j++)

{




309
//calculating activations

NodeValues[i][j] = activation(SumForNode(NodeValues[i - 1], weights[i - 1][j]) + NodeBiases[i][j]);
// sum of all weighted nodes before + the bias for the current node

DesiredValues[i][j] = NodeValues[i][j]; // setup nodes before taining

}

}

return NodeValues[NodeValues.Length - 1];

}



public float[] runNetworkSigmoidSpecific(float[] inputs)

{

for (int i = 0; i < NodeValues[0].Length; i++) // setting values

{

NodeValues[0][i] = inputs[i];



}



for (int i = 0; i < NodeValues.Length; i++)

{

for (int j = 0; j < NodeValues[i].Length; j++)

{

//calculating activations

NodeValues[i][j] = Sigmoid(SumForNode(NodeValues[i - 1], weights[i - 1][j]) + NodeBiases[i][j]); //
sum of all weighted nodes before + the bias for the current node

DesiredValues[i][j] = NodeValues[i][j]; // setup nodes before training

}

}

return NodeValues[NodeValues.Length - 1];

}



public float[] runNetworkTanhSoftmax(float[] inputs)




310
{

for (int i = 0; i < NodeValues[0].Length; i++) // setting values

{

NodeValues[0][i] = inputs[i];



}



for (int i = 0; i < NodeValues.Length-1; i++)

{

for (int j = 0; j < NodeValues[i].Length-1; j++)

{

//calculating activations

NodeValues[i][j] = Tanh(SumForNode(NodeValues[i - 1], weights[i - 1][j]) + NodeBiases[i][j]); //
sum of all weighted nodes before + the bias for the current node

DesiredValues[i][j] = NodeValues[i][j]; // setup nodes before training

}

}

int finalLayer = NodeValues.Length-1;

for (int i = 0; i < NodeValues[finalLayer].Length; i++)

{

NodeValues[finalLayer][i] = Softmax(NodeValues[finalLayer], SumForNode(NodeValues[finalLayer],
weights[finalLayer - 1][i]) + NodeBiases[finalLayer][i]); // sum of all weighted nodes before + the bias for the
current node

DesiredValues[finalLayer][i] = NodeValues[finalLayer][i]; // setup nodes before training

}

return NodeValues[NodeValues.Length - 1];

}

public float[] runNetworkTanh(float[] inputs)

{

for (int i = 0; i < NodeValues[0].Length; i++) // setting values

{




311
NodeValues[0][i] = inputs[i];



}



for (int i = 0; i < NodeValues.Length; i++)

{

for (int j = 0; j < NodeValues[i].Length; j++)

{

//calculating activations

NodeValues[i][j] = Tanh(SumForNode(NodeValues[i - 1], weights[i - 1][j]) + NodeBiases[i][j]); //
sum of all weighted nodes before + the bias for the current node

DesiredValues[i][j] = NodeValues[i][j]; // setup nodes before training

}

}

return NodeValues[NodeValues.Length - 1];

}



public void TrainWithBackProp(float[][] Tinputs, float[][] Toutputs)

{

int epoch = 0;

for (int i = 0; i < Tinputs.Length; i++)

{

epoch++;

runNetwork(Tinputs[i]); // test the network for every set of trianing data given



for (int j = 0; j < DesiredValues[DesiredValues.Length - 1].Length; j++)

{

DesiredValues[DesiredValues.Length - 1][j] = Toutputs[i][j]; // adding the wanted outputs to the
desired node values

}




312
for (int j = NodeValues.Length - 1; j > 0; j--) // back prop up to but excluding the input layer

{

for (int k = 0; k < NodeValues[j].Length; k++)

{

var biasNudge = derivativeB(j, k);

//// chain rule differentiation for dc/db first - easiest to start of this way as the weights annd value
differentiations include/are the bias differentials in some cases

BiasNudges[j][k] += biasNudge;

for (int l = 0; l < NodeValues[j - 1][l]; l++)

{

var weightNudge = derivativeW(j, l, biasNudge);

weightNudges[j - 1][k][l] += weightNudge;



var valueNudge = derivativeV(j, k, l, biasNudge); // again shown by diff - need to have wanted
value for node behind to continue back prop

DesiredValues[j - 1][l] += valueNudge; // needed for calculating in previous layers

}

}

}



if (epoch % MINI_BATCH == 0)

{

for (int p = NodeValues.Length - 1; p > 0; p--) // for every layer bar the inputs

{

for (int j = 0; j < NodeValues[i].Length; j++) // for every node

{

NodeBiases[p][j] -= BiasNudges[p][j] * ETA / MINI_BATCH; // adjusting the biases

BiasNudges[p][j] = 0; // resetting for more training



DesiredValues[p][j] = 0;




313
for (int k = 0; k < NodeValues[p - 1].Length; k++)

{

weights[p - 1][j][k] *= (1 - ETA) * LAMBDA / N; //adjust weights with accordance to l2
regularisation

weights[p - 1][j][k] -= weightNudges[p - 1][j][k] * ETA / MINI_BATCH; //continuation of
weight adjustement equation

weightNudges[p - 1][j][k] = 0; //reset

}

}

}

}

}

}



public void TrainWithBackPropAndCrossEntropyWithL2Regularisation(float[][] Tinputs, float[][] Toutputs)

{

int epoch = 0;

for (int i = 0; i < Tinputs.Length; i++)

{

epoch++;

runNetwork(Tinputs[i]); // test the network for every set of trianing data given



for (int j = 0; j < DesiredValues[DesiredValues.Length - 1].Length; j++)

{

DesiredValues[DesiredValues.Length - 1][j] = Toutputs[i][j]; // adding the wanted outputs to the
desired node values

}



for (int j = NodeValues.Length - 1; j > 0; j--) // back prop up to but excluding the input layer

{

for (int k = 0; k < NodeValues[j].Length; k++)




314
{

var biasNudge = DesiredValues[j][k] - NodeValues[j][k];

// chain rule diff for dc/db as dc/db = error delta = sigmod prime Zl * dc/da where dc/da = al -y

BiasNudges[j][k] += biasNudge;

for (int l = 0; l < NodeValues[j - 1][l]; l++)

{

var weightNudge = (NodeValues[j-1][l] * biasNudge)/N; // since the weight differential has a
terms that equal the bias in it the bias can be used here

weightNudges[j - 1][k][l] += weightNudge;



var valueNudge = biasNudge; // again shown by diff (they are the same) - need to have wanted
value for node behind to continue back prop

DesiredValues[j - 1][l] += valueNudge; // needed for calculating in previous layers

}

}

}



if (epoch % MINI_BATCH == 0)

{

for (int p = NodeValues.Length - 1; p > 0; p--) // for every layer bar the inputs

{

for (int j = 0; j < NodeValues[i].Length; j++) // for every node

{

NodeBiases[p][j] -= BiasNudges[p][j] * ETA / MINI_BATCH; // adjusting the biases

BiasNudges[p][j] = 0; // resetting for more training



DesiredValues[p][j] = 0;



for (int k = 0; k < NodeValues[p - 1].Length; k++)

{




315
weights[p - 1][j][k] *= (1 - ETA) * LAMBDA / N; //adjust weights with accordance to l2
regularisation

weights[p - 1][j][k] -= weightNudges[p - 1][j][k] * ETA / MINI_BATCH; //continuation of
weight adjustement equation

weightNudges[p - 1][j][k] = 0; //reset

}

}

}

}

}

}



public void TrainWithBackPropAndTanhSoftmaxWithCustomCostAndL2Regularisation(float[][] Tinputs,
float[][] Toutputs)

{

int epoch = 0;

for (int i = 0; i < Tinputs.Length; i++)

{

epoch++;

runNetworkTanhSoftmax(Tinputs[i]); // test the network for every set of trianing data given



for (int j = 0; j < DesiredValues[DesiredValues.Length - 1].Length; j++)

{

DesiredValues[DesiredValues.Length - 1][j] = Toutputs[i][j]; // adding the wanted outputs to the
desired node values

}



for (int j = NodeValues.Length - 1; j > 0; j--) // back prop up to but excluding the input layer

{

for (int k = 0; k < NodeValues[j].Length; k++)

{

var biasNudge = DesiredValues[j][k] - NodeValues[j][k];



316
// chain rule diff for dc/db as dc/db = error delta = sigmod prime Zl * dc/da where dc/da = al -y

BiasNudges[j][k] += biasNudge;

for (int l = 0; l < NodeValues[j - 1][l]; l++)

{

var weightNudge = (NodeValues[j - 1][l] * biasNudge); // since the weight differential has a
terms that equal the bias in it the bias can be used here

weightNudges[j - 1][k][l] += weightNudge;



var valueNudge = biasNudge; // again shown by diff (they are the same) - need to have wanted
value for node behind to continue back prop

DesiredValues[j - 1][l] += valueNudge; // needed for calculating in previous layers

}

}

}



if (epoch % MINI_BATCH == 0)

{

for (int p = NodeValues.Length - 1; p > 0; p--) // for every layer bar the inputs

{

for (int j = 0; j < NodeValues[i].Length; j++) // for every node

{

NodeBiases[p][j] -= BiasNudges[p][j] * ETA / MINI_BATCH; // adjusting the biases

BiasNudges[p][j] = 0; // resetting for more training



DesiredValues[p][j] = 0;



for (int k = 0; k < NodeValues[p - 1].Length; k++)

{

weights[p - 1][j][k] *= (1 - ETA) * LAMBDA / N; //adjust weights with accordance to l2
regularisation

weights[p - 1][j][k] -= weightNudges[p - 1][j][k] * ETA / MINI_BATCH; //continuation of
weight adjustement equation



317
weightNudges[p - 1][j][k] = 0; //reset

}

}

}

}

}

}



public void TrainWithBackPropAndTanhWithCustomCostAndL2Regularisation(float[][] Tinputs, float[][]
Toutputs)

{

int epoch = 0;

for (int i = 0; i < Tinputs.Length; i++)

{

epoch++;

runNetworkTanh(Tinputs[i]); // test the network for every set of trianing data given



for (int j = 0; j < DesiredValues[DesiredValues.Length - 1].Length; j++)

{

DesiredValues[DesiredValues.Length - 1][j] = Toutputs[i][j]; // adding the wanted outputs to the
desired node values

}



for (int j = NodeValues.Length - 1; j > 0; j--) // back prop up to but excluding the input layer

{

for (int k = 0; k < NodeValues[j].Length; k++)

{

var biasNudge = DesiredValues[j][k] - NodeValues[j][k];

// chain rule diff for dc/db as dc/db = error delta = sigmod prime Zl * dc/da where dc/da = al -y

BiasNudges[j][k] += biasNudge;

for (int l = 0; l < NodeValues[j - 1][l]; l++)




318
{

var weightNudge = (NodeValues[j - 1][l] * biasNudge); // since the weight differential has a
terms that equal the bias in it the bias can be used here

weightNudges[j - 1][k][l] += weightNudge;



var valueNudge = biasNudge; // again shown by diff (they are the same) - need to have wanted
value for node behind to continue back prop

DesiredValues[j - 1][l] += valueNudge; // needed for calculating in previous layers

}

}

}



if (epoch % MINI_BATCH == 0)

{

for (int p = NodeValues.Length - 1; p > 0; p--) // for every layer bar the inputs

{

for (int j = 0; j < NodeValues[i].Length; j++) // for every node

{

NodeBiases[p][j] -= BiasNudges[p][j] * ETA / MINI_BATCH; // adjusting the biases

BiasNudges[p][j] = 0; // resetting for more training



DesiredValues[p][j] = 0;



for (int k = 0; k < NodeValues[p - 1].Length; k++)

{

weights[p - 1][j][k] *= (1 - ETA) * LAMBDA / N; //adjust weights with accordance to l2
regularisation

weights[p - 1][j][k] -= weightNudges[p - 1][j][k] * ETA / MINI_BATCH; //continuation of
weight adjustement equation

weightNudges[p - 1][j][k] = 0; //reset

}

}



319
}

}

}

}



private static float SumForNode(float[] nodeValues, float[] weights)

{

float finalSum = 0;

for (int i = 0; i < nodeValues.Length; i++) // for each node in the layer

{

finalSum += nodeValues[i] * weights[i]; // multiply weight for the working on node by the last node
activation

}

return finalSum;

}




////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



private float activation(float z, float[] zs = null) //nice way to chose between activation fucntion for small
incomplex data

{

if (a_type == 's')

{

return Sigmoid(z);

}

else if (a_type == 'm')

{

return Softmax(zs, z);

}

else if (a_type == 'r')




320
{

return reLU(z);

}

else if (a_type == 'l')

{

return LeakyreLU(z);

}

else

{

return 0f;

}

}



private float derivativeB(int j, int k) //nice way to chose between cost fucntion for small incomplex data

{

if (c_type == 'q')

{

return MSL_SIG_B(j, k);

}

else if (c_type == 'c')

{

return CE_SIG_B(j, k);

}

else if (c_type == 'l')

{

return LL_SM_B(j, k);

}

else

{

return 0f;




321
}

}



private float derivativeW(int j, int l, float bias, int k = 0) //nice way to chose between cost fucntion for small
incomplex data

{

if (c_type == 'q')

{

return MSL_SIG_W(j, k, l, bias);

}

else if (c_type == 'c')

{

return CE_SIG_W(j, k, bias);

}

else if (c_type == 'l')

{

return LL_SM_W(j, k, bias);

}

else

{

return 0f;

}

}

private float derivativeV(int j, int k, int l, float bias) //nice way to chose between cost fucntion for small
incomplex data

{

if (c_type == 'q')

{

return MSL_SIG_V(j, k, l, bias);

}

else if (c_type == 'c')




322
{

return CE_SIG_V(bias);

}

else if (c_type == 'l')

{

return LL_SM_V(j, k);

}

else

{

return 0f;

}

}




///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



private static float Sigmoid(float input)

{

return 1f / (1f + (float)(Math.Exp(-input))); // sigmoid squishification function to get value 0-1

}



private static float Tanh(float input)

{

return 2f / (1f + (float)(Math.Exp(-2 * input))); // tanh gives fast learning than sigmoid and works well with
softmax

}



private static float derivativeTanh(float input)

{

return 2f / (1f + (float)(Math.Exp(-2 * input))); // tanh gives fast learning than sigmoid and works well with
softmax




323
}



private static float DerivativeSigmoid(float input) // this is needed for the calclus involved in back prop

{

return input * (1 - input); // technically this should be sig(x) * (1-sig(x)) but delt with above

}



private static float reLU(float input) // perhaps try using this - reduces vanishing gradient - perhaps also try
leaky relu

{

if (input < 0)

{

return 0;

}

return input;

}



private static float LeakyreLU(float input) //stop problems with dying weights due to small graient with
negatives

{

if (input < 0)

{

return input / SCALE;

}

else

{

return input;

}

}



private static float HardSigmoid(float input) // use for large data sets or lots of iterations




324
{

if (input < -2.5f)

{

return 0f;

}

if (input > 2.5f)

{

return 1f;

}

return 0.2f * input + 0.5f;

}

private static float Softmax(float[] layerinput, float input) //for classification could be used for determining
best moves from a small predefined set

{

float sum = 0;

foreach (float num in layerinput)

{

sum += (float)(Math.Exp((num)));

}

return (float)(Math.Exp(input) / sum);

}



private float MSL_SIG_W(int j, int k, int l, float biasNudge) //mean sqaure loss with sigmoid weight change

{

return NodeValues[j - 1][l] * biasNudge;

}



private float MSL_SIG_B(int j, int k) //mean sqaure loss with sigmoid bias change

{

return DerivativeSigmoid(NodeValues[j][k]) * (DesiredValues[j][k] - NodeValues[j][k]);




325
}

private float MSL_SIG_V(int j, int k, int l, float biasNudge) //mean sqaure loss with sigmoid values change

{

return weights[j - 1][k][l] * biasNudge;

}



private float CE_SIG_W(int j, int k, float biasNudge) //cross entropy with sigmoid weight change

{

float sum = 0;

foreach (float item in NodeValues[j - 1])

{

sum += item * biasNudge;

}

return sum / N;

}



private float CE_SIG_B(int j, int k) //cross entropy with sigmoid bias change

{

return (DesiredValues[j][k] - NodeValues[j][k]);

}



private float CE_SIG_V(float biasNudge) //cross entropy with sigmoid values change

{

return biasNudge;

}



private float LL_SM_W(int j, int k, float biasNudge) //Logarithmic loss with softmax weight change -
classification uses only

{

float sum = 0;




326
foreach (float item in NodeValues[j - 1])

{

sum += item * biasNudge;

}

return sum / N;

}



private float LL_SM_B(int j, int k) //Logarithmic loss with softmax bias change - classification uses only

{

return (DesiredValues[j][k] - NodeValues[j][k]);

}



private float LL_SM_V(int j, int k) //Logarithmic loss with softmax value change - classification uses only

{

return (DesiredValues[j][k] - NodeValues[j][k]);

}



///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



public void SaveNNCurrentState()

{

string filePath = "Assets/NNState/W.txt";

string allNums = "";



foreach(float[][] layer in weights)

{

allNums += "l";

foreach(float[] from in layer)

{

allNums += "f";




327
foreach(float weight in from)

{

allNums += "w";

allNums += weight.ToString();

}

}

}

File.WriteAllText(filePath, allNums);



filePath = "Assets/NNState/B.txt";

allNums = "";



foreach(float[] layer in NodeBiases)

{

allNums += "l";

foreach (float bias in layer)

{

allNums += "b";

allNums += bias.ToString();

}

}

File.WriteAllText(filePath, allNums);

}



public void LoadNNCurrentState()

{

string filePath = "Assets/NNState/W.txt";

string allData = File.ReadAllText(filePath);

int layer = -1;

int from = -1;




328
int to = -1;

string weight = "";

for (int i = 0; i < allData.Length; i++)

{

if(allData.Substring(i,1) == "l")

{

layer++;

}

else if (allData.Substring(i, 1) == "f")

{

from++;

}

else if (allData.Substring(i, 1) == "w")

{

to++;

}

else

{

weight += allData.Substring(i, 1);

}

weights[layer][from][to] = (float)(Convert.ToDouble(weight));

}



filePath = "Assets/NNState/B.txt";

allData = File.ReadAllText(filePath);

layer = -1;

to = -1;

string bias = "";

for (int i = 0; i < allData.Length; i++)

{




329
if (allData.Substring(i, 1) == "l")

{

layer++;

}

else if (allData.Substring(i, 1) == "b")

{

to++;

}

else

{

weight += allData.Substring(i, 1);

}

NodeBiases[layer][to] = (float)(Convert.ToDouble(bias));

}

}

}

NN MANAGER
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum activationType
{
tanh,sigmoid,reLU,leakyReLU,hardSigmoid
}

public enum costType
{
tanhCustom,crossEntropy,MSE
}