﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_caro
{
    public class ChessBoardManager
    {
        #region Properties
        private Panel chessBoard;

        private List<Player> player;

        public List<Player> Player { get => player; set => player = value; }
        
        private int currentplayer;

        public int Currentplayer { get => currentplayer; set => currentplayer = value; }

        private TextBox playerName;

        public TextBox PlayerName { get => playerName; set => playerName = value; }

        private PictureBox playerMark;

        public PictureBox PlayerMark { get => playerMark; set => playerMark = value; }
        private List<List<Button>> matrix;
        public List<List<Button>> Matrix
        {
            get { return matrix; }
            set { matrix = value; }
        }
        #endregion
        private event EventHandler playerMarked;
        public event EventHandler PlayerMarked
        {
            add
            {
                playerMarked += value;
            }
            remove
            {
                playerMarked -= value;
            }
        }
        private event EventHandler endedGame;
        public event EventHandler EndedGame
        {
            add
            {
                endedGame += value;
            }
            remove
            {
                endedGame -= value;
            }
        }
        private Stack<PlayInfo> playTimeLine;
        public Stack<PlayInfo> PlayTimeLine
        {
            get { return playTimeLine; }
            set { playTimeLine = value; }
        }
        #region Initialize
        public ChessBoardManager(Panel chessBoard, TextBox playerName, PictureBox mark)
        {
            this.chessBoard = chessBoard;
            this.playerName = playerName;
            this.PlayerMark = mark;
            this.player = new List<Player>() 
            { 
                new Player("P1", Image.FromFile(Application.StartupPath + "\\Resources\\X.png")),
                new Player("P2", Image.FromFile(Application.StartupPath + "\\Resources\\O.png"))
            };
           
        }
        #endregion
        #region Methods
        public void DrawChessBoard()
        {
            chessBoard.Enabled = true;
            chessBoard.Controls.Clear();
            PlayTimeLine = new Stack<PlayInfo>();
            Currentplayer = 0;
            ChangePlayer();
            Matrix = new List<List<Button>>();
            Button oldButton = new Button() { Width = 0, Location = new Point(0, 0) };
            for (int i = 0; i < Cons.CHESS_BOARD_HEIGHT; i++)
            {
                Matrix.Add(new List<Button>());
                for (int j = 0; j < Cons.CHESS_BOARD_WIDTH; j++)
                {
                    Button btn = new Button()
                    {
                        Width = Cons.CHESS_WITH,
                        Height = Cons.CHESS_HEIGHT,
                        Location = new Point(oldButton.Location.X + oldButton.Width, oldButton.Location.Y),
                        BackgroundImageLayout = ImageLayout.Stretch,
                        Tag = i.ToString()
                    };
                    btn.Click += Btn_Click;
                    chessBoard.Controls.Add(btn);
                    Matrix[i].Add(btn);
                    oldButton = btn;
                }
                oldButton.Location = new Point(0, oldButton.Location.Y + Cons.CHESS_HEIGHT);
                oldButton.Width = 0;
                oldButton.Height = 0;
            }
        }
        private void Btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.BackgroundImage != null) return;
            Mark(btn);
            PlayTimeLine.Push(new PlayInfo(GetChessPoint(btn), Currentplayer));
            Currentplayer = Currentplayer == 1 ? 0 : 1;
            ChangePlayer();
            if (playerMarked != null)
                playerMarked(this, new EventArgs());
            if (isEndGame(btn))
            {
                EndGame();
            } 
        }
        public void EndGame()
        {
            if (endedGame != null)
                endedGame(this, new EventArgs());
        }
        public bool Undo()
        {
            if(PlayTimeLine.Count <= 0) return false;
            PlayInfo oldPoint = PlayTimeLine.Pop();
            Button btn = Matrix[oldPoint.Point.Y][oldPoint.Point.X];
            btn.BackgroundImage = null;
            if (PlayTimeLine.Count <= 0)
            { 
                Currentplayer = 0; 
            }  
            else
            {
                oldPoint = PlayTimeLine.Peek();
                Currentplayer = oldPoint.CurrentPlayer == 1 ? 0 : 1;
            }    
            ChangePlayer();
            return true;
        }
        private bool isEndGame(Button btn)
        {
            return isEndHorizonta(btn) || isEndVertical(btn) || isEndPrimary(btn) || isEndSub(btn);
        }
        private Point GetChessPoint(Button btn)
        { 
            int vertical = Convert.ToInt32(btn.Tag);
            int horizontal = Matrix[vertical].IndexOf(btn);
            Point point = new Point(horizontal,vertical);
            return point;
        }
        private bool isEndHorizonta(Button btn)
        {
            Point point = GetChessPoint(btn);
            int countLeft = 0;
            for(int i= point.X;i>=0;i--)
            {
                if (Matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    countLeft++;
                }
                else break;
            }    
            int countRight = 0;
            for (int i = point.X +1; i < Cons.CHESS_BOARD_WIDTH; i++)
            {
                if (Matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    countRight++;
                }
                else break;
            }
            return countLeft + countRight ==5;
        }
        private bool isEndVertical(Button btn)
        {
            Point point = GetChessPoint(btn);
            int countTop = 0;
            for (int i = point.Y; i >= 0; i--)
            {
                if (Matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else break;
            }
            int countButtom = 0;
            for (int i = point.Y + 1; i < Cons.CHESS_BOARD_HEIGHT; i++)
            {
                if (Matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                {
                    countButtom++;
                }
                else break;
            }
            return countTop + countButtom == 5;
        }
        private bool isEndPrimary(Button btn)
        {
            Point point = GetChessPoint(btn);
            int countTop = 0;
            for (int i = 0; i <= point.X; i++)
            {
                if (point.X - i < 0 || point.Y - i < 0) break;
                if (Matrix[point.Y - i][point.X -i ].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else break;
            }
            int countButtom = 0;
            for (int i = 1; i <=Cons.CHESS_BOARD_WIDTH -  point.X; i++)
            {
                if (point.Y + i >= Cons.CHESS_BOARD_HEIGHT || point.X + i >= Cons.CHESS_BOARD_WIDTH)
                    break;
                if (Matrix[point.Y + i][point.X + i].BackgroundImage == btn.BackgroundImage)
                {
                    countButtom++;
                }
                else break;
            }
            return countTop + countButtom == 5;
        }
        private bool isEndSub(Button btn)
        {
            Point point = GetChessPoint(btn);
            int countTop = 0;
            for (int i = 0; i <= point.X; i++)
            {
                if (point.X + i > Cons.CHESS_BOARD_WIDTH || point.Y - i < 0) break;
                if (Matrix[point.Y - i][point.X + i].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else break;
            }
            int countButtom = 0;
            for (int i = 1; i <= Cons.CHESS_BOARD_WIDTH - point.X; i++)
            {
                if (point.Y + i >= Cons.CHESS_BOARD_HEIGHT || point.X - i < 0) break;
                if (Matrix[point.Y + i][point.X - i].BackgroundImage == btn.BackgroundImage)
                {
                    countButtom++;
                }
                else break;
            }
            return countTop + countButtom == 5;
        }
        private void Mark(Button btn)
        {
            btn.BackgroundImage = Player[Currentplayer].Mark; 
        }
        private void ChangePlayer()
        {
            PlayerName.Text = Player[Currentplayer].Name;
            PlayerMark.Image = Player[Currentplayer].Mark;
        }
        #endregion
    }
}
