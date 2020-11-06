using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AirCovid.Domain;
using Newtonsoft.Json;

namespace AirCovid.Api.Models
{
    /// <summary>
    /// User input model for CheckIn process
    /// </summary>
    public class CreateCheckInModel
    {
        /// <summary>
        /// Reference to Passengers 
        /// </summary>
        [Required]
        public Guid? PassengerId { get; set; }

        /// <summary>
        /// List of bags for CheckIn
        /// </summary>
        public IList<Bag> Bags { get; set; } = new List<Bag>();

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}