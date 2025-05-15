using Project_GameKimCuong._Scripts;
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
        JewelGrid jewelGrid;
        public Form1()
        {
            InitializeComponent();
            jewelGrid = new JewelGrid(this);
        }

    }
}
