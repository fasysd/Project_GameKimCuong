using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace Project_GameKimCuong._Scripts {
    public partial class JewelGrid
    {
        private const int gridSize = 10;//kích thước bàn cờ
        private const int tileSize = 50;//kích cỡ ô chọn
        private int[,] _jewelGrid = new int[gridSize, tileSize];
        private Panel[,] grid = new Panel[gridSize, gridSize];
        private Random rand = new Random();
        private Point? firstJewel = null;
        private bool canAction = true;

        //Màu sắc của kim cương, màu cuối cùng là màu kim cương đã thu thập
        private Color[] colors = new Color[]
        {
            Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Orange, Color.Purple, Color.White
        };

        public JewelGrid( Form form)
        {
            for (int pX = 0; pX < gridSize; pX++)
            {
                for (int pY = 0; pY < gridSize; pY++)
                {
                    _jewelGrid[pX, pY] = rand.Next(colors.Length - 1);//xác định loại ngẫu nhiên cho từng ô

                    //Tạo bảng grib để hiện thị kim cương
                    Panel panel = new Panel();
                    grid[pX, pY] = panel;
                    panel.Size = new Size(tileSize, tileSize);
                    panel.Location = new Point(pX * tileSize, pY * tileSize);
                    panel.BackColor = colors[_jewelGrid[pX, pY]];
                    panel.Click += _clickJewel;
                    panel.Tag = new Point(pX, pY);

                    form.Controls.Add(panel);
                }
            }
            _handleJewelGrid();
        }

        private void _updateColors()//Cập nhật lại toàn phần hiển thị
        {
            for (int pX = 0; pX < gridSize; pX++)
            {
                for (int pY = 0; pY < gridSize; pY++)
                {
                    grid[pX, pY].BackColor = colors[_jewelGrid[pX, pY]];
                }
            }
        }
        private void _clickJewel(object sender, EventArgs e)
        {
            Panel btn = sender as Panel;
            if (btn == null | !canAction) return;

            Point point = (Point)btn.Tag;

            if (firstJewel == null)
            {
                firstJewel = point;
                btn.BackColor = Color.Black;
            }
            else
            {
                Point secondTile = point;
                if (_isAdjacent(firstJewel.Value, secondTile))//Kiểm tra liền kề
                {
                    _swapJewels(firstJewel.Value, secondTile);//Đổi vị trí
                    _handleJewelGrid();
                }
                _updateColors();
                firstJewel = null;//Thiết lập lại ô được chọn
            }
        }
        private async Task _handleJewelGrid()//Xử lý bảng
        {
            List<Point> listPoints = _checkMatch();//Kiểm tra bộ 3
            if (listPoints.Count == 0) _minusPoints();
            canAction = false;
            while (listPoints.Count > 0)//Có bộ 3
            {
                _plusPoint(listPoints.Count);
                foreach (var item in listPoints)//Chuyển các bộ 3 thành ô trống
                {
                    _jewelGrid[item.X, item.Y] = colors.Length - 1;
                    await Task.Delay(30);
                    grid[item.X, item.Y].BackColor = colors[colors.Length - 1];
                }

                while (_updateJewelGrid() != 0)//Chờ các ô bị đẩy xuống hết đến khi không còn ô trống
                {
                    await Task.Delay(200);
                    _updateColors();
                }

                listPoints = _checkMatch();//Kiểm tra bộ 3 lại lần nữa
            }
            canAction = true;
        }
        private void _swapJewels(Point point1, Point point2)
        {
            //Thay đổi giá trị của 2 ô point1 với point2
            int oldValue = _jewelGrid[point1.X, point1.Y];
            _jewelGrid[point1.X, point1.Y] = _jewelGrid[point2.X, point2.Y];
            _jewelGrid[point2.X, point2.Y] = oldValue;
        }

        private bool _isAdjacent(Point point1, Point point2)
        {
            //Kiểm tra liền kề của point1 với point2
            int dx = Math.Abs(point1.X - point2.X);
            int dy = Math.Abs(point1.Y - point2.Y);
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
                    int pointValue = _jewelGrid[pX, pY];

                    //Kiểm tra hàng ngang
                    if (pX < gridSize - 2 && pointValue == _jewelGrid[pX + 1, pY] && pointValue == _jewelGrid[pX + 2, pY])
                    {
                        points.Add(new Point(pX, pY));
                        points.Add(new Point(pX + 1, pY));
                        points.Add(new Point(pX + 2, pY));
                        //Cộng điểm
                    }

                    //Kiểm tra hàng dọc
                    if (pY < gridSize - 2 && pointValue == _jewelGrid[pX, pY + 1] && pointValue == _jewelGrid[pX, pY + 2])
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

        private int _updateJewelGrid()//Cập nhật lại bảng ô để lấp các vị trí trống, nhưng chỉ lấp xuống 1 ô
        {
            int numberObBlackTime = 0;
            int vaLueOfBlackTile = colors.Length - 1;
            for (int pX = gridSize - 1; pX >= 0; pX--)
            {
                for (int pY = gridSize - 1; pY >= 0; pY--)
                {
                    int tileValue = _jewelGrid[pX, pY];
                    if (tileValue == vaLueOfBlackTile)
                    {
                        for (int pY_1 = pY; pY_1 > 0; pY_1--)
                        {
                            _jewelGrid[pX, pY_1] = _jewelGrid[pX, pY_1 - 1];
                        }
                        _jewelGrid[pX, 0] = rand.Next(vaLueOfBlackTile);
                        numberObBlackTime++;
                    }
                }
            }
            return numberObBlackTime;
        }
        private void _plusPoint(int point)
        {
            //textBox_point.Text = (Convert.ToInt32(textBox_point.Text) + point).ToString();
        }
        private void _minusPoints()
        {
            //textBox_point.Text = (Convert.ToInt32(textBox_point.Text) - 10).ToString();
        }
    }
}
