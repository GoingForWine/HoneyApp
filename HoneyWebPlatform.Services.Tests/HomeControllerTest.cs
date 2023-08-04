﻿namespace CarRentingSystem.Test.Controllers
{
    using System;
    using System.Collections.Generic;
    using CarRentingSystem.Controllers;
    using CarRentingSystem.Services.Cars.Models;
    using FluentAssertions;
    using MyTested.AspNetCore.Mvc;
    using Xunit;

    using static Data.Cars;
    using static WebConstants.Cache;

    public class HomeControllerTest
    {
        [Fact]
        public void IndexShouldReturnCorrectViewWithModel()
            => MyController<HomeController>
                .Instance(controller => controller
                    .WithData(TenPublicCars))
                .Calling(c => c.Index())
                .ShouldHave()
                .MemoryCache(cache => cache
                    .ContainingEntry(entry => entry
                        .WithKey(LatestCarsCacheKey)
                        .WithAbsoluteExpirationRelativeToNow(TimeSpan.FromMinutes(15))
                        .WithValueOfType<List<LatestCarServiceModel>>()))
                .AndAlso()
                .ShouldReturn()
                .View(view => view
                    .WithModelOfType<List<LatestCarServiceModel>>()
                    .Passing(model => model.Should().HaveCount(3)));

        [Fact]
        public void indexshouldreturnviewwithcorrectmodelanddata()    //controller test 
            =>
                //arrange
                MyController<HomeController>
                    .Instance(controller => controller
                        .WithData(GetCars))
                    //act
                    .calling(c => c.Index())
                    //assert
                    .shouldreturn()
                    .view(view => view
                        .withmodeloftype<IndexViewModel>()
                        .passing(m => m.cars.should().havecount(3)));

        [Fact]
        public void indexshouldreturnviewwithcorrectmodelanddaaata()    //pipeline test = integration + route test
            =>
                //arrange
                MyMvc
                    .pipeline()                                         //route test
                    .shouldmap("/")
                    .to<HomeController>(c => c.index())                 //controller test
                    
                    //act                                               //return controller test
                    .which(controller => controller
                        .withdata(GetCars)))
                    //assert
                    .shouldreturn()
                    .view(view => view
                        .withmodeloftype<IndexViewModel>()
                        .passing(m => m.cars.should().havecount(3)));



        [Fact]
        public void ErrorShouldReturnView()
            => MyController<HomeController>
                .Instance()
                .Calling(c => c.Error())
                .ShouldReturn()
                .View();
    }
}