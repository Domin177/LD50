namespace DefaultNamespace
{
    public class Utils
    {
        public static int GetPercent(float val, float max)
        {
            return (int) (val * 100 / max);
        }

        public static float normalize(float val, float maxVal, float to)
        {
            return val * to / maxVal;
        }
    }
}