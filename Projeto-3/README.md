# AGENDA

Um sistema de gerenciamento de contatos desenvolvido com Django. Este projeto inclui um app chamado `contact` e utiliza pastas para arquivos estáticos e templates.

---

## Estrutura do Projeto

- **App Principal:** `contact`
- **Pastas para arquivos estáticos:** `base_static`
- **Pastas para templates:** `base_templates` e templates adicionais dentro do app `contact`

---

## Funcionalidades do Site

1. **Página Inicial:**
   - Exibe um cabeçalho dinâmico que altera dependendo se o usuário está logado ou deslogado.
   - Lista todos os contatos visíveis, independentemente do estado de login.

2. **Página de Login e Registro:**
   - Permite que os usuários façam login.
   - Registro de novos usuários.

3. **Gerenciamento de Perfil:**
   - Usuários podem registrar e atualizar seus perfis.

4. **Contatos:**
   - **Criar Contato:** Permite criar contatos com nome, imagem e categoria.
   - **Editar/Apagar Contato:** Apenas o criador do contato pode realizar essas ações.
   - **Visualizar Contatos:** Qualquer pessoa (mesmo deslogada) pode visualizar os contatos.

---

## Requisitos

Para rodar este projeto, você precisa das seguintes dependências instaladas:

- **asgiref==3.8.1**  
- **Django==5.1.3**  
- **Faker==33.1.0**  
- **pillow==11.0.0**  
- **python-dateutil==2.9.0.post0**  
- **six==1.16.0**  
- **sqlparse==0.5.2**  
- **typing_extensions==4.12.2**  
- **tzdata==2024.2**  

---

## Como Inicializar o Servidor Web

1. Clone o repositório:
   ```
   git clone https://github.com/seu-usuario/agenda.git
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