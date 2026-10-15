
require('dotenv').config();

const express = require('express');
const { Pool } = require('pg');
const path = require('path'); 

const app = express();

app.use(express.json());
app.use(express.static(__dirname)); 

const db = new Pool({
    connectionString: process.env.DATABASE_URL,
    ssl: { rejectUnauthorized: false }
});

app.get('/', async (req, res) => {
    try {
        const resultado = await db.query('SELECT NOW()');
        res.send('Conectado ao Neon! Horário do banco: ' + resultado.rows[0].now);
    } catch (erro) {
        console.error(erro);
        res.status(500).send('Erro ao conectar ao banco.');
    }
});

app.listen(3000, () => {
    console.log('Servidor rodando em http://localhost:3000');
});

app.post('/cadastrar', async (req, res) => {
    try {
        const { nome, email, senha, telefone} = req.body;

        await db.query(
            'INSERT INTO cliente (nome, email, senha) VALUES ($1, $2, $3, $4)',
            [nome, email, senha, telefone]
        );

        res.send('Cadastro realizado com sucesso!');
    } catch (erro) {
        console.error(erro);
        res.status(500).send('Erro ao cadastrar cliente.');
    }
});
