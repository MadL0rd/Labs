using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using CommandLine;

namespace Lab1 {
    public class Program {

        static StreamWriter output;
        static string statsHeader = "    Avg.; Dev. Runtime; Defragged; Queued;     Avg.; Dev. Timeloss; Iterations";
        static string summary = "";

        static Random rnd = new Random();

        static void Main(string[] args) {

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(opts => RunProgram(opts))
                .WithNotParsed((errs) => {
                    foreach(Error err in errs) { 
                        Console.WriteLine(err);
                    }
                });
        }

        static void RunProgram(Options opts) {

            string input;
            output = new StreamWriter("output.txt");

            if (!opts.ReadFromFile) {
                input = GetRandomizedProcesses(
                    opts.ProcessAmount,
                    opts.Mem.ElementAt(0),
                    opts.Mem.ElementAt(1),
                    opts.StartTime,
                    opts.Lifespan.ElementAt(0),
                    opts.Lifespan.ElementAt(1)
                );
                File.WriteAllText("generated-input.txt", input);
            }
            else {
                input = File.ReadAllText(opts.InputFile);
            }

            output.WriteLine("          " + statsHeader);

            var fitters = new MemoryFitter[] {
                new BestFit(),
                new WorstFit(),
                new FirstFit(),
                new NextFit()
            };

            foreach(int i in opts.Fitters) {

                int j = i - 1;
                if (j < 0 || j >= fitters.Length) continue;

                RunManager(
                    opts.MemoryLimit == null ? opts.MemoryLimitDefault : opts.MemoryLimit.Value,
                    opts.DefragTime == null ? opts.DefragTimeDefault : opts.DefragTime.Value,
                    fitters[j], input, opts.Interval, opts.DebugMode
                );
            }

            Console.WriteLine("          " + statsHeader);
            Console.WriteLine(summary);

            output.Close();
        }

        static void RunManager(int memoryLimit, int defragTime, MemoryFitter fitter, string input, int interval, int debugMode) {

            var manager = new MemoryManager(memoryLimit, defragTime, fitter, debugMode);

            CreateProcesses(manager, input);

            var stats = manager.Run(interval);

            if (debugMode != 0) {
                Console.WriteLine(statsHeader);
                Console.WriteLine(stats);
            }

            Console.WriteLine();

            stats = String.Format("{1, 8}; {0}", stats, manager.Fitter.name);
            output.WriteLine(stats);
            summary += stats + '\n';

            if (debugMode == 3) {
                Console.ReadKey();
            }
        }

        static void CreateProcesses(MemoryManager manager, string input) {
            var procs = input.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );

            for(int i = 0; i < procs.Length; i++) {

                try {
                    var proc = procs[i].Split();
                    int mem = Int32.Parse(proc[0]);
                    int possStart = Int32.Parse(proc[1]);
                    int lifespan = Int32.Parse(proc[2]);
                    manager.CreateProcess(mem, possStart, lifespan);
                }
                catch (Exception e) {
                    Console.WriteLine("Couldn't create process from line {0} '{1}'", i, procs[i]);
                    Console.WriteLine(e.ToString());
                }
            }
        }

        static string GetRandomizedProcesses(long amount, int memMin, int memMax, int startTimeMax, int lifespanMin, int lifespanMax) {
            Console.WriteLine("Creating info for {0} random processes", amount);
            Console.WriteLine("Memory: {0}-{1}, start time: {2}-{3}, lifespan: {4}-{5}\n", memMin, memMax, 0, startTimeMax, lifespanMin, lifespanMax);

            var input = new StringBuilder(); ;

            for (int i = 0; i < amount; i++) {
                input.Append(String.Format("{0} {1} {2}\n", rnd.Next(memMin, memMax + 1), rnd.Next(0, startTimeMax + 1), rnd.Next(lifespanMin, lifespanMax + 1)));
            }

            input = input.Remove(input.Length - 1, 1);
            return input.ToString();
        }
    }

    public class Process {

        public int Id;
        public int MemoryReq;
        public int DesiredStartTime;
        public int Lifespan;

        public int MemoryAddress;

        public int QueueTime;
        public int StartTime;
        public int StartTimeWithoutDefrag;
        public int FinishTime;

        public bool Defragged;

        public bool Running = false;

        public Process(int id, int memoryReq, int desiredStartTime, int lifespan) {
            Id = id;
            MemoryReq = memoryReq;
            DesiredStartTime = desiredStartTime;
            Lifespan = lifespan;
        }

        public void Start(int memoryAddress, int startTime, int startTimeWithoutDefrag) {
            StartTime = startTime;
            StartTimeWithoutDefrag = startTimeWithoutDefrag;
            MemoryAddress = memoryAddress;
            Running = true;
        }

        public void Finish(int finishTime) {
            FinishTime = finishTime;
            Running = false;
        }
    }

    public class MemoryManager {

        int[] memory;
        int memoryLeft;
        int defragTime;

        int currentTime = 0;
        int currentTimeWithoutDefrag = 0;

        int processesTotal = 0;
        int processIdCounter = 0;

        int statDefragged = 0;
        int statQueued = 0;
        int debugMode;

        string stats = "";

        List<Process> startedProcesses = new List<Process>();
        List<Process> notStartedProcesses = new List<Process>();
        List<Process> queuedProcesses = new List<Process>();
        List<Process> finishedProcesses = new List<Process>();

        public MemoryFitter Fitter;

        public MemoryManager(int memoryLimit, int defragTime, MemoryFitter fitter, int debugMode) {
            memory = new int[memoryLimit];
            memoryLeft = memoryLimit;
            Fitter = fitter;
            this.defragTime = defragTime;
            this.debugMode = debugMode;
        }

        public string Run(int interval) {
            Console.WriteLine("Started {0}", Fitter.name);
            Console.WriteLine("Memory limit: {0}, defrag time: {1}", memory.Length, defragTime);

            if(debugMode != 0) {
                VisualizeMemory();
                if(debugMode == 3) {
                    Console.ReadKey();
                }
            }

            while (Update()) {

                if (debugMode == 3) {
                    Console.ReadKey();
                }
                else if(debugMode == 2) {
                    System.Threading.Thread.Sleep(interval);
                }
            }

            return stats;
        }

        void Stop() {
            stats = OutputStats();
            Console.WriteLine("Finished {0} with {1} processes in {2} ticks", Fitter.name, processesTotal, currentTime);
        }

        bool Update() {

            if (debugMode != 0) {
                Console.WriteLine("Tick {0} ({1} not counting defrag)", currentTime, currentTimeWithoutDefrag);
            }

            UpdateStarted();
            UpdateQueue();
            UpdateNotStarted();

            if (debugMode != 0) {
                VisualizeMemory();
                NormalizeMemory();
            }

            if (IsFinished()) {
                Stop();
                return false;
            }

            currentTimeWithoutDefrag++;
            currentTime++;

            return true;
        }

        void UpdateStarted() {

            for (int i = 0; i < startedProcesses.Count; i++) {
                Process process = startedProcesses[i];

                // Останавливаем процесс, если он отработал свое время
                if (process.StartTimeWithoutDefrag + process.Lifespan == currentTimeWithoutDefrag) {
                    i--;
                    StopAndDestroyProcess(process);
                }
            }
        }

        void UpdateQueue() {

            // Пытаемся запустить первый процесс в очереди
            while(queuedProcesses.Count != 0) {
                Process process = queuedProcesses[0];
                int? memoryAddress = AllocateMemory(process);

                if (memoryAddress != null) {
                    queuedProcesses.RemoveAt(0);
                    StartProcess(process, memoryAddress.Value);
                }
                else {
                    break;
                }
            }
        }

        void UpdateNotStarted() {

            for (int i = 0; i < notStartedProcesses.Count; i++) {

                Process process = notStartedProcesses[i];

                // Процесс еще не нужно запускать
                if (process.DesiredStartTime > currentTime) continue;

                process.QueueTime = currentTime;

                // Пытаемся запустить процесс, если очередь пуста
                bool started = false;
                if (queuedProcesses.Count == 0) {
                    int? memoryAddress = AllocateMemory(process);

                    if (memoryAddress != null) {
                        StartProcess(process, memoryAddress.Value);
                        started = true;
                    }
                }

                // Ставим процесс в очередь, если мы его не запустили
                if (!started) {
                    QueueUpProcess(process);
                }

                notStartedProcesses.RemoveAt(i);
                i--;
            }
        }

        // Возвращает true, если все процессы завершены
        bool IsFinished() {
            var finished = processesTotal == finishedProcesses.Count;
            var actuallyFinished = startedProcesses.Count == 0 && notStartedProcesses.Count == 0 && queuedProcesses.Count == 0;

            Debug.Assert(finished == actuallyFinished, "Processes don't add up");

            return finished;
        }



        public Process CreateProcess(int memoryReq, int possibleStartTime, int lifespan) {

            if (memoryReq <= 0) {
                throw new Exception("Process memory requirement cannot be 0 or less");
            }

            if (memoryReq > memory.Length) {
                throw new Exception("Process memory requirement cannot be more than total memory amount");
            }

            var process = new Process(processIdCounter++, memoryReq, possibleStartTime, lifespan);
            notStartedProcesses.Add(process);
            processesTotal++;
            return process;
        }

        void QueueUpProcess(Process process) {

            if (debugMode != 0) {
                Console.WriteLine("q Process {0} needs {1} cells", process.Id, process.MemoryReq);
            }

            queuedProcesses.Add(process);
            statQueued++;
        }

        void StartProcess(Process process, int memoryAddress) {

            if (debugMode != 0) {
                Console.WriteLine("+ Process {0} took {1} cells on {2}", process.Id, process.MemoryReq, memoryAddress);
            }

            startedProcesses.Add(process);
            FillMemory(memoryAddress, process.MemoryReq, true);
            process.Start(memoryAddress, currentTime, currentTimeWithoutDefrag);
        }

        void StopAndDestroyProcess(Process process) {

            if (debugMode != 0) {
                Console.WriteLine("- Process {0} released {1} cells on {2}", process.Id, process.MemoryReq, process.MemoryAddress);
            }
            ReleaseMemory(process);
            startedProcesses.Remove(process);
            finishedProcesses.Add(process);
            process.Finish(currentTime);
        }



        public int? AllocateMemory(Process process) {

            // Проверяем хватает ли памяти под процесс
            if(memoryLeft - process.MemoryReq >= 0) {
                int? memoryAddress = Fitter.Fit(process, memory);

                // Памяти хватает, но фиттер не нашел адрес,
                // значит нужно дефрагментировать память и попробовать еще раз
                if(memoryAddress == null) {
                    DefragmentMemory();
                    process.Defragged = true;
                    memoryAddress = Fitter.Fit(process, memory);

                    // Т.к. мы проверили что памяти хватает,
                    // после дефрагментации фиттер должен найти адрес для процесса
                    Debug.Assert(memoryAddress != null, "Coudln't fit after defrag");
                }

                return memoryAddress;
            }
            return null;
        }

        void ReleaseMemory(Process process) {
            FillMemory(process.MemoryAddress, process.MemoryReq, false);
        }

        // Заполняет/очищает память, изменяет memoryLeft
        // Кидает ошибки, если значение ячейки уже в переданном состоянии
        void FillMemory(int start, int length, bool value) {

            Debug.Assert(length > 0, "Cannot fill zero or less amount of memory");

            for(int n = 0; n < length; n++) {

                if (value) {
                    Debug.Assert(memory[n + start] == 0, "Memory overwrite");
                    memoryLeft--;
                }
                else {
                    Debug.Assert(memory[n + start] != 0, "Memory already empty, use ClearMemory()");
                    memoryLeft++;
                }

                memory[n + start] = value ? 3 : 0;
            }

            // Отмечаем начало части памяти для вывода в консоль
            memory[start] = value ? 2 : 0;
        }

        void NormalizeMemory() {
            for (int i = 0; i < memory.Length; i++) {
                if (memory[i] == 3) {
                    memory[i] = 1;
                }
            }
        }

        void CheckForMemoryLeaks() {

            int actualMemoryLeft = 0;
            for (int i = 0; i < memory.Length; i++) {
                if (memory[i] == 0) {
                    actualMemoryLeft++;
                }
            }

            Debug.Assert(actualMemoryLeft == memoryLeft, "Memory overwrite");
        }

        // Переносит выделенную процессам память в начало
        void DefragmentMemory() {

            int j = 0;
            for(int i = 0; i < memory.Length; i++) {
                if (memory[i] == 0) {
                    continue;
                }
                else if (j == i) {
                    j++;
                }
                else { 
                    memory[j] = memory[i];
                    memory[i] = 0;
                    j++;
                }

            }

            CheckForMemoryLeaks();

            // Перераспределяем память процессам
            int memoryAddress = 0;
            startedProcesses.ForEach(process => {
                process.MemoryAddress = memoryAddress;
                memoryAddress += process.MemoryReq;
            });

            statDefragged++;
            currentTime += defragTime;

            if (debugMode != 0) {
                Console.WriteLine("Defragmentated (skipped to tick {0})", currentTime);
            }
        }

        // Вывод памяти в консоль
        void VisualizeMemory() {
            int width = 64;
            int row = 0;
            int d = width;

            for(int i = 0; i < memory.Length; i++) {

                if (d == width) {
                    Console.Write("\n{0, 4} ", row);
                    row += width;
                    d = 0;
                }

                char mem;
                switch(memory[i]) {
                    case 0:
                        mem = '_';
                        break;
                    case 2:
                        mem = '!';
                        break;
                    case 3:
                        mem = '+';
                        break;
                    default:
                        mem = '■';
                        break;
                }

                Console.Write(mem);
                d++;
            }

            Console.WriteLine();
            Console.WriteLine(
                "     {0} processes running, {1} queued, {2} not queued, {3} finished",
                startedProcesses.Count,
                queuedProcesses.Count,
                notStartedProcesses.Count,
                finishedProcesses.Count
            );
            Console.WriteLine("     {0}/{1} memory used", memory.Length - memoryLeft, memory.Length);
            Console.WriteLine();
        }

        // Вывод статистики в консоль и файл
        string OutputStats() {

            var file = new StreamWriter("Log-" + Fitter.name + ".txt");
            file.WriteLine("  Id; Memory; Desired; Lifespan; Queued; Started; Finished; Defrag");

            int numProcesses = finishedProcesses.Count;
            double runtime = 0;
            double timeLoss = 0;

            finishedProcesses.ForEach(process => {
                file.WriteLine(
                    "{0, 4}; {1, 6}; {2, 7}; {3, 8}; {4, 6}; {5, 7}; {6, 8}; {7, 6}",
                    process.Id,
                    process.MemoryReq,
                    process.DesiredStartTime,
                    process.Lifespan,
                    process.QueueTime,
                    process.StartTime,
                    process.FinishTime,
                    process.Defragged
                );
                runtime += process.FinishTime - process.DesiredStartTime;
                timeLoss += process.StartTime - process.DesiredStartTime;
            });
            file.Close();

            double runtimeDeviationComponent = 0;
            double timeLossDeviationComponent = 0;
            double runtimeAvg = runtime / numProcesses;
            double timeLossAvg = timeLoss / numProcesses;
            finishedProcesses.ForEach(process => {
                runtimeDeviationComponent += Math.Pow(process.FinishTime - process.DesiredStartTime - runtimeAvg, 2);
                timeLossDeviationComponent += Math.Pow(process.StartTime - process.DesiredStartTime - timeLossAvg, 2);
            });

            var runtimeDeviation = Math.Sqrt(runtimeDeviationComponent / numProcesses);
            var timeLossDeviation = Math.Sqrt(timeLossDeviationComponent / numProcesses);

            return String.Format(
                "{0, 8}; {1, 12}; {2, 9}; {3, 6}; {4, 8}; {5, 13}; {6, 10}",
                runtimeAvg.ToString("#.##"),
                runtimeDeviation.ToString("#.##"),
                statDefragged,
                statQueued,
                timeLossAvg.ToString("#.##"),
                timeLossDeviation.ToString("#.##"),
                Fitter.iterations
            );
        }
    }


    /* Размещатели процессов в памяти */

    public abstract class MemoryFitter {
        public string name;
        public int iterations = 0;

        public abstract int? Fit(Process process, int[] memory);        
    }

    public class FirstFit : MemoryFitter {

        public FirstFit() {
            name = "FirstFit";
        }

        public override int? Fit(Process process, int[] memory) {

            int memoryAddress = 0;
            int memoryAvailable = 0;

            for (int i = 0; i < memory.Length; i++) {
                iterations++;

                if (memory[i] != 0) {
                    memoryAvailable = 0;
                    memoryAddress = i + 1;
                    continue;
                }

                memoryAvailable++;

                if(memoryAvailable == process.MemoryReq) {
                    return memoryAddress;
                }
            }

            return null;
        }
    }

    public class NextFit : MemoryFitter {

        private int lastAddress = 0;

        public NextFit() {
            name = "NextFit";
        }

        public override int? Fit(Process process, int[] memory) {

            int startingPoint = lastAddress;
            int i = startingPoint;
            int memoryAddress = startingPoint;
            int memoryAvailable = 0;
            bool looped = false;

            while (true) {
                iterations++;

                if (memory[i] != 0) {
                    memoryAvailable = 0;
                    memoryAddress = i + 1;
                }
                else {

                    memoryAvailable++;

                    if (memoryAvailable == process.MemoryReq) {
                        lastAddress = (i + 1) == memory.Length ? 0 : i + 1;
                        return memoryAddress;
                    }
                }

                i++;

                if (i == memory.Length) {

                    if(looped) {
                        return null;
                    }
                    else {
                        looped = true;
                    }

                    memoryAvailable = 0;
                    memoryAddress = 0;
                    i = 0;
                }
            }
        }
    }

    public class BestFit : MemoryFitter {

        public BestFit() {
            name = "BestFit";
        }

        public override int? Fit(Process process, int[] memory) {

            int memoryAddress = 0;
            int memoryAvailable = 0;
            int? bestMemoryAddress = null;
            int bestMemoryAvailable = 0;

            for (int i = 0; i <= memory.Length; i++) {
                iterations++;

                if (i == memory.Length || memory[i] != 0) {

                    if(memoryAvailable >= process.MemoryReq && (bestMemoryAddress == null || CheckFit(memoryAvailable, bestMemoryAvailable))) {
                        bestMemoryAddress = memoryAddress;
                        bestMemoryAvailable = memoryAvailable;
                    }

                    memoryAvailable = 0;
                    memoryAddress = i + 1;
                    continue;
                }

                memoryAvailable++;
            }

            return bestMemoryAddress;
        }

        protected virtual bool CheckFit(int newFit, int oldFit) {
            return newFit < oldFit;
        }
    }

    public class WorstFit : BestFit {

        public WorstFit() {
            name = "WorstFit";
        }

        protected override bool CheckFit(int newFit, int oldFit) {
            return newFit > oldFit;
        }
    }

    internal class Options {

        [Value(0, HelpText = "Memory Limit")]
        public int? MemoryLimit { get; set; }

        public int MemoryLimitDefault = 1024;

        [Value(1, HelpText = "Defragmenation Time (in ticks)")]
        public int? DefragTime { get; set; }

        public int DefragTimeDefault = 10;

        [Option("fitters", Default = new int[] { 1, 2, 3, 4 }, HelpText = "Memory fit algorithms. 1 - BestFit, 2 - WorstFit, 3 - FirstFit, 4 - NextFit")]
        public IEnumerable<int> Fitters { get; set; }


        [Option('f', Default = false, HelpText = "Read process info from file")]
        public bool ReadFromFile { get; set; }

        [Option("input", Default = "input.txt", HelpText = "File to read process info from")]
        public string InputFile { get; set; }

        [Option("output", Default = "output.txt", HelpText = "File to write stats to")]
        public string OutputFile { get; set; }


        [Option("procs", Default = 100, HelpText = "Amount of processes")]
        public int ProcessAmount { get; set; }

        [Option("mem", Default = new int[] { 8, 32 }, HelpText = "Process memory range")]
        public IEnumerable<int> Mem { get; set; }

        [Option("start", Default = 15, HelpText = "Maximum process start delay")]
        public int StartTime { get; set; }

        [Option("lifespan", Default = new int[] { 4, 15 }, HelpText = "Process lifespan range")]
        public IEnumerable<int> Lifespan { get; set; }


        [Option("interval", Default = 1000, HelpText = "Time between ticks")]
        public int Interval { get; set; }

        [Option("debug", Default = 0, HelpText = "Debug output mode. 0 - minimal output, 1 - full output, 2 - full output with interval, 2 - full output with manual step through")]
        public int DebugMode { get; set; }
    }
}
