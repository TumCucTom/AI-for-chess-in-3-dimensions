using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIButton : Button
{
private InputReciever reciever;
protected override void Awake()
{
reciever = GetComponent<UIInputReciever>();
onClick.AddListener(() => reciever.OnInputRecieved());
}
}




4.0 TESTING
PRELIMINARY

[Note that each of the testing objectives links directly to those in design and analysis]

[My microphone is not the best – in all testing videos there is speaking but you may have to turn your
volume up to hear it]

You need to be signed into a google account to view the content in the videos.

Table of QR Codes:




Test      QR Code - Links


The       https://drive.google.com/file/d/1fNhfQDFRjIn7LR1m2qxq8_wufa6ohrOn/view?usp=share_lin
Board                                                 k




412
The    https://drive.google.com/file/d/1P_QHI3i7d4_Z3KJNa_NvEuw3T7rGGZh6/view?usp=sharing
Game




UI     https://drive.google.com/file/d/1dlYP3whWLXkN6hpcErFNKTLKL2xOJw8Z/view?usp=sharing




Ai1    https://drive.google.com/file/d/1AviV2v248TPKDxctyYy3FwVjcoFGoMfg/view?usp=sharing




413
Ai2-1   https://drive.google.com/file/d/1S9iJMHfFmvPohUn5oZJ6BBWpzO3tqm_n/view?usp=sharing




Ai2-2   https://drive.google.com/file/d/1g0_UWFSLH6JwvRB8aNCX9XsOu9UYKg-
E/view?usp=sharing




414
Ai3-1   https://drive.google.com/file/d/1xQ-0aF9L2FEhM-
wmQHJz7WtWmFXBN3Vb/view?usp=sharing




Ai3-2   https://drive.google.com/file/d/1bX1qqo6hNbZ35UJKWOxQqVVhQE8INcBo/view?usp=shari
ng




Ai4-1   https://drive.google.com/file/d/1RiMu6ynkAT4-griymNg4189O4wqW62l0/view?usp=sharing




415
Ai4-2   https://drive.google.com/file/d/1kXEfn2JceSxZif9k4h4kiitzFf4n3Yok/view?usp=sharing




Ai4-3   https://drive.google.com/file/d/11PUWT6HrZeCKzv3IzSQ6PcdtrauoYRyM/view?usp=sharing




Ai5-1   https://drive.google.com/file/d/1fucjt-9S3sGcmlnqW9m6hUFV8rbZA8dw/view?usp=sharing




Ai5-2   https://drive.google.com/file/d/1y-xgXZkNu-
9ZOMsdpNKGKroFM8FeUbEg/view?usp=sharing




416
Ai5-3    https://drive.google.com/file/d/1krlhZlWJNknhkTlV1-n4sQKlod7NM1rG/view?usp=sharing




4.1 – THE BOARD



Objective         Normal Data                   Boundary Data   Erroneous Data   Pass/Fail


1,a - 8 Boards    N/A                           N/A             N/A
Stacked above
one another


1.a.i - Each      2 rooks, 2 knights,2          N/A             N/A
Board should      bishops, a queen, a king(or
have a full set   commoner), 8 pawns in
of pieces         each colour




417
1.a.ii – The        Top down the board            N/A           N/A
pieces should       should look as follows:
be set up in
standard
formation
according to
FIDE rules.




1.a.iii - Each      Board 4 has a different       N/A           N/A
board should        coloured piece in the ‘king
have a              square’
commoner
instead of a
king except
board 4


4.2 – THE GAME



Objective            Normal Data          Boundary Data   Erroneous Data   Pass/Fail


2.a – There          Both sides should    N/A             N/A
should be a          be controlled by
player vs player     the user
game mode


2.b – A pawn that    1- test for pawn     N/A             N/A
has not moved        that has not
can move 1 or        moved that it can
two squares and      move one square
one that has
2- test for pawn
moved can only
move one square      that has not
moved that it can
move two
squares

3 - test for pawn
that has moved




418
that it can only
move one square


2.c – There           Both sides should    N/A         N/A
should be ai vs ai    be controller by
mode                  the AI

1 – see that AI
makes moves for
both teams

2 – see that user
input cannot
move pieces


2.d – there           User input should    N/A         N/A
should be player      control one team
vs AI                 and AI should
control the other.

1 – see that the
user can control
the moves for
one team

2 – see that the
AI makes moves
for the other
team

3 – see that
player cannot
make moves for
the same team
that the AI is
making moves
for


2.e – For 2.a,2.c     For 2.a this         N/A         N/A
and 2.d if there is   means user
user input            controls both
allowed it should     (tested already)
be able to be for
For 2.c it is
either black or
irrelevant
white




419
For 2.d the user
should be able to
control white or
black


2.g - a sound         Check that a         N/A         N/A
should play when      sound is played
a piece is moved      when a piece is
moved


2.h – there           Check that           N/A         N/A
should be             throughout the
background            game
music                 background
music plays


2.i.i – There         1- test that         N/A         N/A
should be the         overall time can
ability to limit      be changed
time players can
2 – test that time
use over the
gained after
whole of the
game                  move can be
changed

3 - test that
when time runs
out the player
with no time
loses


2.j.i – rook should   1 – test I can       N/A         N/A
move as               move any
according to          amount of
rules                 squares up

2 – test it can
move any
amount of
squares along a
file

3 – test is can
move any
amount of




420
squares along a
rank


2.j.ii – bishop        1 – test it can       N/A         N/A
should move            move any
according to           amount of
rules                  squares across
the same number
of files and ranks

2 – test that it
can move any
amount of
squares across
the same number
of files, ranks and
levels


2.j.iii – the knight   1 - it can move 2     N/A         N/A
should move            squares in one
according to the       direction and one
rules                  square in the
other two – test
for all three
permutations


2.j.iv – the queen     Test that it          N/A         N/A
should move            moves according
according to the       to the rook and
rules                  the bishop

1 – test it can
move any
amount of
squares across
the same number
of files and ranks

2 – test that it
can move any
amount of
squares across
the same number
of files, ranks and
levels




421
3 – test I can
move any
amount of
squares up

4 – test it can
move any
amount of
squares along a
file

5 – test is can
move any
amount of
squares along a
rank


2.j.v – the           Test that it can   N/A         N/A
commoner and          move a cube in
king should           around itself
move one square
in each direction


2.k.i – pieces        N/A                N/A         Try to move piece
cannot move out                                      out of the bounds
of the board                                         of the board


2.k.ii – king may     N/A                N/A         Try to move a
not stay in check                                    piece that does not
stop the king from
being in check
whilst in check


2.k.iii – a piece     N/A                N/A         Try to move a
may not move if                                      piece that creates
it puts the king in                                  check on the king
check


2.k.iv – pieces of    N/A                N/A         Try to move a
the same team                                        piece onto a piece
may not occupy                                       of the same team –
the same square                                      where the move
would usually be
legal otherwise




422
2.k.v -for all piece   N/A                 N/A         Try moving a piece
except the knight                                      to square that has
there may not be                                       a piece in the way
a piece in the                                         where it would be
path towards                                           legal if said piece
desired square                                         was not in the way.

Test for

1.   Queen
2.   Rook
3.   Bishop
4.   Pawn
5.   Commoner


2.l – no legal         Achieve             N/A         N/A
moves left for a       checkmate and
team is a loss if      see that title
the king is in         screen displays a
check                  win for the
mating team


2.m – no legal         Achieve no legal    N/A         N/A
moves left for a       moves for the
team is a draw if      enemy team
the king is not in     without achieving
check                  checkmate and
see that title
screen displays
draw


2.n – when a           Test that a legal   N/A         N/A
piece is selected      move to a free
the available          square shows
moves should be        green
displayed
Test that a legal
move to where
an enemy piece
is shows red


2.0 – tested           N/A                 N/A         N/A                   N/A
under 4.1 board




423
4.3 – TESTING – UI



Objective          Normal Data                           Boundary        Erroneous         Pass/Fail
Data            Data


3.a.1              There should be a menu screen         N/A             N/A
on launch


3.a.i.1.a –        Test that clicking the settings       N/A             N/A
settings           button opens the setting menu
button


3.a.i.2 – player   1. Clicking on player vs player       4.Entering no   N/A
vs player          will open the time page               time starts a
game still
2. after selecting time controls
and pressing start the game
starts.

3. game starts with the correct
time controls


3.a.i.3 – player   1. Clicking on player vs ai will                      5 – entering
vs ai              open the team, difficulty page                        no difficulty
defaults to
2. after difficulty, and team the
easiest
game starts
6 – entering
3. game starts with correct
no team
controls
defaults player
as white or
last chosen


3.a.i.4 – ai vs    1. Clicking on ai vs ai will open     N/A             5 – entering
ai                 the difficulty page                                   no difficulty
defaults to
2. after selecting difficulty the                     easiest
game should start

3. game starts with correct
controls


3.a.i.6 – save     Play three moves sand save the        N/A             N/A
game               game




424
1 – once save is selected the
check that the same three
move position is loaded

2 - Repeat for different moves

3 – check that no time is loaded


3.b.i – Display    See that when a move is made        See that in a    N/A
moves made         the move counter increases          load game the
number of
moves made


3.b.iii –          Test that taking pieces             Test that
number of          increases the pieces taken          taken moves
taken pieces       counter for each team               stays constant
should be                                              for a loaded
1 – take a white pawn
displayed                                              game
2 – take a black queen
1 - Save the
game used for
normal data
testing

2 – Load the
game and see
the taken
pieces show


3.b.iv –           Tested in 4.2.m                     N/A              N/A
winning team
is displayed


3.c.i –            Test that each button makes its     N/A              N/A
transparent        respective layer translucent and
layers             the button itself translucent.

Test that pieces stay opaque
and stationary


3.d.i – settings   Click settings button and check     N/A              N/A
button             it opens the settings


3.d.ii – mute      1 – press the sounds effect         N/A              N/A
buttons            mute button




425
2 – see that moving makes not
sounds

1 – press the mute background
sound button

2 – see that the background
music stops playing


3.d.iii – Save    [Tested in load game]               N/A              N/A
game


3.d.iv – return   Start player vs player game.        N/A              N/A
to menu           Open settings. Select return to
menu and see that the menu
opens


3.d.v – Restart   Start a player vs Ai game. Make     N/A              N/A
button            a move and wait for ai to move.
Open Settings. Chose restart.
Make and move and see ai
makes a move


3.d.vi – return   Start game. Open settings.          N/A              N/A
to game           Press return. See that game
continues




4.4 – TESTING – AI

Numbers shown in ‘[ ]’ represent the objectives tested by each specific test

When I refer to objectives in testing videos I mean test numbers I,e when I say “testing objective 1 for
AI 1” I mean that I am testing: (1- blunder a piece and check that the opponent ai takes it – testing
[1][2][4][11]). Not that I am testing Objective one from the objective in design.

A note regarding the piece value tests – it is not mentioned in the videos but was brought up by the
client as they thought there was a mistake. The best move being display by the Debug.Log() statement
is taking the moves from white that give no additional theory and therefore we don’t have to consider
the sorts of development that the player can do following that may cancel out to leave us with smaller
values, so this is not a mistake.

Another note regarding the implementation using percentage accuracy. It is briefly touched upon in the
videos but to reiterate: the reason that the percentage accuracy implementation works here is due to
the large state space. One might argue that if you compare this to a normal Chess Ai that this
implementation could never work for the fact that in positions where the obvious move for any level of
player is to capture a blundered piece or to recapture a piece you intended to trade that this



426
implementation will not do as such. This does work for 3D chess however as the computer has a very
large advantage being able to see the whole board which the user cannot. The idea behind the
percentage is to create a human like handicap for the computer where it cannot perceive the board well
and does not have a good understanding of what is happening in every area of the boards.

One final note regarding the testing for Ai-4: I state that test 2 was passed I do not believe this was true
however it was passed in the next video.


Objective         Normal Data                           Boundary        Erroneous         Pass/Fail
Data            Data


4.1 – Ai1         If blunder occurs retry test –        N/A
(where ‘most’ and ‘some’ are
written) - This will be indicated
by a Debug.Log Statement

1- blunder a piece and check
that the opponent ai takes it –
testing [1][2][4][11]

2 – blunder a piece and check
that the ai does not take some
of the time – testing [4][6][7]

3 – create a position where the
ai can take a protected pawn
with a higher valued piece and
that it does not take – testing
[5]

4 – play an opening and show
that ai does not play according
to theory with pieces towards
the centre – [12]

6 – create position where
player can make check the
opponent and see that ai stops
it some of the time – testing
[14]

7 – create position where ai can
check the player’s king and see
that it will then check some of
the time – testing [9][8]

8 – create a position where you
can take the oponents piece




427
that is hanging and test that
the ai protects some of the
time – testing [3]

9 – show that the AI
understands the value of pieces

[10]


4.2 – Ai 2   If blunder occurs retry test –
(where ‘most’ and ‘some’ are
written) - This will be indicated
by a Debug.Log Statement

1- blunder a piece and check
that the opponent ai takes it
most of the time – testing
[1][2][4]

2 – blunder a piece and check
that the ai does not take some
of the time – testing [4][6][7]

3 – create a position where the
ai can take a protected pawn
with a higher valued piece and
that it does not take – testing
[5]

5 – create position where
player can make check the
opponent and see that ai stops
it some of the time – testing
[14]

6 – create position where ai can
check the player’s king and see
that it will then check some of
the time – testing [9][8]

7 – create a position where you
can take the oponents piece
that is hanging and test that
the ai protects most of the time
– testing [3]




428
8 – show that the AI
understands the value of pieces
– [9b]

13 – play 15 moves worth of
opening phase and see the the
ai develops mostly new pieces
towards the centre – testing
[12][10]


4.3 – Ai 3   If blunder occurs retry test –
(where ‘most’ and ‘some’ are
written) - This will be indicated
by a Debug.Log Statement

1- blunder a piece and check
that the opponent ai takes it
most of the time – testing
[1][2][4]

2 – blunder a piece and check
that the ai does not take some
of the time – testing [5]

3 – create a position where the
ai can take a protected pawn
with a higher valued piece and
that it does not take – testing
[5]

5 – create position where
player can make check the
opponent and see that ai stops
it some of the time – testing
[14]

6 – create position where ai can
check the player’s king and see
that it will then check some of
the time – testing [9][8]

7 – create a position where you
can take the oponents piece
that is hanging and test that
the ai protects most of the time
– testing [3]




429
8 – show that the AI
understands the value of pieces
– [9b]

13 – play 15 moves worth of
opening phase and see the the
ai develops mostly new pieces
towards the centre – testing
[9a]

16 – test that some of the time
the ai will make intelligent
moves that require it look
multiple moves ahead– testing
[4a]

17 – in a position where very
little major pieces are left check
the ai tries to move its king
towards pawns of either team –
testing [9c]

18 – test that in a position
where the ai is up material it
will try to trade pieces if they
can – testing [10]

19 - test that in a position
where the ai is down material it
will not try to trade pieces if
they can – testing [10]

20 – test that if a rook can
move to an open file safely
(and it cannot obtain material
in another way) the ai will
chose to do so – testing [8c]

21 – test that in a position
where the ai should move it’s
knight it will decide to not
move it to the edge of the
board most of the time –
testing [8b]

22 – test that in a position
where the ai will make doubled
pawns or isolate a pawn it




430
chooses not to most of the
time – testing [8a]


4.4 – Ai 4   If blunder occurs retry test –
(where ‘most’ and ‘some’ are
written) - This will be indicated
by a Debug.Log Statement

1- blunder a piece and check
that the opponent ai takes it
most of the time – testing
[1][2][4]

2 – see that somewhere during
testing the opponent either
makes a blunder (test is set out
like this due to the low blunder
rate of the AI) – testing [5]

3 – create a position where the
ai can take a protected pawn
with a higher valued piece and
that it does not take – testing
[5]

5 – create position where
player can make check the
opponent and see that ai stops
it some of the time – testing
[14]

6 – create position where ai can
check the player’s king and see
that it will then check some of
the time – testing [9][8]

7 – create a position where you
can take the oponents piece
that is hanging and test that
the ai protects most of the time
– testing [3]

8 – show that the AI
understands the value of pieces
– [9b]

13 – play 15 moves worth of
opening phase and see the the




431
ai develops mostly new pieces
towards the centre – testing
[9a]

16 – test that some of the time
the ai will make intelligent
moves that require it look
multiple moves ahead– testing
[4a]

17 – in a position where very
little major pieces are left check
the ai tries to move its king
towards pawns of either team –
testing [9c]

18 – test that in a position
where the ai is up material it
will try to trade pieces if they
can – testing [10]

19 - test that in a position
where the ai is down material it
will not try to trade pieces if
they can – testing [10]

20 – test that a position where
one of the following can occur:
the ai tries to undergo the
move if it is positive and tries
to stop the move if it is
negative (most of the time):

Pawns protecting each other.
+0.1

Isolated pawns -0.1

Doubled pawns -0.2

Knight on edge of board -0.2

Knight with all moves in bound
of the board +0.2

Bishop pair +0.3

Bishop on same line as queen
+0.1




432
Rook on open file +0.3

Any piece off starting square
(tested under point 13 – except
from checking moves and
capturing moves all moves were
off starting square move – check
video one for evidence) +0.1

Any piece attacking a square
that touches the king (tested in
first video – can been seen with
the various check moves and
especially the queen moves
which stops the king from
moving to desirable squares (out
of the way of more checks))+0.4

Any piece achieving a pin on
king +0.1*the value of the
pinned piece

Any piece achieving pin on the
queen +0.4

Any piece controlling or on the
centre 4 squares of any level
(tested under point 13 – except
from checking moves and
capturing moves all moves were
pawn moves to the centre –
check video one for evidence)
+0.2


4.5 – Ai 5   1- blunder a piece and check
that the opponent ai takes it all
of the time – testing [1][2][4]

3 – create a position where the
ai can take a protected pawn
with a higher valued piece and
that it does not take – testing
[5]

5 – create position where
player can make checkmate
threat on the opponent and see




433
that ai stops it all of the time –
testing [14]

6 – create position where ai can
check the player’s king and see
that it will then check all of the
time where there is not a better
move – testing [9][8]

7 – create a position where you
can take the oponents piece
that is hanging and test that
the ai protects (or offers a
trade) all of the time – testing
[3]

8 – show that the AI
understands the value of pieces
– [9b]

13 – play 15 moves worth of
opening phase and see the the
ai develops allly new pieces
towards the centre – testing
[9a]

16 – test that some of the time
the ai will make intelligent
moves that require it look
multiple moves ahead (this is
done by create a position
where the ai can make two
moves in a row where the
second move gives increased
material) – testing [4a]

17 – in a position where very
little major pieces are left check
the ai tries to move its king
towards pawns of either team –
testing [9c]

18 – test that in a position
where the ai is up material it
will try to trade pieces if they
can – testing [10]




434
19 - test that in a position
where the ai is down material it
will not try to trade pieces if
they can – testing [10]

20 – test that a position where
one of the following can occur:
the ai tries to undergo the
move if it is positive and tries
to stop the move if it is
negative (all of the time):

Pawns protecting each other.
+0.1

Isolated pawns -0.1

Doubled pawns -0.2

Knight on edge of board -0.2

Knight with all moves in bound
of the board +0.2

Bishop pair +0.3

Bishop on same line as queen
+0.1

Rook on open file +0.3

Any piece off starting square
(tested under point 13 – except
from checking moves and
capturing moves all moves were
off starting square move – check
video one for evidence) +0.1

Any piece attacking a square
that touches the king (tested in
first video – can been seen with
the various check moves and
especially the queen moves
which stops the king from
moving to desirable squares (out
of the way of more checks))+0.4




435
Any piece achieving a pin on
king +0.1*the value of the
pinned piece

Any piece achieving pin on the
queen +0.4

Any piece controlling or on the
centre 4 squares of any level
(tested under point 13 – except
from checking moves and
capturing moves all moves were
pawn moves to the centre –
check video one for evidence)
+0.2




5.0 EVALUATION
5.1.1 – OBJECTIVES – COMPLETENESS
From the testing we can see that all the objectives were met, however, the testing regarding the AI was difficult
to set out and conduct due the the nature of how AI needed to be created. Though it seems that the AI is
working how I wish it to work and it under/outperforms myself as necessary there is no conclusive way to
measure such objectives or a way to make the objectives in a way that allows conclusive testing.

5.1.2 – OBJECTIVES – HOW IT WAS ACHIEVED
The objectives were achieved through many hours of play and subsequent note making. Much of the though
process can be seen through the end of analysis and start of the design section. The AI was also made realistic
(regarding the level of human player they are trying to imitate) by the same process of extensive play and note
taking. The thought process can again be seen in the later stages of analysis and throughout design.

The implementation of the other parts was done in small stages using the structure chart shown in design to get
the backbone of the project covered and filling in the specifics afterwards. The complex sections of code were
tackled by generating flow charts and converting these into code. For the neural network specifically some
research into multivariable calculus and the mathematics was needed to understand and code.

5.2 – IMPROVEMENT
Give more time there could have been some improvements. The neural network has had extensive training and
has shown that it can pick up well on the patterns I had hoped. It does not learn fast however, due its large
nature (7168, 3584, 512, 64, 1). For this reason, the network most likely has a long time left of learning and given
more time I could have attempted to make learning faster or train it for longer to achieve better results of the
later AIs. This would future proof the application I have made, keeping the top AI very Strong even though it
does meet the current requirements.

Given more time some of the features from chess.com that are impractical because of the time needed to create
such things are too time consuming. For example, the puzzles and lessons could have been made if had more
time: the puzzles could have been made with an algorithm that works from checkmate backwards and uses the




436
AIs to classify puzzle difficulty (this however would use up computational power that was being used to create
training data and train the network). The lessons would require me to play much more of this game and then
film the lessons.

5.3.1 – USER FEEDBACK – RELATING TO 1 S T AND 2 N D INTERVIEWS
Below are reminders of relevant original questions asked to the user and follow up questions asked after he
played the final product.

[Old Questions in this colour]

Q: With regards to the UI, what would be you be expecting?

A: I would like the board to be able to be split up so that certain or individual layers can be seen
separately. I would also like to be able to rotate the board and see which pieces have been captured.
Additionally, a feature to see a piece's available moves would be nice.

Takeaway: Being able to visualise your moves is very important and as such having an easy to use,
understand and visualise playing field should be a priority.

Q: How do you feel with the UI and how it relates to what you were expecting?

A: I felt like the movement around the board was very easy to use and made the experience enjoyable.
Being able to make the board transparent was surprising very helpful when considering tactics during
the middle game. Displaying taken pieces on the side was good and important for knowing when to
trade down and not, especially later in the game when things were harder to keep track of. My favourite
UI feature by far was the ability to see where pieces can move and capture. For humans is makes it much
more fun and easier to play a competitive game and reduces the stress I felt about keeping track of
danger levels (a side note from me – danger levels are where pieces are to do with how many pieces are
attacking how many other pieces).

Takeaway: It seems that the user is very happy with the outcome of the UI and has no complaints – this
corresponds with what the completeness of the objectives suggest.




Q: What would you expect from the AI?

A: There should be different levels of AI with one that will lose to an average intelligence beginner but
also one that would beat an experienced player. I would like the AI to be able to play as close to as a
human would as possible.

Takeaway: A varying and competitive AI is important for play to be intriguing but also the AI should
behave human-like to provide more realistic play.

Q: How do you feel with the AI and how it relates to what you were expecting?

A: Overall the variation in the AI is amazing. There’s much more variety than I expected. I’m quite glad
that I struggle with the 3rd AI as this means that there is huge headroom above me but also that the
easy AIs are accessible for beginners. Having played all 5 they did feel comparable to humans, of course
due to the nature of probability it felt like sometime the lower AIs played a sequence of too good moves
and the 4th and 3rd AIs would make a couple horrendous blunders in a row. Overall, I wouldn’t expect



437
humans to play much differently to AIs 1-4. It was good to see the positional play from AI 4. The jump
form AI4 to 5 was extreme with AI5 dominating me the entire time. Overall, the AI was much better than
anticipated.

Takeaway: It seems that the user is very happy with the outcome once again. There are more points to
discuss here however, it seems that there could have been an AI between AI4 and 5 to cover this large
gap. The neural network seems to be perfoming well if an experienced chess player can commend and
its positional play.




Q: What time restrictions on time would you like to see?

A: Just as with FIDE chess I think there should be an untimed, standard, blitz and bullet options, however,
at this time I am not sure what these exact times at this moment.

Takeaway: There should be multiplied time modes, however, until the game is played the specifics are
uncertain.

Q: How do you feel about the fully variable time system

A: It is much better for users to find out what time is best for them than with set time rules that only
apply to us. Personally I like 120m 1m inc.

Takeaway: I feel the same as the user with regard to other peopleplaying this game. This is a success
that meets the testing outcome.

____________________________________________________________________________________________________________
Q: What game modes would you like?

A: Player vs AI, player vs player. Perhaps an AI vs AI to learn scenarios.

Takeaway: Having variation in what can be done inside the game is important for the user.

Q: How do you feel about the game modes?

A: They are all there and working so fine.

Takeaway: User’s views align with testing.




Q: How would you like games to be recorded/ displayed back to you?

A: Moves should be displayed on a sidebar as they happen.

Takeaway: A move sidebar feature

Q: How do you feel about the sidebar?

A: It is good. The scrolling feature is a nice touch that I appreciate.



438
Takeaway: User’s views align with testing.




Q: How would you like this game to be available I.e. on a large store front, a website?

A: Preferably as a computer app as I feel a website would be too slow but would be fine if it is not
slower. Perhaps also a phone application.

Takeaway: The user seems to be happy with most forms that it could be available in, however, they seem
keen to play it on a computer.

Q: Having played the game what do you think about how it should be available?

A: I think that computer only, as it is currently fine. It feels like it would be too finickity to play on a small
handheld device.

Takeaway: User’s views align with testing




Q: What hardware/software restrictions may affect the making of this project?

A: I think that this program should be able to be ran on a mid-range system (this may affect the AI). Like
a phone.

Takeaway: The AI nor UI can be so complex that an average device will struggle to compute the game.

Q: Having played run the game on your computer how was the experience?

A: The AI took longer to move than when playing on your PC (the device used for testing) but still slower
than stockfish 15 (chess.com’s top engine) say. So pretty good.

Takeaway: The AI will take much longer to move on the baseline system stated in the design section,
however it is still functional and faster than some normal Chess AIs (that are much more complex than
mty AIs however).




Q: What additional features above gameplay with an AI would you be expecting?

A: Player on player gameplay and AI vs AI gameply. I have no desire to play other people on a WAN as
the games take too long and that is too much time commitment. Sounds would be good too.

Takeaway: The user just wants a way to play against other humans too on the same device. There should
be piece move sound effects and background music.

Q: How ddid you like the ai vs ai and sound effects?




439
A: The ai moved too fast for me to keep track of real time but using save game to see what they were
doing, and the scrollable list made watching what they were doing okay. The sound effects were good,
the background music was obnoxious but I simply mute it.

Takeaway: A break between AI moves may be good for the AI vs AI gamemode. The mute button for
the background music was a good idea. Maybe variety in background music choice would be something
to consider.




Q: What/if any two player capabilities would you like?

A: Just standard play with different time limits

Takeaway: No extras needed.

Takeaway: Already covered.

Q: How/would you change the overall time of the game?

A: Different time modes as mentioned before.

Takeaway: Time is an issue and I need to set objectives that allow for shorter gameplay.

Takeaway: Already covered.




Q: Do you think there should be a limitation on thinking time for the human?

A: Yes, of varying amounts of the user's choice.

Takeaway: There should be an ability to create different time mode modes with different starting times
and added time.

Takeaway: Already covered.




Q: Do you think there should be a limitation on calculation time for the computer?

A: There should be different levels of AI that do fewer calculations.

Takeaway: Having varying AI levels is important not only for fun gameplay but also for competitive
gameplay. (Quick calculating AIs and more complex AI)

Takeaway: Already covered under first AI question.




440
Q: How strong should the AI be and what should it consider?

A: It should be strongly capable of beating the best of players but with abilities to reduce its own ability.
It should prioritise piece taking as opposed to board position – the exception to the this is where the
piece will be taken by a lower value piece if moved to a location. Try to defend or move high value
pieces

Takeaway: A strong AI is important to the user but equally important is one that can vary in ability. After
understanding the core principles of the game, both I and the user have discovered that for 3D chess
taking pieces early is essential for a game to conclude past as board position matter little compared to
FIDE chess till near the end game. As such, this should be factored into the AI.

Takeaway: Already covered under first AI question.




Q: How/would you change the end game and/or how the game can be concluded?

A: Time runout with piece points determining the winner or classic timed play.

Takeaway: This would allow for competitive gameplay with more positioning thinking without the dread
of taking the game on for several hours.

Takeaway: This was something the user discussed with me later during the design process when playing
a late prototype. We decided not the changed the rules from normal chess and keep the time pressure
as it is part of the fun.




Q: Are you happy with how the pieces move?

A: Yes, the 3d capabilities make it much better than Strada.

Takeaway: The rules should not be changed.

Q: How do you feel about the new piece rules after lots of play?

A: The piece moves feel good and of course coincide with the new rules. It feels like the classic pieces’
rules converted very well and it feel natural the way all the pieces move. It takes some getting used to
but I like it.

Takeaway: User’s views align with testing




441
