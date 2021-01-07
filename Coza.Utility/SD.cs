using System;

namespace Coza.Utility
{
    public static class SD
    {
        // static class for utils

        // roles
        public const string Role_User_Indi = "Individual Customer";
        public const string Role_Employee = "Employee";
        public const string Role_Admin = "Admin";

        public const string ssShoppingCart = "Shopping Cart Session";

        // static details for OrderStatus
        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";
    }
}
