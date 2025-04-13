using Microsoft.EntityFrameworkCore;
using Moq;
using Domain;
using MockExams.Infra.CrossCutting.Identity.Interfaces;
using MockExams.Infra.Database;
using MockExams.Infra.Database.UoW;
using MockExams.Service;
using MockExams.Test.Unit.Mocks;
using System;
using System.Linq.Expressions;
using System.Threading;

namespace MockExams.Test.Unit.Services
{
    public class UserServiceTests
    {
        readonly Mock<IUserService> userServiceMock;
        readonly Mock<IApplicationSignInManager> signManagerMock;
        readonly Mock<ApplicationDbContext> contextMock;
        readonly Mock<DbSet<User>> userRepositoryMock;
        readonly Mock<IUnitOfWork> unitOfWorkMock;
        readonly Mock<IUserEmailService> userEmailServiceMock;

        public UserServiceTests()
        {
            // Definindo quais serão as classes mockadas
            userServiceMock = new Mock<IUserService>();
            contextMock = new Mock<ApplicationDbContext>();
            signManagerMock = new Mock<IApplicationSignInManager>();
            unitOfWorkMock = new Mock<IUnitOfWork>();
            userRepositoryMock = new Mock<DbSet<User>>();
            userEmailServiceMock = new Mock<IUserEmailService>();

            //Simula login do usuario
            Thread.CurrentPrincipal = new UserMock().GetClaimsUser();

            //userRepositoryMock.Setup(repo => repo.Insert(It.IsAny<User>())).Returns(() =>
            //{
            //    return UserMock.GetGrantee();
            //});

            //userRepositoryMock.Setup(repo => repo.Update(It.IsAny<User>())).Returns(() =>
            //{
            //    return UserMock.GetGrantee();
            //});

            // preciso mockar esse cara
            // _ctx.Set<TEntity>();
            contextMock.Setup(ctx => ctx.Set<User>()).Returns(() =>
            {
                return userRepositoryMock.Object;
            });

            userRepositoryMock.Setup(repo => repo.Find(It.IsAny<Expression<Func<User, bool>>>())).Returns(() =>
            {
                return UserMock.GetGrantee();
            });

            userRepositoryMock.Setup(repo => repo.Find(It.IsAny<Guid>())).Returns(() =>
            {
                return UserMock.GetGrantee();
            });

            //userRepositoryMock.Setup(repo => repo.Get()).Returns(() =>
            //{
            //    return new List<User>()
            //    {
            //        UserMock.GetGrantee(),
            //        UserMock.GetDonor()
            //    }.AsQueryable();
            //});

            userServiceMock.Setup(service => service.Insert(It.IsAny<User>())).Verifiable();
        }

        #region Register User
        //[Fact]
        //public void RegisterValidUser()
        //{
        //    var service = new UserService(contextMock.Object, unitOfWorkMock.Object, new UserValidator(), userEmailServiceMock.Object);

        //    Result<User> result = service.Insert(new User()
        //    {
        //        Email = "jose@MockExams.com",
        //        Password = "Password.123",
        //        Name = "José da Silva",
        //        Linkedin = @"linkedin.com\jose-silva",
        //        Phone = "55601719"

        //    });
        //    Assert.NotNull(result);
        //    Assert.True(result.Success);
        //}

        //[Fact]
        //public void RegisterInvalidUser()
        //{
           
        //    var service = new UserService(contextMock.Object, unitOfWorkMock.Object, new UserValidator(), userEmailServiceMock.Object);

        //    Result<User> result = service.Insert(new User()
        //    {
        //        Email = "",
        //        Password = ""
        //    });
        //    Assert.NotNull(result);
        //    Assert.False(result.Success);
        //}
        #endregion

        #region Update User

        //[Fact]
        //public void UpdateValidUser()
        //{
        //    var service = new UserService(contextMock.Object, unitOfWorkMock.Object, new UserValidator(), userEmailServiceMock.Object);

        //    Result<User> result = service.Update(new User()
        //    {
        //        Id = new Guid("C53B3552-606C-40C6-9D7F-FFC87572977E"),
        //        Email = "sergioprates.student@gmail.com",
        //        Linkedin = "https://www.linkedin.com/in/sergiopratesdossantos/",
        //        Name = "Sergio1",
        //        Phone = "584558999",
        //        Address = new Address ()
        //        {
        //            PostalCode = "04473-140",
        //            Street = "Av MockExams",
        //            Number = "5",
        //            City = "São Paulo",
        //            Country = "Brasil",
        //            State = "SP",
        //            Neighborhood = "Interlagos"
        //        }
        //    });

        //    Assert.NotNull(result);
        //    Assert.True(result.Success);
        //}

        //[Fact]
        //public void UpdateInvalidUser()
        //{
        //    var service = new UserService(contextMock.Object, unitOfWorkMock.Object, new UserValidator(), userEmailServiceMock.Object);

        //    Result<User> result = service.Update(new User()
        //    {
        //        Email = "",
        //        Linkedin = "",
        //        Name = "",
        //        Phone = "",
        //    });

        //    Assert.NotNull(result);
        //    Assert.False(result.Success);
        //}

        //[Fact]
        //public void UpdateUserNotExists()
        //{
        //    var service = new UserService(contextMock.Object, unitOfWorkMock.Object, new UserValidator(), userEmailServiceMock.Object);

        //    Result<User> result = service.Update(new User()
        //    {
        //        Email = "sss@sss.com",
        //        Linkedin = ""
        //    });

        //    Assert.NotNull(result);
        //    Assert.False(result.Success);
        //}

        #endregion

        #region Login User
        //[Fact]
        //public void LoginValidUser()
        //{
        //    var service = new UserService(contextMock.Object, unitOfWorkMock.Object, new UserValidator(), userEmailServiceMock.Object);
        //    Result<User> result = service.AuthenticationByEmailAndPassword(new User()
        //    {
        //        Email = "walter@MockExams.com",
        //        Password = "123456"
        //    });
        //    Assert.NotNull(result);
        //    Assert.Empty(result.Value.Password);
        //    Assert.Empty(result.Value.PasswordSalt);
        //    Assert.NotEmpty(result.Value.Name);
        //    Assert.True(result.Success);
        //}

        //[Fact]
        //public void LoginInvalidPassword()
        //{
        //    var service = new UserService(contextMock.Object, unitOfWorkMock.Object, new UserValidator(), userEmailServiceMock.Object);
        //    Result<User> result = service.AuthenticationByEmailAndPassword(new User()
        //    {
        //        Email = "walter@MockExams.com",
        //        Password = "wrongpassword"
        //    });
        //    Assert.Equal("Email ou senha incorretos", result.Messages[0]);
        //    Assert.False(result.Success);
        //}

        //[Fact]
        //public void LoginInvalidEmail()
        //{
        //    var service = new UserService(contextMock.Object, unitOfWorkMock.Object, new UserValidator(), userEmailServiceMock.Object);
        //    Result<User> result = service.AuthenticationByEmailAndPassword(new User()
        //    {
        //        Email = "joao@MockExams.com",
        //        Password = "wrongpassword"
        //    });
        //    Assert.Equal("Email ou senha incorretos", result.Messages[0]);
        //    Assert.False(result.Success);
        //}
        #endregion
    }
}
