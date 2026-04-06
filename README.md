# 🔒PassAuth 
Um projeto feito com carinho que é usado para testes de autenticação de usuário. Atualmente, é um mero protótipo, mas futuramente se tornará um app como qualquer outro e totalmente funcional. Sabe-se que um dos maiores erros de um desenvolvedor Back-end é guarda a senha em texto puro no banco de dados, facilitando o roubo de senhas para infratores, e por isso as grandes empresas atualmente fazem um hash da senha dos usuários para prevenir roubos através de tecnologias como Rainbow Tables. O projeto foi criado na total consciência de que a segurança deve estar em primeiro lugar. 
O foco do projeto é a **autenticação e segurança**, e por isso quaisquer outra coisa pode-se ser implementada no sistema de forma simples, visando dar total foco no real objetivo da aplicação.

## ⚙️Funcionalidades
- Cria um hash da senha do usuário e o guarda no banco de dados.
- Múltiplos usuários podem existir no banco de dados.
  
## 🤖Tecnologias usadas
- .NET 8
- PassHasher

## 📂Estrutura do projeto
```bash
PassAuth/
├── Context/
│   └── AppDbContext.cs
├── Controllers/
│   ├── AuthController.cs
│   └── UserController.cs
├── DTOs/
│   └── UserDto.cs
├── Migrations/
│   └── (Arquivos de migração)
├── Models/
│   ├── User.cs
│   └── Enums/
│       └── UserRole.cs
├── Services/
│   ├── IUserService.cs
│   └── UserService.cs
└── Program.cs
```

## 🔍Como rodar o projeto
- Primeiro de tudo, use `git copy <URL do projeto>` em um diretório desejável e certifique-se que tenha o .NET 8 instalado em sua máquina.
- Depois, navegue é a pasta raiz do projeto
- Digite `dotnet run` e aguarde a aplicação inicializar.
- Após isso, poderá começar a testar o sistema!
  
## ❌Fraquezas / Incapacidades
- Só uma única conta é suportada, a qual fica armazenada na memória da máquina. Após registrar uma outra conta, a anterior é totalmente substituída.
- Não há uma UI agradável para usuários comuns.
- Os testes de Login e Registro são totalmente manuais, tendo que "ir e voltar" entre cada método.
  
## ⭐Melhorias futuras
- Implementação de um banco de dados para multiplas contas poderem existir sem que haja "atropelamentos"
- Implementar um jeito de poder recuperar a conta através de Perguntas de Segurança (Security Questions)
- Criação de uma UI simples para mais fácil manuseio do sistema
- Implementação de JWT e tokens
  
## 🔗Links
Segue aí! 😀https://github.com/CarteiroGalx
