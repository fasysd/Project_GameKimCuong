using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace Project_GameKimCuong
{
    public partial class Form1 : Form
    {
        private const int gridSize = 10;//kích thước bàn cờ
        private const int tileSize = 50;//kích cỡ ô chọn
        private int[,] tableTile = new int[gridSize, tileSize];
        private Panel[,] grid = new Panel[gridSize, gridSize];
        private Random rand = new Random();
        private Point? firstTile = null;
        private bool canAction = true;

        private Color[] colors = new Color[]
        {
            Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Orange, Color.Purple, Color.White
        };


        public Form1()
        {
            InitializeComponent();
            _createGrid();
        }

        private void _createGrid()
        {
            for (int pX = 0; pX < gridSize; pX++)
            {
                for (int pY = 0; pY < gridSize; pY++)
                {
                    tableTile[ pX, pY] = rand.Next( colors.Length - 1);//xác định loại ngẫu nhiên cho từng ô

                    //Tạo bảng grib để hiện thị kim cương
                    Panel panel = new Panel();
                    grid[ pX, pY] = panel;
                    panel.Size = new Size( tileSize, tileSize);
                    panel.Location = new Point( pX * tileSize, pY * tileSize);
                    panel.BackColor = colors[ tableTile[pX,pY]];
                    panel.Click += _clickTile;
                    panel.Tag = new Point( pX, pY);
                    this.Controls.Add( panel);
                }
            }
            _handleTableTile();
        }
        private void _updateGrib()//Cập nhật lại phần hiển thị
        {
            for (int pX = 0; pX < gridSize; pX++)
            {
                for (int pY = 0; pY < gridSize; pY++)
                {
                    grid[ pX, pY].BackColor = colors[tableTile[ pX, pY]];
                }
            }
        }
        private void _clickTile( object sender, EventArgs e)
        {
            Panel btn = sender as Panel;
            if (btn == null | !canAction) return;

            Point point = (Point)btn.Tag;

            if (firstTile == null)
            {
                firstTile = point;
                btn.BackColor = Color.Black;
            }
            else
            {
                Point secondTile = point;
                if (_isAdjacent(firstTile.Value, secondTile))//Kiểm tra liền kề
                {
                    _swapTiles(firstTile.Value, secondTile);//Đổi vị trí
                    _handleTableTile();
                }
                _updateGrib();
                firstTile = null;//Thiết lập lại ô được chọn
            }
        }
        private async Task _handleTableTile()//Xử lý bảng
        {
            List<Point> listPoints = _checkMatch();//Kiểm tra bộ 3
            if (listPoints.Count == 0) _minusPoints();
            canAction = false;
            while (listPoints.Count > 0)//Có bộ 3
            {
                _plusPoint(listPoints.Count);
                foreach (var item in listPoints)//Chuyển các bộ 3 thành ô trống
                {
                    tableTile[item.X, item.Y] = colors.Length - 1;
                    await Task.Delay(30);
                    _updateGrib();
                }

                while (_updateTableTile() != 0)//Chờ các ô bị đẩy xuống hết, không còn ô trống
                {
                    await Task.Delay(200);
                    _updateGrib();
                }

                listPoints = _checkMatch();//Kiểm tra bộ 3 lại lần nữa
            }
            canAction = true;
        }
        private void _swapTiles(Point point1, Point point2)
        {
            //Thay đổi giá trị của 2 ô point1 với point2
            int oldValue = tableTile[ point1.X, point1.Y ];
            tableTile[ point1.X, point1.Y ] = tableTile[ point2.X, point2.Y ];
            tableTile[ point2.X, point2.Y ] = oldValue;
        }

        private bool _isAdjacent( Point point1, Point point2)
        {
            //Kiểm tra liền kề của point1 với point2
            int dx = Math.Abs( point1.X - point2.X);
            int dy = Math.Abs( point1.Y - point2.Y);
            return (dx + dy == 1); // chỉ được đổi nếu liền kề
        }

        private List<Point> _checkMatch()
        {
            //Trả về số bộ 3 phù hợp ( bộ 3 cộng 3, bộ 4 cộng 6, bộ 5 cộng 9)
            List<Point> points = new List<Point>();
            for (int pX = 0; pX < gridSize; pX++)
            {
                for (int pY = 0; pY < gridSize; pY++)
                {
                    int pointValue = tableTile[ pX, pY];

                    //Kiểm tra hàng ngang
                    if( pX < gridSize - 2 && pointValue == tableTile[ pX + 1, pY] && pointValue == tableTile[pX + 2, pY])
                    {
                        points.Add(new Point(pX, pY));
                        points.Add(new Point(pX + 1, pY));
                        points.Add(new Point(pX + 2, pY));
                        //Cộng điểm
                    }

                    //Kiểm tra hàng dọc
                    if (pY < gridSize - 2 && pointValue == tableTile[pX, pY + 1] && pointValue == tableTile[pX, pY + 2])
                    {
                        points.Add(new Point(pX, pY));
                        points.Add(new Point(pX, pY + 1));
                        points.Add(new Point(pX, pY + 2));
                        //Cộng điểm
                    }
                }
            }
            return points;
        }

        private int _updateTableTile()//Cập nhật lại bảng ô để lấp các vị trí trống, nhưng chỉ lấp xuống 1 ô
        {
            int numberObBlackTime = 0;
            int vaLueOfBlackTile = colors.Length - 1;
            for (int pX = gridSize - 1; pX >= 0; pX--)
            {
                for (int pY = gridSize - 1; pY >= 0; pY--)
                {
                    int tileValue = tableTile[pX, pY];
                    if (tileValue == vaLueOfBlackTile)
                    {
                        for (int pY_1 = pY; pY_1 > 0; pY_1--)
                        {
                            tableTile[ pX, pY_1] = tableTile[pX, pY_1 - 1];
                        }
                        tableTile[pX, 0] = rand.Next( vaLueOfBlackTile);
                        numberObBlackTime ++;
                    }
                }
            }
            return numberObBlackTime;
        }
        private void _plusPoint( int point)
        {
            textBox_point.Text = (Convert.ToInt32(textBox_point.Text) + point).ToString();
        }
        private void _minusPoints()
        {
            textBox_point.Text = (Convert.ToInt32(textBox_point.Text) - 10).ToString();
        }
    }
}
