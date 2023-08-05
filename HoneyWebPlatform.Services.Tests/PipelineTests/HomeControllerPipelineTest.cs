using HoneyWebPlatform.Web.ViewModels.Home;

namespace HoneyWebPlatform.Services.Tests.PipelineTests
{
    using System.Collections.Generic;

    using FluentAssertions;
    using MyTested.AspNetCore.Mvc;
    using Xunit;

    using HoneyWebPlatform.Web;
    using HoneyWebPlatform.Web.Controllers;
    using HoneyWebPlatform.Web.ViewModels;

    using static DataForTests.Honeys;

    public class HomeControllerPipelineTest
    {
        //[Fact]
        //public void IndexShouldReturnViewWithCorrectModelAndData()
        //    => MyMvc
        //        .Pipeline()
        //        .ShouldMap("/")
        //        .To<HomeController>(c => c.Index())
        //        .Which(controller => controller
        //            .WithData(TenPublicHoneys))
        //        .ShouldReturn()
        //        .View(view => view
        //            .WithModelOfType<List<IndexViewModel>>()
        //            .Passing(m => m.Should().HaveCount(3)));

        //[Fact]
        //public void ErrorShouldReturnView()
        //    => MyMvc
        //        .Pipeline()
        //        .ShouldMap("/Home/Error")
        //        .To<HomeController>(c => c.Error())
        //        .Which()
        //        .ShouldReturn()
        //        .View();
    }
}