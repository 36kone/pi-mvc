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
    Categoria VARCHAR(50)
);

-- Tabela de pedidos
CREATE TABLE IF NOT EXISTS pedidos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ClienteId INT NOT NULL,
    DataPedido DATETIME NOT NULL,
    Status VARCHAR(50) NOT NULL DEFAULT 'Feito',
    Total DECIMAL(10,2) NOT NULL DEFAULT 0,
    FOREIGN KEY (ClienteId) REFERENCES clientes(Id)
);

-- Tabela de itens do pedido
CREATE TABLE IF NOT EXISTS itens_pedido (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    PedidoId INT NOT NULL,
    PizzaId INT NOT NULL,
    Quantidade INT NOT NULL,
    PrecoUnitario DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (PedidoId) REFERENCES pedidos(Id),
    FOREIGN KEY (PizzaId) REFERENCES pizzas(Id)
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

-- Inserir algumas pizzas de exemplo
INSERT INTO pizzas (Nome, Sabor, Descricao, Preco, Categoria) VALUES
('Margherita', 'Queijo e Tomate', 'Molho de tomate, mussarela e manjericao', 35.00, 'Tradicional'),
('Calabresa', 'Calabresa', 'Calabresa fatiada, cebola e queijo', 38.00, 'Tradicional'),
('Portuguesa', 'Portuguesa', 'Presunto, ovo, cebola, ervilha e queijo', 42.00, 'Tradicional'),
('Quatro Queijos', 'Quatro Queijos', 'Mussarela, provolone, gorgonzola e parmesao', 45.00, 'Especial'),
('Chocolate', 'Chocolate', 'Chocolate ao leite e granulado', 40.00, 'Doce'),
('Brigadeiro', 'Brigadeiro', 'Brigadeiro e chocolate granulado', 42.00, 'Doce');
