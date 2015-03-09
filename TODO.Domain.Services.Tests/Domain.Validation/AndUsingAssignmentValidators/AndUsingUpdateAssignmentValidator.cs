﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TODO.Domain.Core.Entities;
using TODO.Domain.Services.Validation;

namespace TODO.Tests.Domain.Validation.AndUsingAssignmentValidators
{
    [TestFixture]
    public class AndUsingUpdateAssignmentValidator
    {
        public DomainTestContext2 DomainTestContext2 { get; set; }

        [SetUp]
        public void SetUp()
        {
            DomainTestContext2 = new DomainTestContext2();
        }

        [Test]
        public void AndAssignmentIsNull()
        {
            // Arrange
            // Action
            // Assert
            Assert.AreEqual("The task is null.", DomainTestContext2.UpdateAssignmentValidator.Validate(null).First());
        }

        [Test]
        public void AndAssignmentIsValid()
        {
            // Arrange
            var goodTask = new Assignment {Id = 0, Name = "Some important work, man!", Done = false, DueDate = DateTime.Today};
            DomainTestContext2.AssignmentService.Create(goodTask);
            // Action
            goodTask.Id = 1;
            goodTask.Name = "New new new name";
            var result = DomainTestContext2.UpdateAssignmentValidator.Validate(goodTask);
            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void AndAssignmentDoesNotExist()
        {
            // Arrange
            var badTask = new Assignment { Id = 985, Done = false, DueDate = DateTime.Today, Name = "Do some work" };
            // Action
            var result = DomainTestContext2.UpdateAssignmentValidator.Validate(badTask);
            // Assert
            Assert.AreEqual("There's no task with this id.", result.First());
        }

        [Test]
        public void AndAssignmentIdIsInvalid()
        {
            // Arrange
            var badTasks = new List<Assignment>
            {
                new Assignment {Id = int.MinValue, Done = false, DueDate = DateTime.Today, Name = "Do some work"},
                new Assignment {Id = -6787, Done = false, DueDate = DateTime.Today, Name = "Do some work"},
                new Assignment {Id = 0, Done = false, DueDate = DateTime.Today, Name = "Do some work"}
            };

            // Action
            var results = new List<DomainValidationException>();
            foreach (var badTask in badTasks)
            {
                results.Add(new DomainValidationException{ValidationErrors = DomainTestContext2.UpdateAssignmentValidator.Validate(badTask).ToList()});
            }

            // Assert
            foreach (var result in results)
            {
                Assert.AreEqual("Id is invalid.", result.ValidationErrors.First());
            }
        }

        [Test]
        public void AndAssignmentNameIsEmptyOrNullOrWhiteSpace()
        {
            // Arrange
            var badTasks = new List<Assignment>
            {
                new Assignment {Id = 2, Done = false, DueDate = DateTime.Today, Name = string.Empty},
                new Assignment {Id = 2, Done = false, DueDate = DateTime.Today, Name = null},
                new Assignment {Id = 2, Done = false, DueDate = DateTime.Today, Name = "    "}
            };

            // Action
            var results = new List<DomainValidationException>();
            foreach (var badTask in badTasks)
            {
                results.Add(new DomainValidationException { ValidationErrors = DomainTestContext2.UpdateAssignmentValidator.Validate(badTask).ToList() });
            }

            // Assert
            foreach (var result in results)
            {
                Assert.AreEqual("Name is invalid.", result.ValidationErrors.First());
            }
        }

        [Test]
        public void AndAssignmentDueDateIsInvalid()
        {
            // Arrange
            var badTask = new Assignment
            {
                Id = 2,
                Done = false,
                DueDate = new DateTime(DateTime.Now.Year, DateTime.Today.Month - 1, DateTime.Today.Day - 1),
                Name = "Do some work"
            };

            // Action
            var results = DomainTestContext2.UpdateAssignmentValidator.Validate(badTask);

            // Assert
            Assert.AreEqual("Date is invalid.", results.First());
        }
    }
}
