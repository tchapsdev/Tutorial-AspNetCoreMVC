using Microsoft.AspNetCore.Mvc;
using Moq;
using WebAppMVC.Controllers;
using WebAppMVC.DAL;
using WebAppMVC.Models;

namespace WebAppMVC.Tests.Controllers
{
    public class CustomersControllerTests
    {
        [Fact]
        public async Task Index_ReturnsAViewResult_WithoutFilters()
        {
            // Arrange
            var mockRepo = new Mock<ICustomerRepository>();

            mockRepo.Setup(x => x.GetCustomers()).ReturnsAsync(GetCustomers());

            var controller = new CustomersController(mockRepo.Object);

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsAssignableFrom<IEnumerable<Customer>>(viewResult.Model);

            Assert.Equal(3, model.Count());
        }


        [Theory]
        [InlineData("tchaps", 1)]
        [InlineData("Da", 2)]
        [InlineData("ALex", 0)]
        [InlineData("438-125", 2)]
        [InlineData("consulting", 1)]
        public async Task Index_ReturnsAViewResult_When_Filters(string q, int qty)
        {
            // Arrange
            var mockRepo = new Mock<ICustomerRepository>();

            mockRepo.Setup(repo => repo.GetCustomers())
                    .ReturnsAsync(GetCustomers())
                    .Verifiable();

            var controller = new CustomersController(mockRepo.Object);

            // Act
            var result = await controller.Index(q);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsAssignableFrom<IEnumerable<Customer>>(viewResult.ViewData.Model);

            Assert.Equal(qty, model.Count());
            mockRepo.Verify();
        }


        [Fact]
        public async Task Details_ReturnsNotFound_WhenIdNull()
        {
            // Arrange
            var mockRepo = new Mock<ICustomerRepository>();

            mockRepo.Setup(repo => repo.GetCustomers())
                .ReturnsAsync(GetCustomers())
                .Verifiable();

            var controller = new CustomersController(mockRepo.Object);

            // Act
            var result = await controller.Details(id: null);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);

            Assert.Equal(404, notFoundResult.StatusCode);
        }




        [Fact]
        public async Task Details_ReturnsNotFound_WhenCustomerNotFound()
        {
            // Arrange
            var mockRepo = new Mock<ICustomerRepository>();
            int testCustomerId = 5;

            mockRepo.Setup(repo => repo.GetCustomerById(testCustomerId))
                    //.ReturnsAsync(default(Customer?))
                    .Verifiable();

            var controller = new CustomersController(mockRepo.Object);

            // Act
            var result = await controller.Details(id: testCustomerId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);

            Assert.Equal(404, notFoundResult.StatusCode);
            mockRepo.Verify(v => v.GetCustomerById(It.IsAny<int>()), Times.Once());
        }




        [Fact]
        public async Task Details_ReturnsCustomer_WhenCustomerExists()
        {
            // Arrange
            var mockRepo = new Mock<ICustomerRepository>();
            int testCustomerId = 1;

            mockRepo.Setup(repo => repo.GetCustomerById(testCustomerId))
                    .ReturnsAsync(GetCustomers().First())
                    .Verifiable();

            var controller = new CustomersController(mockRepo.Object);

            // Act
            var result = await controller.Details(id: testCustomerId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

            mockRepo.Verify();

            var model = Assert.IsAssignableFrom<Customer>(viewResult.ViewData.Model);

            Assert.Equal("Tchaps", model.Name);
            Assert.Equal("CA", model.Country);
            Assert.Equal("438-126-4569", model.Phone);
            Assert.Equal("consulting@tchapssolution.com", model.Email);
        }




        [Fact]
        public async Task PostCreate_Returns_ModelInvalid()
        {
            // Arrange
            var mockRepo = new Mock<ICustomerRepository>();

            mockRepo.Setup(repo => repo.InsertCustomer(GetCustomers().First()))
                    .Verifiable();

            mockRepo.Setup(repo => repo.Save())
                    .Verifiable();

            var controller = new CustomersController(mockRepo.Object);
            controller.ModelState.AddModelError("error", "Invalid model");

            // Act
            var result = await controller.Create(customer: new Customer());

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsAssignableFrom<Customer>(viewResult.ViewData.Model);

            mockRepo.Verify(v => v.InsertCustomer(It.IsAny<Customer>()), Times.Never());
            mockRepo.Verify(v => v.Save(), Times.Never());
        }


        [Fact]
        public async Task PostCreate_Returns_RedirectToAction()
        {
            // Arrange
            var mockRepo = new Mock<ICustomerRepository>();
            int testCustomerId = 1;

            mockRepo.Setup(repo => repo.InsertCustomer(It.IsAny<Customer>()))
                    .Verifiable();

            mockRepo.Setup(repo => repo.Save())
                    .Verifiable();

            var controller = new CustomersController(mockRepo.Object);

            // Act
            var result = await controller.Create(customer: GetCustomers().First());

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", viewResult.ActionName);
            // Assert.Equal("CustomersController", viewResult.ControllerName);

            mockRepo.Verify(v => v.InsertCustomer(It.IsAny<Customer>()), Times.Once());
            mockRepo.Verify(v => v.Save(), Times.Once());
        }


        private IEnumerable<Customer> GetCustomers()
        {
            // This is a mock data source. In a real application, this would be replaced with actual data from a database or other source.
            return new List<Customer> {
                new() { Id = 1, Name = "Tchaps", Country = "CA", Phone = "438-126-4569", Email = "consulting@tchapssolution.com" },
                new() { Id = 2, Name = "Daniel", Country = "CM", Phone = "438-125-4569", Region = "", Email = "" },
                new() { Id = 3, Name = "Daniella", Country = "US", Phone = "438-125-4569", Region = "" , Email = "" }
            };
        }



    }
}
