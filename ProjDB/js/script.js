
const formulario = document.getElementById('formCadastro');

formulario.addEventListener('submit', async function(evento) {
    evento.preventDefault();

    const cliente = {
        nome: document.getElementById('nome').value,
        email: document.getElementById('email').value,
        senha: document.getElementById('senha').value,
        telefone: document.getElementById('telefone').value

    };

    try {
        const resposta = await fetch('/cadastrar', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(cliente)
        });

        const resultado = await resposta.text();

        alert(resultado);

    } catch (erro) {
        alert('tente novamente');
        console.error(erro);
    }
});
