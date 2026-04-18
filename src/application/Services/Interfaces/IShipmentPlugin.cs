namespace application.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;


    public interface IShipmentPlugin
    {

        [Description("Retrieves the total number of containers booked in a booking")]
        public int GetTotalContainers([Description("The booking ID")] string bookingId);


        [Description("Retrieves the current status of a shipment booking")]
        public string GetBookingStatus([Description("The booking ID")] string bookingId);


        [Description("Retrieves the total cargo weight in kilograms for a booking")]
        public double GetTotalCargoWeight([Description("The booking ID")] string bookingId);


        [Description("Retrieves the port of origin for a shipment booking")]

        public string GetOriginPort([Description("The booking ID")] string bookingId);
        [Description("Retrieves the destination port for a shipment booking")]
        public string GetDestinationPort([Description("The booking ID")] string bookingId);


        [Description("Retrieves the estimated time of arrival for a shipment booking")]
        public string GetEstimatedArrival([Description("The booking ID")] string bookingId);


        [Description("Retrieves the vessel name and voyage number assigned to a booking")]
        public string GetVesselDetails([Description("The booking ID")] string bookingId);

        [Description("Retrieves the list of container numbers associated with a booking")]
        public IEnumerable<string> GetContainerNumbers([Description("The booking ID")] string bookingId);

    }
}
