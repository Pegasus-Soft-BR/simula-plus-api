using Domain;
using Domain.DTOs;
using MockExams.Infra.Email;
using MockExams.Test.Unit.Mocks;
using Xunit;

namespace MockExams.Test.Unit.Services
{
    public class EmailTemplateTests
    {
        readonly IEmailTemplate emailTemplate;

        private User user;
        private User administrator;
        private User requestingUser;
        private ContactUs contactUs;

        public EmailTemplateTests()
        {
            emailTemplate = new EmailTemplate();

            user = UserMock.GetDonor();

            requestingUser = UserMock.GetGrantee();

            administrator = UserMock.GetAdmin();
           
            contactUs = new ContactUs()
            {
                Name = "Rafael Rocha",
                Email = "rafael@MockExams.com.br",
                Message = "At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident",
                Phone = "(11) 954422-2765"
            };

        }

        //[Fact]
        //public void VerifyEmailNewBookInsertedParse()
        //{
        //    var result = emailTemplate.GenerateHtmlFromTemplateAsync("NewBookInsertedTemplate", book).Result;
        //    //<!DOCTYPE html>\r\n<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n    <meta charset=\"utf-8\" />\r\n    <title>Novo livro cadastrado - MockExams</title>\r\n</head>\r\n<body>\r\n    <p>\r\n        Olá Cussa Mitre,\r\n    </p>\r\n    <p>\r\n        Um novo livro foi cadastrado. Veja mais informações abaixo:\r\n    </p>\r\n\r\n    <ul>\r\n        <li><strong>Livro: </strong>Lord of the Rings</li>\r\n        <li><strong>Autor: </strong>J. R. R. Tolkien</li>\r\n        <li><strong>Usuário: </strong>Rodrigo</li>\r\n    </ul>\r\n\r\n    <p>MockExams</p>\r\n</body>\r\n</html>

        //    Assert.Contains("Olá Administrador(a),", result);
        //    Assert.Contains("<li><strong>Livro: </strong>Lord of the Rings</li>", result);
        //    Assert.Contains("<li><strong>Autor: </strong>J. R. R. Tolkien</li>", result);
        //    Assert.Contains("<li><strong>Usuário: </strong>Rodrigo</li>", result);
        //}



        //[Fact]
        //public void VerifyEmailBookApprovedParse()
        //{
        //    var vm = new
        //    {
        //        Book = book,
        //    };

        //    var result = emailTemplate.GenerateHtmlFromTemplateAsync("BookApprovedTemplate", vm).Result;

        //    Assert.Contains("<title>Livro aprovado - MockExams</title>", result);
        //    Assert.Contains("Olá Rodrigo", result);
        //    Assert.Contains("O livro Lord of the Rings foi aprovado e já está na nossa vitrine para doação.", result);
        //    Assert.Contains("<li><strong>Livro: </strong>Lord of the Rings</li>", result);
        //    Assert.Contains("<li><strong>Autor: </strong>J. R. R. Tolkien</li>", result);
        //}

        [Fact]
        public void VerifyEmailContactUsNotificationParse()
        {

            var contactUs = new ContactUs()
            {
                Name = "Rafael Rocha",
                Email = "rafael.rochaoliveira@yahoo.com.br"
            };
          

            var result = emailTemplate.GenerateHtmlFromTemplateAsync("ContactUsNotificationTemplate", contactUs).Result;
            Assert.Contains("Olá, Rafael Rocha", result);

        }

        [Fact]
        public void VerifyEmailContactUsTemplateParse()
        {
            var result = emailTemplate.GenerateHtmlFromTemplateAsync("ContactUsTemplate", contactUs).Result;

            Assert.Contains("Olá, Administrador(a)!", result);
            Assert.Contains("Nome: Rafael Rocha", result);
            Assert.Contains("Email: rafael@MockExams.com.br", result);
            Assert.Contains("Telefone: (11) 954422-2765", result);
            Assert.Contains("Mensagem: At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident", result);

        }
    }
}