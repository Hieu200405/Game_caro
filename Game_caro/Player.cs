using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_caro
{
    public class Player
    {
        private string name;    //Ctrl + R + E
        public string Name { get => name; set => name = value; }
        private Image mark;
        public Image Mark { get => mark; set => mark = value; }
        public Player(string name, Image mark) 
        {
            this.Name = name;
            this.Mark = mark;
        }
    }
}
