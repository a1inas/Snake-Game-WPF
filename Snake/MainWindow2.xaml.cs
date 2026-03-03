using Snake;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Snake
{
    public partial class MainWindow2 : Window
    {
        private MediaPlayer player;
        private void Back(object sender, RoutedEventArgs e)
        {
            
            MenuWindow menuWindow = new MenuWindow(player);

            menuWindow.Left = this.Left;
            menuWindow.Top = this.Top;

            menuWindow.Show();

            Window.GetWindow(this)?.Close();
        }

        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            { GridValue.Empty, Images.Empty },
            { GridValue.Snake, Images.Body },
            { GridValue.Food, Images.Food }
        };

        private readonly Dictionary<GridValue, ImageSource> gridValToImage2 = new()
        {
            { GridValue.Empty, Images.Empty2 },
            { GridValue.Snake, Images.Body2 },
            { GridValue.Food, Images.Food2 }
        };

        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            { Direction.Up, 0 },
            { Direction.Right, 90 },
            { Direction.Down, 180 },
            { Direction.Left, 270 },
        };

        private readonly int rows = 25, cols = 25;
        private readonly Image[,] gridImages;
        private GameState gameState1;
        private GameState gameState2;
        private bool gameRunning;


        public MainWindow2(MediaPlayer player)
        {
            InitializeComponent();
            gridImages = SetupGrid();
            gameState1 = new GameState(rows, cols);
            gameState2 = new GameState(rows, cols);
            this.player = player;
        }

        private async Task RunGame()
        {
            Draw();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;

            gameState2.ChangeDirection(Direction.Up);

            await GameLoop();
            await ShowGameOver();
            gameState1 = new GameState(rows, cols);
            gameState2 = new GameState(rows, cols);
        }

        private async void Windows_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }

            if (!gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }

        private void Windows_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState1.GameOver && gameState2.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    gameState1.ChangeDirection(Direction.Left);
                    break;
                case Key.Right:
                    gameState1.ChangeDirection(Direction.Right);
                    break;
                case Key.Up:
                    gameState1.ChangeDirection(Direction.Up);
                    break;
                case Key.Down:
                    gameState1.ChangeDirection(Direction.Down);
                    break;
            }

            switch (e.Key)
            {
                case Key.A:
                    gameState2.ChangeDirection(Direction.Left);
                    break;
                case Key.D:
                    gameState2.ChangeDirection(Direction.Right);
                    break;
                case Key.W:
                    gameState2.ChangeDirection(Direction.Up);
                    break;
                case Key.S:
                    gameState2.ChangeDirection(Direction.Down);
                    break;
            }
        }

        private bool CheckCollisionWithOtherSnake(Position headPos, GameState otherGameState)
        {
            List<Position> otherSnakePositions = otherGameState.SnakePosition().ToList();

            foreach (Position pos in otherSnakePositions)
            {
                if (pos.Equals(headPos))
                {
                    return true;
                }
            }

            return false;
        }

        private async Task GameLoop()
        {
            while (!gameState1.GameOver || !gameState2.GameOver)
            {
                await Task.Delay(100);
                gameState1.Move();
                gameState2.Move();

                if (CheckCollisionWithOtherSnake(gameState1.HeadPosition(), gameState2))
                {
                    gameState1.GameOver = true;
                }

                if (CheckCollisionWithOtherSnake(gameState2.HeadPosition(), gameState1))
                {
                    gameState2.GameOver = true;
                }

                Draw();
            }
        }

        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            GameGrid.Width = GameGrid.Height * (cols / (double)rows);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image()
                    {
                        Source = Images.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };

                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }

            return images;
        }

        private void Draw()
        {
            DrawGrid();
            DrawSnake(gameState1);
            DrawSnake(gameState2);
            ScoreText.Text = $"ИГРОК 1: {gameState1.Score}  |  ИГРОК 2: {gameState2.Score}";
        }

        private void DrawGrid()
        {
            try
            {
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        GridValue gridVal1 = gameState1.Grid[r, c];
                        GridValue gridVal2 = gameState2.Grid[r, c];

                        if (gridVal1 == GridValue.Food)
                        {
                            gridImages[r, c].Source = gridValToImage[GridValue.Food];
                        }
                        else if (gridVal2 == GridValue.Food)
                        {
                            gridImages[r, c].Source = gridValToImage2[GridValue.Food];
                        }
                        else
                        {
                            GridValue combinedGridVal = gridVal1 | gridVal2;
                            gridImages[r, c].Source = gridValToImage[combinedGridVal];
                        }

                        gridImages[r, c].RenderTransform = Transform.Identity;
                    }
                }
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine("Произошло исключение KeyNotFoundException в методе DrawGrid(): " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошло исключение в методе DrawGrid(): " + ex.Message);
            }
        }

        private void DrawSnake(GameState gameState)
        {
            DrawSnakeBody(gameState);
            DrawSnakeHead(gameState);
        }

        private void DrawSnakeBody(GameState gameState)
        {
            List<Position> positions = new List<Position>(gameState.SnakePosition());

            foreach (Position pos in positions)
            {
                GridValue gridVal = gameState.Grid[pos.Row, pos.Col];
                if (gridVal == GridValue.Snake)
                {
                    gridImages[pos.Row, pos.Col].Source = (gameState == gameState1) ? Images.Body : Images.Body2;
                }
            }
        }

        private void DrawSnakeHead(GameState gameState)
        {
            Position headPos = gameState.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Col];
            image.Source = (gameState == gameState1) ? Images.Head : Images.Head2;

            int rotation = dirToRotation[gameState.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }

        private async Task DrawDeadSnake(GameState gameState)
        {
            List<Position> positions = new List<Position>(gameState.SnakePosition());

            for (int i = 0; i < positions.Count; i++)
            {
                Position pos = positions[i];
                ImageSource source = (i == 0) ? Images.DeadHead : Images.DeadBody;
                if (gameState == gameState2)
                {
                    source = (i == 0) ? Images.DeadHead2 : Images.DeadBody2;
                }
                gridImages[pos.Row, pos.Col].Source = source;
                await Task.Delay(50);
            }
        }

        private async Task ShowCountDown()
        {
            for (int i = 3; i >= 1; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }

        private async Task ShowGameOver()
        {
            await DrawDeadSnake(gameState1);
            await DrawDeadSnake(gameState2);
            await Task.Delay(1000);

            MessageBox.Show("Игра завершена :( \n Игрок 1: " + gameState1.Score + "\n Игрок 2: " + gameState2.Score);
            MenuWindow menuWindow = new MenuWindow(player);
            menuWindow.Show();
            Window.GetWindow(this)?.Close();

            if (gameState1.SnakePosition().Count() == 0)
            {
                gameState1.GameOver = true;
            }

            if (gameState2.SnakePosition().Count() == 0)
            {
                gameState2.GameOver = true;
            }

            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "Нажмите любую клавишу, чтобы начать";
        }

    }

}