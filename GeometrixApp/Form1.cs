using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GeometrixApp
{
    public partial class Form1 : Form
    {
        // Parametry planszy
        private const int GridWidth = 20;  // H
        private const int GridHeight = 20; // V
        private const int CellSize = 40;   // 40px

        // Stan gry
        private int[,] boardState = new int[GridWidth, GridHeight]; // 0 = puste, 1 = zajête
        private System.Windows.Forms.Timer gameTimer;
        private Random random = new Random();

        private Token currentToken;
        private int currentX;
        private int currentY;
        private int totalPlacedTokens = 0;
        private bool isRunning = false;

        // Kontrolki UI
        private PictureBox boardCanvas;
        private Button btnStart;
        private Button btnStop;
        private TextBox txtCount;
        private Label lblCount;

        public Form1()
        {
            InitializeComponent();
            SetupFormAndControls();
            InitGameTimer();
        }

        private void SetupFormAndControls()
        {
            // Konfiguracja Formy i w³¹czenie AutoScroll
            this.Text = "Geometrix";
            this.ClientSize = new Size(1100, 880);
            this.AutoScroll = true;
            this.KeyPreview = true; // Przechwytywanie klawiatury
            this.KeyDown += Form1_KeyDown;

            // Tworzenie planszy (20x20 * 40px = 800x800 px)
            boardCanvas = new PictureBox
            {
                Size = new Size(GridWidth * CellSize + 1, GridHeight * CellSize + 1),
                Location = new Point(10, 10),
                BackColor = Color.BurlyWood // Jasno-br¹zowy kolor planszy
            };
            boardCanvas.Paint += BoardCanvas_Paint;
            this.Controls.Add(boardCanvas);

            // Panel steruj¹cy po prawej stronie planszy
            int panelX = boardCanvas.Right + 20;

            btnStart = new Button
            {
                Text = "Start",
                Location = new Point(panelX, 20),
                Size = new Size(100, 35)
            };
            btnStart.Click += BtnStart_Click;
            this.Controls.Add(btnStart);

            btnStop = new Button
            {
                Text = "Stop",
                Location = new Point(panelX, 65),
                Size = new Size(100, 35),
                Enabled = false
            };
            btnStop.Click += BtnStop_Click;
            this.Controls.Add(btnStop);

            lblCount = new Label
            {
                Text = "Iloœæ:",
                Location = new Point(panelX, 115),
                AutoSize = true
            };
            this.Controls.Add(lblCount);

            txtCount = new TextBox
            {
                Text = "0",
                Location = new Point(panelX, 135),
                Size = new Size(100, 25),
                ReadOnly = true
            };
            this.Controls.Add(txtCount);
        }

        private void InitGameTimer()
        {
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 400; // Czas opadania ¿etonu (ms)
            gameTimer.Tick += GameTimer_Tick;
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            ResetGame();
            isRunning = true;
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            SpawnNewToken();
            gameTimer.Start();
            this.Focus();
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            StopGame();
        }

        private void StopGame()
        {
            isRunning = false;
            gameTimer.Stop();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void ResetGame()
        {
            Array.Clear(boardState, 0, boardState.Length);
            totalPlacedTokens = 0;
            txtCount.Text = "0";
            currentToken = null;
            boardCanvas.Invalidate();
        }

        private void SpawnNewToken()
        {
            currentToken = TokenRepository.GetRandomToken(random);
            currentX = (GridWidth - currentToken.Width) / 2;
            // Pozycja pocz¹tkowa tu¿ nad otwart¹ górn¹ krawêdzi¹ planszy
            currentY = -currentToken.Height;
        }

        // Pêtla animacji (jednostajny ruch w dó³)
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (!isRunning || currentToken == null) return;

            if (!CheckCollision(currentX, currentY + 1, currentToken.Shape))
            {
                currentY++;
            }
            else
            {
                // Dotkniêcie dna lub innego ¿etonu
                PlaceTokenAndCheckGameOver();
            }
            boardCanvas.Invalidate();
        }

        private void PlaceTokenAndCheckGameOver()
        {
            bool isGameOver = false;

            for (int r = 0; r < currentToken.Height; r++)
            {
                for (int c = 0; c < currentToken.Width; c++)
                {
                    if (currentToken.Shape[r, c] == 1)
                    {
                        int targetX = currentX + c;
                        int targetY = currentY + r;

                        // Gra koñczy siê gdy choæ jeden ¿eton wystaje ponad górn¹ krawêdŸ planszy (Y < 0)
                        if (targetY < 0)
                        {
                            isGameOver = true;
                        }
                        else if (targetX >= 0 && targetX < GridWidth && targetY < GridHeight)
                        {
                            boardState[targetX, targetY] = 1;
                        }
                    }
                }
            }

            totalPlacedTokens++;
            txtCount.Text = totalPlacedTokens.ToString();

            if (isGameOver)
            {
                StopGame();
                MessageBox.Show("Koniec gry! ¯eton wystaje ponad górn¹ krawêdŸ planszy.", "Geometrix");
            }
            else
            {
                SpawnNewToken();
            }
        }

        private bool CheckCollision(int nx, int ny, int[,] shape)
        {
            int h = shape.GetLength(0);
            int w = shape.GetLength(1);

            for (int r = 0; r < h; r++)
            {
                for (int c = 0; c < w; c++)
                {
                    if (shape[r, c] == 1)
                    {
                        int targetX = nx + c;
                        int targetY = ny + r;

                        // Krawêdzie lewa i prawa
                        if (targetX < 0 || targetX >= GridWidth)
                            return true;

                        // KrawêdŸ dolna
                        if (targetY >= GridHeight)
                            return true;

                        // Kolizja z innymi u³o¿onymi ¿etonami
                        if (targetY >= 0 && boardState[targetX, targetY] == 1)
                            return true;
                    }
                }
            }
            return false;
        }

        // Sterowanie klawiatur¹ z now¹ logik¹ obrotów
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!isRunning || currentToken == null) return;

            if (e.KeyCode == Keys.Left)
            {
                if (!CheckCollision(currentX - 1, currentY, currentToken.Shape))
                {
                    currentX--;
                    boardCanvas.Invalidate();
                }
            }
            else if (e.KeyCode == Keys.Right)
            {
                if (!CheckCollision(currentX + 1, currentY, currentToken.Shape))
                {
                    currentX++;
                    boardCanvas.Invalidate();
                }
            }
            else if (e.KeyCode == Keys.Up)
            {
                // Obrót o 90 stopni W PRAWO
                int[,] rotated = currentToken.GetRotatedRight();
                if (!CheckCollision(currentX, currentY, rotated))
                {
                    currentToken.Shape = rotated;
                    boardCanvas.Invalidate();
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                // Obrót o 90 stopni W LEWO
                int[,] rotated = currentToken.GetRotatedLeft();
                if (!CheckCollision(currentX, currentY, rotated))
                {
                    currentToken.Shape = rotated;
                    boardCanvas.Invalidate();
                }
            }
        }

        // Rysowanie planszy, siatki, krawêdzi oraz ¿etonów
        private void BoardCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 1. Przerywana siatka planszy
            using (Pen dashPen = new Pen(Color.SaddleBrown, 1))
            {
                dashPen.DashStyle = DashStyle.Dash;

                for (int x = 0; x <= GridWidth; x++)
                {
                    g.DrawLine(dashPen, x * CellSize, 0, x * CellSize, GridHeight * CellSize);
                }
                for (int y = 0; y <= GridHeight; y++)
                {
                    g.DrawLine(dashPen, 0, y * CellSize, GridWidth * CellSize, y * CellSize);
                }
            }

            // 2. Krawêdzie zamykaj¹ce: Lewa, Prawa, Dó³ (Górna otwarta)
            using (Pen borderPen = new Pen(Color.DarkSlateGray, 4))
            {
                int w = GridWidth * CellSize;
                int h = GridHeight * CellSize;

                g.DrawLine(borderPen, 0, 0, 0, h); // Lewa
                g.DrawLine(borderPen, w, 0, w, h); // Prawa
                g.DrawLine(borderPen, 0, h, w, h); // Dó³
            }

            // 3. Rysowanie osadzonych ¿etonów
            using (Brush placedBrush = new SolidBrush(Color.DarkKhaki))
            using (Pen borderPen = new Pen(Color.Black, 1))
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    for (int y = 0; y < GridHeight; y++)
                    {
                        if (boardState[x, y] == 1)
                        {
                            Rectangle rect = new Rectangle(x * CellSize, y * CellSize, CellSize, CellSize);
                            g.FillRectangle(placedBrush, rect);
                            g.DrawRectangle(borderPen, rect);
                        }
                    }
                }
            }

            // 4. Rysowanie aktualnie opadaj¹cego ¿etonu
            if (currentToken != null)
            {
                using (Brush activeBrush = new SolidBrush(currentToken.Color))
                using (Pen borderPen = new Pen(Color.Black, 1))
                {
                    for (int r = 0; r < currentToken.Height; r++)
                    {
                        for (int c = 0; c < currentToken.Width; c++)
                        {
                            if (currentToken.Shape[r, c] == 1)
                            {
                                int px = (currentX + c) * CellSize;
                                int py = (currentY + r) * CellSize;

                                // Rysujemy tylko tê czêœæ ¿etonu, która znajduje siê wewn¹trz widocznej planszy (Y >= 0)
                                if (py >= 0)
                                {
                                    Rectangle rect = new Rectangle(px, py, CellSize, CellSize);
                                    g.FillRectangle(activeBrush, rect);
                                    g.DrawRectangle(borderPen, rect);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    #region Repozytorium Obiektów Geometrycznych (¯etonów)

    public class Token
    {
        public int[,] Shape { get; set; }
        public Color Color { get; set; }

        public int Width => Shape.GetLength(1);
        public int Height => Shape.GetLength(0);

        public Token(int[,] shape, Color color)
        {
            Shape = shape;
            Color = color;
        }

        // Obrót o 90 stopni w prawo (Strza³ka w górê)
        public int[,] GetRotatedRight()
        {
            int h = Height;
            int w = Width;
            int[,] rotated = new int[w, h];

            for (int r = 0; r < h; r++)
            {
                for (int c = 0; c < w; c++)
                {
                    rotated[c, h - 1 - r] = Shape[r, c];
                }
            }
            return rotated;
        }

        // Obrót o 90 stopni w lewo (Strza³ka w dó³)
        public int[,] GetRotatedLeft()
        {
            int h = Height;
            int w = Width;
            int[,] rotated = new int[w, h];

            for (int r = 0; r < h; r++)
            {
                for (int c = 0; c < w; c++)
                {
                    rotated[w - 1 - c, r] = Shape[r, c];
                }
            }
            return rotated;
        }
    }

    public static class TokenRepository
    {
        public static Token GetRandomToken(Random rand)
        {
            int type = rand.Next(0, 5);

            switch (type)
            {
                case 0:
                    // Kwadrat (2x2)
                    return new Token(new int[,] {
                        { 1, 1 },
                        { 1, 1 }
                    }, Color.Crimson);

                case 1:
                    // Prostok¹t 2 x 1
                    return new Token(new int[,] {
                        { 1, 1 }
                    }, Color.MediumBlue);

                case 2:
                    // Prostok¹t 3 x 1
                    return new Token(new int[,] {
                        { 1, 1, 1 }
                    }, Color.ForestGreen);

                case 3:
                    // Prostok¹t 2 x 3
                    return new Token(new int[,] {
                        { 1, 1 },
                        { 1, 1 },
                        { 1, 1 }
                    }, Color.DarkOrange);

                case 4:
                    // Prostok¹t 2 x 3 z usuniêtymi przeciwleg³ymi kwadratami
                    return new Token(new int[,] {
                        { 1, 0 },
                        { 1, 1 },
                        { 0, 1 }
                    }, Color.Purple);

                default:
                    return new Token(new int[,] { { 1 } }, Color.Black);
            }
        }
    }

    #endregion
}