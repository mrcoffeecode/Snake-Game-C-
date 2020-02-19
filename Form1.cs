using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace Snake
{
    public partial class GameWindow : Form
    {
        private int glcontrolFieldWidth = 20, glcontrolFieldHeight = 20; //Playing Field 
        public int speedincr = 0; //Speed, Increase by 5% Check out GenerateFood()
        public int GamePoints = -10; //Score 
        private int shaderProgram, color_attribute; //Shader Program
        private int view, model, projection;//Shader Program
        private Matrix4 ViewMatrix, ModelMatrix, ProjectionMatrix; // Matrix4
        private List<Point> snake = new List<Point>() { new Point(1, 1) }; //Where the Snake Start off
        private Point snakeDirection = new Point(1, 0);//Snake Direction
        private Point mouse = new Point(); //Food
        private Random random = new Random(); //Random Genereator 
        public bool bGameOver;
        private static readonly Timer timer = new Timer();

        public GameWindow()
        {
            //Form1.Designer
            InitializeComponent();

            // Centers the form on the current screen
            CenterToScreen();

            // Create a timer 
            //var timer = new Timer();
            timer.Tick += new EventHandler(GameLoop);
            timer.Interval = 150; // This is the snake inital speed
            timer.Start();

            // Generate an initial random position for the food
            GenerateFood();
        }

        public void GameOver(bool bGameOver)
        {
            //Snake Collision with its own body
            //for (int i = 0; i < snake.Count; i++)
            //{
            //    //if Snake equal itself then snake hasn't touch any other parts of its body
            //    if (snake[0].X == snake[i].X || snake[0].Y == snake[i].Y)
            //    {
            //        this.bGameOver = false;
            //        //Console.WriteLine("Snake " + snake2[i].X + "," + snake2[i].Y);
            //        //Console.WriteLine("Snake2 " + snake[i].X + "," + snake[i].Y);
            //    }
            //    else
            //    {
            //        this.bGameOver = true;
            //        //Console.WriteLine(snake[i].X+","+snake[i].Y);
            //    }
            //}



            ////---Display GameOver Message If Where Dead-- - //

            //if (bGameOver == true)
            //{
            //    Console.WriteLine("Game Over! Push Enter to Continue");
            //    if (bGameOver == false)
            //    {
            //        //Game Start Over

            //    }
            //}
            return;
        } //GameOver Reset Game

        public void GameLoop(object sender, System.EventArgs e)
        {
            // Update coordinates of game entities and check collisions
            Update();
            // UpdateWall check collisions with snake to bounce off wall
            UpdateWall();
            GameOver(bGameOver);
            glControl.Invalidate();
        } //Basic of the Game Loop

        private void UpdateWall()
        {
            // Snake collison with Walls
            if (snake[0].X < 1)
            {
                //snake[0].X = (glcontrolFieldWidth - 2) / 10; //Warp to right
                //snakeDirection.X = (snake[0].X + glcontrolFieldWidth-2)/10;
                //snakeDirection.X = (glcontrolFieldWidth-2)/10;
                snakeDirection.X = (int)(glcontrolFieldWidth-0.5) / 10;
                snakeDirection.Y = (int)(glcontrolFieldWidth-0.5) / 10;
                Console.WriteLine("Snake hit left wall");
            }
            else if (snake[0].X > glcontrolFieldWidth - 2)
            {
                //snakeDirection.X = 0; //Warp to left
                snakeDirection.X = (int)(glcontrolFieldWidth - 0.5) / -10;
                snakeDirection.Y = (int)(glcontrolFieldWidth - 0.5) / -10;
                Console.WriteLine("Snake hit right wall");
            }

            if (snake[0].Y < 1)
            {
                //snake.getFirst().y = windowHeight / 10; //Warp to bottom
                //snakeDirection.Y = (snake[0].Y + glcontrolFieldWidth - 2)/10;
                snakeDirection.X = (int)(glcontrolFieldHeight - 0.5)/10;
                snakeDirection.Y = (int)(glcontrolFieldHeight - 0.5)/10;
                Console.WriteLine("Snake hit Top wall");
            }
            else if (snake[0].Y > glcontrolFieldHeight - 2)
            {
                //snake.getFirst().y = 0; //Warp to top
                snakeDirection.X = (int)(glcontrolFieldHeight - 0.5) / -10;
                snakeDirection.Y = (int)(glcontrolFieldHeight - 0.5) / -10;
                Console.WriteLine("Snake hit Bottom wall");
            }

        } //Wall Collision

        private new void Update()
        {
            // Calculate a new position of the head of the snake
            Point newHeadPosition = new Point(snake[0].X + snakeDirection.X, snake[0].Y + snakeDirection.Y);
            // Insert new position in the beginning of the snake list
            snake.Insert(0, newHeadPosition);
            snake.RemoveAt(snake.Count - 1);

            // Check snake collision with the food
            if (snake[0].X != mouse.X || snake[0].Y != mouse.Y)
            {
                return;
            }
            else
            {
                //Snake Grow
                snake.Add(new Point(mouse.X, mouse.Y));
            }
            Console.WriteLine();

            //foreach (Point aPart in snake)
            //{
            //    Console.WriteLine(aPart);
            //}

            // Generate a new food(mouse) position
            GenerateFood();

        } //Events 

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            //Clear Buffer
            GL.Clear(ClearBufferMask.ColorBufferBit);
            //Called Draw which Calls the Drawing inside of Draw();
            Draw();
            //Swap Buffers
            glControl.SwapBuffers();
        }

        private void Draw()
        {
            DrawFood(); // Create Food aka Mouse
            DrawSnake(); // Create Snake
            DrawWall(); // Create Wall
        } //Call other Drawing

        private void DrawSnake()
        {
            foreach (var cell in snake)
            {
                DrawSquare(cell.X, cell.Y, Color4.LawnGreen);
            }
        } //Draws Snake

        private void DrawFood()
        {
            DrawSquare(mouse.X, mouse.Y, Color.LightGray);
        } //Draws Mouse

        private void DrawWall()
        {
            //Draw Top and Bottom Walls
            for (int i = 0; i <= glcontrolFieldWidth; i++)
            {
                DrawSquare(i, 0, Color.DarkRed); //Top
                DrawSquare(i, glcontrolFieldHeight - 1, Color.DarkRed);//Bottom
            }
            //Draw Left and Right Walls
            for (int i = 0; i <= glcontrolFieldHeight; i++)
            {
                DrawSquare(0, i - 1, Color.DarkRed); //Left
                DrawSquare(glcontrolFieldWidth - 1, i, Color.DarkRed); //Right
            }
        } //Draws Walls

        private void GenerateFood()
        {
            mouse.X = random.Next(1, glcontrolFieldWidth - 1);
            mouse.Y = random.Next(1, glcontrolFieldHeight - 1);
            //Score 10 points for each Mouse eaten
            GamePoints += 10;
            Score.Text = GamePoints.ToString();
            //Increase Speed by 5%  
            speedincr -= 10;
            timer.Stop();
            timer.Interval = 200 + speedincr;
            timer.Start();
            // This help from crashing the game its max speed is 90 ms
            if (speedincr < -90)
            {
                timer.Stop();
                timer.Interval = 90;
                timer.Start();
            }

        } //Creates A New Position for New Mouse and Increase Speed of Snake when eating a mouse and Earn GamePoints

        private void glControl_Load(object sender, EventArgs e)
        {
            glControl.MakeCurrent();

            // Backgorund Color --> glcontrol
            GL.ClearColor(Color4.ForestGreen);

            // Initialize Shaders and get a Shader Program
            shaderProgram = InitShadersAndGetProgram();
            if (shaderProgram < 0) return;

            // Initialize Vertex Buffers Drawing Vertex of Snake 
            InitVertexBuffers();

            color_attribute = GL.GetUniformLocation(shaderProgram, "Ccolor");

            // Set a coordinate cell
            projection = GL.GetUniformLocation(shaderProgram, "Projection");
            ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(0f, glcontrolFieldWidth, glcontrolFieldHeight, 0f, -100f, 100f);
            GL.UniformMatrix4(projection, false, ref ProjectionMatrix);

            view = GL.GetUniformLocation(shaderProgram, "view");
            ViewMatrix = new Matrix4();

            model = GL.GetUniformLocation(shaderProgram, "model");
            ModelMatrix = new Matrix4();

            GL.Viewport(0, 0, glControl.Width, glControl.Height);
        }

        private void DrawSquare(int x, int y, Color4 color, int size = 1)
        {
            // Set color to fragment shader
            GL.Uniform3(color_attribute, color.R, color.G, color.B);
            // Set a size of the square
            ViewMatrix = Matrix4.CreateScale(size);
            GL.UniformMatrix4(view, false, ref ViewMatrix);
            // Set a position of the Square
            ModelMatrix = Matrix4.CreateTranslation(new Vector3(x, y, 1f));
            GL.UniformMatrix4(model, false, ref ModelMatrix);
            // Draw Square
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
        }

        private int InitShadersAndGetProgram()
        {
            string vertexShaderSource =
                "#version 140\n" +
                "in vec2 aPosition;" +
                "uniform mat4 Projection;" +
                "uniform mat4 view;" +
                "uniform mat4 model;" +
                "void main()" +
                "{" +
                "    gl_Position = Projection * model * view * vec4(aPosition, 1.0, 1.0);" +
                "}";

            string fragmentShaderSource =
                "#version 140\n" +
                "uniform vec3 Ccolor;" +
                "out vec4 fragColor;" +
                "void main()" +
                "{" +
                "    fragColor = vec4(Ccolor, 1.0);" +
                "}";

            // Vertex Shader
            int vShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vShader, vertexShaderSource);
            GL.CompileShader(vShader);

            // Fragment Shader
            int fShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fShader, fragmentShaderSource);
            GL.CompileShader(fShader);

            int program = GL.CreateProgram();
            GL.AttachShader(program, vShader);
            GL.AttachShader(program, fShader);
            GL.LinkProgram(program);
            GL.UseProgram(program);

            return program;
        } //Martix4 and Vertex & Fragment Shader

        private void InitVertexBuffers()
        {
            //Vertices of the Square Also can be modifly to any shape this also is the shape of the food
            float[] vertices = new float[]
            {
                0f, 0f,
                0f, 1f,
                1f, 0f,
                1f, 1f
            };

            int vbo;
            GL.GenBuffers(1, out vbo);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            // Get an array size in bytes
            int sizeInBytes = vertices.Length * sizeof(float);
            // Send the vertex array to a video card memory
            GL.BufferData(BufferTarget.ArrayBuffer, sizeInBytes, vertices, BufferUsageHint.StaticDraw);
            // Config
            GL.EnableVertexAttribArray(color_attribute);
            GL.VertexAttribPointer(color_attribute, 2, VertexAttribPointerType.Float, false, 0, 0);

        } //Vertex Buffer

        private void glControl_KeyPress(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    snakeDirection.X = 0;
                    snakeDirection.Y = -1;
                    break;
                case Keys.Left:
                    snakeDirection.X = -1;
                    snakeDirection.Y = 0;
                    break;
                case Keys.Down:
                    snakeDirection.X = 0;
                    snakeDirection.Y = 1;
                    break;
                case Keys.Right:
                    snakeDirection.X = 1;
                    snakeDirection.Y = 0;
                    break;
                case Keys.Enter:
                    bGameOver = false;
                    break;
            }

            //glControl.Invalidate();
        } //Controls use for Snake Game
    }
}