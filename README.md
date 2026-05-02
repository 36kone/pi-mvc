# 🍕 Pizzas & Panuzzos - Sistema de Gerenciamento de Pedidos

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-512bd4?style=flat&logo=.net)
![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=c-sharp)
![MySQL](https://img.shields.io/badge/MySQL-4479A1?style=flat&logo=mysql)
![Bootstrap](https://img.shields.io/badge/Bootstrap-7952B3?style=flat&logo=bootstrap)

## 📋 Sobre o Projeto

Este projeto é um sistema web responsivo desenvolvido para a empresa **Pizzas & Panuzzos**, uma pizzaria móvel que atua em eventos, festivais gastronômicos e encomendas de produtos congelados (mini pizzas e panuzzos).

O sistema tem como objetivo digitalizar os processos operacionais, permitindo o registro de pedidos, controle de pagamentos e gerenciamento de clientes e eventos, otimizando a operação da pizzaria.

### 🎓 Contexto Acadêmico

Este projeto foi desenvolvido como parte dos requisitos acadêmicos da **Faculdade de Tecnologia de Atibaia (Fatec Atibaia)**.

**Integrantes do Grupo:**
- Devair Candido Vieira (Líder) - devair.vieira@fatec.sp.gov.br
- Caio Herrera Correia - caio.correia@fatec.sp.gov.br
- Bianca Cardoso Lobo dos Santos - bianca.santos111@fatec.sp.gov.br
- Isabella da Silva Carvalho - isabella.carvalho2@fatec.sp.gov.br
- Matheus Pereira Mendelini - matheus.mendelini@fatec.sp.gov.br
- Pedro dos Santos Camargo - pedro.camargo12@fatec.sp.gov.br

## 🛠️ Tecnologias Utilizadas

- **Backend:** ASP.NET Core 10.0 / C#
- **ORM:** Entity Framework Core 9.0 com Pomelo MySQL
- **Frontend:** HTML5, CSS3, Bootstrap 5.3
- **Banco de Dados:** MySQL
- **Padrão de Arquitetura:** MVC (Model-View-Controller)

## 📁 Estrutura do Projeto

```
pi-mvc/
├── PizzaMvc/
│   ├── Controllers/           # Controladores da aplicação
│   │   ├── HomeController.cs
│   │   ├── PizzaController.cs
│   │   ├── ClienteController.cs
│   │   └── PedidoController.cs
│   ├── Models/                # Entidades do domínio
│   │   ├── Pizza.cs
│   │   ├── Cliente.cs
│   │   ├── Pedido.cs
│   │   ├── ItemPedido.cs
│   │   └── Pagamento.cs
│   ├── Data/                  # Contexto do banco de dados
│   │   └── AppDbContext.cs
│   ├── Views/                 # Interface do usuário (Views)
│   │   ├── Shared/
│   │   │   └── _Layout.cshtml
│   │   ├── Home/
│   │   ├── Pizza/
│   │   ├── Cliente/
│   │   └── Pedido/
│   ├── appsettings.json       # Configurações da aplicação
│   └── Program.cs            # Configuração de serviços
├── docs/
│   └── FACULDADE DE TECNOLOGIA DE ATIBAIA 4343.pdf
└── database.sql               # Script de criação do banco de dados
```

## 🚀 Funcionalidades Implementadas

### 🍕 Gestão de Produtos (Pizzas)
- Cadastro de pizzas com nome, sabor, descrição, preço e categoria
- Categorias: Tradicional, Especial e Doce
- Edição e exclusão de produtos
- Listagem completa de pizzas cadastradas

### 👥 Gestão de Clientes
- Cadastro de clientes com nome, telefone, email e CPF/CNPJ
- Edição e exclusão de clientes
- Listagem de todos os clientes

### 📦 Gestão de Pedidos
- Registro de pedidos com múltiplos itens
- Controle de status do pedido:
  - **Feito** → **Em Produção** → **Pronto** → **Entregue**
- Cálculo automático do valor total
- Atualização de status em tempo real
- Listagem de todos os pedidos com filtros visuais por status

### 💰 Controle de Pagamentos
- Registro de pagamentos (PIX, Cartão, Dinheiro)
- Status de pagamento
- Integração com pedidos

## ⚙️ Como Executar o Projeto

### Pré-requisitos
- .NET SDK 10.0 ou superior
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
| **Cliente** | Pessoas físicas ou jurídicas que realizam pedidos |
| **Pedido** | Registro de pedidos com status e valor total |
| **ItemPedido** | Itens individuais de um pedido (pizza + quantidade) |
| **Pagamento** | Informações de pagamento vinculadas ao pedido |

### Relacionamentos
- Um **Cliente** pode ter vários **Pedidos**
- Um **Pedido** possui vários **ItensPedido**
- Cada **ItemPedido** referencia uma **Pizza**
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

## 📞 Contato

Para dúvidas ou sugestões relacionadas ao projeto acadêmico, entre em contato através dos emails institucionais da Fatec informados acima.

---

<p align="center">Desenvolvido com ❤️ pela turma de Engenharia de Software - Fatec Atibaia</p>
