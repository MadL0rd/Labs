using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Lab3_1 {

    // Рнг генератор
    static class Rnd {
        private static Random rnd = new Random();

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

        private int tramIndex = 0;

        // Конструктор создает депо и диспетчерскую
        public Depot(int tramTechs, int routeTechs) {
            repairStation = new RepairStation(this, tramTechs, routeTechs);
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
            for (int i = 0, len = routes.list.Count(); i < len; i++) {
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

            var tramsReserved = workingReserved.Count();
            Console.WriteLine("{0} trams in reserve", tramsReserved);
        }

        // Создает маршрут
        public Route addRoute(int neededTrams, List<Point> stops, Color color) {
            var route = new Route(routes.list.Count() + 1, neededTrams, stops, color);
            routes.add(route);
            return route;
        }

        // Создает трамвай и добавляет его в резерв
        // Создает и передает радио трамваю
        public Tram addTram() {
            var tram = new Tram(tramIndex++ + 1, new Radio(repairStation));
            trams.add(tram);
            reserved.Add(tram);
            return tram;
        }

        public Tram removeTram() {
            Tram tram = null;

            for (int i = 0; i < reserved.Count; i++) {
                if (reserved[i].working) {
                    tram = reserved[i];
                    reserved.RemoveAt(i);
                    break;
                }
            }

            if(tram == null) {

                for (int i = 0; i < trams.list.Count; i++) {

                    if (trams.list[i].working) {
                        tram = trams.list[i];
                        tram.resetRoute();
                        break;
                    }
                }
            }

            if(tram != null) {
                trams.remove(tram);
                tram.removed = true;
            }

            return tram;
        }

        // Перемещает трамвай в резерв
        public void moveToReserved(Tram tram) {

            if (!reserved.Contains(tram)) {
                reserved.Add(tram);
            }

            tram.resetRoute();
        }

        public void addTech(string actorName) {
            if(actorName == "tram") {
                repairStation.numTramTechs++;
            }
            else if(actorName == "route") {
                repairStation.numRouteTechs++;
            }
        }

        public bool removeTech(string actorName) {
            if (actorName == "tram") {
                if (repairStation.numTramTechs > 1) {
                    repairStation.numTramTechs--;
                    return true;
                }
                else {
                    return false;
                }
            }
            else if (actorName == "route") {
                if (repairStation.numRouteTechs > 1) {
                    repairStation.numRouteTechs--;
                    return true;
                }
                else {
                    return false;
                }
            }
            return false;
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
        public int numTramTechs, numRouteTechs;

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

            updateInfo(repairSpots, numTechs, actorName);
        }

        // Лог
        private void updateInfo(List<RepairSpot> repairSpots, int numTechs, string actorName) {

            for (int i = 0; i < numTechs; i++) {
                if (i < repairSpots.Count()) {
                    Console.WriteLine("Repairing {0} {1} ", actorName, repairSpots[i].brokenActor.id);
                    GUI.Instance.SetTechnicianWorking(actorName, i, repairSpots[i].brokenActor.id, repairSpots[i].timeToRepair);
                }
                else {
                    Console.WriteLine("Technician waiting around");
                    GUI.Instance.SetTechnicianIdling(actorName, i);
                }
            }

            var dif = repairSpots.Count() - numTechs;
            if (dif > 0) {
                Console.WriteLine("{0} more {1}s in queue for repair", dif, actorName);
            }
            else {
                Console.WriteLine("No {0}s in queue for repair", actorName);
            }

            GUI.Instance.SetRepairQueueAmount(actorName, Math.Max(dif, 0));
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
            if(tram.removed) {
                throw new Exception("Can't repair removed");
            }
            repair(tram, Rnd.Next(5) + 10, tramsInRepair);
            //depot.moveToReserved(tram);
        }

        // Помещает маршрут в список на починку
        public void repair(Route route, Tram tram) {
            repair(route, Rnd.Next(9) + 10, routesInRepair);
        }
    }

    // Абстрактый класс объекта
    // Имеет id, рабочее/нерабочее состояние и метод обновления
    abstract class Actor {

        public bool working = true;
        public int id;
        public bool removed = false;

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

            if (!list.Contains(actor) || !list.Remove(actor)) {
                throw new Exception("Not in this updater");
            }            
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
        public List<Point> stops;

        // Сколько трамваев не хватает на маршруте
        private int neededTrams;

        public Color color;

        // Конструктор создает маршрут с указанным id, остановками и кол-вом необходимых трамваев 
        public Route(int id, int neededTrams, List<Point> stops, Color color) {

            if (stops.Count() == 0) {
                throw new Exception("Route doesn't have stops");
            }

            if (stops.Count() == 1) {
                throw new Exception("Route must have at least 2 stops");
            }

            this.stops = stops;
            this.neededTrams = neededTrams;
            this.id = id;
            this.color = color;
        }

        // Возвращает, не хватает ли маршруту трамваев
        public bool needsTrams() {
            return neededTrams != 0;
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
        public Point getFirstStop() {
            return stops[0];
        }

        // Возвращает остановку после переданной
        public Point getNextStop(Point currentStop) {

            if (!stops.Contains(currentStop)) {
                throw new Exception("Route doesn't have this stop" + currentStop);
            }

            var i = stops.IndexOf(currentStop);
            i++;

            if (i >= stops.Count()) {
                i = 0;
            }

            return stops[i];
        }

        // Обновляет состояние маршрута - дает ему сломаться
        public override void update() {
            var status = working ? "online" : "waiting for repair";

            Console.WriteLine(
                "Route {0} is {1}",
                id,
                working ? (status + ", needs " + neededTrams + " more trams") : status
            );
            GUI.Instance.UpdateRouteStatus(id, status, neededTrams);

            if (working && Rnd.Next(1000) < 5) {
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
        private Point nextStop = new Point(-1, -1);
        // Придыдущаяя/текущая остановка
        private Point prevStop = new Point(-1, -1);
        // Время до следующей остановки
        private int timeLeft = 0;
        private int totalTime = 0;
        // Находится ли трамвай на остановке
        private bool stopped = false;
        private bool staying = false;

        // Конструктор создает трамвай с указанным id, создает водителя и передает ему радио
        public Tram(int id, Radio radio) {
            driver = new Driver(this, radio);
            this.id = id;
        }

        // Устанавливает маршрут трамвая и уменьшает число нехватки трамваев маршрута
        public void setRoute(Route route) {
            this.route = route;
            nextStop = route.getFirstStop();
            prevStop = new Point(5, 5);
            timeLeft = -Rnd.Next(15);
            stopped = true;
            staying = true;
            route.tramAdded();
        }

        // Убирает трамвай с маршрута и увеличивает число нехватки трамваев маршрута
        public void resetRoute() {

            if (route == null) return;

            route.tramRemoved();
            route = null;
            nextStop = new Point(-1, -1);
            prevStop = new Point(-1, -1);
        }

        // Обновляет состояние трамвая
        public override void update() {
            if(removed) {
                throw new Exception("Can't update removed tram");
            }

            updateStatus();

            // Трамвай неисправен или стоит в депо
            if (route == null || !driver.checkRoute() || !working) {
                return;
            }

            // Событие поломки
            if (Rnd.Next(100) < 1) {
                working = false;
            }

            // Даем водителю проверить состояние трамвая и маршрута
            // Если все ок, движемся по маршруту
            if (driver.checkTram() && route.working) {
                updatePosition();
            }

        }

        // Лог
        private void updateStatus() {
            string status;

            if (working && route != null) {
                status = (stopped ? "staying" : "traveling");
                Console.WriteLine("Tram {0} is on route {1}, {3} {2}", id, route.id, nextStop, (stopped ? "staying at" : "traveling to"));
            }
            else {
                status = !working ? "waiting for repair" : "no route";
                Console.WriteLine("Tram {0} is stalling, {1}", id, status);
            }

            float timePast = totalTime - timeLeft;
            float percentage = timePast / totalTime;
            GUI.Instance.UpdateTramStatus(id, status, stopped, route?.id, prevStop, nextStop, percentage);
        }

        // Передвигает трамвай по маршруту
        private void updatePosition() {

            // Прибыли?
            if (stopped) {

                // Мы либо только что прибыли, либо время простоя вышло
                if (timeLeft == 0) {
                    
                    // Если мы уже стоим, то едем дальше
                    if (staying) {
                        var x = Math.Abs(nextStop.X - prevStop.X);
                        var y = Math.Abs(nextStop.Y - prevStop.Y);
                        totalTime = (int)Math.Ceiling(Math.Sqrt(x * x + y * y));
                        timeLeft = totalTime;
                        stopped = false;
                        staying = false;
                    }
                    else {
                        // Иначе начинаем стоять
                        timeLeft = -1;
                        staying = true;
                    }
                }
                else {
                    // Стоим
                    timeLeft++;
                }
            }
            else if(timeLeft > 0) {
                // Движемся
                timeLeft--;
            }
            else {
                // Прибываем
                prevStop = nextStop;
                nextStop = route.getNextStop(nextStop);
                stopped = true;
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
