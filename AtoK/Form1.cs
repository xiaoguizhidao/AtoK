using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ConvertToKicad;

namespace AtoK
{

    public partial class Form1 : Form
    {
        ConcurrentQueue<string> dq = new ConcurrentQueue<string>();
        //       Stream myStream;
        //        StreamWriter myWriter;
        public static Thread t; // ues for running the convert in  the background
        private delegate void UpdateOutputDelegate(string s, Color colour);

        private UpdateOutputDelegate updateoutputDelegate = null;

        public void UpdateOutput(string s, Color colour)
        {
            outputList_Add(s, colour);
            outputList.Update();
        }


        public class Line
        {
            public string Str;
            public Color ForeColor;

            public Line(string str, Color color)
            {
                Str = str;
                ForeColor = color;
            }
        };

        int outputlist_width = 0;
        ArrayList lines = new ArrayList();

        public Form1()
        {
            Screen scrn = Screen.FromControl(this);

            InitializeComponent();

            outputList_Initialize();
            SaveExtractedDocs.CheckState = Properties.Settings.Default.SaveDocs ? CheckState.Checked: CheckState.Unchecked;
            LibraryGen.CheckState = Properties.Settings.Default.GenLib ? CheckState.Checked : CheckState.Unchecked;
            Verbose.CheckState = Properties.Settings.Default.Verbose ? CheckState.Checked : CheckState.Unchecked;
            fileName.Text = Properties.Settings.Default.LastFile;
        }

        // shutdown the worker thread when the form closes
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        #region Output window

        bool scrolling = true;

        ContextMenu popUpMenu;

        public Line outputList_Add(string str, Color color)
        {
            Line newLine = new Line(str, color);
            lines.Add(newLine);
            int testWidth = TextRenderer.MeasureText(str,
                                            outputList.Font, outputList.ClientSize,
                                            TextFormatFlags.NoPrefix).Width;
            if (testWidth > outputlist_width)
                outputlist_width = testWidth;

            Invoke((MethodInvoker)(() => outputList.HorizontalExtent = outputlist_width));
            Invoke((MethodInvoker)(() => outputList.Items.Add(newLine)));
            Invoke((MethodInvoker)(() => outputList_Scroll()));
            return newLine;
        }

        public void outputList_Update()
        {
            Invoke((MethodInvoker)(() => outputList.Update()));
        }

        private void outputList_Initialize()
        {
            // owner draw for listbox so we can add color
            outputList.DrawMode = DrawMode.OwnerDrawFixed;
            outputList.DrawItem += new DrawItemEventHandler(outputList_DrawItem);
            outputList.ClearSelected();

            // build the outputList context menu
            popUpMenu = new ContextMenu();
            popUpMenu.MenuItems.Add("&Copy", new EventHandler(outputList_Copy));
            popUpMenu.MenuItems[0].Visible = true;
            popUpMenu.MenuItems[0].Enabled = false;
            popUpMenu.MenuItems[0].Shortcut = Shortcut.CtrlC;
            popUpMenu.MenuItems[0].ShowShortcut = true;

            popUpMenu.MenuItems.Add("Copy All", new EventHandler(outputList_CopyAll));
            popUpMenu.MenuItems[1].Visible = true;

            popUpMenu.MenuItems.Add("Select &All", new EventHandler(outputList_SelectAll));
            popUpMenu.MenuItems[2].Visible = true;
            popUpMenu.MenuItems[2].Shortcut = Shortcut.CtrlA;
            popUpMenu.MenuItems[2].ShowShortcut = true;

            popUpMenu.MenuItems.Add("Unselect", new EventHandler(outputList_ClearSelected));
            popUpMenu.MenuItems[3].Visible = true;
            popUpMenu.MenuItems.Add("Delete selected", new EventHandler(outputList_DeleteSelected));
            popUpMenu.MenuItems[4].Visible = true;
            popUpMenu.MenuItems.Add("Clear All", new EventHandler(outputList_ClearAll));
            popUpMenu.MenuItems[5].Visible = true;
            popUpMenu.MenuItems.Add("Toggle Scrolling", new EventHandler(outputList_ToggleScrolling));

            outputList.ContextMenu = popUpMenu;
        }

        void outputList_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index >= 0 && e.Index < outputList.Items.Count)
            {
                Line line = (Line)outputList.Items[e.Index];

                // if selected, make the text color readable
                Color color = line.ForeColor;
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    color = Color.White;    // make it readable
                }

                e.Graphics.DrawString(line.Str, e.Font, new SolidBrush(color),
                    e.Bounds, StringFormat.GenericDefault);
            }
            e.DrawFocusRectangle();
        }

        void outputList_Scroll()
        {
            if (scrolling)
            {
                int itemsPerPage = (int)(outputList.Height / outputList.ItemHeight);
                outputList.TopIndex = outputList.Items.Count - itemsPerPage;
                if (outputList.HorizontalScrollbar && (itemsPerPage < outputList.Items.Count))
                    outputList.TopIndex++;
            }
        }

        private void outputList_SelectedIndexChanged(object sender, EventArgs e)
        {
            popUpMenu.MenuItems[0].Enabled = (outputList.SelectedItems.Count > 0);
        }

        private void outputList_Copy(object sender, EventArgs e)
        {
            int iCount = outputList.SelectedItems.Count;
            if (iCount > 0)
            {
                String[] source = new String[iCount];
                for (int i = 0; i < iCount; ++i)
                {
                    source[i] = ((Line)outputList.SelectedItems[i]).Str;
                }

                String dest = String.Join("\r\n", source);
                Clipboard.SetText(dest);
            }
        }

        private void outputList_CopyAll(object sender, EventArgs e)
        {
            int iCount = outputList.Items.Count;
            if (iCount > 0)
            {
                String[] source = new String[iCount];
                for (int i = 0; i < iCount; ++i)
                {
                    source[i] = ((Line)outputList.Items[i]).Str;
                }

                String dest = String.Join("\r\n", source);
                Clipboard.SetText(dest);
            }
        }

        private void outputList_SelectAll(object sender, EventArgs e)
        {
            outputList.BeginUpdate();
            for (int i = 0; i < outputList.Items.Count; ++i)
            {
                outputList.SetSelected(i, true);
            }
            outputList.EndUpdate();
        }

        private void outputList_ClearSelected(object sender, EventArgs e)
        {
            outputList.ClearSelected();
            outputList.SelectedItem = -1;
        }

        private void outputList_DeleteSelected(object sender, EventArgs e)
        {
            outputList.BeginUpdate();
            // Remove each item in reverse order to maintain integrity
            var selectedIndices = new List<int>(outputList.SelectedIndices.Cast<int>());
            selectedIndices.Reverse();
            selectedIndices.ForEach(index => outputList.Items.RemoveAt(index));

            outputList.SelectedItem = -1;
            outputList.EndUpdate();
        }

        private void outputList_ClearAll(object sender, EventArgs e)
        {
            outputList.Items.Clear();
            outputList.SelectedItem = -1;
        }

        #endregion

        #region User interaction

        /// <summary>
        /// Close the application
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
		/// toggle scrolling
		/// </summary>
		private void outputList_ToggleScrolling(object sender, EventArgs e)
        {
            scrolling = !scrolling;
            outputList_Scroll();
        }

        #endregion

        bool isPointVisibleOnAScreen(Point p)
        {
            foreach (Screen s in Screen.AllScreens)
            {
                if (p.X < s.Bounds.Right && p.X > s.Bounds.Left && p.Y > s.Bounds.Top && p.Y < s.Bounds.Bottom)
                    return true;
            }
            return false;
        }

        bool isFormFullyVisible(Point p, Size size)
        {
            return isPointVisibleOnAScreen(p)
                && isPointVisibleOnAScreen(new Point(p.X + size.Width, p.Y))
                && isPointVisibleOnAScreen(new Point(p.X + size.Width, p.Y + size.Height));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
             updateoutputDelegate = new UpdateOutputDelegate(UpdateOutput);

            if (!isFormFullyVisible(Properties.Settings.Default.F1Location, Properties.Settings.Default.F1Size) || Properties.Settings.Default.F1Size.Width == 0 || Properties.Settings.Default.F1Size.Height == 0)
            {
                // first start or form not visible due to monitor setup changing since last saved
                // optional: add default values
            }
            else
            {
                this.WindowState = Properties.Settings.Default.F1State;

                // we don't want a minimized window at startup
                if (this.WindowState == FormWindowState.Minimized) this.WindowState = FormWindowState.Normal;

                this.Location = Properties.Settings.Default.F1Location;
                this.Size = Properties.Settings.Default.F1Size;
            }
            SaveExtractedDocs.CheckState = Properties.Settings.Default.SaveDocs ? CheckState.Checked : CheckState.Unchecked;
            LibraryGen.CheckState = Properties.Settings.Default.GenLib ? CheckState.Checked : CheckState.Unchecked;
            Verbose.CheckState = Properties.Settings.Default.Verbose ? CheckState.Checked : CheckState.Unchecked;
            ConvertPCBDoc.Verbose = Verbose.CheckState == CheckState.Checked;
            fileName.Text = Properties.Settings.Default.LastFile;
//            Debug.WriteLine($"File={fileName.Text}");
        }

        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            if ( t != null && t.IsAlive)
            {
                t.Abort();
            }
            Properties.Settings.Default.F1State = this.WindowState;
            if (this.WindowState == FormWindowState.Normal)
            {
                // save location and size if the state is normal
                Properties.Settings.Default.F1Location = this.Location;
                Properties.Settings.Default.F1Size = this.Size;
            }
            else
            {
                // save the RestoreBounds if the form is minimized or maximized!
                Properties.Settings.Default.F1Location = this.RestoreBounds.Location;
                Properties.Settings.Default.F1Size = this.RestoreBounds.Size;
            }
            Properties.Settings.Default.SaveDocs = SaveExtractedDocs.CheckState == CheckState.Checked;
            Properties.Settings.Default.GenLib = LibraryGen.CheckState == CheckState.Checked;
            Properties.Settings.Default.Verbose = Verbose.CheckState == CheckState.Checked;
            Properties.Settings.Default.LastFile = fileName.Text;

            // don't forget to save the settings
            Properties.Settings.Default.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Cancel")
            {
                t.Abort(); // terminate the thread
                while (ConvertPCBDoc.ConvertRunning)
                    Thread.Sleep(1000);
                t = null;
                button1.Text = "Convert to Kicad";
                EnableControls();
                return;
            }
            if (File.Exists(fileName.Text))
            {
                button1.Text = "Cancel";
                this.Update();
                LibraryGen.Enabled = false;
                SaveExtractedDocs.Enabled = false;
                Verbose.Enabled = false;
                fileName.Enabled = false;
//                button1.Enabled = false;
                button2.Enabled = false;
                //start the conversion
                Cursor.Current = Cursors.WaitCursor;
                t = new Thread(() =>
                {
                    Program.ConvertPCB.ConvertFile(fileName.Text, SaveExtractedDocs.CheckState == CheckState.Checked, LibraryGen.CheckState == CheckState.Checked);
                });

                t.Start();
                timer1.Enabled = true;
                timer1.Interval = 500;
            }
            else
                outputList_Add("File doesn't exist", System.Drawing.Color.Red);
        }

        public void EnableControls()
        {
            LibraryGen.Enabled = true;
            LibraryGen.Update();
            SaveExtractedDocs.Enabled = true;
            SaveExtractedDocs.Update();
            Verbose.Enabled = true;
            Verbose.Update();
            fileName.Enabled = true;
            fileName.Update();
            button1.Enabled = true;
            button1.Update();
            button2.Enabled = true;
            button2.Update();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {

                InitialDirectory = @"D:\",
                Title = "Browse Altium PCBDoc Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "pcbdoc",
                Filter = "pcb files (*.pcbdoc)|*.pcbdoc",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName.Text = openFileDialog1.FileName;
            }

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.MinimumSize = new Size(this.Width, 0);
            this.MaximumSize = new Size(this.Width, Int32.MaxValue);
            Control control = (Control)sender;
            // resize the output window 
            // set top to bottom of button1
            // set bottom to bottom of form
            // set left to left of form
            // set right to right of form
            control.Width = button1.Right;
            outputList.Width = this.Width-30;
            outputList.Left = fileName.Left;
            outputList.Top = button1.Bottom + 10;
            outputList.Height = control.Height - button1.Bottom - 50;
        }

        private void Verbose_Click(object sender, EventArgs e)
        {
            ConvertPCBDoc.Verbose = Verbose.Checked;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (busy.BackColor == Color.Red)
                busy.BackColor = Color.White;
            else
                busy.BackColor = Color.Red;
            if (t == null || t.IsAlive == false)
            {
                button1.Text = "Convert to Kicad";
                EnableControls();
                timer1.Enabled = false;
                Cursor.Current = Cursors.Default;
                busy.Enabled = false;
                busy.Visible = false;
                busy.Hide();
                this.Update();
            }
            else
            {
                busy.Enabled = true;
                busy.Visible = true;
            }
        }
    }
 }