using System.Collections.Generic;
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
                    new Bag {Weight = 10},
                    new Bag {Weight = 5}
                },
                PassengerId = Database.Passengers[0].PassengerId
            };
        }
    }
}