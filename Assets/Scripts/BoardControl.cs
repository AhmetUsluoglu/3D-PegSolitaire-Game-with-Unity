using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class BoardControl : MonoBehaviour
{
    // Fields for peg information and Move.
    private int row;
    private int column;
    public static int selectedBoard;
    private bool gameEndFlag = false;

    static string path;
    public Text ScoreText;

    // Fields for playing with mouse click
    private int initialRow, initialColumn, lastRow, lastColumn;
    private float mZCoord;

    private char[,] Board;
    Peg[,] pegs;
    Peg selectedPeg;

    Vector2 mouseLocation;
    public GameObject myPeg;
    public GameObject mySlot;
    // Audio Elements
    public AudioSource sounds;
    public AudioClip clickSound;
    public AudioClip releaseSound;

    private void Awake()
    {
        path = Application.persistentDataPath + "/player.txt";
    }
    void Start()
    {
        Initialize();
    }

    private void Update()
    {
        // if first peg is not chosen
        if (selectedPeg == null)
        {
            // if mouse clicked
            if (Input.GetMouseButtonDown(0))
            {
                // Gets mouse position
                MousePosition();
                initialRow = (int)mouseLocation.x;
                initialColumn = (int)mouseLocation.y;
                // Checks the mouse position
                if (!(initialRow < 0 || initialRow >= row || initialColumn < 0 || initialColumn >= column))
                    selectedPeg = pegs[initialRow, initialColumn];

                mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z + 1f;
                // If it's peg plays sound
                if (selectedPeg != null)
                {
                    sounds.PlayOneShot(clickSound);
                }
            }
        }
        else
        {
            // after peg is chosen, if mouse is holded peg follows mouse location
            if (Input.GetMouseButton(0))
            {
                selectedPeg.transform.position = GetMouseAsWorldPoint();
            }

            // When the mouse is released
            if (Input.GetMouseButtonUp(0))
            {
                // Gets mouse position
                MousePosition();

                lastRow = (int)mouseLocation.x;
                lastColumn = (int)mouseLocation.y;
                selectedPeg = pegs[initialRow, initialColumn];

                // Checks the last position if it's empty and avaliable
                if (Check(initialRow, initialColumn, lastRow, lastColumn)) Move();
                else //If Second Point is out of the bounds of the game board or placed on an invalid location.
                {
                    PutPegAt(selectedPeg, initialRow, initialColumn);
                    selectedPeg = null;
                    return;
                }
            }
        }
    }

    // Finds current mouse position
    void MousePosition()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            mouseLocation.y = (int)(hit.point.x + 0.5f);
            mouseLocation.x = (int)(hit.point.z + 0.5f);
        }
    }

    // Gets mouse point from screen to world
    private Vector3 GetMouseAsWorldPoint()
    {
        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;
        // z coordinate of game object on screen
        mousePoint.z = mZCoord;
        // Convert it to world points
        Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(mousePoint);
        mouseWorldPoint.y = 0.52f;
        return mouseWorldPoint;
    }

    // Checks if the move is valid
    private bool Check(int row1, int column1, int row2, int column2)
    {
        if (row2 < 0 || row2 >= row || column2 < 0 || column2 >= column) return false;

        if (row1 == row2)
        {
            int dis = Mathf.Abs(column1 - column2);

            if (dis == 2 && Board[row1, column1] == 'P' && Board[row1, (column1 + column2) / 2] == 'P' && Board[row2, column2] == '\0')
                return true;
        }
        else if (column1 == column2)
        {
            int dis = Mathf.Abs(row1 - row2);

            if (dis == 2 && Board[row1, column1] == 'P' && Board[(row1 + row2) / 2, column1] == 'P' && Board[row2, column2] == '\0')
                return true;
        }
        return false;
    }

    // Makes the move and updates the score
    private void Move()
    {
        if (initialRow == lastRow)
        {
            Destroy(pegs[initialRow, (initialColumn + lastColumn) / 2].gameObject);
            pegs[initialRow, (initialColumn + lastColumn) / 2] = null;
            Board[initialRow, (initialColumn + lastColumn) / 2] = '\0';
        }
        else
        {
            Destroy(pegs[(initialRow + lastRow) / 2, initialColumn].gameObject);
            pegs[(initialRow + lastRow) / 2, initialColumn] = null;
            Board[(initialRow + lastRow) / 2, initialColumn] = '\0';
        }

        pegs[lastRow, lastColumn] = selectedPeg;
        Board[lastRow, lastColumn] = 'P';

        Board[initialRow, initialColumn] = '\0';
        pegs[initialRow, initialColumn] = null;


        PutPegAt(selectedPeg, lastRow, lastColumn);
        selectedPeg = null;
        sounds.PlayOneShot(releaseSound);
        ScoreText.text = PegCount().ToString();

        if (EndGame())
        {
            gameEndFlag = true;
        }
    }

    // Initializes the game board
    private void Initialize()
    {

        switch (selectedBoard)
        {
            case 1:
                row = column = 7;
                Board = new char[7, 7]
                {
                    {'E','E','P','P','P','E','E'},
                    {'E','E','P','P','P','E','E'},
                    {'P','P','P','P','P','P','P'},
                    {'P','P','P','\0','P','P','P'},
                    {'P','P','P','P','P','P','P'},
                    {'E','E','P','P','P','E','E'},
                    {'E','E','P','P','P','E','E'}
                };
                break;

            case 2:
                row = column = 7;
                Board = new char[7, 7]
                {
                    {'E','E','P','P','P','E','E'},
                    {'E','P','P','P','P','P','E'},
                    {'P','P','P','P','P','P','P'},
                    {'P','P','P','\0','P','P','P'},
                    {'P','P','P','P','P','P','P'},
                    {'E','P','P','P','P','P','E'},
                    {'E','E','P','P','P','E','E'}
                };
                break;

            case 3:
                row = column = 8;
                Board = new char[8, 8]
                {
                    {'E','E','P','P','P','E','E','E'},
                    {'E','E','P','P','P','E','E','E'},
                    {'P','P','P','P','P','P','P','P'},
                    {'P','P','P','\0','P','P','P','P'},
                    {'P','P','P','P','P','P','P','P'},
                    {'E','E','P','P','P','E','E','E'},
                    {'E','E','P','P','P','E','E','E'},
                    {'E','E','P','P','P','E','E','E'},
                };
                break;

            case 4:
                row = column = 9;
                Board = new char[9, 9]
                {
                    {'E','E','E','P','P','P','E','E','E'},
                    {'E','E','E','P','P','P','E','E','E'},
                    {'E','E','E','P','P','P','E','E','E'},
                    {'P','P','P','P','P','P','P','P','P'},
                    {'P','P','P','P','\0','P','P','P','P'},
                    {'P','P','P','P','P','P','P','P','P'},
                    {'E','E','E','P','P','P','E','E','E'},
                    {'E','E','E','P','P','P','E','E','E'},
                    {'E','E','E','P','P','P','E','E','E'},

                };
                break;

            case 5:
                row = column = 9;
                Board = new char[9, 9]
                {
                    {'E','E','E','E','P','E','E','E','E'},
                    {'E','E','E','P','P','P','E','E','E'},
                    {'E','E','P','P','P','P','P','E','E'},
                    {'E','P','P','P','P','P','P','P','E'},
                    {'P','P','P','P','\0','P','P','P','P'},
                    {'E','P','P','P','P','P','P','P','E'},
                    {'E','E','P','P','P','P','P','E','E'},
                    {'E','E','E','P','P','P','E','E','E'},
                    {'E','E','E','E','P','E','E','E','E'},

                };
                break;

            default:
                break;
        }


        pegs = new Peg[row, column];

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (Board[i, j] == 'P')
                {
                    CreatePeg(i, j);
                    CreateSlot(i, j);
                }
                else if (Board[i, j] == '\0') CreateSlot(i, j);
            }
        }
        ScoreText.text = PegCount().ToString();
    }

    // Creates peg for the given location
    private void CreatePeg(int i, int j)
    {
        GameObject go = Instantiate(myPeg) as GameObject;
        Peg peg = go.GetComponent<Peg>();
        pegs[i, j] = peg;
        PutPegAt(peg, i, j);
    }

    // Creates slot for given location
    private void CreateSlot(int i, int j)
    {
        GameObject go = Instantiate(mySlot) as GameObject;
        slot Slot = go.GetComponent<slot>();
        Slot.transform.position = (Vector3.forward * i) + (Vector3.right * j);

    }

    // Puts the peg at the given location
    private void PutPegAt(Peg current, int i, int j)
    {
        current.transform.position = (Vector3.forward * i) + (Vector3.right * j) + (Vector3.up * 0.52f);
        Board[i, j] = 'P';
    }

    // Checks if the game is ended
    bool EndGame()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (Check(i, j, i + 2, j) || Check(i, j, i, j + 2) || Check(i, j, i - 2, j) || Check(i, j, i, j - 2))
                {
                    gameEndFlag = false;
                    return false;
                }
            }
        }
        gameEndFlag = true;
        return true;
    }

    // Counts the remaining pegs
    int PegCount()
    {
        int Count = 0;

        for (int i = 0; i < row; i++)
            for (int j = 0; j < column; j++)
                if (Board[i, j] == 'P')
                    Count++;

        return Count;
    }

    // Plays one move automatically
    public void PlayAuto()
    {
        int[] directions = new int[5] { 0, 1, 2, 3, 4 };
        int rand;

        while (true)
        {
            if (gameEndFlag) return;
            int i = (int)Random.Range(0f, (float)row - 0.1f);
            int j = (int)Random.Range(0f, (float)row - 0.1f);

            rand = (int)Random.Range(1f, 4.99f);
            while (directions[rand] != 0)
            {
                switch (rand)
                {
                    case 1:
                        directions[1] = 0;
                        if (Check(i, j, i + 2, j))
                        {
                            initialRow = i; initialColumn = j; lastRow = i + 2; lastColumn = j;
                            selectedPeg = pegs[initialRow, initialColumn];
                            Move();
                            return;
                        }
                        break;

                    case 2:
                        directions[2] = 0;
                        if (Check(i, j, i, j + 2))
                        {
                            initialRow = i; initialColumn = j; lastRow = i; lastColumn = j + 2;
                            selectedPeg = pegs[initialRow, initialColumn];
                            Move();
                            return;
                        }
                        break;

                    case 3:
                        directions[3] = 0;
                        if (Check(i, j, i - 2, j))
                        {
                            initialRow = i; initialColumn = j; lastRow = i - 2; lastColumn = j;
                            selectedPeg = pegs[initialRow, initialColumn];
                            Move();
                            return;
                        }
                        break;

                    case 4:
                        directions[4] = 0;
                        if (Check(i, j, i, j - 2))
                        {
                            initialRow = i; initialColumn = j; lastRow = i; lastColumn = j - 2;
                            selectedPeg = pegs[initialRow, initialColumn];
                            Move();
                            return;
                        }
                        break;
                }
                rand = (int)Random.Range(1f, 4.99f);
            }
            directions[1] = 1; directions[2] = 2; directions[3] = 3; directions[4] = 4;
        }
    }

    // Resets the board
    public void reset()
    {
        SceneManager.LoadScene("Game");
    }

    // Plays autoamtically until the end
    public void PlayAll()
    {
        while (!gameEndFlag)
        {
            PlayAuto();
        }
    }

    // Loads menu
    public void LoadHome()
    {
        SceneManager.LoadScene("Menu");
    }

    // Saves the last game when clicked on Save Game button.
    public void SaveGame()
    {
        StreamWriter writer = new StreamWriter(path);
        string temp = "";

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                temp += Board[i, j];
            }
            temp += "\n";
        }
        temp += selectedBoard;
        temp += row;
        writer.Write(temp);
        writer.Close();

    }

    // Loads the last saved game
    public void LoadGame()
    {
        int size = 0;
        string temp = File.ReadAllText(path);

        size = temp[temp.Length - 1] - '0';
        row = column = size;
        selectedBoard = temp[temp.Length - 2] - '0';

        char[,] newBoard = new char[size, size];
        int k = 0;
        for (int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                if (temp[k] != '\n')
                {
                    newBoard[i, j] = temp[k];
                    k++;
                }
                else if (temp[k] == '\n')
                {
                    k++; j--;
                }
            }
        }

        foreach (GameObject o in GameObject.FindGameObjectsWithTag("pegs")) Destroy(o);
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("slots")) Destroy(o);

        Board = new char[size, size];
        Board = newBoard;
        
        pegs = new Peg[row, column];

        for (int i = 0; i <size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (Board[i, j] == 'P')
                {
                    CreatePeg(i, j);
                    CreateSlot(i, j);
                }
                else if (Board[i, j] == '\0') CreateSlot(i, j);
            }
        }
        ScoreText.text = PegCount().ToString();
    }

    // Closes the application
    public void QuitonClick()
    {
        Application.Quit();
    }
}




