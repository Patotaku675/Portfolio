# AGENDA

Um sistema de gerenciamento de contatos simples desenvolvido com Django. 

---

## Funcionalidades do Site

1. **Página Admin.**
   - Somente para perfis autorizados como administradores.
   - Permite a manipulação de todos os dados.

2. **Página Inicial:**
   - Exibe um cabeçalho dinâmico que altera dependendo se o usuário está logado ou deslogado.
   - Lista todos os contatos visíveis, independentemente do estado de login.

3. **Login, Registro e Gerenciamento de perfil.**

4. **Contatos:**
   - **Criar Contato:** Permite criar contatos com nome, imagem e categoria.
   - **Editar/Apagar Contato:** Apenas o criador do contato pode realizar essas ações.
   - **Visualizar Contatos:** Qualquer pessoa (mesmo deslogada) pode visualizar os contatos.

---

## Como Inicializar o Servidor Web

1. Clone o repositório:
   ```
   git clone ...
   cd agenda
   ```

2. Crie um ambiente virtual:
   ```
   python -m venv venv
   source venv/bin/activate # No Windows use: venv\Scripts\activate
   ```
3. Instale as dependências:
   ```
   pip install -r requirements.txt
   ```
4. Realize as migrações necessárias:
   ```
   python manage.py makemigrations
   python manage.py migrate
   ```
5. Inicialize o servidor:
   ```
   python manage.py runserver
   ```

## Injetando contatos para teste

Para adicionar 1000 contatos aleatórios no banco de dados, utilize o script localizado em `/utils/create_contacts.py`. Siga os passos abaixo:

1. Certifique-se de que o servidor está rodando e o banco de dados está configurado corretamente.

2. estando no ambiete virtual python, execute o comando:
   ```
   python .\utils\create_contacts.py
   ```

Obrigado por conferir o projeto! Estou sempre aberto a novos desafios e oportunidades.