using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CouplesGame
{
    public partial class Juego1 : Form
    {
        //Creacion de un objeteo de tipo Random
        readonly Random random = new Random();
        //Creacion de un objeto de tipo lista de strings que contiene las letras que generan los iconos de la fuente Webdings
        readonly List<string> icons = new List<string>()
        {
            "!", "!", "N", "N", ",", ",", "k", "k",
            "b", "b", "v", "v", "W", "W", "Z", "Z"
        };
        Label firstClicked = null;      //Variable de referencia al valor del primer click
        Label secondClicked = null;     //Variable de referencia al valor del segundo click
        private short reloj = 30;       //variable para establecer el valor del temporizador
        public static bool sonidoSilencio;    //Variable encargaa de determinar si se quiere sonido o no
        private int contadorTruco = 0;          //Variable encargada del controlar el truco
        readonly SoundPlayer audioBien = new SoundPlayer(Resource.Bien);     //Creo  varios objeto de clase SoundPlayer y le asigno un sonido de los resources
        readonly SoundPlayer audio = new SoundPlayer(Resource.Ganador2);
        readonly SoundPlayer fin = new SoundPlayer(Resource.Fin);


        public Juego1()
        {
            InitializeComponent();
            AssignIconsToSquares();
        }
        //Método encargado de rellenar los label del tablero con iconos random de la lista proporcionada
        private void AssignIconsToSquares()
        {
            foreach (Control control in TableLayoutPanel_Cudaricula.Controls)
            {
                if (control is Label Label_Icono)
                {
                    int randomNumber = random.Next(icons.Count);         //Guarda en una variable entera un numero aleatorio de la posicion en la lista de iconos
                    Label_Icono.Text = icons[randomNumber];              //Establece el icono anterior como valor del texto del label
                    Label_Icono.ForeColor = Label_Icono.BackColor;       //Establece el mismo color a los iconos que el fondo para ocultarlos  
                    icons.RemoveAt(randomNumber);                        //Quita ese valor para que no se repita y salga mas de dos veces
                }
            }
        }
        //Mètodo ejecutado una vez cargado el form que da bienvenida, pregunta si quiere sonidos y activa el contrareloj
        private void Juego1_Shown(object sender, EventArgs e)
        {
            Label_ContraReloj.Text = "";
            DialogResult dr = MessageBox.Show("Bienvenido a Couple Games, haz parejas antes de que acabe el tiempo\n\n¿Quieres jugar con sonido?", "Couple Games", MessageBoxButtons.YesNo, MessageBoxIcon.Question);       //Mensaje de bienvenida
            if (dr == DialogResult.Yes)
            {
                sonidoSilencio = true;
                Button_Sonido.BackgroundImage = Resource.Sonido;
            }
            else
            {
                sonidoSilencio = false;
                Button_Sonido.BackgroundImage = Resource.Silencio;
            }
            Contador();     //Método del temporizador
        }

        private void Label_Icono_Click(object sender, EventArgs e)
        {
            //Si el temporizador de acierto esta activo se sale
            if (Temporizador.Enabled == true)
            {
                return;
            }

            Label clickedLabel = sender as Label;
            if (clickedLabel !=null)
            {
                if (clickedLabel.ForeColor == Color.Black)
                {
                    return;
                }
                
                if (firstClicked == null)
                {
                    firstClicked = clickedLabel;
                    firstClicked.ForeColor = Color.Black;
                    return;
                }
            }
            secondClicked = clickedLabel;
            secondClicked.ForeColor = Color.Black;
            CheckForWinner();

            if (firstClicked.Text == secondClicked.Text)    //Si el texto es igual significa que los iconos son iguales y...
            {
                firstClicked.BackColor = Color.Green;       //Coloco el fondo del primer click en verde para indicar que esta bien      
                secondClicked.BackColor = Color.Green;      //Coloco el fondo del segundo click en verde para indicar que esta bien                    
                //Dependiendo si se ha elegido sonido o no reproduzco sonido de acierto
                if (sonidoSilencio)
                {
                    audioBien.Play();
                }
                firstClicked = null;        //Vacio variables de referenia
                secondClicked = null;       //Vacio variables de referenia
                return;
            }
            if (firstClicked.Text != secondClicked.Text)         //Metodo encargado de que si fallas pone el fondo de los iconos en rojo 
            {
                firstClicked.BackColor = Color.Red;
                secondClicked.BackColor = Color.Red;
            }
            Temporizador.Start();
        }

        private void Temporizador_Tick(object sender, EventArgs e)
        {
            firstClicked.BackColor = Color.CornflowerBlue;      //vuelvo a poner el fondo rojo al original 
            secondClicked.BackColor = Color.CornflowerBlue;       //vuelvo a poner el fondo rojo al original 


            Temporizador.Stop();
            firstClicked.ForeColor = firstClicked.BackColor;
            secondClicked.ForeColor = secondClicked.BackColor;
            firstClicked = null;
            secondClicked = null;

            if (sonidoSilencio)
            {
                SystemSounds.Beep.Play();
            }
        }
        //Recorre el tableLayout y determina si todos los label son forecolor = black es que ha ganado y muestra mensaje
        private void CheckForWinner()
        {
            foreach (Control control in TableLayoutPanel_Cudaricula.Controls)
            {
                if (control is Label iconLabel)
                {
                    if (iconLabel.ForeColor == iconLabel.BackColor)
                    {
                        return;
                    }
                }
            }

            firstClicked.BackColor = Color.Green;           //Para poner fondo verde a la ultima pareja de iconos acertados
            secondClicked.BackColor = Color.Green;           //Para poner fondo verde a la ultima pareja de iconos acertados

            //Si todos las parejas estan completas se escucha un sonido de gratificacion , paro el temporizador, y el juego continua en el nivel 2

            if (sonidoSilencio)
            {
                audio.Play();
            }
            ContraReloj.Stop();
            MessageBox.Show("Has encontado todas las parejas de iconos pasas al nivel 2", "Felicidades");
            SegundoNivel();
        }

        //Metodo encargado de crear un objeto del segundo Form,ocultar este y mostrar el segundo
        private void SegundoNivel()
        {
            Juego2 dos = new Juego2();
            Hide();
            
            dos.ShowDialog();
        }

        
        //Creo objeto clase Timer y lo inicializo ademas de poner el texto del contador en rojo y muestro el valor del reloj
        private void Contador()
        {
            ContraReloj = new Timer();
            ContraReloj.Tick += new EventHandler(ContraReloj_Tick);
            ContraReloj.Interval = 1000; // 1 second
            ContraReloj.Start();
            Label_ContraReloj.ForeColor = Color.Red;
            Label_ContraReloj.Text = reloj.ToString();
        }

        private void ContraReloj_Tick(object sender, EventArgs e)
        {
            reloj--;

            NumerosMasGrandes();

            if (reloj == 0)
            {
                Label_ContraReloj.Text = reloj.ToString();
                ContraReloj.Stop();

                NuevoOsalir();

            }

            Label_ContraReloj.Text = reloj.ToString();
        }
        //Metodo encargado de elevar el tamaño de la letra de segun el temporizador de juego va llegando a 0

        private void NumerosMasGrandes()
        {
            switch (reloj)
            {
                case 10:
                    Label_ContraReloj.Font = new Font(Label_ContraReloj.Text, 12);
                    break;
                case 9:
                    Label_ContraReloj.Font = new Font(Label_ContraReloj.Text, 14);
                    break;
                case 8:
                    Label_ContraReloj.Font = new Font(Label_ContraReloj.Text, 16);
                    break;
                case 7:
                    Label_ContraReloj.Font = new Font(Label_ContraReloj.Text, 18);
                    break;
                case 6:
                    Label_ContraReloj.Font = new Font(Label_ContraReloj.Text, 20);
                    break;
                case 5:
                    Label_ContraReloj.Font = new Font(Label_ContraReloj.Text, 22);
                    break;
                case 4:
                    Label_ContraReloj.Font = new Font(Label_ContraReloj.Text, 24);
                    break;
                case 3:
                    Label_ContraReloj.Font = new Font(Label_ContraReloj.Text, 26);
                    break;
                case 2:
                    Label_ContraReloj.Font = new Font(Label_ContraReloj.Text, 28);
                    break;
                case 1:
                    Label_ContraReloj.Font = new Font(Label_ContraReloj.Text, 30);
                    break;
                case 0:
                    Label_ContraReloj.Font = new Font(Label_ContraReloj.Text, 32);
                    break;
            }
        }
        private void NuevoOsalir()
        {
            if (sonidoSilencio)
            {
                fin.Play();
            }
            
            DialogResult dr = MessageBox.Show("Se ha acabado el tiempo, ¿quieres jugar otra partida?", "Tiempo acabado", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                Hide();
                Juego1 uno = new Juego1();
                uno.Show();
            }
            else
            {
                Application.Exit();
            }
        }
        //Comportamiento del botón del sonido
        private void Button_Sonido_Click(object sender, EventArgs e)
        {
            if (sonidoSilencio)
            {
                Button_Sonido.BackgroundImage = Resource.Silencio;
                sonidoSilencio = false;
            }
            else
            {
                Button_Sonido.BackgroundImage = Resource.Sonido;
                sonidoSilencio = true;
            }
        }
        //Método ejecutado al cerra el form
        private void Juego1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
        //Método truco que desoculta los iconos
        private void Label_ContraReloj_Click(object sender, EventArgs e)
        {

            contadorTruco++;
            if (contadorTruco >= 10)
            {
                MessageBox.Show("Se ha activado el truco ☠️", "Truco", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                foreach (Control control in TableLayoutPanel_Cudaricula.Controls)
                {
                    if (control is Label Label_Icono)
                    {
                        Label_Icono.ForeColor = SystemColors.ControlText;       
                    }
                }
            }
        }
    }

}
