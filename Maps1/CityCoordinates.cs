using System.Collections.Generic;

namespace Maps1
{
    class CityCoordinates
    {
        public static Dictionary<string,Coordinates> CoordinatesMap = new Dictionary<string, Coordinates> {
            {"Kraków", new Coordinates(50.06260146281688, 19.938419861481215) } };
    }
    class Coordinates
    {
        public Coordinates(double lat, double lng)
        {
            this.latitude = lat;
            this.longitude = lng;
        }
        public Coordinates(){}
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
}
