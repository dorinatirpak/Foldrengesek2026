using Földrengések2026.Controllers;

using Földrengések2026.Models;

using Microsoft.AspNetCore.Mvc;


namespace Földrengések2026.Tests.Queries;


public class FeladatTests

{

    [Fact]

    public void Feladat2_Returns_Somogy_Telepulesek_Alphabetical_Order()

    {

        // Arrange

        var ctx = TestDbFactory.CreateContext(nameof(Feladat2_Returns_Somogy_Telepulesek_Alphabetical_Order));


        ctx.Telepulesek.AddRange(

        new Telepules { ID = 1, Nev = "Kaposvár", Varmegye = "Somogy" },

        new Telepules { ID = 2, Nev = "Barcs", Varmegye = "Somogy" },

        new Telepules { ID = 3, Nev = "Siófok", Varmegye = "Somogy" },

        new Telepules { ID = 4, Nev = "Szekszárd", Varmegye = "Tolna" }

        );

        ctx.SaveChanges();


        var controller = new FeladatokController(ctx);


        // Act

        var result = controller.Feladat2() as ViewResult;


        // Assert – ViewResult

        // "0." Az eredmény nem null

        Assert.NotNull(result);


        var model = Assert.IsAssignableFrom<IEnumerable<string>>(result!.Model);


        var list = model.ToList();


        // 1️. Csak Somogy

        Assert.DoesNotContain("Szekszárd", list);


        // 2️. Darabszám

        Assert.Equal(3, list.Count);


        // 3️. ABC sorrend

        Assert.Equal(

        new[] { "Barcs", "Kaposvár", "Siófok" },

        list

        );

    }

}