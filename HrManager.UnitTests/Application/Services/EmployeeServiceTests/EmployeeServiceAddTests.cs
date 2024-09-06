using System;
using AutoFixture;
using HrManager.Application.Models.InputModels;
using HrManager.Application.Services;
using HrManager.Core.Entities;
using HrManager.Core.Exceptions;
using HrManager.Core.Repositories;
using Moq;
using Shouldly;
using Xunit;

namespace HrManager.UnitTests.Application.Services.EmployeeServiceTests
{
	public class EmployeeServiceAddTests
	{
		[Fact]
		//Func ref return
		public void ValidEmployee_AddIsCalled_ReturnValidEmployeeViewModel()
		{
			// Arrange
			var addEmploymentInputModel = new Fixture().Create<AddEmployeeInputModel>();
			addEmploymentInputModel.BirthDate = DateTime.Today.AddYears(-29);

			var employeeRepositoryMock = new Mock<IEmployeeRepository>();

			var employeeService = new EmployeeService(employeeRepositoryMock.Object);

			// Act
			var result = employeeService.Add(addEmploymentInputModel);

			result.FullName.ShouldBe(addEmploymentInputModel.FullName);
			result.Document.ShouldBe(addEmploymentInputModel.Document);
			result.BirthDate.ShouldBe(addEmploymentInputModel.BirthDate);
			result.RoleLevel.ShouldBe(addEmploymentInputModel.RoleLevel);
			result.Role.ShouldBe(addEmploymentInputModel.Role);

			// Assert
			employeeRepositoryMock.Verify(er => er.Add(It.IsAny<Employee>()), Times.Once);
		}

		[Fact]
		public void InvalidBirthDateForEmployee_AddIsCalled_ThrowAnInvalidBirthDateException()
        {
			// Arrange
			var addEmployeeInputModel = new Fixture().Create<AddEmployeeInputModel>();
			addEmployeeInputModel.BirthDate = DateTime.Today.AddDays(1);

			var employeeRepositoryMock = new Mock<IEmployeeRepository>();

			var employeeService = new EmployeeService(employeeRepositoryMock.Object);

			// Act + Assert
			var exception = Assert.Throws<BirthDateCannotBeInTheFutureException>(() => 
				employeeService.Add(addEmployeeInputModel));
			
			Assert.Equal("Birth date cannot be in the future.", exception.Message);

			Should.Throw<BirthDateCannotBeInTheFutureException>(() => 
				employeeService.Add(addEmployeeInputModel))
				.Message.ShouldBe("Birth date cannot be in the future.");
		}
	}
}

