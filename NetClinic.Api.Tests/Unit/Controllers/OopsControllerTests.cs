using Microsoft.AspNetCore.Mvc;
using NetClinic.Api.Controllers;
using Xunit;
using FluentAssertions;
using System;

namespace NetClinic.Api.Tests.Unit.Controllers;

public class OopsControllerTests
{
    private readonly OopsController _controller;

    public OopsControllerTests()
    {
        _controller = new OopsController();
    }

    [Fact]
    public void Get_ShouldThrowException_WithExpectedMessage()
    {
        // Act & Assert
        var exception = Assert.Throws<Exception>(() => _controller.Get());
        exception.Message.Should().Be("Expected: controller used to showcase what happens when an exception is thrown");
    }
}