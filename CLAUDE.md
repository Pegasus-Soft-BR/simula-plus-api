# CLAUDE.md - Memória de Longo Prazo

## 📋 Informações do Projeto 'Simula+ api'

A Pegasus é uma plataforma white label multi-tenant que centraliza autenticação, autorização, gestão de usuários e módulos administrativos, permitindo lançar aplicações personalizadas para diferentes clientes usando a mesma base tecnológica.

Simula+ é um aplicativo de simulados educacionais multi-tenant, parte do ecossistema Pegasus.
Ele permite que usuários pesquisem temas, realizem provas de múltipla escolha e acompanhem seu histórico de desempenho.

Quando o usuário busca por um tema inexistente, o Simula+ aciona a IA para gerar um novo simulado em tempo real. Esse simulado é salvo no banco e disponibilizado imediatamente para uso.

A integração com a Pegasus garante:

Autenticação e autorização via pegasus-account-api

Notificação automática de admins: ao gerar simulados por IA, o Simula+ utiliza um usuário técnico (Simula+ (Sistema)) para chamar o endpoint pegasus-api/email/notify-admins, disparando e-mails para os administradores do app.

Centralização de usuários e permissões: os admins do app são definidos e gerenciados no painel Pegasus Admin.

Essa abordagem mantém o Pegasus como fonte única de verdade para identidade, permissões e notificações cross-app.

### Sobre o Desenvolvedor Raffa
- Clean Code + Clean Architecture: modular, coeso, com separação clara de responsabilidades.
- Valoriza boa organização do projeto, com bons nomes de pastas e arquivos. Vale a pena investir tempo nisso.
- Valoriza nomes significativos e expressivos para componentes, hooks e funções. Vale a pena investir tempo nisso.
- Odeia retrabalho — antes de criar, sempre verifica se já não existe pronto e gratuito.
- Preza por segurança — validação e autorização bem feitas não são opcionais.
- Gosta de impressionar — seja o cliente, o time ou a diretoria, sempre com um toque extra.
- Não gosta de bajulação. Prefere uma personalidade confiante e levemente sarcástica e irônica.
- Caso a tarefa não seja trivial, explique o seu plano antes de colocar a mão na massa.

### Visual Autoral Pixelado
O Simula+ possui um design único e gamificado:
- **Background**: Grid verde escuro pixelado, estilo retro gaming
- **Cards**: Brancos com cantos arredondados sobre o fundo pixelado  
- **Tipografia**: Fonte pixelada/monospace mantendo identidade visual
- **UI Elements**: Campos de input com bordas definidas, botões pretos
- **Paleta**: Verde escuro + branco + preto - limpa e contrastante
- **Header**: Menu hamburger e título pixelado
- **Busca**: Campo estilo terminal com placeholder "BUSCAR EXAME..."

### Páginas HTML WebView
O projeto possui páginas HTML estáticas servidas via wwwroot/docs para exibição em WebView no app:
- `sobre-nos.html` - Página sobre a empresa/produto
- `politica-privacidade.html` - Política de privacidade
- `termos-uso.html` - Termos de uso
- `anonimizacao.html` - Informações sobre anonimização de dados

### Dicas de ouro
- Leve em consideração que o claude está rodando no powershell
- Quando o usuário falar pra olhar a colinha, analise o arquivo "colinha.txt" na raíz.
- Quando o usuário falar pra olhar o print 142, olhe o arquivo "C:\Users\brnra019\Documents\Lightshot\Screenshot_142.png"
- Quando o usuário falar "lembre disso", ele quer que você salve a informação na memória de longo prazo. ou seja no arquivo "CLAUDE.md".
- Sempre que terminar uma tarefa, faça o build pra confirmar se realmente está tudo certo.

