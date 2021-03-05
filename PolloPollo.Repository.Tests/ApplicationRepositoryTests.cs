﻿using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using PolloPollo.Entities;
using PolloPollo.Services.Utils;
using PolloPollo.Shared;
using PolloPollo.Shared.DTO;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PolloPollo.Services.Tests
{
    public class ApplicationRepositoryTests
    {
        [Fact]
        public async Task CreateAsync_given_null_returns_Null()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var result = await repository.CreateAsync(default(ApplicationCreateDTO));

                Assert.Null(result);
            }
        }

        [Fact]
        public async Task CreateAsync_unavailable_product_returns_unavailable_application_status()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var user = new User
                {
                    Id = 1,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK"
                };

                var userEnumRole = new UserRole
                {
                    UserId = 1,
                    UserRoleEnum = UserRoleEnum.Producer
                };


                var product = new Product
                {
                    Id = 42,
                    Title = "5 chickens",
                    UserId = 1,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "Test",
                    Available = false
                };
                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Products.Add(product);
                await context.SaveChangesAsync();

                var applicationDTO = new ApplicationCreateDTO
                {
                    ProductId = product.Id
                    //Nothing
                };

                var testApplicationDTO = new ApplicationDTO
                {
                    Status = ApplicationStatusEnum.Unavailable
                };

                var result = await repository.CreateAsync(applicationDTO);

                Assert.Equal(testApplicationDTO.Status, result.Status);
            }
        }

        [Fact]
        public async Task CreateAsync_given_empty_DTO_returns_Null()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var user = new User
                {
                    Id = 1,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK"
                };

                var userEnumRole = new UserRole
                {
                    UserId = 1,
                    UserRoleEnum = UserRoleEnum.Producer
                };


                var product = new Product
                {
                    Id = 42,
                    Title = "5 chickens",
                    UserId = 1,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "Test",
                    Available = true
                };
                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Products.Add(product);
                await context.SaveChangesAsync();

                var applicationDTO = new ApplicationCreateDTO
                {
                    ProductId = product.Id
                    //Nothing
                };

                var result = await repository.CreateAsync(applicationDTO);

                Assert.Null(result);
            }
        }

        [Fact]
        public async Task CreateAsync_given_invalid_DTO_returns_Null()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var user = new User
                {
                    Id = 1,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK"
                };

                var userEnumRole = new UserRole
                {
                    UserId = 1,
                    UserRoleEnum = UserRoleEnum.Producer
                };


                var product = new Product
                {
                    Id = 42,
                    Title = "5 chickens",
                    UserId = 1,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "Test",
                    Available = true
                };
                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Products.Add(product);
                await context.SaveChangesAsync();

                var applicationDTO = new ApplicationCreateDTO
                {
                    ProductId = product.Id,
                    Motivation = "This is not a very good motivation.",
                };

                var result = await repository.CreateAsync(applicationDTO);

                Assert.Null(result);
            }
        }

        [Fact]
        public async Task CreateAsync_given_DTO_returns_DTO()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var id = 1;

                var user = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK"
                };

                var userEnumRole = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var product = new Product
                {
                    Id = 42,
                    Title = "5 chickens",
                    UserId = 1,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "Test",
                    Available = true
                };

                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Products.Add(product);
                await context.SaveChangesAsync();

                var applicationDTO = new ApplicationCreateDTO
                {
                    UserId = user.Id,
                    ProductId = product.Id,
                    Motivation = "Test",
                };

                var result = await repository.CreateAsync(applicationDTO);

                Assert.Equal(applicationDTO.UserId, result.ReceiverId);
                Assert.Equal(applicationDTO.Motivation, result.Motivation);
                Assert.Equal(ApplicationStatusEnum.Open, result.Status);
                Assert.Equal(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), result.CreationDate);
            }
        }

        [Fact]
        public async Task CreateAsync_given_DTO_sets_Timestamp_in_database()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var id = 1;

                var user = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK"
                };

                var userEnumRole = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var product = new Product
                {
                    Id = 42,
                    Title = "5 chickens",
                    UserId = 1,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "Test",
                    Available = true
                };

                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Products.Add(product);
                await context.SaveChangesAsync();

                var applicationDTO = new ApplicationCreateDTO
                {
                    UserId = user.Id,
                    ProductId = product.Id,
                    Motivation = "Test",
                };

                var result = await repository.CreateAsync(applicationDTO);

                var now = DateTime.UtcNow;
                var dbTimestamp = context.Applications.Find(result.ApplicationId).Created;

                // These checks are to assume the timestamp is set on creation.
                // The now timestamp is some ticks off from the database timestamp.
                Assert.Equal(dbTimestamp.Date, now.Date);
                Assert.Equal(dbTimestamp.Hour, now.Hour);
                Assert.Equal(dbTimestamp.Minute, now.Minute);
            }
        }

        [Fact]
        public async Task CreateAsync_given_DTO_returns_DTO_with_Id()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var id = 1;

                var user = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK"
                };

                var userEnumRole = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var product = new Product
                {
                    Id = 42,
                    Title = "5 chickens",
                    UserId = 1,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "Test",
                    Available = true
                };

                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Products.Add(product);
                await context.SaveChangesAsync();

                var applicationDTO = new ApplicationCreateDTO
                {
                    UserId = user.Id,
                    ProductId = product.Id,
                    Motivation = "Test",
                };

                var result = await repository.CreateAsync(applicationDTO);

                var expectedId = 1;

                Assert.Equal(expectedId, result.ApplicationId);
            }
        }

        [Fact]
        public async Task FindAsync_given_existing_Id_returns_ProductDTO()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {

                var id = 1;

                var user = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK",
                    Thumbnail = "test"
                };

                var userEnumRole = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var product = new Product
                {
                    Id = id,
                    Title = "5 chickens",
                    UserId = 1,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "Test",
                };

                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Products.Add(product);

                var entity = new Application
                {
                    UserId = id,
                    ProductId = id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 04, 08),
                    Status = ApplicationStatusEnum.Open
                };

                context.Applications.Add(entity);
                await context.SaveChangesAsync();

                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var application = await repository.FindAsync(entity.Id);

                Assert.Equal(entity.Id, application.ApplicationId);
                Assert.Equal(entity.UserId, application.ReceiverId);
                Assert.Equal($"{user.FirstName} {user.SurName}", application.ReceiverName);
                Assert.Equal(user.Country, application.Country);
                Assert.Equal(ImageHelper.GetRelativeStaticFolderImagePath(user.Thumbnail), application.Thumbnail);
                Assert.Equal(id, application.ProductId);
                Assert.Equal(product.Title, application.ProductTitle);
                Assert.Equal(product.Price, application.ProductPrice);
                Assert.Equal(product.UserId, application.ProducerId);
                Assert.Equal(entity.Motivation, application.Motivation);
                Assert.Equal(entity.Status, application.Status);
            }
        }

        [Fact]
        public async Task FindAsync_given_nonExisting_Id_returns_null()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var result = await repository.FindAsync(42);

                Assert.Null(result);
            }
        }

        [Fact]
        public async Task ReadOpen_returns_all_open_Applications()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var id = 1;

                var user = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK",
                    Thumbnail = "test"
                };

                var userEnumRole = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var product = new Product
                {
                    Id = id,
                    Title = "5 chickens",
                    UserId = id,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "Test",
                };

                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Products.Add(product);

                var entity1 = new Application
                {
                    UserId = id,
                    ProductId = id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 04, 08),
                    Status = ApplicationStatusEnum.Open
                };

                var entity2 = new Application
                {
                    UserId = id,
                    ProductId = id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 03, 08),
                    Status = ApplicationStatusEnum.Pending
                };

                context.Applications.AddRange(entity1, entity2);
                await context.SaveChangesAsync();

                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var applications = repository.ReadOpen();

                // There should only be one application in the returned list
                // since one of the created applications is not open
                var count = applications.ToList().Count;
                Assert.Equal(1, count);

                var application = applications.First();



                Assert.Equal(entity1.Id, application.ApplicationId);
                Assert.Equal(entity1.UserId, application.ReceiverId);
                Assert.Equal($"{user.FirstName} {user.SurName}", application.ReceiverName);
                Assert.Equal(user.Country, application.Country);
                Assert.Equal(ImageHelper.GetRelativeStaticFolderImagePath(user.Thumbnail), application.Thumbnail);
                Assert.Equal(id, application.ProductId);
                Assert.Equal(product.Title, application.ProductTitle);
                Assert.Equal(product.Price, application.ProductPrice);
                Assert.Equal(product.UserId, application.ProducerId);
                Assert.Equal(entity1.Motivation, application.Motivation);
                Assert.Equal(entity1.Status, application.Status);
            }
        }

        [Fact]
        public async Task ReadOpen_returns_all_open_Applications_order_by_timestamp_descending()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var id = 1;

                var user = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK",
                    Thumbnail = "test"
                };

                var userEnumRole = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var product = new Product
                {
                    Id = id,
                    Title = "5 chickens",
                    UserId = id,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "Test",
                };

                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Products.Add(product);

                var entity1 = new Application
                {
                    UserId = id,
                    ProductId = id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 1, 1, 1, 1, 1),
                    Status = ApplicationStatusEnum.Open
                };

                var entity2 = new Application
                {
                    UserId = id,
                    ProductId = id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 1, 1, 1, 10, 1),
                    Status = ApplicationStatusEnum.Open
                };

                context.Applications.AddRange(entity1, entity2);
                await context.SaveChangesAsync();

                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var applications = await repository.ReadOpen().ToListAsync();

                var application = applications.ElementAt(0);
                var secondApplication = applications.ElementAt(1);

                Assert.Equal(entity2.Id, application.ApplicationId);
                Assert.Equal(entity1.Id, secondApplication.ApplicationId);
            }
        }

        [Fact]
        public async Task ReadFiltered_returns_all_open_Applications()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var id = 1;
                var id2 = 2;

                var user = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK",
                    Thumbnail = "test"
                };

                var userEnumRole = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var producer = new Producer
                {
                    Id = id,
                    PairingSecret = "1234",
                    UserId = id,
                    Street = "Test",
                    StreetNumber = "42",
                    City = "TestBy"
                };

                var product = new Product
                {
                    Id = id,
                    Title = "5 chickens",
                    UserId = id,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "DK",
                };

                var producer2 = new Producer
                {
                    Id = id2,
                    PairingSecret = "1234",
                    UserId = id2,
                    Street = "Test",
                    StreetNumber = "42",
                    City = "TestBy2"
                };

                var user2 = new User
                {
                    Id = id2,
                    Email = "test2@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "NO",
                    Thumbnail = "test"
                };

                var userEnumRole2 = new UserRole
                {
                    UserId = id2,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var product2 = new Product
                {
                    Id = id2,
                    Title = "5 chickens",
                    UserId = id2,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "NO",
                };

                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Producers.Add(producer);
                context.Products.Add(product);

                context.Users.Add(user2);
                context.UserRoles.Add(userEnumRole2);
                context.Producers.Add(producer2);
                context.Products.Add(product2);

                var entity1 = new Application
                {
                    UserId = id,
                    ProductId = id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 04, 08),
                    Status = ApplicationStatusEnum.Open
                };

                var entity2 = new Application
                {
                    UserId = id2,
                    ProductId = id2,
                    Motivation = "Test",
                    Created = new DateTime(2019, 03, 08),
                    Status = ApplicationStatusEnum.Open
                };

                context.Applications.AddRange(entity1, entity2);
                await context.SaveChangesAsync();

                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var applications = repository.ReadFiltered();

                var count = applications.ToList().Count;
                Assert.Equal(2, count);

                var application = applications.First();

                Assert.Equal(entity1.Id, application.ApplicationId);
                Assert.Equal(entity1.UserId, application.ReceiverId);
                Assert.Equal($"{user.FirstName} {user.SurName}", application.ReceiverName);
                Assert.Equal(user.Country, application.Country);
                Assert.Equal(ImageHelper.GetRelativeStaticFolderImagePath(user.Thumbnail), application.Thumbnail);
                Assert.Equal(id, application.ProductId);
                Assert.Equal(product.Title, application.ProductTitle);
                Assert.Equal(product.Price, application.ProductPrice);
                Assert.Equal(product.UserId, application.ProducerId);
                Assert.Equal(entity1.Motivation, application.Motivation);
                Assert.Equal(entity1.Status, application.Status);
            }
        }

        [Fact]
        public async Task ReadFiltered_returns_based_on_country()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var id = 1;
                var id2 = 2;

                var user = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK",
                    Thumbnail = "test"
                };

                var userEnumRole = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var producer = new Producer
                {
                    Id = id,
                    PairingSecret = "1234",
                    UserId = id,
                    Street = "Test",
                    StreetNumber = "42",
                    City = "TestBy"
                };

                var product = new Product
                {
                    Id = id,
                    Title = "5 chickens",
                    UserId = id,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "DK",
                };

                var producer2 = new Producer
                {
                    Id = id2,
                    PairingSecret = "1234",
                    UserId = id2,
                    Street = "Test",
                    StreetNumber = "42",
                    City = "TestBy2"
                };

                var user2 = new User
                {
                    Id = id2,
                    Email = "test2@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "NO",
                    Thumbnail = "test"
                };

                var userEnumRole2 = new UserRole
                {
                    UserId = id2,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var product2 = new Product
                {
                    Id = id2,
                    Title = "5 chickens",
                    UserId = id2,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "NO",
                };

                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Producers.Add(producer);
                context.Products.Add(product);

                context.Users.Add(user2);
                context.UserRoles.Add(userEnumRole2);
                context.Producers.Add(producer2);
                context.Products.Add(product2);

                var entity1 = new Application
                {
                    UserId = id,
                    ProductId = id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 04, 08),
                    Status = ApplicationStatusEnum.Open
                };

                var entity2 = new Application
                {
                    UserId = id2,
                    ProductId = id2,
                    Motivation = "Test",
                    Created = new DateTime(2019, 03, 08),
                    Status = ApplicationStatusEnum.Open
                };

                context.Applications.AddRange(entity1, entity2);
                await context.SaveChangesAsync();

                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var applications = repository.ReadFiltered("DK");

                var count = applications.ToList().Count;
                Assert.Equal(1, count);

                var application = applications.First();

                Assert.Equal(entity1.Id, application.ApplicationId);
                Assert.Equal(entity1.UserId, application.ReceiverId);
                Assert.Equal($"{user.FirstName} {user.SurName}", application.ReceiverName);
                Assert.Equal(user.Country, application.Country);
                Assert.Equal(ImageHelper.GetRelativeStaticFolderImagePath(user.Thumbnail), application.Thumbnail);
                Assert.Equal(id, application.ProductId);
                Assert.Equal(product.Title, application.ProductTitle);
                Assert.Equal(product.Price, application.ProductPrice);
                Assert.Equal(product.UserId, application.ProducerId);
                Assert.Equal(entity1.Motivation, application.Motivation);
                Assert.Equal(entity1.Status, application.Status);
            }
        }

        [Fact]
        public async Task ReadFiltered_returns_based_on_country_and_city()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var id = 1;
                var id2 = 2;

                var user = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK",
                    Thumbnail = "test"
                };

                var userEnumRole = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var producer = new Producer
                {
                    Id = id,
                    PairingSecret = "1234",
                    UserId = id,
                    Street = "Test",
                    StreetNumber = "42",
                    City = "TestBy"
                };

                var product = new Product
                {
                    Id = id,
                    Title = "5 chickens",
                    UserId = id,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "DK",
                };

                var producer2 = new Producer
                {
                    Id = id2,
                    PairingSecret = "1234",
                    UserId = id2,
                    Street = "Test",
                    StreetNumber = "42",
                    City = "AnotherCity"
                };

                var user2 = new User
                {
                    Id = id2,
                    Email = "test2@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK",
                    Thumbnail = "test"
                };

                var userEnumRole2 = new UserRole
                {
                    UserId = id2,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var product2 = new Product
                {
                    Id = id2,
                    Title = "5 chickens",
                    UserId = id2,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "DK",
                };

                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Producers.Add(producer);
                context.Products.Add(product);

                context.Users.Add(user2);
                context.UserRoles.Add(userEnumRole2);
                context.Producers.Add(producer2);
                context.Products.Add(product2);

                var entity1 = new Application
                {
                    UserId = id,
                    ProductId = id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 04, 08),
                    Status = ApplicationStatusEnum.Open
                };

                var entity2 = new Application
                {
                    UserId = id,
                    ProductId = id2,
                    Motivation = "Test",
                    Created = new DateTime(2019, 03, 08),
                    Status = ApplicationStatusEnum.Open
                };

                context.Applications.AddRange(entity1, entity2);
                await context.SaveChangesAsync();

                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var applications = repository.ReadFiltered("DK", "TestBy");

                var count = applications.ToList().Count;
                Assert.Equal(1, count);

                var application = applications.First();

                Assert.Equal(entity1.Id, application.ApplicationId);
                Assert.Equal(entity1.UserId, application.ReceiverId);
                Assert.Equal($"{user.FirstName} {user.SurName}", application.ReceiverName);
                Assert.Equal(user.Country, application.Country);
                Assert.Equal(ImageHelper.GetRelativeStaticFolderImagePath(user.Thumbnail), application.Thumbnail);
                Assert.Equal(id, application.ProductId);
                Assert.Equal(product.Title, application.ProductTitle);
                Assert.Equal(product.Price, application.ProductPrice);
                Assert.Equal(product.UserId, application.ProducerId);
                Assert.Equal(entity1.Motivation, application.Motivation);
                Assert.Equal(entity1.Status, application.Status);
            }
        }

        [Fact]
        public async Task ReadCompleted_returns_all_open_Applications()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var id = 1;

                var user = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK",
                    Thumbnail = "test"
                };

                var userEnumRole = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var product = new Product
                {
                    Id = id,
                    Title = "5 chickens",
                    UserId = id,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "Test",
                };

                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Products.Add(product);

                var entity1 = new Application
                {
                    UserId = id,
                    ProductId = id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 04, 08),
                    Status = ApplicationStatusEnum.Completed,
                    DateOfDonation = DateTime.UtcNow
                };

                var entity2 = new Application
                {
                    UserId = id,
                    ProductId = id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 03, 08),
                    Status = ApplicationStatusEnum.Pending,
                    DateOfDonation = DateTime.UtcNow
        };

                context.Applications.AddRange(entity1, entity2);
                await context.SaveChangesAsync();

                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var applications = repository.ReadCompleted();

                // There should only be one application in the returned list
                // since one of the created applications is not open
                var count = applications.ToList().Count;
                Assert.Equal(1, count);

                var application = applications.First();



                Assert.Equal(entity1.Id, application.ApplicationId);
                Assert.Equal(entity1.UserId, application.ReceiverId);
                Assert.Equal($"{user.FirstName} {user.SurName}", application.ReceiverName);
                Assert.Equal(user.Country, application.Country);
                Assert.Equal(ImageHelper.GetRelativeStaticFolderImagePath(user.Thumbnail), application.Thumbnail);
                Assert.Equal(id, application.ProductId);
                Assert.Equal(product.Title, application.ProductTitle);
                Assert.Equal(product.Price, application.ProductPrice);
                Assert.Equal(product.UserId, application.ProducerId);
                Assert.Equal(entity1.Motivation, application.Motivation);
                Assert.Equal(entity1.Status, application.Status);
                Assert.Equal(entity1.DateOfDonation.ToString("yyyy-MM-dd"), application.DateOfDonation);
            }
        }

        [Fact]
        public async Task ReadCompleted_returns_all_open_Applications_order_by_timestamp_descending()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var id = 1;

                var user = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK",
                    Thumbnail = "test"
                };

                var userEnumRole = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var product = new Product
                {
                    Id = id,
                    Title = "5 chickens",
                    UserId = id,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "Test",
                };

                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Products.Add(product);

                var entity1 = new Application
                {
                    UserId = id,
                    ProductId = id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 04, 08),
                    Status = ApplicationStatusEnum.Completed,
                    DateOfDonation = new DateTime(2019, 04, 08),
                };

                var entity2 = new Application
                {
                    UserId = id,
                    ProductId = id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 03, 08),
                    Status = ApplicationStatusEnum.Completed,
                    DateOfDonation = new DateTime(2019, 03, 08),
                };

                context.Applications.AddRange(entity1, entity2);
                await context.SaveChangesAsync();

                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var applications = await repository.ReadCompleted().ToListAsync();

                var application = applications.ElementAt(0);
                var secondApplication = applications.ElementAt(1);

                Assert.Equal(entity1.Id, application.ApplicationId);
                Assert.Equal(entity2.Id, secondApplication.ApplicationId);
            }
        }

        [Fact]
        public async Task Read_given_existing_id_returns_all_products_by_specified_user_id()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var id = 1;

                var user = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK",
                    Thumbnail = "test"
                };

                var userEnumRole = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var otherId = 2; //

                var otherUser = new User
                {
                    Id = otherId,
                    Email = "other@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK"
                };

                var otherUserEnumRole = new UserRole
                {
                    UserId = otherId,
                    UserRoleEnum = UserRoleEnum.Producer
                };

                var product = new Product
                {
                    Id = 1,
                    Title = "5 chickens",
                    UserId = id,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "Test",
                };

                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Users.Add(otherUser);
                context.UserRoles.Add(otherUserEnumRole);
                context.Products.Add(product);

                var entity1 = new Application
                {
                    UserId = id,
                    ProductId = product.Id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 1, 1, 1, 1, 1),
                    Status = ApplicationStatusEnum.Open
                };

                var entity2 = new Application
                {
                    UserId = id,
                    ProductId = product.Id,
                    Motivation = "Test",
                    Status = ApplicationStatusEnum.Pending
                };

                var entity3 = new Application
                {
                    UserId = otherId,
                    ProductId = product.Id,
                    Motivation = "Test",
                    Status = ApplicationStatusEnum.Pending
                };

                context.Applications.AddRange(entity1, entity2, entity3);
                await context.SaveChangesAsync();

                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var applications = repository.Read(id);

                // There should only be two products in the returned list
                // since one of the created products is by another producer
                var count = applications.ToList().Count;
                Assert.Equal(2, count);

                var application = applications.First();
                var secondApplication = applications.Last();

                Assert.Equal(entity1.Id, application.ApplicationId);
                Assert.Equal(entity1.UserId, application.ReceiverId);
                Assert.Equal($"{user.FirstName} {user.SurName}", application.ReceiverName);
                Assert.Equal(user.Country, application.Country);
                Assert.Equal(ImageHelper.GetRelativeStaticFolderImagePath(user.Thumbnail), application.Thumbnail);
                Assert.Equal(product.Title, application.ProductTitle);
                Assert.Equal(product.Price, application.ProductPrice);
                Assert.Equal(product.Id, application.ProductId);
                Assert.Equal(product.UserId, application.ProducerId);
                Assert.Equal(entity1.Motivation, application.Motivation);
                Assert.Equal(entity1.Status, application.Status);

                Assert.Equal(entity2.Id, secondApplication.ApplicationId);
                Assert.Equal(entity2.UserId, secondApplication.ReceiverId);
                Assert.Equal(entity2.ProductId, secondApplication.ProductId);
                Assert.Equal(product.UserId, secondApplication.ProducerId);
            }
        }

        [Fact]
        public async Task Read_given_existing_id_returns_all_products_by_specified_user_id_order_by_timestamp_descending()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var id = 1;

                var user = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK",
                    Thumbnail = "test"
                };

                var userEnumRole = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var otherId = 2; //

                var otherUser = new User
                {
                    Id = otherId,
                    Email = "other@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK"
                };

                var otherUserEnumRole = new UserRole
                {
                    UserId = otherId,
                    UserRoleEnum = UserRoleEnum.Producer
                };

                var product = new Product
                {
                    Id = 1,
                    Title = "5 chickens",
                    UserId = id,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "Test",
                };

                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Users.Add(otherUser);
                context.UserRoles.Add(otherUserEnumRole);
                context.Products.Add(product);

                var entity1 = new Application
                {
                    UserId = id,
                    ProductId = product.Id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 1, 1, 1, 10, 1),
                    Status = ApplicationStatusEnum.Open
                };

                var entity2 = new Application
                {
                    UserId = id,
                    ProductId = product.Id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 1, 1, 1, 1, 1),
                    Status = ApplicationStatusEnum.Pending
                };

                var entity3 = new Application
                {
                    UserId = otherId,
                    ProductId = product.Id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 1, 1, 1, 1, 1),
                    Status = ApplicationStatusEnum.Pending
                };

                context.Applications.AddRange(entity1, entity2, entity3);
                await context.SaveChangesAsync();

                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var applications = repository.Read(id);

                var application = applications.First();
                var secondApplication = applications.Last();

                Assert.Equal(entity1.Id, application.ApplicationId);
                Assert.Equal(entity2.Id, secondApplication.ApplicationId);
            }
        }

        [Fact]
        public async Task Read_given_nonExisting_id_returns_emptyCollection()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var result = repository.Read(42);
                Assert.Empty(result);
            }
        }

        [Fact]
        public async Task ReadWithdrawableByProducer_return_correct_application()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var id = 1;

                var user = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK",
                    Thumbnail = "test"
                };

                var userEnumRole = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var otherId = 2; //

                var otherUser = new User
                {
                    Id = otherId,
                    Email = "other@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK"
                };

                var otherUserEnumRole = new UserRole
                {
                    UserId = otherId,
                    UserRoleEnum = UserRoleEnum.Producer
                };

                var product = new Product
                {
                    Id = 1,
                    Title = "5 chickens",
                    UserId = otherId,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "Test",
                };

                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Users.Add(otherUser);
                context.UserRoles.Add(otherUserEnumRole);
                context.Products.Add(product);

                var entity1 = new Application
                {
                    Id = id,
                    UserId = id,
                    ProductId = product.Id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 1, 1, 1, 1, 1),
                    Status = ApplicationStatusEnum.Completed
                };

                var entity2 = new Application
                {
                    Id = otherId,
                    UserId = id,
                    ProductId = product.Id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 1, 1, 1, 1, 1),
                    Status = ApplicationStatusEnum.Completed
                };

                var entity3 = new Application
                {
                    Id = 3,
                    UserId = id,
                    ProductId = product.Id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 1, 1, 1, 1, 1),
                    Status = ApplicationStatusEnum.Pending
                };

                var contract1 = new Contracts
                {
                    ApplicationId = entity1.Id,
                    CreationTime = new DateTime(2019, 1, 1, 1, 1, 1),
                    Completed = 1,
                    ConfirmKey = "key",
                    SharedAddress = "address",
                    DonorWallet = "dwallet",
                    DonorDevice = "ddevice",
                    ProducerWallet = "pwallet",
                    ProducerDevice = "pdevice",
                    Price = product.Price,
                    Bytes = 25
                };

                var contract2 = new Contracts
                {
                    ApplicationId = entity2.Id,
                    CreationTime = new DateTime(2019, 1, 1, 1, 1, 1),
                    Completed = 1,
                    ConfirmKey = "key",
                    SharedAddress = "address",
                    DonorWallet = "dwallet",
                    DonorDevice = "ddevice",
                    ProducerWallet = "pwallet",
                    ProducerDevice = "pdevice",
                    Price = product.Price,
                    Bytes = 0
                };

                context.Applications.AddRange(entity1, entity2, entity3);
                context.Contracts.AddRange(contract1, contract2);
                await context.SaveChangesAsync();

                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var applications = repository.ReadWithdrawableByProducer(otherId);

                // There should only be two products in the returned list
                // since one of the created products is by another producer
                var count = applications.ToList().Count;
                Assert.Equal(1, count);

                var application = applications.First();

                Assert.Equal(entity1.Id, application.ApplicationId);
                Assert.Equal(entity1.UserId, application.ReceiverId);
                Assert.Equal($"{user.FirstName} {user.SurName}", application.ReceiverName);
                Assert.Equal(user.Country, application.Country);
                Assert.Equal(ImageHelper.GetRelativeStaticFolderImagePath(user.Thumbnail), application.Thumbnail);
                Assert.Equal(product.Title, application.ProductTitle);
                Assert.Equal(product.Price, application.ProductPrice);
                Assert.Equal(product.Id, application.ProductId);
                Assert.Equal(product.UserId, application.ProducerId);
                Assert.Equal(entity1.Motivation, application.Motivation);
                Assert.Equal(entity1.Status, application.Status);
            }
        }

        [Fact]
        public async Task ReadWithdrawable_given_nonExisting_id_returns_emptyCollection()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var result = repository.Read(42);
                Assert.Empty(result);
            }
        }

        [Fact]
        public async Task UpdateAsync_given_existing_id_returns_True()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var id = 1;

                var user = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK",
                    Thumbnail = "test"
                };

                var userEnumRole = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var product = new Product
                {
                    Id = id,
                    Title = "5 chickens",
                    UserId = 1,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "Test",
                };

                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Products.Add(product);

                var entity = new Application
                {
                    UserId = id,
                    ProductId = id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 04, 08),
                    Status = ApplicationStatusEnum.Pending
                };


                context.Applications.Add(entity);
                await context.SaveChangesAsync();

                var expected = new ApplicationUpdateDTO
                {
                    ApplicationId = entity.Id,
                    ReceiverId = id,
                    Status = ApplicationStatusEnum.Locked,
                };

                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var (result, email) = await repository.UpdateAsync(expected);

                Assert.True(result);
            }
        }

        [Fact]
        public async Task UpdateAsync_given_existing_id_with_status_open_sets_donationDate_null()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var id = 1;

                var user = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK",
                    Thumbnail = "test"
                };

                var userEnumRole = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var product = new Product
                {
                    Id = id,
                    Title = "5 chickens",
                    UserId = 1,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "Test",
                };

                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Products.Add(product);

                var entity = new Application
                {
                    UserId = id,
                    ProductId = id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 04, 08),
                    Status = ApplicationStatusEnum.Pending,
                    DateOfDonation = DateTime.UtcNow
                };

                context.Applications.Add(entity);
                await context.SaveChangesAsync();

                var expected = new ApplicationUpdateDTO
                {
                    ApplicationId = entity.Id,
                    ReceiverId = id,
                    Status = ApplicationStatusEnum.Open,
                };

                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var (result, email) = await repository.UpdateAsync(expected);

                var updated = await context.Applications.FindAsync(entity.Id);

                Assert.Equal(DateTime.MinValue, updated.DateOfDonation);
            }
        }

        [Fact]
        public async Task UpdateAsync_given_existing_id_updates_product()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var id = 1;
                var id2 = 2;

                var user = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK",
                    Thumbnail = "test"
                };

                var userEnumRole = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var user2 = new User
                {
                    Id = id2,
                    FirstName = "Test",
                    SurName = "Test",
                    Email = "Test@Test",
                    Country = "CountryCode",
                    Password = "1234",
                    Created = new DateTime(1, 1, 1, 1, 1, 1)
                };

                var userEnumRole2 = new UserRole
                {
                    UserId = user2.Id,
                    UserRoleEnum = UserRoleEnum.Producer
                };

                var producer = new Producer
                {
                    Id = user2.Id,
                    UserId = user2.Id,
                    WalletAddress = "test",
                    PairingSecret = "abcd",
                    Street = "Test",
                    StreetNumber = "Some number",
                    City = "City"
                };

                var product = new Product
                {
                    Id = id,
                    Title = "5 chickens",
                    UserId = user2.Id,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "Test",
                };

                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Users.Add(user2);
                context.UserRoles.Add(userEnumRole2);
                context.Producers.Add(producer);
                context.Products.Add(product);

                var entity = new Application
                {
                    UserId = id,
                    ProductId = product.Id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 04, 08),
                    Status = ApplicationStatusEnum.Open
                };


                context.Applications.Add(entity);
                await context.SaveChangesAsync();

                var expected = new ApplicationUpdateDTO
                {
                    ApplicationId = entity.Id,
                    ReceiverId = id,
                    Status = ApplicationStatusEnum.Pending,
                };

                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                await repository.UpdateAsync(expected);

                var updated = await context.Applications.FindAsync(entity.Id);

                Assert.Equal(expected.ApplicationId, updated.Id);
                Assert.Equal(expected.ReceiverId, updated.UserId);
                Assert.Equal(expected.Status, updated.Status);
                Assert.Equal(DateTime.UtcNow.ToString("yyyy-MM-dd"), updated.DateOfDonation.ToString("yyyy-MM-dd"));

                var now = DateTime.UtcNow;
                // These checks are to assume the timestamp is set on update.
                // The now timestamp is some ticks off from the database timestamp.
                Assert.Equal(updated.LastModified.Date, now.Date);
                Assert.Equal(updated.LastModified.Hour, now.Hour);
                Assert.Equal(updated.LastModified.Minute, now.Minute);
            }
        }

        [Fact]
        public async Task UpdateAsync_given_existing_id_with_pending_status_sends_donation_email()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var id = 1;
                var id2 = 2;

                var user = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK",
                    Thumbnail = "test"
                };

                var userEnumRole = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var user2 = new User
                {
                    Id = id2,
                    FirstName = "Test",
                    SurName = "Test",
                    Email = "Test@Test",
                    Country = "CountryCode",
                    Password = "1234",
                    Created = new DateTime(1, 1, 1, 1, 1, 1)
                };

                var userEnumRole2 = new UserRole
                {
                    UserId = user2.Id,
                    UserRoleEnum = UserRoleEnum.Producer
                };

                var producer = new Producer
                {
                    Id = user2.Id,
                    UserId = user2.Id,
                    WalletAddress = "test",
                    PairingSecret = "abcd",
                    Street = "Test",
                    StreetNumber = "Some number",
                    City = "City"
                };

                var product = new Product
                {
                    Id = id,
                    Title = "5 chickens",
                    UserId = user2.Id,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "Test",
                };

                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Users.Add(user2);
                context.UserRoles.Add(userEnumRole2);
                context.Producers.Add(producer);
                context.Products.Add(product);

                var entity = new Application
                {
                    UserId = id,
                    ProductId = product.Id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 04, 08),
                    Status = ApplicationStatusEnum.Open
                };


                context.Applications.Add(entity);
                await context.SaveChangesAsync();

                var expected = new ApplicationUpdateDTO
                {
                    ApplicationId = entity.Id,
                    ReceiverId = id,
                    Status = ApplicationStatusEnum.Pending,
                };

                var producerAddress = producer.Zipcode != null
                                        ? producer.Street + " " + producer.StreetNumber + ", " + producer.Zipcode + " " + producer.City
                                        : producer.Street + " " + producer.StreetNumber + ", " + producer.City;
                var subject = "You received a donation on PolloPollo!";
                string body = $"Congratulations!\n\nA donation has just been made to fill your application for {product.Title}. You can now go and receive the product at the shop with address: {producerAddress}. You must confirm reception of the product when you get there.\n\nFollow these steps to confirm reception:\n-Log on to pollopollo.org\n-Click on your user and select \"profile\"\n-Change \"Open applications\" to \"Pending applications\"\n-Click on \"Confirm Receival\"\n\nAfter 10-15 minutes, the confirmation goes through and the shop will be notified of your confirmation.\n\nIf you have questions or experience problems, please join https://discord.pollopollo.org or write an email to pollopollo@pollopollo.org\n\nSincerely,\nThe PolloPollo Project";

                var emailClient = new Mock<IEmailClient>();
                emailClient.Setup(e => e.SendEmail(user.Email, subject, body)).Returns((true, null));

                var repository = new ApplicationRepository(emailClient.Object, context);

                (bool status, (bool emailSent, string emailError)) = await repository.UpdateAsync(expected);

                emailClient.Verify(e => e.SendEmail(user.Email, subject, body));
                Assert.True(emailSent);
            }
        }

        [Fact]
        public async Task UpdateAsync_given_existing_id_with_pending_status_propagates_emailError()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var id = 1;
                var id2 = 2;

                var user = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK",
                    Thumbnail = "test"
                };

                var userEnumRole = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var user2 = new User
                {
                    Id = id2,
                    FirstName = "Test",
                    SurName = "Test",
                    Email = "Test@Test",
                    Country = "CountryCode",
                    Password = "1234",
                    Created = new DateTime(1, 1, 1, 1, 1, 1)
                };

                var userEnumRole2 = new UserRole
                {
                    UserId = user2.Id,
                    UserRoleEnum = UserRoleEnum.Producer
                };

                var producer = new Producer
                {
                    Id = user2.Id,
                    UserId = user2.Id,
                    WalletAddress = "test",
                    PairingSecret = "abcd",
                    Street = "Test",
                    StreetNumber = "Some number",
                    City = "City"
                };

                var product = new Product
                {
                    Id = id,
                    Title = "5 chickens",
                    UserId = user2.Id,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "Test",
                };

                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Users.Add(user2);
                context.UserRoles.Add(userEnumRole2);
                context.Producers.Add(producer);
                context.Products.Add(product);

                var entity = new Application
                {
                    UserId = id,
                    ProductId = product.Id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 04, 08),
                    Status = ApplicationStatusEnum.Open
                };


                context.Applications.Add(entity);
                await context.SaveChangesAsync();

                var expected = new ApplicationUpdateDTO
                {
                    ApplicationId = entity.Id,
                    ReceiverId = id,
                    Status = ApplicationStatusEnum.Pending,
                };

                var producerAddress = producer.Zipcode != null
                                        ? producer.Street + " " + producer.StreetNumber + ", " + producer.Zipcode + " " + producer.City
                                        : producer.Street + " " + producer.StreetNumber + ", " + producer.City;
                var subject = "You received a donation on PolloPollo!";
                string body = $"Congratulations!\n\nA donation has just been made to fill your application for {product.Title}. You can now go and receive the product at the shop with address: {producerAddress}. You must confirm reception of the product when you get there.\n\nFollow these steps to confirm reception:\n-Log on to pollopollo.org\n-Click on your user and select \"profile\"\n-Change \"Open applications\" to \"Pending applications\"\n-Click on \"Confirm Receival\"\n\nAfter 10-15 minutes, the confirmation goes through and the shop will be notified of your confirmation.\n\nIf you have questions or experience problems, please join https://discord.pollopollo.org or write an email to pollopollo@pollopollo.org\n\nSincerely,\nThe PolloPollo Project";

                var emailClient = new Mock<IEmailClient>();
                emailClient.Setup(e => e.SendEmail(user.Email, subject, body)).Returns((false, "Email error"));

                var repository = new ApplicationRepository(emailClient.Object, context);

                (bool status, (bool emailSent, string emailError)) = await repository.UpdateAsync(expected);

                emailClient.Verify(e => e.SendEmail(user.Email, subject, body));
                Assert.False(emailSent);
                Assert.Equal("Email error", emailError);
            }
        }

        [Fact]
        public async Task UpdateAsync_given_existing_id_with_completed_status_sends_emails()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var id = 1;
                var id2 = 2;

                var user = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK",
                    Thumbnail = "test"
                };

                var userEnumRole = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var user2 = new User
                {
                    Id = id2,
                    FirstName = "Test",
                    SurName = "Test",
                    Email = "Test@Test",
                    Country = "CountryCode",
                    Password = "1234",
                    Created = new DateTime(1, 1, 1, 1, 1, 1)
                };

                var userEnumRole2 = new UserRole
                {
                    UserId = user2.Id,
                    UserRoleEnum = UserRoleEnum.Producer
                };

                var producer = new Producer
                {
                    Id = user2.Id,
                    UserId = user2.Id,
                    WalletAddress = "test",
                    PairingSecret = "abcd",
                    Street = "Test",
                    StreetNumber = "Some number",
                    City = "City"
                };

                var product = new Product
                {
                    Id = id,
                    Title = "5 chickens",
                    UserId = user2.Id,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "Test",
                };

                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Users.Add(user2);
                context.UserRoles.Add(userEnumRole2);
                context.Producers.Add(producer);
                context.Products.Add(product);

                var entity = new Application
                {
                    UserId = id,
                    ProductId = product.Id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 04, 08),
                    Status = ApplicationStatusEnum.Open
                };


                context.Applications.Add(entity);
                await context.SaveChangesAsync();

                string subject = "Thank you for using PolloPollo";
                string body = $"Thank you very much for using PolloPollo.\n\n" +
                        "If you have suggestions for improvements or feedback, please join our Discord server: https://discord.pollopollo.org and let us know.\n\n" +
                        "The PolloPollo project is created and maintained by volunteers. We rely solely on the help of volunteers to grow the platform.\n\n" +
                        "You can help us help more people by asking shops to join and add products that people in need can apply for." +
                        "\n\nWe hope you enjoyed using PolloPollo" +
                        "\n\nSincerely," +
                        "\nThe PolloPollo Project";

                string subject1 = $"{user.FirstName} {user.SurName} confirmed receipt of application #{entity.Id}";
                string body1 = $"{user.FirstName} {user.SurName} has just confirmed receipt of the product {product.Title} (${product.Price}).\n\n" +
                        $"The application ID is #{entity.Id} and contains {0} bytes which is roughly ${0} at current rates.\n\n" +
                        $"To withdraw the money, open your Obyte Wallet and find the Smart Wallet address starting with {""}.\n\n" +
                        "Thank you for using PolloPollo and if you have suggestions for improvements, please join our Discord server: https://discord.pollopollo.org and let us know.\n\n" +
                        "The PolloPollo project is created and maintained by volunteers. We rely solely on the help of volunteers to grow the platform.\n\n" +
                        "You can help us help more people by adding more products or encouraging other shops to join and add their products that people in need can apply for." +
                        "\n\nWe hope you enjoyed using PolloPollo." +
                        "\n\nSincerely," +
                        "\nThe PolloPollo Project";

                var expected = new ApplicationUpdateDTO
                {
                    ApplicationId = entity.Id,
                    ReceiverId = id,
                    Status = ApplicationStatusEnum.Completed
                };

                var emailClient = new Mock<IEmailClient>();
                emailClient.Setup(e => e.SendEmail(user.Email, subject, body)).Returns((true, null));
                //emailClient.Setup(e => e.SendEmail(user2.Email, subject1, body1)).Returns((true, null));

                var repository = new ApplicationRepository(emailClient.Object, context);

                var (status, (emailSent, emailError)) = await repository.UpdateAsync(expected);

                emailClient.Verify(e => e.SendEmail(user.Email, subject, body));
                //emailClient.Verify(e => e.SendEmail(user2.Email, subject1, body1));

                Assert.True(emailSent);
            }
        }

        [Fact]
        public async Task GetContractInformationAsync_given_nonExistng_Id_Returns_Null() 
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var result = await repository.GetContractInformationAsync(1);

                Assert.Null(result);
            }
        }


        [Fact]
        public async Task GetContractInformationAsync_given_existng_Id_Returns_DTO()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var id = 1;

                var receiver = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK",
                    Thumbnail = "test",
                    Created = new DateTime(1, 1, 1, 1, 1, 1)
                };

                var userEnumRoleReceiver = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var producerUser = new User
                {
                    Id = 2,
                    Email = "test2@itu.dk",
                    Password = "12345",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK",
                    Thumbnail = "test",
                    Created = new DateTime(1, 1, 1, 1, 1, 1)
                };

                var userEnumRoleProducer = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Producer
                };

                var producer = new Producer
                {
                    UserId = producerUser.Id,
                    PairingSecret = "secret",
                    DeviceAddress = "ABCD",
                    WalletAddress = "EFGH",
                    Street = "Test",
                    StreetNumber = "Some number",
                    City = "City"
                };

                var product = new Product
                {
                    Id = id,
                    Title = "5 chickens",
                    UserId = producerUser.Id,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "Test",
                };

                var entity = new Application
                {
                    UserId = id,
                    ProductId = id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 04, 08),
                    Status = ApplicationStatusEnum.Open
                };

                context.Users.Add(receiver);
                context.UserRoles.Add(userEnumRoleReceiver);
                context.Users.Add(producerUser);
                context.UserRoles.Add(userEnumRoleProducer);
                context.Producers.Add(producer);
                context.Products.Add(product);
                context.Applications.Add(entity);
                await context.SaveChangesAsync();

                var result = await repository.GetContractInformationAsync(id);

                Assert.NotNull(result);
                Assert.Equal(product.Price, result.Price);
                Assert.Equal(producer.DeviceAddress, result.ProducerDevice);
                Assert.Equal(producer.WalletAddress, result.ProducerWallet);
            }
        }

        [Fact]
        public async Task UpdateAsync_given_non_existing_id_returns_false()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var expected = new ApplicationUpdateDTO
                {
                    ApplicationId = 1,
                    ReceiverId = 1,
                    Status = ApplicationStatusEnum.Locked,
                };

                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var (result, email) = await repository.UpdateAsync(expected);

                Assert.False(result);
            }
        }


        [Fact]
        private async Task DeleteAsync_given_invalid_id_returns_false()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var result = await repository.DeleteAsync(42, 42);

                Assert.False(result);
            }
        }

        [Fact]
        private async Task DeleteAsync_given_existing_id_with_not_owner_returns_false()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var id = 1;
                var input = 2;

                var user = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK"
                };


                var user1 = new User
                {
                    Id = input,
                    Email = "test@test.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK"
                };
                var userEnumRole = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Producer
                };

                var receiver = new Receiver
                {
                    UserId = id
                };

                var product = new Product
                {
                    Id = id,
                    Title = "test",
                    UserId = id,
                    Thumbnail = "",
                };

                var application = new Application
                {
                    Id = id,
                    UserId = id,
                    ProductId = id,
                    Motivation = "test",
                    Status = ApplicationStatusEnum.Open
                };

                context.Users.AddRange(user, user1);
                context.UserRoles.Add(userEnumRole);
                context.Receivers.Add(receiver);
                context.Products.Add(product);
                context.Applications.Add(application);
                await context.SaveChangesAsync();

                var deletion = await repository.DeleteAsync(input, id);

                Assert.False(deletion);
            }
        }

        [Fact]
        private async Task DeleteAsync_given_existing_id_deletes()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var id = 1;

                var user = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK"
                };

                var userEnumRole = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Producer
                };

                var receiver = new Receiver
                {
                    UserId = id
                };

                var product = new Product
                {
                    Id = id,
                    Title = "test",
                    UserId = id,
                    Thumbnail = "",
                };

                var application = new Application
                {
                    Id = id,
                    UserId = id,
                    ProductId = id,
                    Motivation = "test",
                    Status = ApplicationStatusEnum.Open
                };

                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Receivers.Add(receiver);
                context.Products.Add(product);
                context.Applications.Add(application);
                await context.SaveChangesAsync();

                var deletion = await repository.DeleteAsync(id, id);

                var find = await repository.FindAsync(id);

                Assert.True(deletion);
                Assert.Null(find);
            }
        }

        [Fact]
        private async Task DeleteAsync_deleting_not_open_application_returns_false()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var id = 1;

                var user = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK"
                };

                var userEnumRole = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Producer
                };

                var receiver = new Receiver
                {
                    UserId = id
                };

                var product = new Product
                {
                    Id = id,
                    Title = "test",
                    UserId = id,
                    Thumbnail = "",
                };

                var application = new Application
                {
                    Id = id,
                    UserId = id,
                    ProductId = id,
                    Motivation = "test",
                    Status = ApplicationStatusEnum.Pending
                };

                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Receivers.Add(receiver);
                context.Products.Add(product);
                context.Applications.Add(application);
                await context.SaveChangesAsync();

                var deletion = await repository.DeleteAsync(id, id);

                var find = await repository.FindAsync(id);

                Assert.False(deletion);
                Assert.NotNull(find);
            }
        }

        [Fact]
        public async Task GetCountries_returns_empty_list()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var countries = await repository.GetCountries().ToListAsync();

                Assert.Empty(countries);
            }
        }

        [Fact]
        public async Task GetCountries_returns_list()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var id = 1;
                var id2 = 2;

                var user = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK",
                    Thumbnail = "test"
                };

                var userEnumRole = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var product = new Product
                {
                    Id = id,
                    Title = "5 chickens",
                    UserId = id,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "DK",
                };

                var user2 = new User
                {
                    Id = id2,
                    Email = "test2@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "NO",
                    Thumbnail = "test"
                };

                var userEnumRole2 = new UserRole
                {
                    UserId = id2,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var product2 = new Product
                {
                    Id = id2,
                    Title = "5 chickens",
                    UserId = id2,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "NO",
                };

                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Products.Add(product);

                context.Users.Add(user2);
                context.UserRoles.Add(userEnumRole2);
                context.Products.Add(product2);

                var entity1 = new Application
                {
                    UserId = id,
                    ProductId = id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 1, 1, 1, 1, 1),
                    Status = ApplicationStatusEnum.Open
                };

                var entity2 = new Application
                {
                    UserId = id2,
                    ProductId = id2,
                    Motivation = "Test",
                    Created = new DateTime(2019, 1, 1, 1, 10, 1),
                    Status = ApplicationStatusEnum.Open
                };

                context.Applications.AddRange(entity1, entity2);
                await context.SaveChangesAsync();

                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var countries = await repository.GetCountries().ToListAsync();

                Assert.Equal(2, countries.Count());

                var country = countries.ElementAt(0);
                var secondCountry = countries.ElementAt(1);

                Assert.Equal(user.Country, country);
                Assert.Equal(user2.Country, secondCountry);
            }
        }

        [Fact]
        public async Task GetCities_returns_empty_list()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var cities = await repository.GetCities("DK").ToListAsync();

                Assert.Empty(cities);
            }
        }

        [Fact]
        public async Task GetCities_returns_list()
        {
            using (var connection = await CreateConnectionAsync())
            using (var context = await CreateContextAsync(connection))
            {
                var id = 1;
                var id2 = 2;

                var user = new User
                {
                    Id = id,
                    Email = "test@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK",
                    Thumbnail = "test"
                };

                var userEnumRole = new UserRole
                {
                    UserId = id,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var producer = new Producer
                {
                    Id = id,
                    PairingSecret = "1234",
                    UserId = id,
                    Street = "Test",
                    StreetNumber = "42",
                    City = "TestBy"
                };

                var product = new Product
                {
                    Id = id,
                    Title = "5 chickens",
                    UserId = id,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "DK",
                };

                var user2 = new User
                {
                    Id = id2,
                    Email = "test2@itu.dk",
                    Password = "1234",
                    FirstName = "test",
                    SurName = "test",
                    Country = "DK",
                    Thumbnail = "test"
                };

                var userEnumRole2 = new UserRole
                {
                    UserId = id2,
                    UserRoleEnum = UserRoleEnum.Receiver
                };

                var producer2 = new Producer
                {
                    Id = id2,
                    PairingSecret = "1234",
                    UserId = id2,
                    Street = "Test",
                    StreetNumber = "42",
                    City = "TestBy2"
                };

                var product2 = new Product
                {
                    Id = id2,
                    Title = "5 chickens",
                    UserId = id2,
                    Price = 42,
                    Description = "Test",
                    Location = "Test",
                    Country = "DK",
                };

                context.Users.Add(user);
                context.UserRoles.Add(userEnumRole);
                context.Producers.Add(producer);
                context.Products.Add(product);

                context.Users.Add(user2);
                context.UserRoles.Add(userEnumRole2);
                context.Producers.Add(producer2);
                context.Products.Add(product2);

                var entity1 = new Application
                {
                    UserId = id,
                    ProductId = id,
                    Motivation = "Test",
                    Created = new DateTime(2019, 1, 1, 1, 1, 1),
                    Status = ApplicationStatusEnum.Open
                };

                var entity2 = new Application
                {
                    UserId = id2,
                    ProductId = id2,
                    Motivation = "Test",
                    Created = new DateTime(2019, 1, 1, 1, 10, 1),
                    Status = ApplicationStatusEnum.Open
                };

                context.Applications.AddRange(entity1, entity2);
                await context.SaveChangesAsync();

                var emailClient = new Mock<IEmailClient>();
                var repository = new ApplicationRepository(emailClient.Object, context);

                var cities = await repository.GetCities("DK").ToListAsync();

                Assert.Equal(2, cities.Count());

                var city = cities.ElementAt(0);
                var secondCity = cities.ElementAt(1);

                Assert.Equal(producer.City, city);
                Assert.Equal(producer2.City, secondCity);
            }
        }


        //Below are internal methods for use during testing
        private async Task<DbConnection> CreateConnectionAsync()
        {
            var connection = new SqliteConnection("datasource=:memory:");
            await connection.OpenAsync();

            return connection;
        }

        private async Task<PolloPolloContext> CreateContextAsync(DbConnection connection)
        {
            var builder = new DbContextOptionsBuilder<PolloPolloContext>().UseSqlite(connection);

            var context = new PolloPolloContext(builder.Options);
            await context.Database.EnsureCreatedAsync();

            return context;
        }
    }
}
