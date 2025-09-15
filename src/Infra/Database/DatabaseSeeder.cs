using Domain;
using Domain.DTOs;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MockExams.Infra.Database;

public class DatabaseSeeder
{

    private readonly ApplicationDbContext _context;


    public DatabaseSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Seed()
    {
        _context.Database.EnsureCreated();

        if (_context.Exams.Any()) return;

        AddExams();
        AddJavaQuestions();
        AddCSharpQuestions();
        AddNodeJsQuestions();

        _context.SaveChanges();
    }

    private void AddExams()
    {
        var exams = new List<Exam>
        {
            new Exam
            {
                Title = "C# Fundamentals",
                Description = "Teste seus conhecimentos básicos em C#.",
                TimeSpentMaxSeconds = 1800,
                TotalQuestionsPerAttempt = 5,
                ImageSlug = "csharp-fundamentals"
            },
            new Exam
            {
                Title = "Java Basics",
                Description = "Teste seus conhecimentos básicos em Java.",
                TimeSpentMaxSeconds = 1800,
                TotalQuestionsPerAttempt = 5,
                ImageSlug = "java-basics"
            },
            new Exam
            {
                Title = "Node.js Essentials",
                Description = "Teste seus conhecimentos básicos em Node.js.",
                TimeSpentMaxSeconds = 1800,
                TotalQuestionsPerAttempt = 5,
                ImageSlug = "nodejs-essentials"
            }
        };

        _context.Exams.AddRange(exams);
        _context.SaveChanges();
    }

    private void AddJavaQuestions()
    {
        var javaExam = _context.Exams.FirstOrDefault(e => e.Title == "Java Basics");
        if (javaExam == null) return;

        var questions = new List<Question>
    {
        new Question
        {
            ExamId = javaExam.Id,
            Title = "O que é a JVM?",
            Option1 = "Java Virtual Machine",
            Option2 = "Java Version Manager",
            Option3 = "Java Variable Method",
            Option4 = "Java Verified Mode",
            Option5 = "Nenhuma das anteriores",
            CorrectOptions = "1",
            DifficultyLevel = 1
        },
        new Question
        {
            ExamId = javaExam.Id,
            Title = "Quais são os principais componentes do JDK?",
            Option1 = "JVM",
            Option2 = "JRE",
            Option3 = "Compiler",
            Option4 = "Debugger",
            Option5 = "Nenhuma das anteriores",
            CorrectOptions = "1,2,3,4",
            DifficultyLevel = 2
        },
        new Question
        {
            ExamId = javaExam.Id,
            Title = "Qual palavra-chave é usada para criar uma classe em Java?",
            Option1 = "new",
            Option2 = "class",
            Option3 = "define",
            Option4 = "struct",
            Option5 = "instance",
            CorrectOptions = "2",
            DifficultyLevel = 1
        },
        new Question
        {
            ExamId = javaExam.Id,
            Title = "Quais modificadores de acesso tornam um método acessível dentro da mesma classe?",
            Option1 = "public",
            Option2 = "private",
            Option3 = "protected",
            Option4 = "default",
            Option5 = "static",
            CorrectOptions = "2,4",
            DifficultyLevel = 2
        },
        new Question
        {
            ExamId = javaExam.Id,
            Title = "Qual interface é usada para manipular listas em Java?",
            Option1 = "Set",
            Option2 = "Map",
            Option3 = "List",
            Option4 = "Queue",
            Option5 = "Array",
            CorrectOptions = "3",
            DifficultyLevel = 2
        },
        new Question
        {
            ExamId = javaExam.Id,
            Title = "O que acontece se um método `main()` não for declarado como `public static void`?",
            Option1 = "O código compila normalmente",
            Option2 = "O código gera erro de compilação",
            Option3 = "O código roda, mas sem a execução do `main`",
            Option4 = "Nada, o Java ajusta automaticamente",
            Option5 = "Nenhuma das anteriores",
            CorrectOptions = "2",
            DifficultyLevel = 3
        },
        new Question
        {
            ExamId = javaExam.Id,
            Title = "Quais destas estruturas podem armazenar pares chave-valor?",
            Option1 = "List",
            Option2 = "Set",
            Option3 = "Queue",
            Option4 = "Map",
            Option5 = "HashMap",
            CorrectOptions = "4,5",
            DifficultyLevel = 1
        },
        new Question
        {
            ExamId = javaExam.Id,
            Title = "Quais são os tipos de dados numéricos inteiros em Java?",
            Option1 = "byte",
            Option2 = "short",
            Option3 = "int",
            Option4 = "long",
            Option5 = "double",
            CorrectOptions = "1,2,3,4",
            DifficultyLevel = 1
        },
        new Question
        {
            ExamId = javaExam.Id,
            Title = "Qual operador é usado para comparação de igualdade em Java?",
            Option1 = "=",
            Option2 = "==",
            Option3 = "===",
            Option4 = "!=",
            Option5 = "<>",
            CorrectOptions = "2",
            DifficultyLevel = 1
        },
        new Question
        {
            ExamId = javaExam.Id,
            Title = "O que significa a sigla JDK?",
            Option1 = "Java Development Kit",
            Option2 = "Java Deployment Kit",
            Option3 = "Java Dynamic Kit",
            Option4 = "Java Debug Kit",
            Option5 = "Java Data Kernel",
            CorrectOptions = "1",
            DifficultyLevel = 2
        },
        new Question
        {
            ExamId = javaExam.Id,
            Title = "Qual método é chamado automaticamente ao criar um objeto em Java?",
            Option1 = "main()",
            Option2 = "constructor()",
            Option3 = "init()",
            Option4 = "new()",
            Option5 = "Nenhuma das anteriores",
            CorrectOptions = "5",
            DifficultyLevel = 2
        },
        new Question
        {
            ExamId = javaExam.Id,
            Title = "Qual estrutura de controle pode ser usada para repetir um bloco de código?",
            Option1 = "if",
            Option2 = "while",
            Option3 = "for",
            Option4 = "switch",
            Option5 = "do-while",
            CorrectOptions = "2,3,5",
            DifficultyLevel = 1
        },
        new Question
        {
            ExamId = javaExam.Id,
            Title = "O que acontece se um método em Java lançar uma exceção sem tratá-la?",
            Option1 = "O programa ignora a exceção",
            Option2 = "O código continua normalmente",
            Option3 = "O programa encerra com erro",
            Option4 = "O compilador corrige a exceção",
            Option5 = "Nenhuma das anteriores",
            CorrectOptions = "3",
            DifficultyLevel = 3
        },
        new Question
        {
            ExamId = javaExam.Id,
            Title = "Quais palavras-chave são usadas para herança e implementação de interfaces?",
            Option1 = "extends",
            Option2 = "inherits",
            Option3 = "implements",
            Option4 = "super",
            Option5 = "override",
            CorrectOptions = "1,3",
            DifficultyLevel = 2
        },
        new Question
        {
            ExamId = javaExam.Id,
            Title = "Quais dessas coleções NÃO permitem elementos duplicados?",
            Option1 = "List",
            Option2 = "ArrayList",
            Option3 = "HashSet",
            Option4 = "LinkedList",
            Option5 = "TreeSet",
            CorrectOptions = "3,5",
            DifficultyLevel = 2
        }
    };

        _context.Questions.AddRange(questions);
    }

    private void AddCSharpQuestions()
    {
        var csharpExam = _context.Exams.FirstOrDefault(e => e.Title == "C# Fundamentals");
        if (csharpExam == null) return;

        var questions = new List<Question>
    {
        new Question
        {
            ExamId = csharpExam.Id,
            Title = "O que é o CLR no .NET?",
            Option1 = "Common Language Runtime",
            Option2 = "C# Language Reference",
            Option3 = "Code Link Runtime",
            Option4 = "Central Language Repository",
            Option5 = "Nenhuma das anteriores",
            CorrectOptions = "1",
            DifficultyLevel = 1
        },
        new Question
        {
            ExamId = csharpExam.Id,
            Title = "Qual dessas palavras-chave é usada para definir um método assíncrono em C#?",
            Option1 = "await",
            Option2 = "async",
            Option3 = "task",
            Option4 = "future",
            Option5 = "defer",
            CorrectOptions = "2",
            DifficultyLevel = 1
        },
        new Question
        {
            ExamId = csharpExam.Id,
            Title = "Quais são as interfaces mais usadas para manipular coleções genéricas em C#?",
            Option1 = "IList",
            Option2 = "IEnumerable",
            Option3 = "ICollection",
            Option4 = "IDictionary",
            Option5 = "IReadOnlyList",
            CorrectOptions = "2,3,4",
            DifficultyLevel = 2
        },
        new Question
        {
            ExamId = csharpExam.Id,
            Title = "O que o operador `??` faz em C#?",
            Option1 = "Verifica se um valor é nulo e retorna um valor alternativo",
            Option2 = "Concatena duas strings",
            Option3 = "Compara dois números inteiros",
            Option4 = "Cria um delegate anonimamente",
            Option5 = "Nenhuma das anteriores",
            CorrectOptions = "1",
            DifficultyLevel = 1
        },
        new Question
        {
            ExamId = csharpExam.Id,
            Title = "Qual dessas estruturas de controle NÃO existe em C#?",
            Option1 = "foreach",
            Option2 = "do-while",
            Option3 = "switch-case",
            Option4 = "loop-until",
            Option5 = "try-catch",
            CorrectOptions = "4",
            DifficultyLevel = 1
        },
        new Question
        {
            ExamId = csharpExam.Id,
            Title = "Quais tipos de referência existem no C#?",
            Option1 = "Classes",
            Option2 = "Structs",
            Option3 = "Interfaces",
            Option4 = "Delegates",
            Option5 = "Records",
            CorrectOptions = "1,3,4,5",
            DifficultyLevel = 2
        },
        new Question
        {
            ExamId = csharpExam.Id,
            Title = "O que a palavra-chave `var` faz em C#?",
            Option1 = "Declara uma variável sem definir seu tipo explicitamente",
            Option2 = "Define uma variável global",
            Option3 = "Cria uma variável do tipo dynamic",
            Option4 = "É um sinônimo para `object`",
            Option5 = "Nenhuma das anteriores",
            CorrectOptions = "1",
            DifficultyLevel = 1
        },
        new Question
        {
            ExamId = csharpExam.Id,
            Title = "Quais são os modificadores de acesso em C#?",
            Option1 = "public",
            Option2 = "private",
            Option3 = "protected",
            Option4 = "internal",
            Option5 = "readonly",
            CorrectOptions = "1,2,3,4",
            DifficultyLevel = 2
        },
        new Question
        {
            ExamId = csharpExam.Id,
            Title = "Qual palavra-chave é usada para implementar interfaces em C#?",
            Option1 = "extends",
            Option2 = "inherits",
            Option3 = "implements",
            Option4 = ":",
            Option5 = "interface",
            CorrectOptions = "4",
            DifficultyLevel = 1
        },
        new Question
        {
            ExamId = csharpExam.Id,
            Title = "Quais dessas coleções NÃO permitem elementos duplicados?",
            Option1 = "List",
            Option2 = "HashSet",
            Option3 = "Dictionary",
            Option4 = "Queue",
            Option5 = "SortedSet",
            CorrectOptions = "2,3,5",
            DifficultyLevel = 2
        },
        new Question
        {
            ExamId = csharpExam.Id,
            Title = "Qual operador é usado para comparação de igualdade em C#?",
            Option1 = "=",
            Option2 = "==",
            Option3 = "===",
            Option4 = "!=",
            Option5 = "<>",
            CorrectOptions = "2",
            DifficultyLevel = 1
        },
        new Question
        {
            ExamId = csharpExam.Id,
            Title = "O que acontece se um método em C# lançar uma exceção sem tratá-la?",
            Option1 = "O programa ignora a exceção",
            Option2 = "O código continua normalmente",
            Option3 = "O programa encerra com erro",
            Option4 = "O compilador corrige a exceção",
            Option5 = "Nenhuma das anteriores",
            CorrectOptions = "3",
            DifficultyLevel = 3
        },
        new Question
        {
            ExamId = csharpExam.Id,
            Title = "Quais dessas palavras-chave são usadas para tratamento de exceções em C#?",
            Option1 = "try",
            Option2 = "catch",
            Option3 = "finally",
            Option4 = "throw",
            Option5 = "handle",
            CorrectOptions = "1,2,3,4",
            DifficultyLevel = 2
        },
        new Question
        {
            ExamId = csharpExam.Id,
            Title = "Quais dessas palavras-chave são usadas para declarar variáveis somente leitura?",
            Option1 = "readonly",
            Option2 = "const",
            Option3 = "var",
            Option4 = "static",
            Option5 = "sealed",
            CorrectOptions = "1,2",
            DifficultyLevel = 2
        },
        new Question
        {
            ExamId = csharpExam.Id,
            Title = "Qual destas características define uma expressão lambda em C#?",
            Option1 = "Uso da sintaxe `=>`",
            Option2 = "Pode ser armazenada em um delegate",
            Option3 = "Deve sempre retornar um valor",
            Option4 = "É compilada como uma classe anônima",
            Option5 = "Nenhuma das anteriores",
            CorrectOptions = "1,2",
            DifficultyLevel = 3
        }
    };

        _context.Questions.AddRange(questions);
    }


    private void AddNodeJsQuestions()
    {
        var nodeExam = _context.Exams.FirstOrDefault(e => e.Title == "Node.js Essentials");
        if (nodeExam == null) return;

        var questions = new List<Question>
        {
        new Question
        {
            ExamId = nodeExam.Id,
            Title = "O que é o Node.js?",
            Option1 = "Um framework JavaScript",
            Option2 = "Um interpretador de JavaScript baseado no V8",
            Option3 = "Uma biblioteca para manipulação de DOM",
            Option4 = "Um banco de dados NoSQL",
            Option5 = "Nenhuma das anteriores",
            CorrectOptions = "2",
            DifficultyLevel = 1
        },
        new Question
        {
            ExamId = nodeExam.Id,
            Title = "Qual módulo embutido do Node.js é usado para trabalhar com arquivos?",
            Option1 = "http",
            Option2 = "fs",
            Option3 = "path",
            Option4 = "file",
            Option5 = "stream",
            CorrectOptions = "2",
            DifficultyLevel = 1
        },
        new Question
        {
            ExamId = nodeExam.Id,
            Title = "Qual comando é usado para iniciar um projeto Node.js com um arquivo package.json?",
            Option1 = "`npm install`",
            Option2 = "`npm start`",
            Option3 = "`npm init`",
            Option4 = "`node package.json`",
            Option5 = "`npm generate`",
            CorrectOptions = "3",
            DifficultyLevel = 2
        },
        new Question
        {
            ExamId = nodeExam.Id,
            Title = "Quais métodos podem ser usados para lidar com operações assíncronas em Node.js?",
            Option1 = "setTimeout",
            Option2 = "Callbacks",
            Option3 = "Promises",
            Option4 = "Async/Await",
            Option5 = "Todas as anteriores",
            CorrectOptions = "5",
            DifficultyLevel = 2
        },
        new Question
        {
            ExamId = nodeExam.Id,
            Title = "Qual dessas bibliotecas/módulos é usada para criar um servidor HTTP em Node.js?",
            Option1 = "Express",
            Option2 = "Socket.io",
            Option3 = "Mongoose",
            Option4 = "Lodash",
            Option5 = "Jest",
            CorrectOptions = "1",
            DifficultyLevel = 3
        },
        new Question
        {
            ExamId = nodeExam.Id,
            Title = "Quais destes módulos são embutidos no Node.js?",
            Option1 = "fs",
            Option2 = "path",
            Option3 = "http",
            Option4 = "os",
            Option5 = "express",
            CorrectOptions = "1,2,3,4",
            DifficultyLevel = 2
        },
        new Question
        {
            ExamId = nodeExam.Id,
            Title = "Qual comando é usado para instalar um pacote globalmente via npm?",
            Option1 = "`npm install <package>`",
            Option2 = "`npm install -g <package>`",
            Option3 = "`npm add <package>`",
            Option4 = "`npm require <package>`",
            Option5 = "`npm global <package>`",
            CorrectOptions = "2",
            DifficultyLevel = 1
        },
        new Question
        {
            ExamId = nodeExam.Id,
            Title = "Quais são as diferenças entre `require` e `import` no Node.js?",
            Option1 = "`require` é usado em CommonJS, enquanto `import` é usado em ES Modules",
            Option2 = "`require` é assíncrono, enquanto `import` é síncrono",
            Option3 = "Ambos podem ser usados de forma intercambiável sem configuração extra",
            Option4 = "Apenas `import` pode ser usado com arquivos JSON",
            Option5 = "Nenhuma das anteriores",
            CorrectOptions = "1",
            DifficultyLevel = 3
        },
        new Question
        {
            ExamId = nodeExam.Id,
            Title = "Qual das opções é verdadeira sobre o Event Loop no Node.js?",
            Option1 = "Permite execução assíncrona não bloqueante",
            Option2 = "É baseado no mecanismo V8",
            Option3 = "É responsável por gerenciar threads de execução",
            Option4 = "Só funciona com funções síncronas",
            Option5 = "Nenhuma das anteriores",
            CorrectOptions = "1,2,3",
            DifficultyLevel = 2
        },
        new Question
        {
            ExamId = nodeExam.Id,
            Title = "O que faz o comando `npm run`?",
            Option1 = "Executa um script definido no package.json",
            Option2 = "Inicia o servidor Node.js",
            Option3 = "Atualiza pacotes npm",
            Option4 = "Verifica vulnerabilidades de pacotes",
            Option5 = "Nenhuma das anteriores",
            CorrectOptions = "1",
            DifficultyLevel = 1
        },
        new Question
        {
            ExamId = nodeExam.Id,
            Title = "O que significa o arquivo package-lock.json no Node.js?",
            Option1 = "Lista as versões exatas dos pacotes instalados",
            Option2 = "Define os scripts que podem ser executados via npm",
            Option3 = "Configura o ambiente de execução do Node.js",
            Option4 = "Bloqueia pacotes que não podem ser atualizados",
            Option5 = "Nenhuma das anteriores",
            CorrectOptions = "1",
            DifficultyLevel = 2
        },
        new Question
        {
            ExamId = nodeExam.Id,
            Title = "Quais métodos do módulo fs podem ser usados para ler arquivos?",
            Option1 = "fs.readFile",
            Option2 = "fs.readFileSync",
            Option3 = "fs.open",
            Option4 = "fs.read",
            Option5 = "fs.loadFile",
            CorrectOptions = "1,2,4",
            DifficultyLevel = 2
        },
        new Question
        {
            ExamId = nodeExam.Id,
            Title = "Qual biblioteca pode ser usada para conectar o Node.js a um banco de dados MongoDB?",
            Option1 = "Sequelize",
            Option2 = "Mongoose",
            Option3 = "pg",
            Option4 = "Lodash",
            Option5 = "Jest",
            CorrectOptions = "2",
            DifficultyLevel = 2
        },
        new Question
        {
            ExamId = nodeExam.Id,
            Title = "O que o comando `npx` faz no Node.js?",
            Option1 = "Executa pacotes sem instalá-los globalmente",
            Option2 = "É um gerenciador de pacotes alternativo ao npm",
            Option3 = "Executa scripts npm definidos no package.json",
            Option4 = "Atualiza pacotes do projeto",
            Option5 = "Nenhuma das anteriores",
            CorrectOptions = "1",
            DifficultyLevel = 3
        },
        new Question
        {
            ExamId = nodeExam.Id,
            Title = "Quais são os principais benefícios do uso de Streams em Node.js?",
            Option1 = "Eficiência na manipulação de grandes volumes de dados",
            Option2 = "Menos uso de memória",
            Option3 = "Execução mais rápida de operações assíncronas",
            Option4 = "Permite o processamento de dados em partes menores",
            Option5 = "São síncronos e bloqueiam a execução",
            CorrectOptions = "1,2,4",
            DifficultyLevel = 3
        }
    };

        _context.Questions.AddRange(questions);
    }

}