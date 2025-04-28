using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//Add a process class to track performance data
public class Process
{
    public int ID { get; set; }
    public int ArrivalTime { get; set; }
    public int BurstTime { get; set; }
    public int Priority { get; set; }

 //scheduling results:
    public int RemainingTime { get; set; }   
    public double StartTime { get; set; }     
    public double FinishTime { get; set; }    
    public double WaitingTime { get; set; }   
    public double TurnaroundTime { get; set; } 
    public double ResponseTime { get; set; }
    public int TimeSlice { get; set; }
    public double AvgWaitingTime { get; set; }
    public double AvgTurnaroundTime { get; set; }
    public double AvgResponseTime { get; set; }
    public double CpuUtilization { get; set; }
    public double Throughput { get; set; }
    
    //Create Process Class - represents processes in CPU scheduling
    public Process(int id, int arrivalTime, int burstTime, int priority)
    {
        //Declare variables
        ID = id;
        ArrivalTime = arrivalTime;
        BurstTime = burstTime;
        Priority = priority;//used in priority scheduling
        RemainingTime = burstTime; //At start, remaining time = burst time

        // Initialize fields to 0
        StartTime = 0;
        FinishTime = 0;
        WaitingTime = 0;
        TurnaroundTime = 0;
        TimeSlice = 0;

    }
}

namespace CpuSchedulingWinForms
{
    //Class for algorithms
    public static class Algorithms
    {
        //First Come First Served 
        public static void fcfsAlgorithm(string userInput)
        {
            // Parse the input string to extract process details
            string[] processEntries = userInput.Split(';');
            int np = processEntries.Length; // Number of processes

            double[] bp = new double[np]; // Burst times
            double[] wtp = new double[np]; // Waiting times
            double twt = 0.0, awt; // Total and average waiting time

            // Parse burst times from the input
            for (int i = 0; i < np; i++)
            {
                string[] processDetails = processEntries[i].Split(',');
                if (processDetails.Length < 3)
                {
                    MessageBox.Show("Invalid input format for process data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Extract burst time (3rd value in each process entry)
                if (!double.TryParse(processDetails[2], out bp[i]))
                {
                    MessageBox.Show($"Invalid burst time for process {i + 1}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Calculate waiting times
            for (int i = 0; i < np; i++)
            {
                if (i == 0)
                {
                    wtp[i] = 0; // First process has no waiting time
                }
                else
                {
                    wtp[i] = wtp[i - 1] + bp[i - 1];
                }
                MessageBox.Show($"Waiting time for P{i + 1} = {wtp[i]}", "Job Queue", MessageBoxButtons.OK, MessageBoxIcon.None);
            }

            // Calculate total and average waiting time
            for (int i = 0; i < np; i++)
            {
                twt += wtp[i];
            }
            awt = twt / np;

            // Display average waiting time
            MessageBox.Show($"Average waiting time for {np} processes = {awt} sec(s)", "Average Waiting Time", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //Shortest Job First
        public static void sjfAlgorithm(string userInput)
        {
            int np = Convert.ToInt16(userInput);

            double[] bp = new double[np];
            double[] wtp = new double[np];
            double[] p = new double[np];
            double twt = 0.0, awt; 
            int x, num;
            double temp = 0.0;
            bool found = false;

            DialogResult result = MessageBox.Show("Shortest Job First Scheduling", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                for (num = 0; num <= np - 1; num++)
                {
                    string input =
                        Microsoft.VisualBasic.Interaction.InputBox("Enter burst time: ",
                                                           "Burst time for P" + (num + 1),
                                                           "",
                                                           -1, -1);

                    bp[num] = Convert.ToInt64(input);
                }
                for (num = 0; num <= np - 1; num++)
                {
                    p[num] = bp[num];
                }
                for (x = 0; x <= np - 2; x++)
                {
                    for (num = 0; num <= np - 2; num++)
                    {
                        if (p[num] > p[num + 1])
                        {
                            temp = p[num];
                            p[num] = p[num + 1];
                            p[num + 1] = temp;
                        }
                    }
                }
                for (num = 0; num <= np - 1; num++)
                {
                    if (num == 0)
                    {
                        for (x = 0; x <= np - 1; x++)
                        {
                            if (p[num] == bp[x] && found == false)
                            {
                                wtp[num] = 0;
                                MessageBox.Show("Waiting time for P" + (x + 1) + " = " + wtp[num], "Waiting time:", MessageBoxButtons.OK, MessageBoxIcon.None);
                                //Console.WriteLine("\nWaiting time for P" + (x + 1) + " = " + wtp[num]);
                                bp[x] = 0;
                                found = true;
                            }
                        }
                        found = false;
                    }
                    else
                    {
                        for (x = 0; x <= np - 1; x++)
                        {
                            if (p[num] == bp[x] && found == false)
                            {
                                wtp[num] = wtp[num - 1] + p[num - 1];
                                MessageBox.Show("Waiting time for P" + (x + 1) + " = " + wtp[num], "Waiting time", MessageBoxButtons.OK, MessageBoxIcon.None);
                                //Console.WriteLine("\nWaiting time for P" + (x + 1) + " = " + wtp[num]);
                                bp[x] = 0;
                                found = true;
                            }
                        }
                        found = false;
                    }
                }
                for (num = 0; num <= np - 1; num++)
                {
                    twt = twt + wtp[num];
                }
                MessageBox.Show("Average waiting time for " + np + " processes" + " = " + (awt = twt / np) + " sec(s)", "Average waiting time", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //Priority Algorithm
        public static void priorityAlgorithm(string userInput)
        {
            int np = Convert.ToInt16(userInput);

            DialogResult result = MessageBox.Show("Priority Scheduling ", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                double[] bp = new double[np];
                double[] wtp = new double[np + 1];
                int[] p = new int[np];
                int[] sp = new int[np];
                int x, num;
                double twt = 0.0;
                double awt;
                int temp = 0;
                bool found = false;
                for (num = 0; num <= np - 1; num++)
                {
                    string input =
                        Microsoft.VisualBasic.Interaction.InputBox("Enter burst time: ",
                                                           "Burst time for P" + (num + 1),
                                                           "",
                                                           -1, -1);

                    bp[num] = Convert.ToInt64(input);
                }
                for (num = 0; num <= np - 1; num++)
                {
                    string input2 =
                        Microsoft.VisualBasic.Interaction.InputBox("Enter priority: ",
                                                           "Priority for P" + (num + 1),
                                                           "",
                                                           -1, -1);

                    p[num] = Convert.ToInt16(input2);
                }
                for (num = 0; num <= np - 1; num++)
                {
                    sp[num] = p[num];
                }
                for (x = 0; x <= np - 2; x++)
                {
                    for (num = 0; num <= np - 2; num++)
                    {
                        if (sp[num] > sp[num + 1])
                        {
                            temp = sp[num];
                            sp[num] = sp[num + 1];
                            sp[num + 1] = temp;
                        }
                    }
                }
                for (num = 0; num <= np - 1; num++)
                {
                    if (num == 0)
                    {
                        for (x = 0; x <= np - 1; x++)
                        {
                            if (sp[num] == p[x] && found == false)
                            {
                                wtp[num] = 0;
                                MessageBox.Show("Waiting time for P" + (x + 1) + " = " + wtp[num], "Waiting time", MessageBoxButtons.OK);
                                //Console.WriteLine("\nWaiting time for P" + (x + 1) + " = " + wtp[num]);
                                temp = x;
                                p[x] = 0;
                                found = true;
                            }
                        }
                        found = false;
                    }
                    else
                    {
                        for (x = 0; x <= np - 1; x++)
                        {
                            if (sp[num] == p[x] && found == false)
                            {
                                wtp[num] = wtp[num - 1] + bp[temp];
                                MessageBox.Show("Waiting time for P" + (x + 1) + " = " + wtp[num], "Waiting time", MessageBoxButtons.OK);
                                //Console.WriteLine("\nWaiting time for P" + (x + 1) + " = " + wtp[num]);
                                temp = x;
                                p[x] = 0;
                                found = true;
                            }
                        }
                        found = false;
                    }
                }
                for (num = 0; num <= np - 1; num++)
                {
                    twt = twt + wtp[num];
                }
                MessageBox.Show("Average waiting time for " + np + " processes" + " = " + (awt = twt / np) + " sec(s)", "Average waiting time", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //Console.WriteLine("\n\nAverage waiting time: " + (awt = twt / np));
                //Console.ReadLine();
            }
            else
            {
                //this.Hide();
            }
        }

        //Round Robin
        public static void roundRobinAlgorithm(List<Process> processes, string userInput)
        {
            int np = Convert.ToInt16(userInput);
            int i, counter = 0;
            double total = 0.0;
            double timeQuantum;
            double waitTime = 0, turnaroundTime = 0;
            double averageWaitTime, averageTurnaroundTime;
            double[] arrivalTime = new double[10];
            double[] burstTime = new double[10];
            double[] temp = new double[10];
            int x = np;

            DialogResult result = MessageBox.Show("Round Robin Scheduling", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                for (i = 0; i < np; i++)
                {
                    string arrivalInput =
                            Microsoft.VisualBasic.Interaction.InputBox("Enter arrival time: ",
                                                               "Arrival time for P" + (i + 1),
                                                               "",
                                                               -1, -1);

                    arrivalTime[i] = Convert.ToInt64(arrivalInput);

                    string burstInput =
                            Microsoft.VisualBasic.Interaction.InputBox("Enter burst time: ",
                                                               "Burst time for P" + (i + 1),
                                                               "",
                                                               -1, -1);

                    burstTime[i] = Convert.ToInt64(burstInput);

                    temp[i] = burstTime[i];
                }
                string timeQuantumInput =
                            Microsoft.VisualBasic.Interaction.InputBox("Enter time quantum: ", "Time Quantum",
                                                               "",
                                                               -1, -1);

                timeQuantum = Convert.ToInt64(timeQuantumInput);
                Helper.QuantumTime = timeQuantumInput;

                for (total = 0, i = 0; x != 0;)
                {
                    if (temp[i] <= timeQuantum && temp[i] > 0)
                    {
                        total = total + temp[i];
                        temp[i] = 0;
                        counter = 1;
                    }
                    else if (temp[i] > 0)
                    {
                        temp[i] = temp[i] - timeQuantum;
                        total = total + timeQuantum;
                    }
                    if (temp[i] == 0 && counter == 1)
                    {
                        x--;
                       
                        MessageBox.Show("Turnaround time for Process " + (i + 1) + " : " + (total - arrivalTime[i]), "Turnaround time for Process " + (i + 1), MessageBoxButtons.OK);
                        MessageBox.Show("Wait time for Process " + (i + 1) + " : " + (total - arrivalTime[i] - burstTime[i]), "Wait time for Process " + (i + 1), MessageBoxButtons.OK);
                        turnaroundTime = (turnaroundTime + total - arrivalTime[i]);
                        waitTime = (waitTime + total - arrivalTime[i] - burstTime[i]);                        
                        counter = 0;
                    }
                    if (i == np - 1)
                    {
                        i = 0;
                    }
                    else if (arrivalTime[i + 1] <= total)
                    {
                        i++;
                    }
                    else
                    {
                        i = 0;
                    }
                }
                averageWaitTime = Convert.ToInt64(waitTime * 1.0 / np);
                averageTurnaroundTime = Convert.ToInt64(turnaroundTime * 1.0 / np);
                MessageBox.Show("Average wait time for " + np + " processes: " + averageWaitTime + " sec(s)", "", MessageBoxButtons.OK);
                MessageBox.Show("Average turnaround time for " + np + " processes: " + averageTurnaroundTime + " sec(s)", "", MessageBoxButtons.OK);
            }
        }


        //Additional ALgorithms

        //First Algorithm - Highest Response Ratio Next
        public static void hrrnAlgorithm(List<Process> processes)
        {
            int currentTime = 0;
            int completed = 0;
            int n = processes.Count;
            List<Process> readyQueue = new List<Process>();

            // To track the total times for calculations
            double totalWaitingTime = 0;
            double totalTurnaroundTime = 0;
            double totalResponseTime = 0;
            double totalCPUTime = 0;
            double totalIdleTime = 0;

            while (completed < n)
            {
                // Add processes that have arrived by current time to the ready queue
                foreach (var process in processes)
                {
                    if (process.ArrivalTime <= currentTime && !readyQueue.Contains(process) && process.FinishTime == 0)
                    {
                        readyQueue.Add(process);
                    }
                }

                if (readyQueue.Count == 0)
                {
                    // No process is ready, move time forward
                    currentTime++;
                    totalIdleTime++;  // Increment idle time
                    continue;
                }

                // Calculate Response Ratio = (Waiting Time + Burst Time) / Burst Time
                foreach (var process in readyQueue)
                {
                    process.WaitingTime = currentTime - process.ArrivalTime;
                }

                var nextProcess = readyQueue
                    .OrderByDescending(p => ((p.WaitingTime + p.BurstTime) / (double)p.BurstTime))
                    .First();

                // Set StartTime only if it hasn't been set before
                nextProcess.StartTime = currentTime;

                // Move time forward based on burst time
                currentTime += nextProcess.BurstTime;
                nextProcess.FinishTime = currentTime;

                // Calculate Turnaround Time and Waiting Time
                nextProcess.TurnaroundTime = nextProcess.FinishTime - nextProcess.ArrivalTime;
                nextProcess.WaitingTime = nextProcess.TurnaroundTime - nextProcess.BurstTime;

                // Calculate Response Time
                nextProcess.ResponseTime = nextProcess.StartTime - nextProcess.ArrivalTime;

                // Update totals for the metrics
                totalWaitingTime += nextProcess.WaitingTime;
                totalTurnaroundTime += nextProcess.TurnaroundTime;
                totalResponseTime += nextProcess.ResponseTime;
                totalCPUTime += nextProcess.BurstTime;

                // Mark as completed
                completed++;
                readyQueue.Remove(nextProcess);
            }

            // Calculate average metrics
            double avgWaitingTime = totalWaitingTime / n;
            double avgTurnaroundTime = totalTurnaroundTime / n;
            double avgResponseTime = totalResponseTime / n;

            // Calculate CPU Utilization
            double cpuUtilization = (totalCPUTime / (totalCPUTime + totalIdleTime)) * 100;

            // Calculate Throughput
            double throughput = (double)n / (totalCPUTime + totalIdleTime);

            // Display results
            Console.WriteLine("\nHRRN Scheduling Results:");
            Console.WriteLine("ID\tArrival\tBurst\tStart\tFinish\tWaiting\tTurnaround\tResponse");
            foreach (var p in processes)
            {
                Console.WriteLine($"{p.ID}\t{p.ArrivalTime}\t{p.BurstTime}\t{p.StartTime}\t{p.FinishTime}\t{p.WaitingTime}\t{p.TurnaroundTime}\t{p.ResponseTime}");
            }

            // Display additional metrics
            Console.WriteLine($"\nAverage Waiting Time (AWT): {avgWaitingTime:F2} ms");
            Console.WriteLine($"Average Turnaround Time (ATT): {avgTurnaroundTime:F2} ms");
            Console.WriteLine($"Average Response Time (RT): {avgResponseTime:F2} ms");
            Console.WriteLine($"CPU Utilization: {cpuUtilization:F2}%");
            Console.WriteLine($"Throughput: {throughput:F4} processes/ms");
        }



        internal static void hrrnAlgorithm(string text)
        {
            throw new NotImplementedException();
        }


        // Second Algorithm - MLFQ [Multilevel Feedback Queue] CPU Scheduling Algorithm
        public static void mlfqAlgorithm(List<Process> processes, int[] timeQuantums)
        {
            // Initialize queues based on the time quantums
            Queue<Process>[] queues = new Queue<Process>[timeQuantums.Length];
            for (int i = 0; i < queues.Length; i++)
            {
                queues[i] = new Queue<Process>();
            }

            int currentTime = 0;
            int completed = 0;
            int n = processes.Count;
            double totalWaitingTime = 0;
            double totalTurnaroundTime = 0;
            double totalResponseTime = 0;
            double totalCPUTime = 0;
            double totalIdleTime = 0;

            while (completed < n)
            {
                // Add processes that have arrived by current time to the first queue
                foreach (var process in processes)
                {
                    if (process.ArrivalTime <= currentTime && !queues[0].Contains(process) && process.FinishTime == 0)
                    {
                        queues[0].Enqueue(process);
                    }
                }

                bool processScheduled = false;

                // Iterate through each queue, starting from highest priority (index 0) to lowest (index n-1)
                for (int i = 0; i < queues.Length; i++)
                {
                    if (queues[i].Count > 0)
                    {
                        var currentQueue = queues[i];
                        var currentProcess = currentQueue.Dequeue();

                        // Set StartTime only if it hasn't been set before
                        if (currentProcess.StartTime == 0)
                        {
                            currentProcess.StartTime = currentTime;
                            totalResponseTime += currentProcess.StartTime - currentProcess.ArrivalTime; // Calculate Response Time
                        }

                        // Execute the process for the time quantum or until completion
                        int executionTime = Math.Min(currentProcess.RemainingTime, timeQuantums[i]);
                        currentProcess.RemainingTime -= executionTime;
                        currentTime += executionTime;

                        // If the process is completed
                        if (currentProcess.RemainingTime == 0)
                        {
                            currentProcess.FinishTime = currentTime;
                            completed++;
                            currentProcess.TurnaroundTime = currentProcess.FinishTime - currentProcess.ArrivalTime;
                            currentProcess.WaitingTime = currentProcess.TurnaroundTime - currentProcess.BurstTime;

                            totalTurnaroundTime += currentProcess.TurnaroundTime;
                            totalWaitingTime += currentProcess.WaitingTime;
                            totalCPUTime += currentProcess.BurstTime;
                        }
                        else
                        {
                            // If not completed, move to the next queue or back to the same queue
                            if (i < queues.Length - 1)
                            {
                                // Move to next lower-priority queue
                                queues[i + 1].Enqueue(currentProcess);
                            }
                            else
                            {
                                // If in the lowest priority queue, stay in the same queue
                                queues[i].Enqueue(currentProcess);
                            }
                        }

                        processScheduled = true;
                        break; // Move to the next process after scheduling one
                    }
                }

                // If no process was scheduled in this cycle, increment currentTime to avoid infinite loop
                if (!processScheduled)
                {
                    currentTime++;
                    totalIdleTime++;
                }
            }

            // Calculate metrics
            double avgWaitingTime = totalWaitingTime / n;
            double avgTurnaroundTime = totalTurnaroundTime / n;
            double avgResponseTime = totalResponseTime / n;
            double cpuUtilization = (totalCPUTime / (totalCPUTime + totalIdleTime)) * 100;
            double throughput = n / (totalCPUTime + totalIdleTime);

            // Display results
            Console.WriteLine("\nMLFQ Scheduling Results:");
            Console.WriteLine("ID\tArrival\tBurst\tStart\tFinish\tWaiting\tTurnaround\tResponse");
            foreach (var p in processes)
            {
                Console.WriteLine($"{p.ID}\t{p.ArrivalTime}\t{p.BurstTime}\t{p.StartTime:F2}\t{p.FinishTime:F2}\t{p.WaitingTime:F2}\t{p.TurnaroundTime:F2}\t{p.ResponseTime:F2}");
            }

            // Display additional metrics
            Console.WriteLine($"\nAverage Waiting Time (AWT): {avgWaitingTime:F2} ms");
            Console.WriteLine($"Average Turnaround Time (ATT): {avgTurnaroundTime:F2} ms");
            Console.WriteLine($"Average Response Time (RT): {avgResponseTime:F2} ms");
            Console.WriteLine($"CPU Utilization: {cpuUtilization:F2}%");
            Console.WriteLine($"Throughput: {throughput:F4} processes/ms");
        }
        internal static void mlfqAlgorithm(string text)
        {
            throw new NotImplementedException();
        }

        internal static List<Process> GetProcesses()
        {
            throw new NotImplementedException();
        }

     
    }
}

                        

