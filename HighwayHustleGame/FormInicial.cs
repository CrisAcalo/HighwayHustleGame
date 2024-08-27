using HighwayHustleGame.Components;
using HighwayHustleGame.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HighwayHustleGame
{
    public partial class FormInicial : Form
    {
        private BufferedPanel gamePanel;
        private Road road;
        private Timer gameTimer;
        private Timer levelTimer;
        private HashSet<Keys> keysPressed;
        private List<Car> enemyCars;
        private Random random;
        private Car car;
        private int laneCount = 3;
        private int[] lanePositions;
        private string modoJuego;
        private int nivel;
        private int tiempoRestante;
        private float velMinEnemigos = 1;
        private float velMaxEnemigos = 5;
        private Label lblStatus; // Nuevo Label para mostrar el nivel o puntuación
        private int puntuacion=0;

        public string ModoJuego { get => modoJuego; set => modoJuego = value; }

        public FormInicial(string modoJuego, int nivel)
        {
            InitializeComponent();

            this.modoJuego = modoJuego;
            this.nivel = nivel;

            // Configurar el gamePanel
            gamePanel = new BufferedPanel();
            gamePanel.Dock = DockStyle.Fill;
            this.Controls.Add(gamePanel);
            gamePanel.Paint += new PaintEventHandler(GamePanel_Paint);


            // Inicializar otros componentes
            int width = gamePanel.Width;
            int height = gamePanel.Height;
            road = new Road(level: nivel, initialSpeed: 5.0f, width: width, height: height);

            Image carImage = Image.FromFile("G:\\My Drive\\D-ESPE\\V\\GRAFICA\\TERCER PARCIAL\\HighwayHustleGame\\HighwayHustleGame\\images\\Car1.png");
            car = new Car(1, carImage, new Point(width / 2 - 74 / 2, height - 110), 7.0f, width, height, 60, 100);
            gamePanel.Controls.Add(car);

            enemyCars = new List<Car>();
            random = new Random();
            lanePositions = new int[laneCount];
            CalculateLanePositions();

            gameTimer = new Timer();
            gameTimer.Interval = 16;
            gameTimer.Tick += new EventHandler(GameTimer_Tick);
            gameTimer.Start();

            keysPressed = new HashSet<Keys>();

            // Configurar el Label
            lblStatus = new Label();
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = Color.White;
            lblStatus.BackColor = Color.Transparent;
            lblStatus.Font = new Font("Arial", 14, FontStyle.Bold);
            lblStatus.Location = new Point(this.ClientSize.Width - 150, 30); // Posición en la esquina superior derecha
            lblStatus.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            gamePanel.Controls.Add(lblStatus);

            if (modoJuego == "Historia")
            {
                tiempoRestante = nivel * 60;
                road.IncreaseSpeed(nivel / 2);
                velMinEnemigos += nivel;
                velMaxEnemigos += nivel;

                levelTimer = new Timer();
                levelTimer.Interval = 1000;
                levelTimer.Tick += new EventHandler(LevelTimer_Tick);
                levelTimer.Start();

                lblStatus.Text = $"Nivel {nivel}";
            }
            else if (modoJuego == "Desafio")
            {
                tiempoRestante = int.MaxValue;
                levelTimer = new Timer();
                levelTimer.Interval = 5000;
                levelTimer.Tick += new EventHandler(ChallengeModeTimer_Tick);
                levelTimer.Start();

                puntuacion = 0;
                lblStatus.Text = $"Puntuación: {puntuacion}";
            }
        }

        private void GamePanel_Paint(object sender, PaintEventArgs e)
        {
            road.Draw(e.Graphics);
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            road.Update();
            MoveCar();
            MoveEnemyCars();
            RemoveOffscreenEnemyCars();
            SpawnEnemyCarsPeriodically();
            CheckCollisions();
            gamePanel.Invalidate();
        }

        private void LevelTimer_Tick(object sender, EventArgs e)
        {
            tiempoRestante--;

            if (modoJuego == "Historia")
            {
                if (tiempoRestante <= 0)
                {
                    levelTimer.Stop();
                    gameTimer.Stop();
                    MessageBox.Show($"¡Has superado el nivel {nivel}!");
                    this.Close();
                }
            }
        }

        private void ChallengeModeTimer_Tick(object sender, EventArgs e)
        {
            // Aumentar la velocidad cada 60 segundos en el modo "Desafio"
            road.IncreaseSpeed(0.1f); // Aumenta la velocidad de los coches enemigos
            velMinEnemigos += 0.1f;
            velMaxEnemigos += 0.1f;

            if (modoJuego == "Desafio")
            {
                puntuacion++;
                lblStatus.Text = $"Puntuación: {puntuacion}";
            }
        }

        private void FormInicial_KeyDown(object sender, KeyEventArgs e)
        {
            keysPressed.Add(e.KeyCode);
        }

        private void FormInicial_KeyUp(object sender, KeyEventArgs e)
        {
            keysPressed.Remove(e.KeyCode);
        }

        private void MoveCar()
        {
            if (keysPressed.Contains(Keys.Up) && keysPressed.Contains(Keys.Right))
            {
                car.Move(Direction.UpRight);
            }
            else if (keysPressed.Contains(Keys.Up) && keysPressed.Contains(Keys.Left))
            {
                car.Move(Direction.UpLeft);
            }
            else if (keysPressed.Contains(Keys.Down) && keysPressed.Contains(Keys.Right))
            {
                car.Move(Direction.DownRight);
            }
            else if (keysPressed.Contains(Keys.Down) && keysPressed.Contains(Keys.Left))
            {
                car.Move(Direction.DownLeft);
            }
            else if (keysPressed.Contains(Keys.Up))
            {
                car.Move(Direction.Up);
            }
            else if (keysPressed.Contains(Keys.Down))
            {
                car.Move(Direction.Down);
            }
            else if (keysPressed.Contains(Keys.Left))
            {
                car.Move(Direction.Left);
            }
            else if (keysPressed.Contains(Keys.Right))
            {
                car.Move(Direction.Right);
            }
        }

        private void CalculateLanePositions()
        {
            int laneWidth = gamePanel.Width / laneCount;
            for (int i = 0; i < laneCount; i++)
            {
                lanePositions[i] = laneWidth * i + (laneWidth / 2) - car.Width / 2;
            }
        }

        private void SpawnEnemyCarsPeriodically()
        {
            if (enemyCars.Count < 5)
            {
                int laneIndex = random.Next(0, laneCount);
                int laneX = lanePositions[laneIndex];

                bool isLaneOccupied = enemyCars.Any(car =>
                    Math.Abs(car.Left - laneX) < car.Width && car.Top < car.Height * 2);

                if (!isLaneOccupied)
                {
                    Image enemyCarImage = GetRandomCarImage();
                    int y = -enemyCarImage.Height;
                    Car enemyCar = new Car(2, enemyCarImage, new Point(laneX, y), random.Next((int)velMinEnemigos, (int)velMaxEnemigos), gamePanel.Width, gamePanel.Height, 60, 100);
                    enemyCars.Add(enemyCar);
                    gamePanel.Controls.Add(enemyCar);
                }
            }
        }

        private Image GetRandomCarImage()
        {
            string[] carImages = new string[]
            {
                "G:\\My Drive\\D-ESPE\\V\\GRAFICA\\TERCER PARCIAL\\HighwayHustleGame\\HighwayHustleGame\\images\\Car2.png",
                "G:\\My Drive\\D-ESPE\\V\\GRAFICA\\TERCER PARCIAL\\HighwayHustleGame\\HighwayHustleGame\\images\\Car3.png",
                "G:\\My Drive\\D-ESPE\\V\\GRAFICA\\TERCER PARCIAL\\HighwayHustleGame\\HighwayHustleGame\\images\\Car4.png",
                "G:\\My Drive\\D-ESPE\\V\\GRAFICA\\TERCER PARCIAL\\HighwayHustleGame\\HighwayHustleGame\\images\\Car5.png",
                "G:\\My Drive\\D-ESPE\\V\\GRAFICA\\TERCER PARCIAL\\HighwayHustleGame\\HighwayHustleGame\\images\\Car6.png",
            };

            int index = random.Next(0, carImages.Length);
            return Image.FromFile(carImages[index]);
        }

        private void MoveEnemyCars()
        {
            foreach (var enemyCar in enemyCars)
            {
                enemyCar.Move(Direction.Down);
            }
        }

        private void RemoveOffscreenEnemyCars()
        {
            for (int i = enemyCars.Count - 1; i >= 0; i--)
            {
                if (enemyCars[i].Top > gamePanel.Height)
                {
                    gamePanel.Controls.Remove(enemyCars[i]);
                    enemyCars.RemoveAt(i);
                }
            }
        }

        private void CheckCollisions()
        {
            foreach (var enemyCar in enemyCars)
            {
                if (car.Bounds.IntersectsWith(enemyCar.Bounds))
                {
                    gameTimer.Stop();
                    levelTimer?.Stop(); // Detener el temporizador de nivel si está en modo Historia
                    MessageBox.Show("¡Colisión! Fin del juego.");
                    break;
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(FormInicial_KeyDown);
            this.KeyUp += new KeyEventHandler(FormInicial_KeyUp);
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menúPrincipalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuInicial menuInicial = new MenuInicial();
            menuInicial.Show();
            this.Close();
        }

        private void slegirNivelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NivelesForm nivelesForm = new NivelesForm();
            nivelesForm.Show();
            this.Close();
        }
    }
}
