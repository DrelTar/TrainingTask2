using System;
using System.IO;
using System.Collections.Generic;

// Bulls and Cows game. Feature scoresboard for each number length, small menu and actuall game. Can manipulate with
// any number length from 1 to 10 any any digit from 1 to 10. Can easily be upgraded to longer numbers and bigger digits.
// Surely have some stuff to improve.

class Program
{
    // Const var which represents maximum possible value of digit + one. [1, )
    const ulong maxValue = 10;
    // Const var which represents maximum possible length of generated number. [minLength, )
    const ulong maxLength = 10;
    // Const var which represents minimum possible length of generated number. [1, )
    const ulong minLength = 1;
    // Var which represents length of generated number. [minLength, maxLength]
    public static ulong lengthOfNumber;
    // Array which contains digits of generated number. [0, maxValue)
    public static ulong[] digitsOfNumber;
    // Array which represents if we use digit(number form 1 to maxValue) in generated number. True - used, False - not used 
    public static bool[] isDigitUsed;

    /// <summary>
    /// Number generation for game
    /// </summary>
    public static void GenerateNumber()
    {
        // Declare rand object
        Random rand = new Random();
        // Create empty digitsOfNumber array
        digitsOfNumber = new ulong[lengthOfNumber];
        // Create new isDigitUsed array
        isDigitUsed = new bool[maxValue];
        // Cicle where we generate our digits
        for (ulong i = 0; i < lengthOfNumber; ++i)
        {
            // Generate new possible digit for our number
            ulong newDigit = (ulong)rand.Next((int)maxValue);
            // Cicle in which we add 1 until newDigit isn't used
            while (isDigitUsed[newDigit])
            {
                // Add 1 and mod maxValue as all digits must be under maxValue
                newDigit = (newDigit + 1) % maxValue;
            }
            // Mark our new digit as taken
            isDigitUsed[newDigit] = true;
            // Add newDigit to digitsOfNumber on the first not used position
            digitsOfNumber[i] = newDigit;
        }
    }

    /// <summary>
    /// Start the game, get length of playble number and write the rules
    /// </summary>
    public static void StartOfGame()
    {
        // Welcoming player anf telling them the rules 
        Console.WriteLine("Welcome to the bulls and cows game.");
        Console.WriteLine("In this game you should guess the number which the computer will generate.");
        Console.Write($"At first you should choose the number from {minLength} to {maxLength}, length of generated number: ");
        // Read player chosen length of number, they can chose any length from minLength to maxLength 
        while (!ulong.TryParse(Console.ReadLine(), out lengthOfNumber) || (lengthOfNumber > maxLength) ||
                (lengthOfNumber < minLength))
        {
            Console.WriteLine("Sorry, that's wrong number. Choose number from 1 to 10");
        }
        // Telling player the rules
        Console.WriteLine("Now, the game starts. But at first we tell you some rules.");
        Console.WriteLine("When you want to guess the number, you shoud type it and press Enter.");
        Console.WriteLine("After that, computer will give you number of 'bulls' and 'cows'.");
        Console.WriteLine("Bulls - digits which stand on correct postions");
        Console.WriteLine("Cows - digits which exist in generated number but stand on wrong positions");
        Console.WriteLine("Remember, all digits of generated number are different and can not be repeated.");
        // Generate number to guess
        GenerateNumber();
    }

    /// <summary>
    /// calculate bulls and cows in player's number 
    /// </summary>
    /// <param name="playerNumber">number that player think is right</param>
    /// <returns>Return ciphered number of bylls and cows. More below</returns>
    public static ulong calucuateBullsAndCows(ulong playerNumber)
    {
        // Array which will contain digits of player's number
        ulong[] digitsOfPlayerNumber = new ulong[lengthOfNumber];
        // Var which represents last not used position in player's array of digits [0, maxLength)
        ulong currentDigit = lengthOfNumber - 1;
        // Array which represents if we use certain digit in player's number. True - used, False - not used 
        bool[] isPlayerDigitUsed = new bool[maxValue];
        //Cicle where we convert player's number from number to array of digits 
        while (playerNumber > 0)
        {
            // Place last digit of player's number on the last not used position in player's array of digits
            digitsOfPlayerNumber[currentDigit] = playerNumber % maxValue;
            // Mark last digit of player's number as used
            isPlayerDigitUsed[playerNumber % maxValue] = true;
            // Remove last digit of player's number 
            playerNumber /= 10;
            // Move currentPosition to the last not used position in player's array of digits
            --currentDigit;
        }
        // Var which represents bulls number. [0, maxLength)
        ulong bulls = 0;
        // Var which represents cows number. [0, maxLength)
        ulong cows = 0;
        // Cicle where we count bullas and cows
        for (ulong i = 0; i < lengthOfNumber; ++i)
        {
            // Incrementing bulls if digit on current position in generated number equals to
            // digit in player's number on the same postion 
            if (digitsOfNumber[i] == digitsOfPlayerNumber[i])
            {
                ++bulls;
            }
            // Incrementing cows if digit on current position in generated number not equal to 
            // digit on in player's number on the same postion, but still exist in player's number
            else if (isPlayerDigitUsed[digitsOfNumber[i]])
            {
                ++cows;
            }
        }
        // Return bulls two times multiply maxValue plus cows as it's easier to return one number than array of two elemnts.
        // To return to normal bulls number you need to do (this number / (maxValue * maxValue)) operation
        // To return to normal cows number you need to do (this number % (maxValue * maxValue)) operation 
        return bulls * (ulong)Math.Pow(maxValue, 2) + cows;
    }

    /// <summary>
    /// Player try to guess the number 
    /// </summary>
    /// <returns>Returns amount of steps which player do to guess the generated number</returns>
    public static ulong GuessNumber()
    {
        // Counter of amount of steps for which player complete the game. [0, )
        ulong steps = 0;
        // Var which represents if game ended
        bool gameOver = false;
        // Cicle wheree player guess generated number and we check if them right
        do
        {   
            // Var which represents number which player try as right number. 
            ulong playerNumber;
            // Var which represents number which player try as right number but in string format. 
            // Exist only to manage zeros at the first postions of number
            string stringPlayerNumber;
            // Cicle where we try to take positive number from player. If player's input isn't positive, isn't number or
            // isn't the right length, ask again
            do
            {
                Console.Write("Enter valid positive guessed number: ");
                // Take number from player
                stringPlayerNumber = Console.ReadLine();
            } while (!ulong.TryParse(stringPlayerNumber, out playerNumber) || (ulong)stringPlayerNumber.Length != lengthOfNumber);
            // Var which represents amount of bulls and cows in strange way of checkNumber returns value
            ulong bullsAndCows = calucuateBullsAndCows(playerNumber);
            // Var which represents bulls number. [0, maxLength) 
            ulong bulls = bullsAndCows / (ulong)Math.Pow(maxValue, 2);
            // Var which represents cows number [0, maxLength)
            ulong cows = bullsAndCows % (ulong)Math.Pow(maxValue, 2);
            // Check if player guess the generated number
            if (bulls == lengthOfNumber)
            {
                // Game ends
                gameOver = true;
            }
            else
            {
                // Write number of bulls and cows
                Console.WriteLine($"Bulls: {bulls} Cows: {cows}");
            }
            // Incrementing steps
            ++steps;
        } while (!gameOver);
        return steps;
    }

    /// <summary>
    /// Congratulations for player and telling them their score
    /// </summary>
    /// <param name="steps"></param>
    public static void EndOfGame(ulong steps)
    {
        // Writing player's score, ending of game and asking for exit or new game
        Console.WriteLine($"Yay! You did it within {steps} steps!");
        Console.WriteLine("Your score is now in scoreboard. You can see it in menu.");
        Console.WriteLine("If you want new try, press any key. If you want to end game and return to menu, press Escape");
        // Var which represents path to currrent scoreboard file
        string path = Directory.GetCurrentDirectory();
        path += "\\scoreboard" + lengthOfNumber.ToString() + ".txt";
        // Check if file exists
        if (File.Exists(path))
        {
            // Resording new line to scoreboard
            using (StreamWriter scoreboard = new StreamWriter(path, true, System.Text.Encoding.Default))
            {
                scoreboard.WriteLine(steps);
            }
        }
        else
        {
            // Creating scoreboard file and recording new line to scoreboard
            using (StreamWriter scoreboard = new StreamWriter(path, true, System.Text.Encoding.Default))
            {
                scoreboard.WriteLine(steps);
            }
        }
    }
    /// <summary>
    /// Actual game, runs all other functions
    /// </summary>
    public static void Game()
    {
        // Cicle where we make all stuff of game playing. Quit if player press Escape after the game 
        do
        {
            // Clear console
            Console.Clear();
            StartOfGame();
            // Actuall game and also taking the amount of player's steps
            ulong steps = GuessNumber();
            EndOfGame(steps);
        } while (Console.ReadKey().Key != ConsoleKey.Escape);
        Menu();
    }
    /// <summary>
    /// Write menu with scoresboard and game functions
    /// </summary>
    public static void Menu()
    {
        // Clear console and making menu
        Console.Clear();
        Console.WriteLine("Welcome to the menu");
        Console.WriteLine($"If you want to see scoreboard press any number from {minLength} to {maxLength}. (0 for 10)");
        Console.WriteLine("If you want to play the game press Enter.");
        Console.WriteLine("If you want to quit press any other key.");
        Console.Write("Your choice: ");
        // Read player's choice
        var pressedKey = Console.ReadKey().Key;
        // Process player's choice
        switch (pressedKey)
        {
            case ConsoleKey.D1:
                Score(1);
                break;
            case ConsoleKey.D2:
                Score(2);
                break;
            case ConsoleKey.D3:
                Score(3);
                break;
            case ConsoleKey.D4:
                Score(4);
                break;
            case ConsoleKey.D5:
                Score(5);
                break;
            case ConsoleKey.D6:
                Score(6);
                break;
            case ConsoleKey.D7:
                Score(7);
                break;
            case ConsoleKey.D8:
                Score(8);
                break;
            case ConsoleKey.D9:
                Score(9);
                break;
            case ConsoleKey.D0:
                Score(10);
                break;
            case ConsoleKey.Enter:
                Game();
                break;
            default:
                return;
        }
    }
    /// <summary>
    /// Write score for chosen scoresboard
    /// </summary>
    /// <param name="length">represent which scoresboard should we use</param>
    public static void Score(ulong length)
    {
        // Clear console
        Console.Clear();
        Console.WriteLine($"Scoreboard for {length} length:");
        // Var which represents path to current scoreboard file
        string path = Directory.GetCurrentDirectory();
        path += "\\scoreboard" + length.ToString() + ".txt";
        // List which represents all scores.
        List<ulong> top = new List<ulong>();
        // Check if scores exist
        if (File.Exists(path))
        {
            // Read all scores from scoresboard file
            using (StreamReader scoreboard = new StreamReader(path, System.Text.Encoding.Default))
            {
                // Var which represents Score
                ulong line;
                // Cicle where we read all scores
                while (ulong.TryParse(scoreboard.ReadLine(), out line))
                {
                    // Adding new score to list of scores
                    top.Add(line);
                }
            }
            // Sorting all scores
            top.Sort();
            // Cicle where we write best 20 or less scores
            for (ulong i = 1; i <= (ulong)Math.Min(top.Count, 20); ++i)
            {
                Console.WriteLine($"{i} - {top[(int)(i - 1)]}");
            }
            //If no scores write about it
            if (top.Count == 0)
            {
                Console.WriteLine("Sorry, no scores yet");
            }
        }
        else
        {
            Console.WriteLine("Sorry, no scores yet");
        }
        // Asking to quit to menu
        Console.Write("Press Escape to return to main menu.");
        // Quiting only on Escape 
        while (Console.ReadKey().Key != ConsoleKey.Escape)
        {
            continue;
        }
        // Returning to menu
        Menu();
    }
    /// <structure>
    /// Run Game() function
    /// </structure>
    static void Main()
    {
        Menu();
    }
}
