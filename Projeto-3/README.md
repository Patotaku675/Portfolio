# E-Commerce

Um sistema de E-Commerce desenvolvido com Django. Mostrando a base de um sistema, sem metodo de pagamento.

---

## Funcionalidades do Site

1. **Página Admin.**
   - Somente para perfis autorizados como administradores.
   - Permite a visualização de todos os dados criados. (Perfil, Pedido, Produto)
   - Permite adicionar, remover ou editar produtos e suas variações.

2. **Página Inicial.**
   - Exibe um cabeçalho dinâmico que altera dependendo se o usuário está logado ou deslogado.
   - Exibe o carrinho do usuário, caso tenha produto dentro dele.
   - Lista todos os produtos visíveis, independentemente do estado de login.
   - Campo de busca simples.

3. **Login, Registro e Gerenciamento de perfil.**

4. **Detalhe do produto.**
   - Exibe a imagem, a descrição longa, variações e o preço do produto.
   - Permite adicionar ao carrinho do usuário.

5. **Visualizar Carinho.**
   - Permite ter um resumo de todos os produtos dentro do carrinho.
   - Permite finalizar a compra.

6. **Resumo da Compra**
   - Exibe os detalhes do perfil do usuário e endereço apos fechar o carrinho.
   - Lista os produtos dentro do carrinho.

7. **Pedido Realizado**
   - Informa um resumo da compra finalizada e redireciona para o pagamento.

8. **Lista de Pedidos**
   - Informa todos os pedidos feitos pelo usuário.
   - Permite ver detalhes mais aprofundados de cada pedido.

---

## Como Inicializar o Servidor Web Local

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