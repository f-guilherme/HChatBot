using System;
using System.Windows.Forms;

namespace GChatBot
{
    public partial class MainWindow : Form
    {
        public GChatBot ExtensionChild;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            ExtensionChild = new GChatBot(this);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ExtensionChild.ownerName = textBox1.Text;
        }
    }
}