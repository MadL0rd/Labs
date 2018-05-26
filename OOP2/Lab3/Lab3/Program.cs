using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab3 {
    class Program {
        static void Main(string[] args) {

            // ������� ����
            var depot = new Depot();

            // ��������� �������
            for (int i = 0; i < 29; i++) {
                depot.addTram();
            }

            // ��������� ����
            depot.addRoute(3, new List<string>{ "Stop 1", "Stop 2", "Stop 3"});
            depot.addRoute(8, new List<string>{ "Stop 4", "Stop 5", "Stop 6"});
            depot.addRoute(6, new List<string>{ "Stop 7", "Stop 8", "Stop 9"});
            depot.addRoute(5, new List<string>{ "Stop 10", "Stop 11", "Stop 12"});
            depot.addRoute(5, new List<string> { "Stop 11", "Stop 12", "Stop 13"});

            // ��������� ���� ���������� ���������
            while (true) {
                depot.update();
                System.Threading.Thread.Sleep(1000);
            }
        }
    }

    // ��� ���������
    class Rnd {
        private static Random rnd = new Random();

        private Rnd() {}

        public static int Next(int min, int max) {
            return rnd.Next(min, max);
        }

        public static int Next(int max) {
            return rnd.Next(max);
        }
    }

    // ����
    class Depot {

        // �������� � �������
        private ActorUpdater<Route> routes = new ActorUpdater<Route>();
        private ActorUpdater<Tram> trams = new ActorUpdater<Tram>();

        // ������ ��������
        private List<Tram> reserved = new List<Tram>();

        // �������������
        private RepairStation repairStation;

        // ����������� ������� ���� � �������������
        public Depot() {
            repairStation = new RepairStation(this, 3, 1);
        }

        // ��������� ��������� �������������, ������������ �������, 
        // ��������� ��������� ��������� � ��������
        public void update() {
            Console.WriteLine("TRAM DEPOT LOG");
            Console.WriteLine();

            repairStation.update();
            Console.WriteLine();

            dispatchReserved();
            Console.WriteLine();

            routes.update();
            Console.WriteLine();

            trams.update();
            Console.WriteLine();
        }

        // ������������ ������� �� ������� �� ��������� � ������ ������ ��������
        private void dispatchReserved() {

            // ������� ��� ������� ������� � �������
            var workingReserved = reserved.FindAll(tram => {
                return tram.working;
            });

            // ������������ ������� �� ���������, ������� �� ������� ��������
            for(int i = 0, len = routes.list.Count(); i < len; i++) {
                var route = routes.list[i];
                while (route.needsTrams()) {

                    // � ������� �� �������� ��������
                    if (workingReserved.Count() == 0) {
                        Console.WriteLine("0 trams in reserve");
                        return;
                    }

                    // ������������� �������
                    workingReserved[0].setRoute(route);

                    // ������� �� �������
                    reserved.Remove(workingReserved[0]);
                    workingReserved.RemoveAt(0);
                }
            }
            Console.WriteLine("{0} trams in reserve", workingReserved.Count());
        }

        // ������� �������
        public void addRoute(int neededTrams, List<string> stops) {
            routes.add(new Route(routes.list.Count() + 1, neededTrams, stops));
        }

        // ������� ������� � ��������� ��� � ������
        // ������� � �������� ����� �������
        public void addTram() {
            var tram = new Tram(trams.list.Count() + 1, new Radio(repairStation));
            trams.add(tram);
            reserved.Add(tram);
        }

        // ���������� ������� � ������
        public void moveToReserved(Tram tram) {
            if (!reserved.Contains(tram)) {
                reserved.Add(tram);
            }
            tram.resetRoute();
        }
    }

    // ������������� (���������� �������)
    class RepairStation {

        // ������ �� ����
        private Depot depot;

        // ���������� ����� - ������ ��������� ������ � ����� �� ��� �������
        private class RepairSpot {
            public Actor brokenActor { get; set; }
            public int timeToRepair { get; set; }

            public RepairSpot(Actor brokenActor, int timeToRepair) {
                this.brokenActor = brokenActor;
                this.timeToRepair = timeToRepair;
            }
        }

        // ���-�� ��������
        private int numTramTechs, numRouteTechs;

        // ������ �������� � ��������� �� �������
        private List<RepairSpot> tramsInRepair = new List<RepairSpot>();
        private List<RepairSpot> routesInRepair = new List<RepairSpot>();

        // ����������� ������� �������������, ���������, � ����� ���� ����� ���������
        // ��������� ���-�� ��������
        public RepairStation(Depot depot, int numTramTechs, int numRouteTechs) {
            this.depot = depot;
            this.numTramTechs = numTramTechs;
            this.numRouteTechs = numRouteTechs;
        }

        // ����� �������� � �������
        public void update() {
            updateRepair(tramsInRepair, numTramTechs, "tram");
            updateRepair(routesInRepair, numRouteTechs, "route");
        }

        // ����� ������� �� ���������� ������
        private void updateRepair(List<RepairSpot> repairSpots, int numTechs, string actorName) {

            // ���� ���� ������� � ���� ��� ������, ��������� ����� ������� �������� � ������
            var n = numTechs;
            for (int i = 0; i < repairSpots.Count(); i++) {
                if (n <= 0) break;
                repairSpots[i].timeToRepair--;
                n--;
            }

            // ������� ��� ������������ �������, ������������� �� ��������������� ������ � ������� �� ������
            for (int i = repairSpots.Count() - 1; i >= 0; i--) {
                if (repairSpots[i].timeToRepair == 0) {
                    repairSpots[i].brokenActor.working = true;
                    repairSpots.RemoveAt(i);
                }
            }
            
            // ���
            for (int i = 0; i < numTechs; i++) {
                if (i < repairSpots.Count()) {
                    Console.WriteLine("Repairing {0} {1} ", actorName, repairSpots[i].brokenActor.id);
                }
                else {
                    Console.WriteLine("Technician waiting around");
                }
            }

            var dif = repairSpots.Count() - numTechs;
            if (dif > 0) {
                Console.WriteLine("{0} more {1}s in queue for repair", dif, actorName);
            }
            else {
                Console.WriteLine("No {0}s in queue for repair", actorName);
            }
        }

        // �������� ���������� ������ � ������ �� �������
        private void repair(Actor actor, int timeToRepair, List<RepairSpot> repairSpots) {
            // ���������, ��� ������� ��� � ������
            if (
                repairSpots.Find(repairSpot => {
                    return repairSpot.brokenActor == actor;
                }) == null
            ) {
                repairSpots.Add(new RepairSpot(actor, timeToRepair));
            }
        }

        // �������� ������� � ������ �� �������, ������� ���� ����� ��� � �����
        public void repair(Tram tram) {
            repair(tram, Rnd.Next(4) + 2, tramsInRepair);
            depot.moveToReserved(tram);
        }

        // �������� ������� � ������ �� �������, ������� ���� ����� ������� � �����
        public void repair(Route route, Tram tram) {
            repair(route, Rnd.Next(9) + 2, routesInRepair);
            depot.moveToReserved(tram);
        }
    }

    // ���������� ����� �������
    // ����� id, �������/��������� ��������� � ����� ����������
    abstract class Actor {

        public bool working = true;
        public int id;

        public abstract void update();
    }

    // ������ � ��������� �������
    class ActorUpdater<T> where T : Actor {

        // ������ ��������
        public List<T> list = new List<T>();

        // ��������� ������
        public void add(T actor) {
            if (list.Contains(actor)) return;
            list.Add(actor);
        }

        // ������� ������
        public void remove(T actor) {
            if (!list.Contains(actor)) return;
            list.Remove(actor);
        }

        // ��������� ��������� ��������
        public void update() {
            list.ForEach(subscriber => {
                subscriber.update();
            });
        }
    }

    // �������
    class Route : Actor {

        // ������ ���������
        private List<string> stops;        

        // ������� �������� �� ������� �� ��������
        private int neededTrams;

        // ����������� ������� ������� � ��������� id, ����������� � ���-��� ����������� �������� 
        public Route(int id, int neededTrams, List<string> stops) {
            if(stops.Count() == 0) {
                throw new Exception("Route doesn't have stops");
            }
            if(stops.Count() == 1) {
                throw new Exception("Route must have at least 2 stops");
            }
            this.stops = stops;
            this.neededTrams = neededTrams;
            this.id = id;
        }

        // ����������, �� ������� �� �������� ��������
        public bool needsTrams() {
            return working && neededTrams != 0;
        }

        // ��������� ����� �������� ��������
        public void tramAdded() {
            neededTrams--;
        }

        // ����������� ����� �������� ��������
        public void tramRemoved() {
            neededTrams++;
        }

        // ���������� ��������� ���������
        public string getFirstStop() {
            return stops[Rnd.Next(stops.Count())];
        }

        // ���������� ��������� ����� ����������
        public string getNextStop(string currentStop) {
            if (!stops.Contains(currentStop)) {
                throw new Exception("Route doesn't have this stop" + currentStop);
            }
            var i = stops.IndexOf(currentStop);
            i++;
            if(i >= stops.Count()) {
                i = 0;
            }
            return stops[i];
        }

        // ��������� ��������� �������� - ���� ��� ���������
        public override void update() {

            Console.WriteLine(
                "Route {0} is {1}",
                id,
                working ? ("online, needs " + neededTrams + " more trams") : "broken"
            );

            if (working && Rnd.Next(100) < 3) {
                working = false;
            }
        }
    }

    // �������
    class Tram : Actor {

        // ��������
        private Driver driver;
        // ������� �������
        public Route route {
            get; private set;
        }
        
        // ��������� ���������
        private string nextStop = "";
        // ����� �� ��������� ���������
        private int timeLeft = 0;
        // ��������� �� ������� �� ���������
        private bool stopped = false;

        // ����������� ������� ������� � ��������� id, ������� �������� � �������� ��� �����
        public Tram(int id, Radio radio) {
            driver = new Driver(this, radio);
            this.id = id;
        }

        // ������������� ������� ������� � ��������� ����� �������� �������� ��������
        public void setRoute(Route route) {
            this.route = route;
            nextStop = route.getFirstStop();
            timeLeft = 0;
            stopped = true;
            route.tramAdded();
        }

        // ������� ������� � �������� � ����������� ����� �������� �������� ��������
        public void resetRoute() {
            if (route == null) return;
            route.tramRemoved();
            route = null;
        }

        // ��������� ��������� �������
        public override void update() {
            printStatus();

            // ������� ���������� ��� ����� � ����
            if (route == null || !working) {
                return;
            }

            // ������� �������
            if (Rnd.Next(100) < 4) {
                working = false;
            }

            // ���� �������� ��������� ��������� ������� � ��������
            // ���� ��� ��, �������� �� ��������
            if(driver.checkTram() && driver.checkRoute()) {
                updatePosition();                
            }

        }

        // ���
        private void printStatus() {
            if (working && route != null) {
                Console.WriteLine("Tram {0} is on route {1}, traveling to {2}", id, route.id, nextStop);
            }
            else {
                Console.WriteLine("Tram {0} is stalling, {1}", id, !working ? "broke down" : "no route");
            }
        }

        // ����������� ������� �� ��������
        private void updatePosition() {
            if (timeLeft == 0) {
                if (stopped) {
                    nextStop = route.getNextStop(nextStop);
                    timeLeft = Rnd.Next(5) + 1;
                    stopped = false;
                }
                else {
                    stopped = true;
                }
            }
            else {
                timeLeft--;
            }
        }
    }

    // ��������
    class Driver {

        // ����� ��� ����� � �������������
        private Radio radio;

        // ������ �� �������
        private Tram tram;

        // ����������� ������� ��������, ��������� ������ �� ������� � �����
        public Driver(Tram tram, Radio radio) {
            this.radio = radio;
            this.tram = tram;
        }

        // ��������� ��������� ������� � ��������� ������������� �����
        public bool checkTram() {
            var working = tram.working;
            if (!working) {
                radio.brokeNotify(tram);
            }
            return working;
        }

        // ��������� ��������� ���� � ��������� ������������� �� �����
        public bool checkRoute() {
            var working = tram.route.working;
            if (!working) {
                radio.brokeNotify(tram.route, tram);
            }
            return working;
        }
    }

    // �����
    class Radio {

        // ������ �� �������������
        private RepairStation repairStation;

        // ����������� ������� ����� � ������� �� �������������
        public Radio(RepairStation repairStation) {
            this.repairStation = repairStation;
        }

        // �������� � ������������� � ������� �������
        public void brokeNotify(Tram tram) {
            repairStation.repair(tram);
        }
        // �������� � ������������� � ������� ����
        public void brokeNotify(Route route, Tram tram) {
            repairStation.repair(route, tram);
        }
    }

}
