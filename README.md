# 🍕 Pizzas & Panuzzos - Sistema de Gerenciamento de Pedidos

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-512bd4?style=flat&logo=.net)
![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=c-sharp)
![MySQL](https://img.shields.io/badge/MySQL-4479A1?style=flat&logo=mysql)

## 📋 Sobre o Projeto

Este projeto é um sistema web responsivo desenvolvido para a empresa **Pizzas & Panuzzos**, uma pizzaria móvel que atua em eventos, festivais gastronômicos e encomendas de produtos congelados (mini pizzas e panuzzos).

O sistema tem como objetivo digitalizar os processos operacionais, permitindo o registro de pedidos, controle de pagamentos e gerenciamento de clientes e eventos, otimizando a operação da pizzaria.

### 🎓 Contexto Acadêmico

Este projeto foi desenvolvido como parte dos requisitos acadêmicos da **Faculdade de Tecnologia de Atibaia (Fatec Atibaia)**.

**Integrantes do Grupo:**
- Caio Herrera Correia - caio.correia@fatec.sp.gov.br
- Isabella da Silva Carvalho - isabella.carvalho2@fatec.sp.gov.br

## 🛠️ Tecnologias Utilizadas

- **Backend:** ASP.NET Core 8.0 / C#
- **ORM:** Entity Framework Core 8.0 com Pomelo MySQL
- **Frontend:** Razor Views (MVC), HTML5, CSS3, JavaScript
- **Banco de Dados:** MySQL
- **Padrão de Arquitetura:** MVC (Model-View-Controller)

## 📁 Estrutura do Projeto

```
pi-mvc/
├── PizzaMvc/
│   ├── Controllers/                 # Controladores da aplicação
│   │   ├── AdminController.cs
│   │   ├── BebidaController.cs
│   │   ├── CartController.cs
│   │   ├── ClienteController.cs
│   │   ├── EventoController.cs
│   │   ├── HomeController.cs
│   │   ├── PagamentoController.cs
│   │   ├── PedidoController.cs
│   │   ├── PizzaController.cs
│   │   └── UsuarioController.cs
│   ├── Data/                        # Contexto do banco de dados
│   │   └── AppDbContext.cs
│   ├── Models/                      # Entidades do domínio
│   │   ├── Bebida.cs
│   │   ├── Cliente.cs
│   │   ├── Evento.cs
│   │   ├── ItemPedido.cs
│   │   ├── Pagamento.cs
│   │   ├── Pedido.cs
│   │   ├── Pizza.cs
│   │   └── Usuario.cs
│   ├── Views/                       # Interface do usuário (Views)
│   │   ├── Admin/
│   │   ├── Bebida/
│   │   ├── Cart/
│   │   ├── Cliente/
│   │   ├── Evento/
│   │   ├── Home/
│   │   ├── Pedido/
│   │   ├── Pizza/
│   │   ├── Usuario/
│   │   └── Shared/
│   │       └── _Layout.cshtml
│   ├── wwwroot/                     # Arquivos estáticos (CSS/JS/imagens)
│   │   ├── css/
│   │   ├── images/
│   │   └── js/
│   ├── appsettings.json             # Configurações da aplicação
│   └── Program.cs                   # Configuração de serviços
├── docs/
│   └── FACULDADE DE TECNOLOGIA DE ATIBAIA 4343.pdf
│   └── mer.jpg   
└── database.sql               # Script de criação do banco de dados
```

## 🚀 Funcionalidades Implementadas

### 🍕 Gestão de Produtos (Pizzas)
- Cadastro de pizzas com nome, sabor, descrição, preço e categoria
- Categorias: Tradicional, Especial e Doce
- Edição e exclusão de produtos
- Listagem completa de pizzas cadastradas

### 🥤 Gestão de Produtos (Bebidas)
- Cadastro, edição, exclusão e listagem de bebidas

### 👥 Gestão de Clientes
- Cadastro de clientes com nome, telefone, email e CPF/CNPJ
- Edição e exclusão de clientes
- Listagem de todos os clientes

### 📦 Gestão de Pedidos
- Registro de pedidos com múltiplos itens (pizzas e bebidas) e cálculo automático do total
- Checkout a partir do carrinho (com forma de pagamento)
- Tela **Meus Pedidos** (cliente) com busca por CPF/CNPJ e memória local (localStorage) para não precisar de login
- Listagem de pedidos (admin) com botões para transição de status:
  - **Pendente** → **Em Andamento** → **Concluido**

### 💰 Controle de Pagamentos
- Registro de pagamentos (PIX, Cartão, Dinheiro)
- Status de pagamento
- Integração com pedidos

### 📅 Gestão de Eventos
- Cadastro, edição, exclusão e listagem de eventos

### 🧑‍💼 Gestão de Usuários
- Cadastro, edição, exclusão e listagem de usuários

## ⚙️ Como Executar o Projeto

### Pré-requisitos
- .NET SDK 8.0 ou superior
- MySQL Server instalado e rodando
- Visual Studio 2022 ou VS Code (opcional)

### Passo a Passo

1. **Clone o repositório:**
   ```bash
   git clone <url-do-repositorio>
   cd pi-mvc/PizzaMvc
   ```

2. **Configure o banco de dados:**
   - Abra o MySQL e execute o script `database.sql`:
   ```bash
   mysql -u root -p < ../database.sql
   ```
   - Ou copie e cole o conteúdo do arquivo no seu cliente MySQL preferido

3. **Ajuste a connection string:**
   - Edite o arquivo `PizzaMvc/appsettings.json`
   - Altere a senha do MySQL conforme sua configuração:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=pizza_mvc;User=root;Password=SUA_SENHA;"
   }
   ```

4. **Restaure os pacotes e execute:**
   ```bash
   dotnet restore
   dotnet run
   ```

5. **Acesse a aplicação:**
   - Abra o navegador em: `https://localhost:5001` ou `http://localhost:5000`

## 🗄️ Modelo de Dados

### Entidades Principais

| Entidade | Descrição |
|----------|-----------|
| **Pizza** | Produtos do cardápio com preço e categoria |
| **Bebida** | Bebidas com preço e categoria |
| **Cliente** | Pessoas físicas ou jurídicas que realizam pedidos |
| **Pedido** | Registro de pedidos com status e valor total |
| **ItemPedido** | Itens individuais de um pedido (pizza/bebida + quantidade) |
| **Pagamento** | Informações de pagamento vinculadas ao pedido |
| **Evento** | Eventos cadastrados para divulgação/controle |
| **Usuario** | Usuários administradores/operadores do sistema |

### Relacionamentos
- Um **Cliente** pode ter vários **Pedidos**
- Um **Pedido** possui vários **ItensPedido**
- Cada **ItemPedido** referencia uma **Pizza** ou uma **Bebida**
- Um **Pedido** possui um **Pagamento**

## 📊 Requisitos Atendidos (Baseado no Documento de Requisitos)

| Código | Requisito | Status |
|--------|-----------|--------|
| RF01 | Registrar pedidos | ✅ Implementado |
| RF02 | Controlar fluxo do pedido (status) | ✅ Implementado |
| RF03 | Emitir comandas numeradas | ✅ Implementado (ID do pedido) |
| RF05 | Registrar pagamentos | ✅ Implementado |
| RF08 | Controlar pagamentos | ✅ Implementado |
| RF14 | Cadastro de produtos | ✅ Implementado |

## 🔮 Melhorias Futuras

Como este é um projeto acadêmico simplificado, as seguintes funcionalidades podem ser implementadas em versões futuras:

- [ ] Emissão de recibos e notas fiscais (DANFE)
- [ ] Geração de relatórios gerenciais
- [ ] Sistema de feedback e reclamações
- [ ] Agendamento de eventos
- [ ] Notificações via WhatsApp/Email
- [ ] Integração com PIX para pagamentos
- [ ] Controle de estoque de ingredientes
- [ ] Aplicativo mobile (PWA)
- [ ] Autenticação e autorização de usuários

## 📄 Licença

Este projeto é desenvolvido para fins acadêmicos na Faculdade de Tecnologia de Atibaia.
