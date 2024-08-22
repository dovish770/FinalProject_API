using PojectFinal___API.Modles;

namespace PojectFinal___API.Services
{
    public class TimeDistanceService
    {
        //חישוב מרחק בין סוכן למטרה
        public static double Distance(int x1, int y1, int x2, int y2)
        {            

            var distance = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));

            return distance;
        }

        //חישוב ועידכון זמן שנותר 
        public static string UpdateTimeLeft(Mission mission)
        {
            if (mission.Status.ToString() != "Actice")
            {
                return "0";
            }

            var x1 = mission.Agent.x;
            var y1 = mission.Agent.y;

            var x2 = mission.Target.x;
            var y2 = mission.Target.y;

            var distance = Distance(x1, y1, x2, y2);
             (distance/5).ToString();

            return (distance/5).ToString();
        }

        //חישוב ועידכון משך המשימה
        public Mission UpdateDuration(Mission mission)
        {
            if (mission.Status.ToString() != "Actice")
            {
                return mission;
            }            
            mission.Duration += (1/5);
            return mission;
        }
    }
}
