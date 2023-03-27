# Robot Navigation Problem


Author: Kai Siang Kao

Student ID: 103819380

Email: 103819380@student.swin.edu.au


# WARNING
If this repository becomes public before the due date of this assignment (21st April 2023), please contact me IMMEDIATELY via the email address above.

## Introduction
This C# repository is for the individual assignment for the subject COS30019 - Introduction to Artificial Intelligence. More specifically, Option B which solves the Robot Navigation Problem. The goal of this problem is to find a solution to navigate the robot in a NxM grid environment. The environment will contain two goal nodes, which the robot has to navigate to one of them using a path finding algorithm (can be informed or uninformed) while avoiding obstacles.

The published executable can be found in `/bin/Release/net6.0-windows10.0.17763.0/publish`.

## Instruction
To run the program:
1. Open the terminal in the folder path specified above.
1. Execute the command `search <filename> <method>`. This repository has already contain a sample `test.txt` file that contains the environment configuration. Alternatively, you can provide your own custom environment configuration in a `.txt` file.
1. There are only 7 options `<method>`, `dfs`, `bfs`, `gbfs`, `as`, `cus1`, `cus2`, and `gui`. The first six will run the program in CLI mode, which will output the path using the search algorithm specified (`cus1` and `cus2` are custom search algorithms). The `gui` option will run the program in GUI mode.
