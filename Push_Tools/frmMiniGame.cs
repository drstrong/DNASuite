using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Push_Tools
{
    public partial class frmMiniGame : Form
    {       
        //Initialization
        public frmMiniGame()
        {
            InitializeComponent();
            this.InitializeValues();
            this.UpdateValues();
        }
        //Properties
        private int Str;
        private int Dex;
        private int Con;
        private int Int;
        private int Wis;
        private int Cha;
        //Buttons
        private void buttonFinish_Click(object sender, EventArgs e)
        {

            this.Close();
        }
        private void buttonRoll_Click(object sender, EventArgs e)
        {
            this.RollDice();
        }
        private void RollDice()
        {
            //Roll new values
            //If you're reading this, I know it's an incredibly bad way to do it
            //I don't really care
            this.Str = ThreeDeeSix(2791);
            this.Dex = ThreeDeeSix(4397);
            this.Con = ThreeDeeSix(14551);
            this.Int = ThreeDeeSix(30677);
            this.Wis = ThreeDeeSix(46099);
            this.Cha = ThreeDeeSix(58057);
            //Change the labels
            labelStr.Text = this.Str.ToString();
            labelDex.Text = this.Dex.ToString();
            labelCon.Text = this.Con.ToString();
            labelInt.Text = this.Int.ToString();
            labelWis.Text = this.Wis.ToString();
            labelCha.Text = this.Cha.ToString();
            //Check for a final condition
            this.UpdateValues();
        }
        private int ThreeDeeSix(int randinit)
        {
            //If you're reading this, I know it's an incredibly bad way to do it
            //I don't really care
            int ticks = (int) DateTime.Now.Ticks;
            Random rnd = new Random(ticks + randinit);
            int dice1 = rnd.Next(1, 7);
            int dice2 = rnd.Next(1, 7);
            int dice3 = rnd.Next(1, 7);
            return dice1 + dice2 + dice3;
        }
        private void InitializeValues()
        {
            buttonFinish.Text = "I give up";
            this.Str = 0;
            this.Dex = 0;
            this.Con = 0;
            this.Int = 0;
            this.Wis = 0;
            this.Cha = 0;
        }
        private void UpdateValues()
        {
            if (this.Str > 0)
            {
                if (this.Cha > 17)
                {
                    buttonFinish.Text = "Hey there ;)";
                }
                else if (this.Wis < 7)
                {
                    buttonFinish.Text = "I do what I want!";
                }
                else if (this.Int > 13)
                {
                    buttonFinish.Text = "I'm a genius!";
                }
                else
                {
                    buttonFinish.Text = "I give up";
                }
            }
        }
    }
}
