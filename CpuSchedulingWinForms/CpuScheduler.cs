using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zen.Barcode;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;

namespace CpuSchedulingWinForms
{
    public partial class CpuScheduler : Form
    {
        private System.Windows.Forms.Button btnHRRN;
        private System.Windows.Forms.Button btnMLFQ;

        public CpuScheduler()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //dashBoardTab.Show();
            this.tabSelection.SelectTab(0);
            this.sidePanel.Height = btnDashBoard.Height;
            this.sidePanel.Top = btnDashBoard.Top;

            //this.btnProductCode.BackColor = Color.Transparent;
            //this.btnCpuScheduler.BackColor = Color.Transparent;
            //this.btnDashBoard.BackColor = Color.DimGray;
        }

        private void btnCpuScheduler_Click(object sender, EventArgs e)
        {
            //this.dashBoardTab.Show();
            this.tabSelection.SelectTab(1);
            this.sidePanel.Height = btnCpuScheduler.Height;
            this.sidePanel.Top = btnCpuScheduler.Top;

            //this.btnProductCode.BackColor = Color.Transparent;         
            //this.btnDashBoard.BackColor = Color.Transparent;
            //this.btnCpuScheduler.BackColor = Color.DimGray;
        }


        //Method used to display metrics needed to demonstrate CPU scheduling
        public static void DisplayMetrics(List<Process> processes, string algorithmName)
        {
            // Calculate metrics
            double avgWaitingTime = processes.Average(p => p.WaitingTime);
            double avgTurnaroundTime = processes.Average(p => p.TurnaroundTime);
            double avgResponseTime = processes.Average(p => p.ResponseTime);
            double cpuUtilization = (processes.Sum(p => p.BurstTime) / (double)(processes.Max(p => p.FinishTime))) * 100;
            double throughput = processes.Count / (double)(processes.Max(p => p.FinishTime));

            // Display metrics in a MessageBox
            StringBuilder metrics = new StringBuilder();
            metrics.AppendLine($"Algorithm: {algorithmName}");
            metrics.AppendLine($"Average Waiting Time (AWT): {avgWaitingTime:F2} ms");
            metrics.AppendLine($"Average Turnaround Time (ATT): {avgTurnaroundTime:F2} ms");
            metrics.AppendLine($"Average Response Time (RT): {avgResponseTime:F2} ms");
            metrics.AppendLine($"CPU Utilization: {cpuUtilization:F2}%");
            metrics.AppendLine($"Throughput: {throughput:F4} processes/ms");
            MessageBox.Show(metrics.ToString(), $"{algorithmName} Metrics");

        }


        private void btnFCFS_Click(object sender, EventArgs e)
        {
            if (txtProcess.Text != "")
            {
                int numberOfProcess = Int16.Parse(txtProcess.Text);

                // Create a list of processes (dummy data for demonstration)
                List<Process> processes = new List<Process>();
                for (int i = 0; i < numberOfProcess; i++)
                {
                    processes.Add(new Process(i + 1, i * 2, (i + 1) * 3, 0)); // Example: ID, ArrivalTime, BurstTime
                }

                // Sort processes by arrival time (FCFS logic)
                processes = processes.OrderBy(p => p.ArrivalTime).ToList();

                // Calculate scheduling metrics
                double currentTime = 0;
                foreach (var process in processes)
                {
                    process.StartTime = Math.Max(currentTime, process.ArrivalTime);
                    process.FinishTime = process.StartTime + process.BurstTime;
                    process.TurnaroundTime = process.FinishTime - process.ArrivalTime;
                    process.WaitingTime = process.TurnaroundTime - process.BurstTime;
                    process.ResponseTime = process.StartTime - process.ArrivalTime;

                    currentTime = process.FinishTime;
                }

                // Update progress bars
                if (numberOfProcess <= 10)
                {
                    this.progressBar1.Increment(4); // CPU progress bar
                    this.progressBar1.SetState(1);
                    this.progressBar2.Increment(13);
                    this.progressBar2.SetState(1);
                }
                else if (numberOfProcess > 10)
                {
                    this.progressBar1.Increment(15);
                    this.progressBar1.SetState(1);
                    this.progressBar2.Increment(38); // Memory progress bar
                    this.progressBar2.SetState(3);
                }

                // Update ListView
                listView1.Clear();
                listView1.View = View.Details;

                listView1.Columns.Add("Process ID", 150, HorizontalAlignment.Center);
                listView1.Columns.Add("Arrival Time", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Burst Time", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Start Time", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Finish Time", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Waiting Time", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Turnaround Time", 100, HorizontalAlignment.Center);

                foreach (var process in processes)
                {
                    var item = new ListViewItem(process.ID.ToString());
                    item.SubItems.Add(process.ArrivalTime.ToString());
                    item.SubItems.Add(process.BurstTime.ToString());
                    item.SubItems.Add(process.StartTime.ToString("F2"));
                    item.SubItems.Add(process.FinishTime.ToString("F2"));
                    item.SubItems.Add(process.WaitingTime.ToString("F2"));
                    item.SubItems.Add(process.TurnaroundTime.ToString("F2"));
                    listView1.Items.Add(item);
                }

                // Display metrics using the DisplayMetrics method
                DisplayMetrics(processes, "FCFS");
            }
            else
            {
                MessageBox.Show("Enter number of processes", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtProcess.Focus();
            }
        }



        private void btnSJF_Click(object sender, EventArgs e)
        {
            if (txtProcess.Text != "")
            {
                // Parse the number of processes
                int numberOfProcess = Int16.Parse(txtProcess.Text);

                // Create a list of processes
                List<Process> processes = new List<Process>();

                // Get burst times for each process
                for (int i = 0; i < numberOfProcess; i++)
                {
                    string burstInput = Microsoft.VisualBasic.Interaction.InputBox($"Enter burst time for Process {i + 1}:", "Burst Time", "", -1, -1);
                    double burstTime = Convert.ToDouble(burstInput);
                    processes.Add(new Process(i + 1, 0, (int)burstTime, 0)); // Assuming arrival time is 0 for simplicity
                }

                // Sort processes by burst time (Shortest Job First)
                processes = processes.OrderBy(p => p.BurstTime).ToList();

                // Calculate metrics
                double currentTime = 0;
                foreach (var process in processes)
                {
                    process.StartTime = currentTime;
                    process.FinishTime = currentTime + process.BurstTime;
                    process.TurnaroundTime = process.FinishTime - process.ArrivalTime;
                    process.WaitingTime = process.TurnaroundTime - process.BurstTime;
                    process.ResponseTime = process.StartTime - process.ArrivalTime;
                    currentTime = process.FinishTime;
                }

                // Update progress bars
                if (numberOfProcess <= 10)
                {
                    this.progressBar1.Increment(4); // CPU progress bar
                    this.progressBar1.SetState(1);
                    this.progressBar2.Increment(13);
                    this.progressBar2.SetState(1);
                }
                else if (numberOfProcess > 10)
                {
                    this.progressBar1.Increment(15);
                    this.progressBar1.SetState(1);
                    this.progressBar2.Increment(38); // Memory progress bar
                    this.progressBar2.SetState(3);
                }

                // Update ListView
                listView1.Clear();
                listView1.View = View.Details;

                listView1.Columns.Add("Process ID", 150, HorizontalAlignment.Center);
                listView1.Columns.Add("Arrival Time", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Burst Time", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Start Time", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Finish Time", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Waiting Time", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Turnaround Time", 100, HorizontalAlignment.Center);

                foreach (var process in processes)
                {
                    var item = new ListViewItem(process.ID.ToString());
                    item.SubItems.Add(process.ArrivalTime.ToString());
                    item.SubItems.Add(process.BurstTime.ToString());
                    item.SubItems.Add(process.StartTime.ToString("F2"));
                    item.SubItems.Add(process.FinishTime.ToString("F2"));
                    item.SubItems.Add(process.WaitingTime.ToString("F2"));
                    item.SubItems.Add(process.TurnaroundTime.ToString("F2"));
                    listView1.Items.Add(item);
                }

                // Display metrics using the DisplayMetrics method
                DisplayMetrics(processes, "SJF");
            }
            else
            {
                MessageBox.Show("Enter number of processes", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtProcess.Focus();
            }

        }

        private void txtProcess_TextChanged(object sender, EventArgs e)
        {

        }

        private void restartApp_Click(object sender, EventArgs e)
        {
            this.Hide();
            CpuScheduler cpuScheduler = new CpuScheduler();
            cpuScheduler.ShowDialog();
        }

        private void btnBarcode_Click(object sender, EventArgs e)
        {
            if (txtCodeInput.Text != "")
            {
                string barcode = txtCodeInput.Text;
                //Code128BarcodeDraw barcode = BarcodeDrawFactory.Code128WithChecksum;
                //pictureBoxCodeOutput.Image = barcode.Draw(barcodeInput, 30);
                //pictureBoxCodeOutput.Height = barcode.Draw(txtCodeInput.Text, 150).Height;
                //pictureBoxCodeOutput.Width = barcode.Draw(txtCodeInput.Text, 150).Width;

                Bitmap bitmap = new Bitmap(barcode.Length * 36, 109);   //40, 150
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    Font font = new Font("IDAutomationHC39M Free Version", 25);
                    PointF point = new PointF(2f, 2f);
                    SolidBrush black = new SolidBrush(Color.Black);
                    SolidBrush white = new SolidBrush(Color.White);
                    graphics.FillRectangle(white, 0, 0, bitmap.Width, bitmap.Height);
                    graphics.DrawString(barcode, font, black, point);
                }
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    bitmap.Save(memoryStream, ImageFormat.Png);
                    pictureBoxCodeOutput.Image = bitmap;
                    //pictureBoxCodeOutput.Height = bitmap.Height;
                    //pictureBoxCodeOutput.Width = bitmap.Width;
                }
            }
            else
            {
                MessageBox.Show("No Input", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCodeInput.Focus();
            }

        }

        private void btnQrcode_Click(object sender, EventArgs e)
        {
            if (txtCodeInput.Text != "")
            {
                CodeQrBarcodeDraw codeQr = BarcodeDrawFactory.CodeQr;
                pictureBoxCodeOutput.Image = codeQr.Draw(txtCodeInput.Text, 100);
                //string barcode = txtCodeInput.Text;
                //Bitmap bitmap = new Bitmap(barcode.Length * 40, 150);
                //pictureBoxCodeOutput.Image = bitmap; 
            }
            else
            {
                MessageBox.Show("No Input", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCodeInput.Focus();
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (pictureBoxCodeOutput.Image == null)
            {
                return;
            }
            else if (pictureBoxCodeOutput.Image != null)
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "PNG|*.png|JPEG|*.jpeg|ICON|*.ico" })
                {
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        pictureBoxCodeOutput.Image.Save(saveFileDialog.FileName);
                    }
                }
            }
        }

        private void btnProductCode_Click(object sender, EventArgs e)
        {
            //this.dashBoardTab.Show();
            this.tabSelection.SelectTab(2);
            this.sidePanel.Height = btnProductCode.Height;
            this.sidePanel.Top = btnProductCode.Top;

            //this.btnCpuScheduler.BackColor = Color.Transparent;
            //this.btnDashBoard.BackColor = Color.Transparent;
            //this.btnProductCode.BackColor = Color.DimGray;
        }

        private void CpuScheduler_Load(object sender, EventArgs e)
        {
            this.sidePanel.Height = btnDashBoard.Height;
            this.sidePanel.Top = btnDashBoard.Top;
            this.progressBar1.Increment(5);
            this.progressBar2.Increment(17);
            listView1.View = View.Details;
            listView1.GridLines = true;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //Application.Exit();
            timer1.Start();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void btnRoundRobin_Click(object sender, EventArgs e)
        {
            if (txtProcess.Text != "")
            {
                // Parse the number of processes
                int numberOfProcess = Int16.Parse(txtProcess.Text);

                // Create a list of processes
                List<Process> processes = new List<Process>();

                // Get arrival time and burst time for each process
                for (int i = 0; i < numberOfProcess; i++)
                {
                    int arrivalTime = Convert.ToInt32(Microsoft.VisualBasic.Interaction.InputBox($"Enter Arrival Time for Process {i + 1}:", "Arrival Time"));
                    int burstTime = Convert.ToInt32(Microsoft.VisualBasic.Interaction.InputBox($"Enter Burst Time for Process {i + 1}:", "Burst Time"));

                    // Add each process to the list
                    processes.Add(new Process(i + 1, arrivalTime, burstTime, 0));
                }

                // Get the time quantum
                int timeQuantum = Convert.ToInt32(Microsoft.VisualBasic.Interaction.InputBox("Enter Time Quantum:", "Time Quantum"));

               

                // Update progress bars
                if (numberOfProcess <= 10)
                {
                    this.progressBar1.Increment(4); // CPU progress bar
                    this.progressBar1.SetState(1);
                    this.progressBar2.Increment(13);
                    this.progressBar2.SetState(1);
                }
                else if (numberOfProcess > 10)
                {
                    this.progressBar1.Increment(15);
                    this.progressBar1.SetState(1);
                    this.progressBar2.Increment(38); // Memory progress bar
                    this.progressBar2.SetState(3);
                }

                // Update ListView
                listView1.Clear();
                listView1.View = View.Details;

                listView1.Columns.Add("Process ID", 150, HorizontalAlignment.Center);
                listView1.Columns.Add("Arrival Time", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Burst Time", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Start Time", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Finish Time", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Waiting Time", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Turnaround Time", 100, HorizontalAlignment.Center);

                foreach (var process in processes)
                {
                    var item = new ListViewItem(process.ID.ToString());
                    item.SubItems.Add(process.ArrivalTime.ToString());
                    item.SubItems.Add(process.BurstTime.ToString());
                    item.SubItems.Add(process.StartTime.ToString("F2"));
                    item.SubItems.Add(process.FinishTime.ToString("F2"));
                    item.SubItems.Add(process.WaitingTime.ToString("F2"));
                    item.SubItems.Add(process.TurnaroundTime.ToString("F2"));
                    listView1.Items.Add(item);
                }

                // Display metrics using the DisplayMetrics method
                DisplayMetrics(processes, "Round Robin");
            }
            else
            {
                MessageBox.Show("Enter number of processes", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtProcess.Focus();
            }
        }

        private void btnPriority_Click(object sender, EventArgs e)
        {
            if (txtProcess.Text != "")
            {
                // Parse the number of processes
                int numberOfProcesses = Convert.ToInt32(txtProcess.Text);

                // Create a list of processes
                List<Process> processes = new List<Process>();

                // Get burst time and priority for each process
                for (int i = 0; i < numberOfProcesses; i++)
                {
                    int burstTime = Convert.ToInt32(Microsoft.VisualBasic.Interaction.InputBox($"Enter Burst Time for Process {i + 1}:", "Burst Time"));
                    int priority = Convert.ToInt32(Microsoft.VisualBasic.Interaction.InputBox($"Enter Priority for Process {i + 1}:", "Priority"));

                    // Add each process to the list
                    processes.Add(new Process(i + 1, 0, burstTime, priority)); // Assuming arrival time is 0 for simplicity
                }

                // Sort processes by priority (lower value = higher priority)
                processes = processes.OrderBy(p => p.Priority).ToList();

                // Calculate metrics
                double currentTime = 0;
                foreach (var process in processes)
                {
                    process.StartTime = currentTime;
                    process.FinishTime = currentTime + process.BurstTime;
                    process.TurnaroundTime = process.FinishTime - process.ArrivalTime;
                    process.WaitingTime = process.TurnaroundTime - process.BurstTime;
                    process.ResponseTime = process.StartTime - process.ArrivalTime;
                    currentTime = process.FinishTime;
                }

                // Update ListView
                listView1.Clear();
                listView1.View = View.Details;

                listView1.Columns.Add("Process ID", 150, HorizontalAlignment.Center);
                listView1.Columns.Add("Priority", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Burst Time", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Start Time", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Finish Time", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Waiting Time", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Turnaround Time", 100, HorizontalAlignment.Center);

                foreach (var process in processes)
                {
                    var item = new ListViewItem(process.ID.ToString());
                    item.SubItems.Add(process.Priority.ToString());
                    item.SubItems.Add(process.BurstTime.ToString());
                    item.SubItems.Add(process.StartTime.ToString("F2"));
                    item.SubItems.Add(process.FinishTime.ToString("F2"));
                    item.SubItems.Add(process.WaitingTime.ToString("F2"));
                    item.SubItems.Add(process.TurnaroundTime.ToString("F2"));
                    listView1.Items.Add(item);
                }

                // Display metrics using the DisplayMetrics method
                DisplayMetrics(processes, "Priority Scheduling");
            }
            else
            {
                MessageBox.Show("Enter number of processes", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtProcess.Focus();
            }
        }


        //event handler for Highest Response Ration Next 
        private void btnHRRN_Click(object sender, EventArgs e)
        {
            // Check if the user entered the number of processes
            if (txtProcess.Text != "")
            {
                // Create a list of processes
                List<Process> processes = new List<Process>();

                // Get the number of processes from the user input
                int numberOfProcesses = Convert.ToInt32(txtProcess.Text);

                // Get arrival time and burst time for each process from the user
                for (int i = 0; i < numberOfProcesses; i++)
                {
                    int arrivalTime = Convert.ToInt32(Microsoft.VisualBasic.Interaction.InputBox($"Enter Arrival Time for Process {i + 1}:", "Arrival Time"));
                    int burstTime = Convert.ToInt32(Microsoft.VisualBasic.Interaction.InputBox($"Enter Burst Time for Process {i + 1}:", "Burst Time"));

                    // Add each process to the list with initial values for start time, finish time, waiting time, and turnaround time
                    processes.Add(new Process(i + 1, arrivalTime, burstTime, 0));
                }

                // Call the HRRN algorithm
                Algorithms.hrrnAlgorithm(processes);

                // Calculate metrics
                double avgWaitingTime = processes.Average(p => p.WaitingTime);
                double avgTurnaroundTime = processes.Average(p => p.TurnaroundTime);
                double avgResponseTime = processes.Average(p => p.ResponseTime);
                double cpuUtilization = (processes.Sum(p => p.BurstTime) / (double)(processes.Max(p => p.FinishTime))) * 100;
                double throughput = numberOfProcesses / (double)(processes.Max(p => p.FinishTime));

                // Display metrics in a MessageBox
                StringBuilder metrics = new StringBuilder();
                metrics.AppendLine($"Average Waiting Time (AWT): {avgWaitingTime:F2} ms");
                metrics.AppendLine($"Average Turnaround Time (ATT): {avgTurnaroundTime:F2} ms");
                metrics.AppendLine($"Average Response Time (RT): {avgResponseTime:F2} ms");
                metrics.AppendLine($"CPU Utilization: {cpuUtilization:F2}%");
                metrics.AppendLine($"Throughput: {throughput:F4} processes/ms");
                MessageBox.Show(metrics.ToString(), "HRRN Metrics");

                // Display results in the ListView
                listView1.Clear();
                listView1.View = View.Details;
                listView1.Columns.Add("Process ID", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Arrival Time", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Burst Time", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Start Time", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Finish Time", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Waiting Time", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Turnaround Time", 100, HorizontalAlignment.Center);
                listView1.Columns.Add("Response Time", 100, HorizontalAlignment.Center);

                foreach (var process in processes)
                {
                    var item = new ListViewItem(process.ID.ToString());
                    item.SubItems.Add(process.ArrivalTime.ToString());
                    item.SubItems.Add(process.BurstTime.ToString());
                    item.SubItems.Add(process.StartTime.ToString("F2"));
                    item.SubItems.Add(process.FinishTime.ToString("F2"));
                    item.SubItems.Add(process.WaitingTime.ToString("F2"));
                    item.SubItems.Add(process.TurnaroundTime.ToString("F2"));
                    item.SubItems.Add(process.ResponseTime.ToString("F2"));
                    listView1.Items.Add(item);
                }
            }
            else
            {
                // Show an error message if the number of processes is not entered
                MessageBox.Show("Enter number of processes", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtProcess.Focus();
            }
        }


        private void btnMLFQ_Click(object sender, EventArgs e)
        {
            if (txtProcess.Text != "")
            {
                // Parse the number of processes
                if (int.TryParse(txtProcess.Text, out int numberOfProcesses))
                {
                    // Create a list of processes
                    List<Process> processes = new List<Process>();

                    // Get process details from the user
                    for (int i = 0; i < numberOfProcesses; i++)
                    {
                        int arrivalTime = Convert.ToInt32(Microsoft.VisualBasic.Interaction.InputBox($"Enter Arrival Time for Process {i + 1}:", "Arrival Time"));
                        int burstTime = Convert.ToInt32(Microsoft.VisualBasic.Interaction.InputBox($"Enter Burst Time for Process {i + 1}:", "Burst Time"));
                        processes.Add(new Process(i + 1, arrivalTime, burstTime, 0));
                    }

                    // Define time quantums for MLFQ
                    int[] timeQuantums = { 4, 8, 16 };

                    // Call the MLFQ algorithm
                    Algorithms.mlfqAlgorithm(processes, timeQuantums);

                    // Calculate metrics
                    double avgWaitingTime = processes.Average(p => p.WaitingTime);
                    double avgTurnaroundTime = processes.Average(p => p.TurnaroundTime);
                    double avgResponseTime = processes.Average(p => p.ResponseTime);
                    double cpuUtilization = (processes.Sum(p => p.BurstTime) / (double)(processes.Max(p => p.FinishTime))) * 100;
                    double throughput = numberOfProcesses / (double)(processes.Max(p => p.FinishTime));

                    // Display metrics in a MessageBox
                    StringBuilder metrics = new StringBuilder();
                    metrics.AppendLine($"Average Waiting Time (AWT): {avgWaitingTime:F2} ms");
                    metrics.AppendLine($"Average Turnaround Time (ATT): {avgTurnaroundTime:F2} ms");
                    metrics.AppendLine($"Average Response Time (RT): {avgResponseTime:F2} ms");
                    metrics.AppendLine($"CPU Utilization: {cpuUtilization:F2}%");
                    metrics.AppendLine($"Throughput: {throughput:F4} processes/ms");
                    MessageBox.Show(metrics.ToString(), "MLFQ Metrics");

                    // Display results in the ListView
                    listView1.Clear();
                    listView1.View = View.Details;
                    listView1.Columns.Add("Process ID", 100, HorizontalAlignment.Center);
                    listView1.Columns.Add("Arrival Time", 100, HorizontalAlignment.Center);
                    listView1.Columns.Add("Burst Time", 100, HorizontalAlignment.Center);
                    listView1.Columns.Add("Start Time", 100, HorizontalAlignment.Center);
                    listView1.Columns.Add("Finish Time", 100, HorizontalAlignment.Center);
                    listView1.Columns.Add("Waiting Time", 100, HorizontalAlignment.Center);
                    listView1.Columns.Add("Turnaround Time", 100, HorizontalAlignment.Center);
                    listView1.Columns.Add("Response Time", 100, HorizontalAlignment.Center);

                    foreach (var process in processes)
                    {
                        var item = new ListViewItem(process.ID.ToString());
                        item.SubItems.Add(process.ArrivalTime.ToString());
                        item.SubItems.Add(process.BurstTime.ToString());
                        item.SubItems.Add(process.StartTime.ToString("F2")); // Format to 2 decimal places
                        item.SubItems.Add(process.FinishTime.ToString("F2")); // Format to 2 decimal places
                        item.SubItems.Add(process.WaitingTime.ToString("F2")); // Format to 2 decimal places
                        item.SubItems.Add(process.TurnaroundTime.ToString("F2"));
                        item.SubItems.Add(process.ResponseTime.ToString("F2"));
                        listView1.Items.Add(item);
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid number of processes.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtProcess.Focus();
                }
            }
            else
            {
                MessageBox.Show("Enter number of processes", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtProcess.Focus();
            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if(this.Opacity > 0.0)
            {
                this.Opacity -= 0.021;
            } else
            {
                timer1.Stop();
                Application.Exit();
            }
        }

        private void txtCodeInput_Click(object sender, EventArgs e)
        {
            this.txtCodeInput.Clear();
        }
    }
}
