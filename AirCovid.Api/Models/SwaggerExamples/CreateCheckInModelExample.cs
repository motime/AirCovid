using System;
using System.Collections.Generic;
using System.Linq;
using AirCovid.Data;
using AirCovid.Domain;
using Swashbuckle.AspNetCore.Filters;

namespace AirCovid.Api.Models.SwaggerExamples
{
    public class CreateCheckInModelExample : IExamplesProvider<CreateCheckInModel>
    {
        public CreateCheckInModel GetExamples()
        {
            return new CreateCheckInModel
            {
                Bags = new List<Bag>()
                {
                    new Bag {Weight = 1},
                    new Bag {Weight = 2}
                },
                PassengerId = new Guid("00000000-0000-0000-0000-000000000010")
            };
        }
    }
}