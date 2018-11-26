using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public partial class MAVLink
{
    public interface IMAVLink_API
    {
        byte[] arm();
        byte[] land();
        byte[] pause();
        byte[] disarm();
        byte[] guided();
        byte[] return2launch();
        byte[] takeoff(float altitude);
        byte[] changealtitude(float altitude);
        byte[] gotolocation(float longitude, float latitude, float altitude);
    }
}




