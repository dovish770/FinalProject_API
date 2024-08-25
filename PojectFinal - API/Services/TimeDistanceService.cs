using PojectFinal___API.Enums;
using PojectFinal___API.Modles;

namespace PojectFinal___API.Services
{
    public class TimeDistanceService
    {
        //חישוב מרחק בין סוכן למטרה
        public static double Distance(double x1, double y1, double x2, double y2)
        {            

            var distance = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));

            return distance;
        }

        //חישוב ועידכון זמן שנותר 
        public static double UpdateTimeLeft(Mission mission)
        {            
            var x1 = mission.Agent.x;
            var y1 = mission.Agent.y;

            var x2 = mission.Target.x;
            var y2 = mission.Target.y;

            var distance = Distance(x1, y1, x2, y2);
             

            return distance/5;
        }

        //חישוב ועידכון משך המשימה
        public static double UpdateDuration(Mission mission)
        {
            if (mission.Status.ToString() != "Actice")
            {
                return 0;
            }                       
            return 1/5;
        }
    }
}
