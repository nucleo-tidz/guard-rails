namespace infrastructure.Agents.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;

    using application.Services.Interfaces;

    internal class ShipmentPlugin : IShipmentPlugin
    {
        [Description("Retrieves the total number of containers booked in a booking")]
        public int GetTotalContainers([Description("The booking ID")] string bookingId)
        {
            return 894;
        }

        [Description("Retrieves the current status of a shipment booking")]
        public string GetBookingStatus([Description("The booking ID")] string bookingId)
        {
            return "In Transit";
        }

        [Description("Retrieves the total cargo weight in kilograms for a booking")]
        public double GetTotalCargoWeight([Description("The booking ID")] string bookingId)
        {
            return 21500.75;
        }

        [Description("Retrieves the port of origin for a shipment booking")]
        public string GetOriginPort([Description("The booking ID")] string bookingId)
        {
            return "Port of Shanghai (SHA)";
        }

        [Description("Retrieves the destination port for a shipment booking")]
        public string GetDestinationPort([Description("The booking ID")] string bookingId)
        {
            return "Port of Rotterdam (RTM)";
        }

        [Description("Retrieves the estimated time of arrival for a shipment booking")]
        public string GetEstimatedArrival([Description("The booking ID")] string bookingId)
        {
            return DateTime.UtcNow.AddDays(14).ToString("yyyy-MM-dd");
        }

        [Description("Retrieves the vessel name and voyage number assigned to a booking")]
        public string GetVesselDetails([Description("The booking ID")] string bookingId)
        {
            return "Vessel: MSC Eleonora | Voyage: ME241W";
        }

        [Description("Retrieves the list of container numbers associated with a booking")]
        public IEnumerable<string> GetContainerNumbers([Description("The booking ID")] string bookingId)
        {
            return ["MSCU1234567", "MSCU2345678", "MSCU3456789"];
        }
    }
}
