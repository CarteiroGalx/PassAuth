# 🔒PassAuth 
Um projeto de autenticação de usuário utilizando de tecnologias avançadas e utilizadas hoje no mercado. Sabe-se que um dos maiores erros de um desenvolvedor Back-end é guardar a senha em texto puro no banco de dados, facilitando o roubo de senhas para infratores, e por isso as grandes empresas atualmente fazem um hash da senha dos usuários para prevenir roubos através de tecnologias como Rainbow Tables. O projeto foi criado na total consciência de que a segurança deve estar em primeiro lugar. Além disso, há diversas camadas de segurança e validação em cada ação do usuário 
O foco do projeto é a **autenticação e segurança**, e por isso quaisquer outra coisa pode-se ser implementada no sistema de forma simples, visando dar total foco no real objetivo da aplicação.

## ⚙️Funcionalidades
Começando do básico, tem o sistema de Login e Register. Criando uma conta, você recebe o cargo de User, o cargo básico e de menor nível que te dá acesso à alguns pontos não tão restritos mas que são próprios do usuário que está logado. Ninguém além de você mesmo consegue acessar seus dados, já que o sistema usa um sistema de Token, uma chave digital e criptografada que basicamente garante que um infrator não consiga acessar seus dados.

Tem também o fato de que o único ser que tem a senha do usuário é o próprio sistema, e não os admins. A senha não cai em texto puro no banco de dados e sim também criptografada, sem falar que mesmo que um Admin mal intencionado busque pelos dados de um usuário específico, a senha não é entregue pelo sistema mesmo que esteja criptografada. Um outro detalhe também é que existem pontos no sistema aonde são restritos a pessoas com cargos maiores, como a solicitação de pedidos chamado Requests que é exclusivo dos Gerentes (Managers), e também o painel de controle dos usuários que é restrito aos Administrador (Admin). Existe também e provavelmente o maior trunfo é o Registro de Auditoria, aonde o sistema literalmente grava tudo que você faz de importante no sistema, seja modificar dados de um registro específico, criação de novos usuários de forma manual e até mesmo por buscar dados de uma pessoa específico. Isso foi criado na intenção de expor possíveis comportamentos suspeitos e de prover proteção à própria empresa, visando buscar provas sobre quem fez oque.
  
## 🤖Tecnologias usadas
- .NET 8
- PassHasher
- JWT (JSON Web Tokens)
- SQLite
- Visual Studio

## 📂Estrutura do projeto
```bash
PassAuth/
├── Context/
│   └── AppDbContext.cs
├── DTOs/
│   ├── Request/
│   │   └── DTOs de Request...
│   └── User/
│       └── DTOs de User...
├── Migrations/
│   └── Arquivos de Migrations...
├── Models/
│   ├── Enums/
│   │   ├── RequestStatus.cs
│   │   ├── UserRoles.cs
│   │   └── UserStatus.cs
│   ├── Author.cs
│   ├── User.cs
│   ├── ManagerRequest.cs
│   └── AuditLog.cs
├── Services/
│   ├── Interfaces/
│   │   ├── IAccountService.cs
│   │   ├── IAuditLogService.cs
│   │   ├── IAuthService.cs
│   │   ├── IRequestService.cs
│   │   └── IUserService.cs
│   ├── AccountService.cs
│   ├── AuditLogService.cs
│   ├── AuthService.cs
│   ├── RequestService.cs
│   └── UserService.cs
└── Program.cs
```

## 🔗Controladores e Serviços: suas funções no projeto
### Auth
Sua principal e única função é validar e autenticar acessos ao sistema através do Registro e Login. Foca totalmente na segurança assim como a proposta principal deste projeto. Não só isso como também ajuda a validar as ações críticas de cargos altos como Manager ou Admin dentro do sistema, e se a ação não puder ser validada, o sistema nem chega a concluir. Ou seja, um usuário só pode fazer coisas importantes no sistema se isso puder ser gravado.
Ele também verifica o Status de sua conta, fazendo com que caso esteja banido ou suspenso, não irá poder prosseguir.

### Users
Para acesso restrito a usuários com uma Role de Admin (administrador), sendo o cargo total do sistema. Pode ver informações de cada usuário como nome, email e ID. Pode também promover outros Users para Managers ou fazer o inverso, além de suspender contas, banir e etc. Mesmo quem tenha acesso ao Controller, ainda é impossibilitado de roubar senhas de usuários para más intenções.

### Account
Para atualizações de informações ou checagem dos próprios dados da conta, tipo um "Meu Perfil". Permite também trocar a senha de forma simples mas segura.

### Requests
Aqui é onde os Gerentes podem enviar seus Requests, que são basicamente pedidos que são enviados ao Administrador aonde podem ser aceitas ou negadas, mostrando também o autor daquela Request, o dia enviado e também o dia que foi avaliado pelo Administrador. Todos os Requests registrados no sistema são imutáveis, oque significa que nenhuma entidade dentro do sistema pode editar suas informações e apenas o Administrador pode checar por todos os Requests, impedindo com que haja espiões dentro da empresa.

### Audit Logs
A real magia ocorre aqui. Aqui é onde o Administrador pode conferir todas as ações que foram feitas dentro do sistema, seja enviamento de Requests, validações de Requests, alteração de Status de um usuário e tudo mais. Cada Log diz o autor, o dia e revela também o seu ID, ou seja, é impossível um mal-intencionado fazer uma ação e ainda sair indetectável.  

## 🔍Como rodar o projeto
- Primeiro de tudo, use `git copy <URL do projeto>` em um diretório desejável e certifique-se que tenha o .NET 8 instalado em sua máquina.
- Depois, navegue é a pasta raiz do projeto
- Digite `dotnet run` e aguarde a aplicação inicializar.
- Após isso, poderá começar a testar o sistema!

### Contas iniciais
Quando você botar o projeto pra rodar, pode notar que já irá ter 50 registros iniciais no banco de dados para justamente facilitar os testes da API. Pra ser mais preciso, tem 1 Admin, 6 Managers e todo o resto é User.
Para acessar qualquer uma dessas contas é super simples:
- Digite "user" junto de sua ID como nome de usuário, como por exemplo **user34**, **user45** e por aí vai.
- O registro do Admin é simplesmente **Admin** como nome de usuário.
- A senha de qualquer uma dessas contas é **Password123!**
  
## ❌Fraquezas / Incapacidades
- Não há uma UI agradável para usuários comuns
- Tecnicamente, nada impede de um Admin alterar os dados de outro Admin, abrindo uma brecha para uma ameaça interna.
- Alguns logs de auditoria podem ser "mentirosos", dizendo o nome do alvo, mas não seu ID (por exemplo "Admin baniu José", se José mudar de nome de usuário, o log perde o sentido). Não existe uma funcionalidade de troca de nome de usuário, mas ainda sim é considerado uma falha que pode ser resolvida futuramente
- Há pouco poder para os Managers, já que a única coisa que o cargo lhe dá como privilégio é enviar Requests ao Admin. Num ambiente mais realista, um Admin pode ficar sobrecarregado com tantos Requests e de coisas que os próprios Managers poderiam resolver.
- Um manager não há um poder direto sobre um User, apenas indireto, fazendo com que leve algum tempo mais que o necessário até um User seja punido por suas ações.
- Ainda não há como recuperar a senha. Se a senha for perdida, a conta também é perdida.
  
## ⭐Melhorias futuras
- Apenas um Admin poder promover outros Users para serem Managers ou Admins.
- Um Manager poder suspender um User, mas não deletar
- Apenas um Admin poder suspender/deletar um Manager ou User, estando no topo de hierarquia
- Implementar um jeito de poder recuperar a conta através de Perguntas de Segurança (Security Questions)
- Criação de uma UI simples para mais fácil manuseio do sistema
- Implementação de JWT e tokens
  
## 🔗Links
Segue aí! 😀https://github.com/CarteiroGalx
