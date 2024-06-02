using ImageScan.Core;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ImageScan.UX
{
    public partial class MainWindow : Form
    {
        private string _ActiveImage = string.Empty;
        private string ActiveImage
        {
            get { return _ActiveImage; }
            set 
            { 
                _ActiveImage = value;
                label1.Text = $"Active Image: {_ActiveImage}";
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            FormClosing += (s, e) =>
            {
                OCR.Shutdown();
            };
            LoadOcrLanguages();
        }

        private void LoadOcrLanguages()
        {
            string subfolder = "./tessdata";
            string root = Application.StartupPath;
            string path = System.IO.Path.Combine(root, subfolder);
            if (!System.IO.Directory.Exists(path))
            {
                MessageBox.Show("Tesseract data not found");
                return;
            }
            int selection = 0;
            FileInfo[] files = new DirectoryInfo(path).GetFiles("*.traineddata");
            foreach (FileInfo file in files)
            {
                string name = file.Name;
                string language = name.Substring(0, name.IndexOf('.'));
                cb_Language.Items.Add(language);
                if (language == "eng")
                {
                    selection = cb_Language.Items.Count - 1;
                }
            }
            cb_Language.SelectedIndex = selection;
        }

        private void LoadImage(string path)
        {
            Image img = Image.FromFile(path);
            pictureBox1.Image = img;
        }

        private void ScanImage(string path)
        {
            string text = Core.OCR.ParseByLine(path); //Core.OCR.ReadText(path);
            textBox1.Text = text;
        }

        private void tbar_Open_Click(object sender, EventArgs e)
        {
            FileDialog fd = new OpenFileDialog();
            fd.CheckFileExists = true;
            fd.CheckPathExists = true;
            fd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            fd.Title = "Select an image file";
            if (fd.ShowDialog() == DialogResult.OK)
            {
                ActiveImage = fd.FileName;
                LoadingScreen.Visible = true;
                LoadImage(ActiveImage);
                ScanImage(ActiveImage);
                LoadingScreen.Visible = false;
            }
        }

        private void btn_Copy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox1.Text);
            MessageBox.Show("Text copied to clipboard");
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            FileDialog fd = new SaveFileDialog();
            fd.Filter = "Text Files|*.txt";
            fd.Title = "Save text to file";
            if (fd.ShowDialog() == DialogResult.OK)
            {
                string path = fd.FileName;
                System.IO.File.WriteAllText(path, textBox1.Text);
                MessageBox.Show("Text saved to file");
            }
        }

        private void cb_Language_SelectedIndexChanged(object sender, EventArgs e)
        {
            OCR.SetLanguage(cb_Language.SelectedItem.ToString());
            if(pictureBox1.Image != null)
            {
                LoadingScreen.Visible = true;
                ScanImage(ActiveImage);
                LoadingScreen.Visible = false;
            }
        }
    }
}
