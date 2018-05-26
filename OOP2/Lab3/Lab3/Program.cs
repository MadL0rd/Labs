using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab3 {
    class Program {
        static void Main(string[] args) {

            // Создаем депо
            var depot = new Depot();

            // Добавляем трамваи
            for (int i = 0; i < 29; i++) {
                depot.addTram();
            }

            // Добавляем пути
            depot.addRoute(3, new List<string>{ "Stop 1", "Stop 2", "Stop 3"});
            depot.addRoute(8, new List<string>{ "Stop 4", "Stop 5", "Stop 6"});
            depot.addRoute(6, new List<string>{ "Stop 7", "Stop 8", "Stop 9"});
            depot.addRoute(5, new List<string>{ "Stop 10", "Stop 11", "Stop 12"});
            depot.addRoute(5, new List<string> { "Stop 11", "Stop 12", "Stop 13"});

            // Запускаем цикл обновления состояний
            while (true) {
                depot.update();
                System.Threading.Thread.Sleep(1000);
            }
        }
    }

    // Рнг генератор
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

    // Депо
    class Depot {

        // Маршруты и трамваи
        private ActorUpdater<Route> routes = new ActorUpdater<Route>();
        private ActorUpdater<Tram> trams = new ActorUpdater<Tram>();

        // Резерв трамваев
        private List<Tram> reserved = new List<Tram>();

        // Диспетчерская
        private RepairStation repairStation;

        // Конструктор создает депо и диспетчерскую
        public Depot() {
            repairStation = new RepairStation(this, 3, 1);
        }

        // Обновляет состояние диспетчерской, распределяет трамваи, 
        // обновляет состояния маршрутов и трамваев
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

        // Распределяет трамваи из резерва по маршрутам с низким числом трамваев
        private void dispatchReserved() {

            // Находим все рабочие трамваи в резерве
            var workingReserved = reserved.FindAll(tram => {
                return tram.working;
            });

            // Распределяем трамваи по маршрутам, которым не хватает трамваев
            for(int i = 0, len = routes.list.Count(); i < len; i++) {
                var route = routes.list[i];
                while (route.needsTrams()) {

                    // В резерве не осталось трамваев
                    if (workingReserved.Count() == 0) {
                        Console.WriteLine("0 trams in reserve");
                        return;
                    }

                    // Устанавливаем маршрут
                    workingReserved[0].setRoute(route);

                    // Удаляем из резерва
                    reserved.Remove(workingReserved[0]);
                    workingReserved.RemoveAt(0);
                }
            }
            Console.WriteLine("{0} trams in reserve", workingReserved.Count());
        }

        // Создает маршрут
        public void addRoute(int neededTrams, List<string> stops) {
            routes.add(new Route(routes.list.Count() + 1, neededTrams, stops));
        }

        // Создает трамвай и добавляет его в резерв
        // Создает и передает радио трамваю
        public void addTram() {
            var tram = new Tram(trams.list.Count() + 1, new Radio(repairStation));
            trams.add(tram);
            reserved.Add(tram);
        }

        // Перемещает трамвай в резерв
        public void moveToReserved(Tram tram) {
            if (!reserved.Contains(tram)) {
                reserved.Add(tram);
            }
            tram.resetRoute();
        }
    }

    // Диспетчерская (починочная станция)
    class RepairStation {

        // Ссылка на депо
        private Depot depot;

        // Починочное место - хранит сломанный объект и время на его починку
        private class RepairSpot {
            public Actor brokenActor { get; set; }
            public int timeToRepair { get; set; }

            public RepairSpot(Actor brokenActor, int timeToRepair) {
                this.brokenActor = brokenActor;
                this.timeToRepair = timeToRepair;
            }
        }

        // Кол-ва техников
        private int numTramTechs, numRouteTechs;

        // Списки трамваев и маршрутов на починку
        private List<RepairSpot> tramsInRepair = new List<RepairSpot>();
        private List<RepairSpot> routesInRepair = new List<RepairSpot>();

        // Конструктор создает диспетчерскую, указывает, с каким депо нужно связаться
        // Сохраняет кол-во техников
        public RepairStation(Depot depot, int numTramTechs, int numRouteTechs) {
            this.depot = depot;
            this.numTramTechs = numTramTechs;
            this.numRouteTechs = numRouteTechs;
        }

        // Чинит маршруты и станции
        public void update() {
            updateRepair(tramsInRepair, numTramTechs, "tram");
            updateRepair(routesInRepair, numRouteTechs, "route");
        }

        // Чинит объекты из указанного списка
        private void updateRepair(List<RepairSpot> repairSpots, int numTechs, string actorName) {

            // Пока есть техники и есть что чинить, уменьшаем время починки объектов в списке
            var n = numTechs;
            for (int i = 0; i < repairSpots.Count(); i++) {
                if (n <= 0) break;
                repairSpots[i].timeToRepair--;
                n--;
            }

            // Находим все исправленные объекты, устанавливаем им соответствующий статус и убираем из списка
            for (int i = repairSpots.Count() - 1; i >= 0; i--) {
                if (repairSpots[i].timeToRepair == 0) {
                    repairSpots[i].brokenActor.working = true;
                    repairSpots.RemoveAt(i);
                }
            }
            
            // Лог
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

        // Помещает переданный объект в список на починку
        private void repair(Actor actor, int timeToRepair, List<RepairSpot> repairSpots) {
            // Проверяем, что объекта нет в списке
            if (
                repairSpots.Find(repairSpot => {
                    return repairSpot.brokenActor == actor;
                }) == null
            ) {
                repairSpots.Add(new RepairSpot(actor, timeToRepair));
            }
        }

        // Помещает трамвай в список на починку, говорит депо снять его с линии
        public void repair(Tram tram) {
            repair(tram, Rnd.Next(4) + 2, tramsInRepair);
            depot.moveToReserved(tram);
        }

        // Помещает маршрут в список на починку, говорит депо снять трамвай с линии
        public void repair(Route route, Tram tram) {
            repair(route, Rnd.Next(9) + 2, routesInRepair);
            depot.moveToReserved(tram);
        }
    }

    // Абстрактый класс объекта
    // Имеет id, рабочее/нерабочее состояние и метод обновления
    abstract class Actor {

        public bool working = true;
        public int id;

        public abstract void update();
    }

    // Хранит и обновляет объекты
    class ActorUpdater<T> where T : Actor {

        // Список объектов
        public List<T> list = new List<T>();

        // Добавляет объект
        public void add(T actor) {
            if (list.Contains(actor)) return;
            list.Add(actor);
        }

        // Удаляет объект
        public void remove(T actor) {
            if (!list.Contains(actor)) return;
            list.Remove(actor);
        }

        // Обновляет состояния объектов
        public void update() {
            list.ForEach(subscriber => {
                subscriber.update();
            });
        }
    }

    // Маршрут
    class Route : Actor {

        // Список остановок
        private List<string> stops;        

        // Сколько трамваев не хватает на маршруте
        private int neededTrams;

        // Конструктор создает маршрут с указанным id, остановками и кол-вом необходимых трамваев 
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

        // Возвращает, не хватает ли маршруту трамваев
        public bool needsTrams() {
            return working && neededTrams != 0;
        }

        // Уменьшает число нехватки трамваев
        public void tramAdded() {
            neededTrams--;
        }

        // Увеличивает число нехватки трамваев
        public void tramRemoved() {
            neededTrams++;
        }

        // Возвращает случайную остановку
        public string getFirstStop() {
            return stops[Rnd.Next(stops.Count())];
        }

        // Возвращает остановку после переданной
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

        // Обновляет состояние маршрута - дает ему сломаться
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

    // Трамвай
    class Tram : Actor {

        // Водитель
        private Driver driver;
        // Текущий маршрут
        public Route route {
            get; private set;
        }
        
        // Следующая остановка
        private string nextStop = "";
        // Время до следующей остановки
        private int timeLeft = 0;
        // Находится ли трамвай на остановке
        private bool stopped = false;

        // Конструктор создает трамвай с указанным id, создает водителя и передает ему радио
        public Tram(int id, Radio radio) {
            driver = new Driver(this, radio);
            this.id = id;
        }

        // Устанавливает маршрут трамвая и уменьшает число нехватки трамваев маршрута
        public void setRoute(Route route) {
            this.route = route;
            nextStop = route.getFirstStop();
            timeLeft = 0;
            stopped = true;
            route.tramAdded();
        }

        // Убирает трамвай с маршрута и увеличивает число нехватки трамваев маршрута
        public void resetRoute() {
            if (route == null) return;
            route.tramRemoved();
            route = null;
        }

        // Обновляет состояние трамвая
        public override void update() {
            printStatus();

            // Трамвай неисправен или стоит в депо
            if (route == null || !working) {
                return;
            }

            // Событие поломки
            if (Rnd.Next(100) < 4) {
                working = false;
            }

            // Даем водителю проверить состояние трамвая и маршрута
            // Если все ок, движемся по маршруту
            if(driver.checkTram() && driver.checkRoute()) {
                updatePosition();                
            }

        }

        // Лог
        private void printStatus() {
            if (working && route != null) {
                Console.WriteLine("Tram {0} is on route {1}, traveling to {2}", id, route.id, nextStop);
            }
            else {
                Console.WriteLine("Tram {0} is stalling, {1}", id, !working ? "broke down" : "no route");
            }
        }

        // Передвигает трамвай по маршруту
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

    // Водитель
    class Driver {

        // Радио для связи с диспетчерской
        private Radio radio;

        // Ссылка на трамвай
        private Tram tram;

        // Конструктор создает водителя, сохраняет ссылку на трамвай и радио
        public Driver(Tram tram, Radio radio) {
            this.radio = radio;
            this.tram = tram;
        }

        // Проверяет состояние трамвая и оповещает диспетчерскую радио
        public bool checkTram() {
            var working = tram.working;
            if (!working) {
                radio.brokeNotify(tram);
            }
            return working;
        }

        // Проверяет состояние пути и оповещает диспетчерскую по радио
        public bool checkRoute() {
            var working = tram.route.working;
            if (!working) {
                radio.brokeNotify(tram.route, tram);
            }
            return working;
        }
    }

    // Радио
    class Radio {

        // Ссылка на диспетчерскую
        private RepairStation repairStation;

        // Конструктор создает радио с ссылкой на диспетчерскую
        public Radio(RepairStation repairStation) {
            this.repairStation = repairStation;
        }

        // Сообщает в диспетчерскую о поломке трамвая
        public void brokeNotify(Tram tram) {
            repairStation.repair(tram);
        }
        // Сообщает в диспетчерскую о поломке пути
        public void brokeNotify(Route route, Tram tram) {
            repairStation.repair(route, tram);
        }
    }

}
