-- Script para criar o banco de dados e tabelas do Pizzas & Panuzzos
-- Execute este script no MySQL

CREATE DATABASE IF NOT EXISTS pizza_mvc;
USE pizza_mvc;

-- Tabela de clientes
CREATE TABLE IF NOT EXISTS clientes (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    Telefone VARCHAR(20),
    Email VARCHAR(100),
    CpfCnpj VARCHAR(20)
);

-- Tabela de pizzas
CREATE TABLE IF NOT EXISTS pizzas (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    Sabor VARCHAR(100) NOT NULL,
    Descricao TEXT,
    Preco DECIMAL(10,2) NOT NULL,
    Categoria VARCHAR(50),
    Image VARCHAR(255)
);

-- Tabela de bebidas
CREATE TABLE IF NOT EXISTS bebidas (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    Sabor VARCHAR(100) NOT NULL,
    Descricao TEXT,
    Preco DECIMAL(10,2) NOT NULL,
    Categoria VARCHAR(50),
    Image VARCHAR(255)
);

-- Tabela de eventos
CREATE TABLE IF NOT EXISTS eventos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    Descricao TEXT,
    DataEvento DATETIME NOT NULL,
    Local VARCHAR(100),
    Image VARCHAR(255)
);

-- Tabela de pedidos
CREATE TABLE IF NOT EXISTS pedidos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ClienteId INT NOT NULL,
    DataPedido DATETIME NOT NULL,
    Status VARCHAR(50) NOT NULL DEFAULT 'Pendente',
    Total DECIMAL(10,2) NOT NULL DEFAULT 0,
    FOREIGN KEY (ClienteId) REFERENCES clientes(Id)
);

-- Tabela de itens do pedido
CREATE TABLE IF NOT EXISTS itens_pedido (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    PedidoId INT NOT NULL,
    PizzaId INT NULL,
    BebidaId INT NULL,
    Quantidade INT NOT NULL,
    PrecoUnitario DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (PedidoId) REFERENCES pedidos(Id),
    FOREIGN KEY (PizzaId) REFERENCES pizzas(Id),
    FOREIGN KEY (BebidaId) REFERENCES bebidas(Id)
);

-- Tabela de pagamentos
CREATE TABLE IF NOT EXISTS pagamentos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    PedidoId INT NOT NULL UNIQUE,
    FormaPagamento VARCHAR(50) NOT NULL,
    Valor DECIMAL(10,2) NOT NULL,
    DataPagamento DATETIME NOT NULL,
    Status VARCHAR(50) NOT NULL DEFAULT 'Pago',
    FOREIGN KEY (PedidoId) REFERENCES pedidos(Id)
);

-- Tabela de usuarios (admin)
CREATE TABLE IF NOT EXISTS usuarios (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Senha VARCHAR(255) NOT NULL,
    Tipo VARCHAR(50) NOT NULL DEFAULT 'Admin',
    DataCriacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Inserir algumas pizzas de exemplo
INSERT INTO pizzas (Id, Nome, Sabor, Descricao, Preco, Categoria, Image) VALUES (19, 'Margherita', 'Queijo e Tomate', 'Molho de tomate, mussarela e manjericao', 35.00, 'Tradicional', '/files/pizzas/2cbe3650-108e-43e3-b9f7-d6dbd93b91b9.jpeg');
INSERT INTO pizzas (Id, Nome, Sabor, Descricao, Preco, Categoria, Image) VALUES (20, 'Calabresa', 'Calabresa', 'Calabresa fatiada, cebola e queijo', 38.00, 'Tradicional', '/files/pizzas/0f5ce7af-9729-4f89-8cb4-f90fb8d7d474.jpeg');
INSERT INTO pizzas (Id, Nome, Sabor, Descricao, Preco, Categoria, Image) VALUES (22, 'Quatro Queijos', 'Quatro Queijos', 'Mussarela, provolone, gorgonzola e parmesao', 45.00, 'Especial', '/files/pizzas/5bdb2b0d-985c-45e7-9288-140c58109096.jpeg');
INSERT INTO pizzas (Id, Nome, Sabor, Descricao, Preco, Categoria, Image) VALUES (23, 'Banana com Canela', 'Banana com Canela', 'Banana com Canela', 40.00, 'Doce', '/files/pizzas/b8859b9e-dd67-4457-a703-7d3699b9d12e.jpeg');
INSERT INTO pizzas (Id, Nome, Sabor, Descricao, Preco, Categoria, Image) VALUES (25, 'teste', 'teste', 'sdaas', 0.00, 'Tradicional', '/files/pizzas/8caddd11-e74f-4241-8113-5c5d9d26fef0.png');

-- Inserir algumas bebidas de exemplo
INSERT INTO bebidas (Id, Nome, Sabor, Descricao, Preco, Categoria, Image) VALUES (3, 'Água Mineral', 'Sem sabor', 'Água mineral sem gás 500ml', 2.00, 'Água', '/files/bebidas/43d1495b-8935-4a70-b465-da4359a7d87d.jpg');
INSERT INTO bebidas (Id, Nome, Sabor, Descricao, Preco, Categoria, Image) VALUES (4, 'Suco de Laranja', 'Laranja', 'Suco natural de laranja 300ml', 5.00, 'Suco', '/files/bebidas/1f147002-a0b3-43c3-b2bf-d7327cef4567.jpeg');
INSERT INTO bebidas (Id, Nome, Sabor, Descricao, Preco, Categoria, Image) VALUES (5, 'Coca-Cola', 'Cola', 'Refrigerante Coca-Cola 2L', 8.00, 'Refrigerante', '/files/bebidas/18c33812-d1ea-463a-8a82-ae4afee4a489.png');

-- Inserir alguns eventos de exemplo
INSERT INTO eventos (Id, Nome, Descricao, DataEvento, Local, Image) VALUES (1, 'Reserva Evento', 'Reserva Evento', '2026-06-07 20:19:00', 'Atibaia', '/files/eventos/b6ac79e9-5e18-436c-8711-31109566cca3.jpeg');
INSERT INTO eventos (Id, Nome, Descricao, DataEvento, Local, Image) VALUES (2, 'Pizza na sua casa', 'Pizza na sua casa', '2026-06-07 20:20:00', 'Atibaia', '/files/eventos/11d1be81-3435-442e-a8cd-e19460f2fdfa.jpeg');
INSERT INTO eventos (Id, Nome, Descricao, DataEvento, Local, Image) VALUES (3, 'Festival Atibaia', 'Festival Atibaia', '2026-06-07 20:20:00', 'Atibaia', '/files/eventos/ac586e45-5255-412f-b319-82090b4188cc.jpeg');
