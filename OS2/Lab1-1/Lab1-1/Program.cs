using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CommandLine;
using CommandLine.Text;

namespace Lab1_1 {
    public class Program {

        public static StreamWriter output = new StreamWriter("output.txt");
        public static string statsHeader = "Avg. Runtime; Defragged; Queued; Avg. Timeloss";

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

            if (!opts.ReadFromFile) {
                input = GetRandomizedProcesses(
                    opts.ProcessAmount,
                    opts.MemMin,
                    opts.MemMax,
                    opts.StartMin,
                    opts.StartMax,
                    opts.LifespanMin,
                    opts.LifespanMax
                );
            }
            else {
                input = File.ReadAllText(opts.FileName);
            }

            output.WriteLine("          " + statsHeader);

            var fitters = new MemoryFitter[] {
                new BestFit(),
                new WorstFit(),
                new FirstFit(),
                new NextFit()
            };

            for(int i = 0; i < fitters.Length; i++) {
                RunManager(
                    opts.MemoryLimit == null ? 1024 : opts.MemoryLimit.Value,
                    opts.DefragTime == null ? 2 : opts.DefragTime.Value,
                    fitters[i], input, opts.Interval, opts.DebugMode
                );
            }

            output.Close();
        }

        internal class Options {

            [Value(0, HelpText = "Memory Limit")]
            public int? MemoryLimit { get; set; }

            [Value(1, HelpText = "Defragmenation Time (in ticks)")]
            public int? DefragTime { get; set; }


            [Option('f', "file", Default = false, HelpText = "Read process info from file")]
            public bool ReadFromFile { get; set; }

            [Option("filename", Default = "input.txt", HelpText = "File to read process info from")]
            public string FileName { get; set; }


            [Option("procs", Default = 50, HelpText = "Amount of processes")]
            public int ProcessAmount { get; set; }

            [Option("mem-min", Default = 16, HelpText = "Minimum process memory")]
            public int MemMin { get; set; }

            [Option("mem-max", Default = 128, HelpText = "Maximum process memory")]
            public int MemMax { get; set; }

            [Option("start-min", Default = 0, HelpText = "Minimum process start delay")]
            public int StartMin { get; set; }

            [Option("start-max", Default = 15, HelpText = "Maximum process start delay")]
            public int StartMax { get; set; }

            [Option("lifespan-min", Default = 4, HelpText = "Minimum process start delay")]
            public int LifespanMin { get; set; }

            [Option("lifespan-max", Default = 15, HelpText = "Maximum process start memory")]
            public int LifespanMax { get; set; }


            [Option("interval", Default = 0, HelpText = "Time between ticks")]
            public int Interval { get; set; }

            [Option("debug", Default = 0, HelpText = "Debug mode. 0 - off, 1 - ticks with interval, 2 - manual step through ticks")]
            public int DebugMode { get; set; }
        }

        static void RunManager(int memoryLimit, int defragTime, MemoryFitter fitter, string input, int interval, int debugMode) {
            var manager = new MemoryManager(memoryLimit, defragTime, fitter, debugMode);
            CreateProcesses(manager, input);
            manager.Start(interval);
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
                catch {
                    Console.WriteLine("Couldn't parse line " + i);
                }
            }
        }

        static string GetRandomizedProcesses(long amount, int memMin, int memMax, int possibleStartTimeMin, int possibleStartTimeMax, int lifespanMin, int lifespanMax) {
            Console.WriteLine("Creating info for {0} random processes", amount);
            Console.WriteLine("Memory: {0}-{1}, start time: {2}-{3}, lifespan: {4}-{5}\n", memMin, memMax, possibleStartTimeMin, possibleStartTimeMax, lifespanMin, lifespanMax);

            string input = "";

            for (int i = 0; i < amount; i++) {
                input += String.Format("{0} {1} {2}\n", rnd.Next(memMin, memMax), rnd.Next(possibleStartTimeMin, possibleStartTimeMax), rnd.Next(lifespanMin, lifespanMax));
            }

            input = input.Remove(input.Length - 1);
            return input;
        }
    }

    public class Process {

        public int memoryReq;
        public int memoryAddress;
        public int id;
        public int queueTime;
        public int startTime;
        public int finishTime;
        public int possibleStartTime;
        public bool running = false;
        public int lifespan;
        public bool defragged;

        public Process(int id, int memoryReq, int possibleStartTime, int lifespan) {
            this.id = id;
            this.memoryReq = memoryReq;
            this.possibleStartTime = possibleStartTime;
            this.lifespan = lifespan;
        }

        public void Start(int memoryAddress, int startTime) {
            this.startTime = startTime;
            this.memoryAddress = memoryAddress;
            running = true;
        }
    }

    public class MemoryManager {

        int[] memory;
        int memoryLeft;
        int processIdCounter = 0;
        int currentTime = 0;
        int defragTime;
        int processesTotal = 0;

        List<Process> startedProcesses = new List<Process>();
        List<Process> notStartedProcesses = new List<Process>();
        List<Process> queuedProcesses = new List<Process>();
        List<Process> finishedProcesses = new List<Process>();

        int statDefragged = 0, statQueued = 0;
        int debugMode;

        MemoryFitter fitter;

        public MemoryManager(int memoryLimit, int defragTime, MemoryFitter fitter, int debugMode) {
            memory = new int[memoryLimit];
            memoryLeft = memoryLimit;
            this.defragTime = defragTime;
            this.fitter = fitter;
            this.debugMode = debugMode;
        }

        public void Start(int interval) {
            Console.WriteLine("Started memory manager with {0} fitter", fitter.name);
            Console.WriteLine("Memory limit: {0}, defrag time: {1}", memory.Length, defragTime);
            while (Update()) {
                if (debugMode == 2) {
                    Console.ReadKey();
                }
                else {
                    System.Threading.Thread.Sleep(interval);
                }
            }
        }

        void Stop() {
            OutputStats();
            if (debugMode != 0) {
                Console.ReadKey();
            }
        }

        bool Update() {

            UpdateStarted();
            UpdateQueue();
            UpdateNotStarted();

            CheckForMemoryOverwrites();

            if (debugMode != 0) {
                VisualizeMemory();
            }


            if (IsFinished()) {
                Stop();
                return false;
            }

            currentTime++;

            return true;
        }

        void UpdateStarted() {
            for (int i = 0; i < startedProcesses.Count; i++) {
                Process process = startedProcesses[i];

                if (process.startTime + process.lifespan == currentTime) {
                    i--;
                    StopAndDestroyProcess(process);
                }
            }
        }

        void UpdateQueue() {

            for (int i = 0; i < queuedProcesses.Count; i++) {
                Process process = queuedProcesses[i];
                int? memoryAddress = AllocateMemory(process);

                if (memoryAddress != null) {
                    queuedProcesses.RemoveAt(i);
                    i--;
                    StartProcess(process, memoryAddress.Value);
                }
            }
        }

        void UpdateNotStarted() {

            for (int i = 0; i < notStartedProcesses.Count; i++) {
                Process process = notStartedProcesses[i];

                if (process.possibleStartTime > currentTime) continue;

                process.queueTime = currentTime;

                int? memoryAddress = AllocateMemory(process);
                if (memoryAddress != null) {
                    StartProcess(process, memoryAddress.Value);
                }
                else {
                    queuedProcesses.Add(process);
                    statQueued++;
                }

                notStartedProcesses.RemoveAt(i);
                i--;
            }
        }

        void CheckForMemoryOverwrites() {
            int totalMem = memory.Length;
            for (int i = 0; i < memory.Length; i++) {
                if (memory[i] != 0) {
                    totalMem--;
                }
            }
            if (totalMem != memoryLeft) {
                Console.WriteLine("{0} != {1}", totalMem, memoryLeft);
                throw new Exception("Memory overwrite");
            }
        }



        public Process CreateProcess(int memoryReq, int possibleStartTime, int lifespan) {
            var process = new Process(processIdCounter++, memoryReq, possibleStartTime, lifespan);
            notStartedProcesses.Add(process);
            processesTotal++;
            return process;
        }

        void StartProcess(Process process, int memoryAddress) {
            startedProcesses.Add(process);
            FillMemory(memoryAddress, process.memoryReq, true);
            process.Start(memoryAddress, currentTime);
        }

        void StopAndDestroyProcess(Process process) {
            ReleaseMemory(process);
            startedProcesses.Remove(process);
            finishedProcesses.Add(process);
            process.finishTime = currentTime;
        }



        public int? AllocateMemory(Process process) {

            // Проверяем хватает ли памяти под процесс
            if(memoryLeft - process.memoryReq >= 0) {
                int? memoryAddress = fitter.Fit(process, memory);

                // Памяти хватает, но фиттер не нашел адрес,
                // значит нужно дефрагментировать память и попробовать еще раз
                if(memoryAddress == null) {
                    DefragmentMemory();
                    process.defragged = true;
                    memoryAddress = fitter.Fit(process, memory);

                    // Т.к. мы проверили что памяти хватает,
                    // после дефрагментации фиттер должен найти адрес для процесса
                    if (memoryAddress == null) {
                        throw new Exception("Coudln't fit after defrag");
                    }
                }

                return memoryAddress;
            }
            return null;
        }

        void ReleaseMemory(Process process) {
            FillMemory(process.memoryAddress, process.memoryReq, false);
        }

        // Заполняет/очищает память, изменяет memoryLeft
        // Кидает ошибки, если значение ячейки уже в переданном состоянии
        void FillMemory(int start, int length, bool value) {

            for(int n = 0; n < length; n++) {

                if (value) {
                    if (memory[n + start] == 0) {
                        memoryLeft--;
                    }
                    else {
                        throw new Exception("Memory overwrite");
                    }
                }
                else {
                    if (memory[n + start] != 0) {
                        memoryLeft++;
                    }
                    else {
                        throw new Exception("Memory already empty, use ClearMemory()");
                    }
                }

                memory[n + start] = value ? 1 : 0;
            }

            // Отмечаем начало части памяти для вывода в консоль
            memory[start] = value ? 2 : 0;
        }

        void ClearMemory() {
            for (int i = 0; i < memory.Length; i++) {
                memory[i] = 0;
            }
            memoryLeft = memory.Length;
        }

        // Переносит выделенную процессам память в начало
        void DefragmentMemory() {
            int memoryAddress = 0;

            // Очищаем память
            // При настоящей дефрагментации мы потеряли бы данные, 
            // но здесь нам важен лишь факт заполненности ячеек
            ClearMemory();

            // Перераспределяем память процессам
            startedProcesses.ForEach(process => {
                process.memoryAddress = memoryAddress;
                FillMemory(memoryAddress, process.memoryReq, true);
                memoryAddress += process.memoryReq;
            });

            statDefragged++;

            if (debugMode != 0) {
                Console.WriteLine("Defragmentated");
            }
        }

        bool IsFinished() {
            return startedProcesses.Count == 0 && notStartedProcesses.Count == 0 && queuedProcesses.Count == 0;
        }

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

        void OutputStats() {


            var file = new StreamWriter("Log-" + fitter.name + ".txt");
            file.WriteLine(" Id; Queued; Started; Finished; Defrag");
            int runtime = 0;
            int timeLoss = 0;

            finishedProcesses.ForEach(process => {
                file.WriteLine("{0, 3}; {1, 6}; {2, 7}; {3, 8}; {4, 6}", process.id, process.queueTime, process.startTime, process.finishTime, process.defragged);
                runtime += process.finishTime - process.possibleStartTime;
                timeLoss += process.startTime - process.possibleStartTime + (process.defragged ? defragTime : 0);
            });

            file.Close();
            var stats = String.Format("{0, 12}; {1, 9}; {2, 6}; {3, 13}", runtime / finishedProcesses.Count, statDefragged, statQueued, timeLoss / finishedProcesses.Count);
            Program.output.WriteLine("{1, 8}; {0}", stats, fitter.name);

            Console.WriteLine("Finished memory manager with {0} fitter and {1} processes in {2} ticks", fitter.name, processesTotal, currentTime);
            Console.WriteLine(Program.statsHeader);
            Console.WriteLine(stats);
            Console.WriteLine();
        }
    }


    /* Размещатели процессов в памяти */

    public abstract class MemoryFitter {
        public string name;

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

                if(memory[i] != 0) {
                    memoryAvailable = 0;
                    memoryAddress = i + 1;
                    continue;
                }

                memoryAvailable++;

                if(memoryAvailable == process.memoryReq) {
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

                if (memory[i] != 0) {
                    memoryAvailable = 0;
                    memoryAddress = i + 1;
                }
                else {

                    memoryAvailable++;

                    if (memoryAvailable == process.memoryReq) {
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

                if (i == memory.Length || memory[i] != 0) {

                    if(memoryAvailable >= process.memoryReq && (bestMemoryAddress == null || CheckFit(memoryAvailable, bestMemoryAvailable))) {
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
}
