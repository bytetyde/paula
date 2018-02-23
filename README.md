# paula
Procedural and unique level arrangement (PAULA) is a procedural level generator for
the game engine Unity3D. Procedural content generation gives the ability to create
game contents through algorithms, which helps to reduce development time and
enables new possibilities in games. However only a few procedural generated
platformer levels exist. The generator PAULA, which was developed as part of this work,
offers the possibility to arrange two- and three-dimensional level blocks to complete
platformer levels. Besides the arrangement of the level blocks, PAULA offers the
possibility to fill generated levels with additional game contents. The levels can be
arranged vertically as well as horizontally, whereby a wide range of games is made
possible. With PAULA, a fast development of games is possible, which furthermore have
a great replayability through the different generated levels. 

## How it works
Paula uses a text replacement algorithm to create a string representation of a level based on a used provided replacement grammar.
This grammar give the ability to structure levels and still use randomization in a controlled way.
As a user you provide a grammar on how your level should be structured and some basic building blocks which can later be used to instantiate the level.
The generator flow looks something like this:
Grammar -> Text Replacement -> Level Instatiation baseed on the result string -> Level Population with asset based on a second grammar

## Generator Settings Example for the Game Craft
![Generator Settings](https://drive.google.com/open?id=16L3oXX8TtSZmcJlkwA3MyC7V1JSsTfGW)
![Generator grammar and axiom](https://drive.google.com/open?id=1h5kQhyo9cYZGkwP_Wv6WxQvrsL69mU7X)
![Text replacer result and instatiation](https://drive.google.com/open?id=1ViQDtgtPfZ-w7xysbEOCET1_u-8ypRWx)
![Level pupulator](https://drive.google.com/open?id=1nh22053dtxg78-3nHmLogAOr5Z5PMYER)
