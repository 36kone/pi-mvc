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
INSERT INTO pizzas (Nome, Sabor, Descricao, Preco, Categoria) VALUES
('Margherita', 'Queijo e Tomate', 'Molho de tomate, mussarela e manjericao', 35.00, 'Tradicional'),
('Calabresa', 'Calabresa', 'Calabresa fatiada, cebola e queijo', 38.00, 'Tradicional'),
('Portuguesa', 'Portuguesa', 'Presunto, ovo, cebola, ervilha e queijo', 42.00, 'Tradicional'),
('Quatro Queijos', 'Quatro Queijos', 'Mussarela, provolone, gorgonzola e parmesao', 45.00, 'Especial'),
('Chocolate', 'Chocolate', 'Chocolate ao leite e granulado', 40.00, 'Doce'),
('Brigadeiro', 'Brigadeiro', 'Brigadeiro e chocolate granulado', 42.00, 'Doce');

-- Inserir algumas bebidas de exemplo
INSERT INTO bebidas (Nome, Sabor, Descricao, Preco, Categoria) VALUES
('Coca-Cola', 'Cola', 'Refrigerante Coca-Cola 2L', 8.00, 'Refrigerante'),
('Guaraná Antarctica', 'Guaraná', 'Refrigerante Guaraná Antarctica 2L', 7.50, 'Refrigerante'),
('Água Mineral', 'Sem sabor', 'Água mineral sem gás 500ml', 2.00, 'Água'),
('Suco de Laranja', 'Laranja', 'Suco natural de laranja 300ml', 5.00, 'Suco');

INSERT INTO clientes (Nome, Telefone, Email, CpfCnpj) VALUES
('João Silva', '11999999999', 'joao@gmail.com', '12345678900'),
('Maria Oliveira', '11988888888', 'maria@gmail.com', '98765432100');

-- PEDIDOS
INSERT INTO pedidos (ClienteId, DataPedido, Status, Total) VALUES
(1, NOW(), 'Em preparo', 73.00),
(2, NOW(), 'Entregue', 42.00);

-- ITENS DO PEDIDO
INSERT INTO itens_pedido (PedidoId, PizzaId, BebidaId, Quantidade, PrecoUnitario) VALUES
(1, 1, NULL, 1, 35.00), -- Margherita
(1, 2, NULL, 1, 38.00), -- Calabresa
(2, NULL, 1, 2, 8.00), -- 2 Coca-Cola
(2, 3, NULL, 1, 42.00); -- Portuguesa

-- PAGAMENTOS
INSERT INTO pagamentos (PedidoId, FormaPagamento, Valor, DataPagamento, Status) VALUES
(1, 'Cartão de Crédito', 73.00, NOW(), 'Pago'),
(2, 'PIX', 42.00, NOW(), 'Pago');

-- Inserir usuario admin de exemplo
INSERT INTO usuarios (Nome, Email, Senha, Tipo) VALUES
('Admin', 'admin@admin.com', '$2a$11$r3i6tKzq7K7lL7lL7lL7lO8vW9xXyYzZ1234567890123456789012', 'Admin'),
('Usuario', 'user@email.com', '$2a$11$A1b2C3d4E5f6G7h8I9j0kLmNoPqRsTuVwXyZ12345678901234', 'User');
