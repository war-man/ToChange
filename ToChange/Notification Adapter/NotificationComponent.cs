using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using ToChange.DB_Connections;
using ToChange.Models;

namespace ToChange.Notification_Adapter
{
    public class NotificationComponent
    {

        public void RegisterNotification(DateTime currentTime)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ToChangeDBModelEntities"].ConnectionString;
            string SqlCommand = @"Select ID,firstCustomerID,secondCustomerID,ProductID,AddedOn,Pending,Accepted,ProductIDToSwapWith,RespondAt
                from Poke where AddedOn > @AddedOn";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(SqlCommand , con);
                cmd.Parameters.AddWithValue("@AddedOn", currentTime);
                if (con.State != System.Data.ConnectionState.Open)
                    con.Open();
                cmd.Notification = null;
                SqlDependency sqlDep = new SqlDependency(cmd);
                sqlDep.OnChange += sqlDep_OnChange;
                using (SqlDataReader reader = cmd.ExecuteReader()) { }
            }
        }

        void sqlDep_OnChange(object sender , SqlNotificationEventArgs e)
        {
            if(e.Type == SqlNotificationType.Change)
            {
                SqlDependency sqlDep = sender as SqlDependency;
                sqlDep.OnChange -= sqlDep_OnChange;

                //send notification message to client
                var notificationHub = GlobalHost.ConnectionManager.GetHubContext<NofiticationHub>();
                notificationHub.Clients.All.notify("added");

                //re-register notification
                RegisterNotification(DateTime.Now);

            }
        }

        public List<Poke> GetAllPokes(DateTime afterNow , string UserID)
        {
            //DateTime1.CompareTo.DateTime2 ==> -1 =>DateTime1 Earlier than DateTime2 , 0 => Same Date , 1 => DateTime1 Later than DateTime2
            return DAL.Pokes.Where(a => (a.AddedOn.CompareTo(afterNow) == -1 && a.secondCustomerID.Equals(UserID) && a.Pending.Equals(true))
            || (a.AddedOn.CompareTo(afterNow) == -1 && a.firstCustomerID.Equals(UserID) && a.Pending.Equals(true) && a.ProductIDToSwapWith != 0)).OrderByDescending(a => a.AddedOn).ToList();
        }

    }
}