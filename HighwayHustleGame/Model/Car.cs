using System;
using System.Drawing;
using System.Windows.Forms;

namespace HighwayHustleGame.Model
{
    public class Car : PictureBox
    {
        private float speed; // Velocidad del coche
        private int typeOfCar;

        public Car(int typeOfCar,Image carImage, Point initialPosition, float speed, int panelWidth, int panelHeight, int width, int height)
        {
            this.Image = carImage;
            this.Size = carImage.Size;
            this.Location = initialPosition;
            this.speed = speed;
            this.BackColor = Color.Transparent; // Fondo transparente
            this.SizeMode = PictureBoxSizeMode.AutoSize; // Ajustar automáticamente al tamaño de la imagen
            this.Width = width;
            this.Height = height;
            this.SizeMode = PictureBoxSizeMode.StretchImage; // Ajustar la imagen al tamaño del PictureBox
            this.BorderStyle = BorderStyle.FixedSingle; // Sin borde
            this.typeOfCar = typeOfCar;
            // Asegurarse de que el coche no se mueva fuera de los límites del panel
            this.MaximumSize = new Size(panelWidth, panelHeight);
        }

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        public int TypeOfCar
        {
            get { return typeOfCar; }
            set { typeOfCar = value; }
        }

        public void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    this.Top -= (int)speed;
                    break;
                case Direction.Down:
                    this.Top += (int)speed;
                    break;
                case Direction.Left:
                    this.Left -= (int)speed;
                    break;
                case Direction.Right:
                    this.Left += (int)speed;
                    break;
                case Direction.UpRight:
                    this.Top -= (int)(speed * 0.7071); // Mover en diagonal (1/sqrt(2))
                    this.Left += (int)(speed * 0.7071);
                    break;
                case Direction.UpLeft:
                    this.Top -= (int)(speed * 0.7071);
                    this.Left -= (int)(speed * 0.7071);
                    break;
                case Direction.DownRight:
                    this.Top += (int)(speed * 0.7071);
                    this.Left += (int)(speed * 0.7071);
                    break;
                case Direction.DownLeft:
                    this.Top += (int)(speed * 0.7071);
                    this.Left -= (int)(speed * 0.7071);
                    break;
            }
            // Limitar el movimiento dentro del panel
            if (typeOfCar == 1)
            {
                this.Left = Math.Max(0, Math.Min(this.Left, this.Parent.ClientSize.Width - this.Width));
                this.Top = Math.Max(0, Math.Min(this.Top, this.Parent.ClientSize.Height - this.Height));
            }
        }

        
    }
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        UpRight,
        UpLeft,
        DownRight,
        DownLeft
    }
}
