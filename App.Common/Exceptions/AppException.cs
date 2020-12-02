using App.Common.SeedWork;
using System;
using System.Collections.Generic;
using System.Net;

namespace App.Common.Exceptions
{
    public class AppException : Exception
    {
        public ActionResponse ActionResponse { get; set; }

        public AppException(string message) : base(message)
        {
            ActionResponse = new ActionResponse
            {
                StatusCode = HttpStatusCode.BadRequest,
                Title = message
            };
        }
    }
}
