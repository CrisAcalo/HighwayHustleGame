using HighwayHustleGame.Components;
using HighwayHustleGame.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HighwayHustleGame
{
    public partial class FormInicial : Form
    {
        private BufferedPanel gamePanel; // Usa la nueva clase BufferedPanel
        private Road road;
        private Timer gameTimer;
        private HashSet<Keys> keysPressed; // Almacena las teclas que están siendo presionadas
        private List<Car> enemyCars;
        private Random random;
        private Car car; // Campo para el coche

        private int laneCount = 3; // Número de carriles
        private int[] lanePositions;

        public FormInicial()
        {
            InitializeComponent();

            gamePanel = new BufferedPanel(); // Usa BufferedPanel en lugar de Panel
            gamePanel.Dock = DockStyle.Fill; // Ajusta el panel al tamaño del formulario
            this.Controls.Add(gamePanel); // Agrega el panel al formulario

            gamePanel.Paint += new PaintEventHandler(GamePanel_Paint);

            // Inicializar la carretera
            int width = gamePanel.Width;
            int height = gamePanel.Height;
            road = new Road(level: 0, initialSpeed: 5.0f, width: width, height: height);

            // Cargar la imagen del coche
            Image carImage = Image.FromFile("G:\\My Drive\\D-ESPE\\V\\GRAFICA\\TERCER PARCIAL\\HighwayHustleGame\\HighwayHustleGame\\images\\Car1.png"); // Reemplaza con la ruta a la imagen del coche
            car = new Car(1,carImage, new Point(width / 2 - 74 / 2, height - 110), 7.0f, width, height, 60, 100);

            // Agregar el coche al panel
            gamePanel.Controls.Add(car);
            enemyCars = new List<Car>();
            random = new Random();



            lanePositions = new int[laneCount];
            CalculateLanePositions();


            // Configurar el temporizador para actualizar y redibujar la carretera
            gameTimer = new Timer();
            gameTimer.Interval = 16; // Aproximadamente 60 FPS
            gameTimer.Tick += new EventHandler(GameTimer_Tick);
            gameTimer.Start();
            //road.Update();

            keysPressed = new HashSet<Keys>(); // Inicializa el conjunto de teclas presionadas
        }

        private void GamePanel_Paint(object sender, PaintEventArgs e)
        {
            // Dibujar la carretera en el panel
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

        // Puedes agregar más lógica aquí para manejar los controles del juego
        private void FormInicial_KeyDown(object sender, KeyEventArgs e)
        {
            keysPressed.Add(e.KeyCode); // Agrega la tecla al conjunto cuando se presiona
        }

        private void FormInicial_KeyUp(object sender, KeyEventArgs e)
        {
            keysPressed.Remove(e.KeyCode); // Remueve la tecla del conjunto cuando se libera
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
            if (enemyCars.Count < 5) // Máximo número de autos enemigos en la carretera
            {
                int laneIndex = random.Next(0, laneCount);
                int laneX = lanePositions[laneIndex];

                // Verificar que no haya otro auto en el mismo carril y suficientemente cerca
                bool isLaneOccupied = enemyCars.Any(car =>
                    Math.Abs(car.Left - laneX) < car.Width && car.Top < car.Height * 2);

                if (!isLaneOccupied)
                {
                    Image enemyCarImage = GetRandomCarImage(); // Obtener una imagen aleatoria de auto enemigo
                    int y = -enemyCarImage.Height;
                    Car enemyCar = new Car(2, enemyCarImage, new Point(laneX, y), random.Next(5, 10), gamePanel.Width, gamePanel.Height, 60, 100);
                    enemyCars.Add(enemyCar);
                    gamePanel.Controls.Add(enemyCar);
                }
            }
        }


        private Image GetRandomCarImage()
        {
            // Lista de imágenes de autos enemigos
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
                    MessageBox.Show("¡Colisión! Fin del juego.");
                    break;
                }
            }
        }

        // Asegúrate de que FormInicial está configurado para recibir eventos de teclado
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(FormInicial_KeyDown);
            this.KeyUp += new KeyEventHandler(FormInicial_KeyUp);
        }
    }
}
