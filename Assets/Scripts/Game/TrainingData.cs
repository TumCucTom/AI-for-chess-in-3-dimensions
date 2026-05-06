public class TrainingData
{
//note that where try catches are used and they can cause a piece to not be placed even if it may be placed
legally this is NOT a mistake - this is a more efficient way of creating adequate trianing data than checkingB for
legal squares
//equally for trianing purposes since the training is soley for positional chess evaluation it does not matter is
black and white pieces occupy the same sqaure as the same principles still apply - this reduces the checking
time by 2 fold
//between sessions of running this program the comments that include (piece name) are changed for differnt
pieces in attempt to stop the network overfitting to that specific piece
private int[][][][][] board = new int[2][][][][];//team -white then balck,x,y,z,last is for pieces in order kingB,
commoner, queen, rook, bishop, knight, pawn
private int[] selectToMaterial = new int[13] { 2,1,2,3,3,3,3,5,1,3,-3,-12,1}; //negatives indicated pieces were
given to other team
private float[] selectToAdvantage = new float[13] { -0.1f, -0.1f,-0.2f,-
0.2f,0.2f,0.3f,0.1f,0.3f,0.1f,0.4f,0.3f,0.4f,0.2f};
private void CreateTD()
{
string filePath = "Assets/Training Data/T.txt";
string allData = "";
float output = 0;
int pieceNum = 0;
int material = 0;
int x = 0;
int y = 0;
int z = 0;
int kingB = 0;
int kingW = 0;
bool placed = false;
System.Random rng = new System.Random();
while (true)
{
for (int j = 0; j < 100; j++)
{
for (int i = 0; i < 8; i++)
{
for (int m = 0; m < 8; m++)
{
for (int k = 0; k < 8; k++)
{
for (int l = 0; l < 7; l++)
{
board[0][i][m][k][l] = 0;
board[1][i][m][k][l] = 0;
}
}
}
}
output = 0;
material = 0;
kingB = 0;
x = rng.Next(0, 8);
y = rng.Next(0, 8);
z = rng.Next(0, 8);
board[1][x][y][z][0] = 1;
kingB = 100 * x + 10 * y + z;
kingW = 0;
x = rng.Next(0, 8);
y = rng.Next(0, 8);
z = rng.Next(0, 8);
board[0][x][y][z][0] = 1;
kingW = 100 * x + 10 * y + z;
for (int i = 0; i < rng.Next(1, 151); i++)
{
placed = false;
pieceNum = rng.Next(1, 15);
x = rng.Next(0, 8);
y = rng.Next(0, 8);
z = rng.Next(0, 8);
int count = 0;
foreach (int piece in board[0][x][y][z])
{
if (piece != 0)
{
break;
}
count++;
}
if (count == 7)
{
if (pieceNum == 1) // protected pawns
{
count = 0;
int one = 1;
int two = 1;
if(rng.Next(1, 3) == 1)
{
one = -1;
}
if (rng.Next(1, 3) == 1)
{
two = -1;
}
try
{
foreach (int piece in board[0][x + one][y + 1][z + two])
{
if (piece != 0)
{
break;
}
count++;
}
if (count == 7)
{
placed = true;
board[0][x][y][z][7] = 1;
board[0][x+one][y+one][z+two][7] = 1;
}
}
catch
{
}
}
else if (pieceNum == 2) // isolated pawn
{
try
{
count = 0;
foreach (int piece in board[0][x + 1][y - 1][z + 1])
{
if (piece != 0)
{
break;
}
count++;
}
if (count == 7)
{
count = 0;
foreach (int piece in board[0][x - 1][y - 1][z + 1])
{
if (piece != 0)
{
break;
}
count++;
}
if (count == 7)
{
count = 0;
foreach (int piece in board[0][x + 1][y - 1][z - 1])
{
if (piece != 0)
{
break;
}
count++;
}
if (count == 7)
{
count = 0;
foreach (int piece in board[0][x - 1][y - 1][z - 1])
{
if (piece != 0)
{
break;
}
count++;
}
if (count != 7)
{
placed = true;
board[0][x][y][z][7] = 1;
}
}
}
}
}
catch
{
}
}
else if (pieceNum == 3) //doubled pawns
{
for(int a = y; a < 8; a++)
{
if (board[0][x][a][z][7] == 1)
{
placed = true;
board[0][x][y][z][7] = 1;
break;
}
}
}
else if (pieceNum == 4) // knight on edge of the board[0]
{
if(x == 0 || x == 7 || y == 0 || y == 7 || z == 0 || z == 7)
{
placed = true;
board[0][x][y][z][6] = 0;
}
}
else if (pieceNum == 5) // knight with all moves
{
if (2<x && x<6 && 2 < y && y < 6 && 2 < z && z < 6)
{
placed = true;
board[0][x][y][z][6] = 0;
}
}
else if (pieceNum == 6)//bishop pair
{
int type = x % 2;
for (int a = 0; a < 4; a+=2)
{
for (int b = 0; b < 4; b+=2)
{
for (int c = 0; c < 8; c++)
{
if (board[0][a + type][b + type][c][5] == 1)
{
placed = true;
board[0][x][y][z][5] = 1;
break;
}
}
if (placed)
{
break;
}
}
if (placed)
{
break;
}
}
}
else if (pieceNum == 7) //bishop and queen
{
for (int a = -7; a < 8; a++)
{
for (int b = -7; b < 8; b++)
{
try
{
if (board[0][a][b][z][2] == 1)
{
placed = true;
board[0][x][y][z][4] = 1;
break;
}
}
catch
{
}
for (int c = -7; c < 8; c++)
{
try {
if (board[0][a][b][c][2] == 1)
{
placed = true;
board[0][x][y][z][4] = 1;
break;
}
}
catch { }
}
if(placed)
{ break; }
}
if (placed) { break; }
}
}
else if (pieceNum == 8) // rook on open file
{
int counter = 0;
for (int a = y; a < 8; a++)
{
for (int b = 0; b < 8; b++)
{
if(board[0][x][a][z][b] == 0)
{
counter++;
}
}
}
if(counter == 8)
{
placed = true;
board[0][x][y][z][3] = 1;
}
}
else if (pieceNum == 9) // off starting square (using bishop)
{
if(x!=2 || (y != 0 && y!=7) || x!=5)
{
board[0][x][y][z][4] = 1;
}
}
else if (pieceNum == 10) // attacking sqaure that touches kingB (pawn)
{
for (int a = 0; a < 7; a++)
{
int counter = 0;
if (board[0][Mathf.FloorToInt(kingB / 100) - 2][kingB % 100 - kingB % 10 - 2][kingB % 10
- 2][a] ==0)
{
counter++;
}
if(counter == 7)
{
placed = true;
board[0][Mathf.FloorToInt(kingB / 100)-2][kingB % 100 - kingB % 10 -2][kingB % 10-
2][6] = 1;
}
}
}
else if (pieceNum == 11) // pin on kingB (with pawn)
{
int counter = 0;
for (int a = 0; a < 7; a++)
{
if (board[0][Mathf.FloorToInt(kingB / 100) - 2][kingB % 100 - kingB % 10 - 2][kingB % 10
- 2][a] == 0 && board[1][Mathf.FloorToInt(kingB / 100) - 1][kingB % 100 - kingB % 10 - 1][kingB % 10 - 1][a]
== 0)
{
counter++;
}
}
if (counter == 7)
{
placed = true;
board[0][Mathf.FloorToInt(kingB / 100) - 2][kingB % 100 - kingB % 10 - 2][kingB % 10 -
2][6] = 1;
board[1][Mathf.FloorToInt(kingB / 100) - 1][kingB % 100 - kingB % 10 - 1][kingB % 10 -
1][5] = 1;
}
}
else if (pieceNum == 12) // pin on queenB (pawn)
{
int counter = 0;
for (int a = 0; a < 7; a++)
{
if(board[1][x][y][z][a] == 0 && board[1][x-1][y-1][z-1][a] == 0 && board[0][x-2][y-2][z-
2][a] == 0)
{
counter++;
}
}
if (counter == 7)
{
placed = true;
board[1][x][y][z][2] = 1;
board[1][x - 1][y - 1][z - 1][5] = 1;
board[0][x - 2][y - 2][z - 2][6] = 1;
}
}
else // controlling centre sqaures (pawn)
{
if((x ==5 || x==4) && (y == 5 || y == 4))
{
board[0][x][y][z][6] = 1;
}
}
if (placed)
{
material += selectToMaterial[pieceNum - 1];
output += selectToAdvantage[pieceNum - 1];
}
placed = false;
pieceNum = rng.Next(1, 15);
x = rng.Next(0, 8);
y = rng.Next(0, 8);
z = rng.Next(0, 8);
foreach (int piece in board[1][x][y][z])
{
if (piece != 0)
{
break;
}
count++;
}
if (count == 7)
{
if (pieceNum == 1) // protected pawns
{
count = 0;
int one = 1;
int two = 1;
if (rng.Next(1, 3) == 1)
{
one = -1;
}
if (rng.Next(1, 3) == 1)
{
two = -1;
}
try
{
foreach (int piece in board[1][x + one][y + 1][z + two])
{
if (piece != 0)
{
break;
}
count++;
}
if (count == 7)
{
placed = true;
board[1][x][y][z][7] = 1;
board[1][x + one][y + one][z + two][7] = 1;
}
}
catch
{
}
}
else if (pieceNum == 2) // isolated pawn
{
try
{
count = 0;
foreach (int piece in board[1][x + 1][y - 1][z + 1])
{
if (piece != 0)
{
break;
}
count++;
}
if (count == 7)
{
count = 0;
foreach (int piece in board[1][x - 1][y - 1][z + 1])
{
if (piece != 0)
{
break;
}
count++;
}
if (count == 7)
{
count = 0;
foreach (int piece in board[1][x + 1][y - 1][z - 1])
{
if (piece != 0)
{
break;
}
count++;
}
if (count == 7)
{
count = 0;
foreach (int piece in board[1][x - 1][y - 1][z - 1])
{
if (piece != 0)
{
break;
}
count++;
}
if (count != 7)
{
placed = true;
board[1][x][y][z][7] = 1;
}
}
}
}
}
catch
{
}
}
else if (pieceNum == 3) //doubled pawns
{
for (int a = y; a < 8; a++)
{
if (board[1][x][a][z][7] == 1)
{
placed = true;
board[1][x][y][z][7] = 1;
break;
}
}
}
else if (pieceNum == 4) // knight on edge of the board[1
// ]
{
if (x == 0 || x == 7 || y == 0 || y == 7 || z == 0 || z == 7)
{
placed = true;
board[1][x][y][z][6] = 0;
}
}
else if (pieceNum == 5) // knight with all moves
{
if (2 < x && x < 6 && 2 < y && y < 6 && 2 < z && z < 6)
{
placed = true;
board[1][x][y][z][6] = 0;
}
}
else if (pieceNum == 6)//bishop pair
{
int type = x % 2;
for (int a = 0; a < 4; a += 2)
{
for (int b = 0; b < 4; b += 2)
{
for (int c = 0; c < 8; c++)
{
if (board[1][a + type][b + type][c][5] == 1)
{
placed = true;
board[1][x][y][z][5] = 1;
break;
}
}
if (placed)
{
break;
}
}
if (placed)
{
break;
}
}
}
else if (pieceNum == 7) //bishop and queen
{
for (int a = -7; a < 8; a++)
{
for (int b = -7; b < 8; b++)
{
try
{
if (board[1][a][b][z][2] == 1)
{
placed = true;
board[1][x][y][z][4] = 1;
break;
}
}
catch
{
}
for (int c = -7; c < 8; c++)
{
try
{
if (board[1][a][b][c][2] == 1)
{
placed = true;
board[1][x][y][z][4] = 1;
break;
}
}
catch { }
}
if (placed)
{ break; }
}
if (placed) { break; }
}
}
else if (pieceNum == 8) // rook on open file
{
int counter = 0;
for (int a = y; a < 8; a++)
{
for (int b = 0; b < 8; b++)
{
if (board[1][x][a][z][b] == 0)
{
counter++;
}
}
}
if (counter == 8)
{
placed = true;
board[1][x][y][z][3] = 1;
}
}
else if (pieceNum == 9) // off starting square (using bishop)
{
if (x != 2 || (y != 0 && y != 7) || x != 5)
{
board[1][x][y][z][4] = 1;
}
}
else if (pieceNum == 10) // attacking sqaure that touches kingW (pawn)
{
for (int a = 0; a < 7; a++)
{
int counter = 0;
if (board[1][Mathf.FloorToInt(kingW / 100) - 2][kingW % 100 - kingW % 10 -
2][kingW % 10 - 2][a] == 0)
{
counter++;
}
if (counter == 7)
{
placed = true;
board[1][Mathf.FloorToInt(kingW / 100) - 2][kingW % 100 - kingW % 10 -
2][kingW % 10 - 2][6] = 1;
}
}
}
else if (pieceNum == 11) // pin on kingW (with pawn)
{
int counter = 0;
for (int a = 0; a < 7; a++)
{
if (board[1][Mathf.FloorToInt(kingW / 100) - 2][kingW % 100 - kingW % 10 -
2][kingW % 10 - 2][a] == 0 && board[0][Mathf.FloorToInt(kingW / 100) - 1][kingW % 100 - kingW % 10 -
1][kingW % 10 - 1][a] == 0)
{
counter++;
}
}
if (counter == 7)
{
placed = true;
board[1][Mathf.FloorToInt(kingW / 100) - 2][kingW % 100 - kingW % 10 - 2][kingW
% 10 - 2][6] = 1;
board[0][Mathf.FloorToInt(kingW / 100) - 1][kingW % 100 - kingW % 10 - 1][kingW
% 10 - 1][5] = 1;
}
}
else if (pieceNum == 12) // pin on queenW (pawn)
{
int counter = 0;
for (int a = 0; a < 7; a++)
{
if (board[0][x][y][z][a] == 0 && board[0][x - 1][y - 1][z - 1][a] == 0 && board[1][x -
2][y - 2][z - 2][a] == 0)
{
counter++;
}
}
if (counter == 7)
{
placed = true;
board[0][x][y][z][2] = 1;
board[0][x - 1][y - 1][z - 1][5] = 1;
board[1][x - 2][y - 2][z - 2][6] = 1;
}
}
else // controlling centre sqaures (pawn)
{
if ((x == 5 || x == 4) && (y == 5 || y == 4))
{
board[1][x][y][z][6] = 1;
}
}
if (placed)
{
material -= selectToMaterial[pieceNum - 1];
output -= selectToAdvantage[pieceNum - 1];
}
}
}
}
int team = 0;
int num = 0;
if (material < 0)
{
team = 1;
material = -1 * material;
}
for (int a = 0; a < 8; a++)
{
for (int b = 0; b < 8; b++)
{
for (int c = 0; c < 8; c++)
{
num = 0;
for (int d = 0; d < 7; d++)
{
if(board[team][a][b][c][d] == 0)
{
num++;
}
}
if (num == 7)
{
material--;
board[team][a][b][c][6] = 1;
//protected
if(board[team][a + 1][b + 1][c + 1][6] == 1 || board[team][a - 1][b + 1][c + 1][6] == 1 ||
board[team][a + 1][b + 1][c - 1][6] == 1 || board[team][a - 1][b + 1][c - 1][6] == 1)
{
output += team == 0 ? 0.1f : -0.1f;
}
else // isolated
{
output += team == 0 ? -0.1f : 0.1f;
}
for (int i = b; i < 8; i++) // doubled
{
if(board[team][a][i][c][6] == 1)
{
output += team == 0 ? -0.2f : 0.2f;
}
}
if (material == 0)
{
break;
}
}
}
if (material == 0)
{
break;
}
}
if (material == 0)
{
break;
}
}
for (int a = 0; a < 2; a++)
{
for (int b = 0; b < 8; b++)
{
for (int c = 0; c < 8; c++)
{
for (int d = 0; d < 8; d++)
{
for (int e = 0; e < 7; e++)
{
allData += board[a][b][c][d][e];
}
}
}
}
}
allData += output;
}
File.WriteAllText(filePath, allData);
}
}
}