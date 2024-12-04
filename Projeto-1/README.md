# Sistema de Stream Service

Este projeto consiste em uma solução de serviço de streaming, com uma API, um aplicativo desktop para criadores e um aplicativo móvel para usuários. A arquitetura do sistema é composta por três partes principais:

- **API**: Desenvolvida com ASP.NET em C# para gerenciar usuários, criadores, conteúdos e playlists.
- **App Criador**: Aplicativo desktop utilizando MAUI.NET em C# para gerenciar conteúdos e playlists.
- **App Usuário**: Aplicativo móvel utilizando Android Studio em Java, permitindo aos usuários visualizar conteúdos e playlists.

## API - ASP.NET (C#)

A API foi construída utilizando ASP.NET e se conecta a um banco de dados com as seguintes tabelas:

- **Usuário**
- **Criador**
- **Conteúdos**
- **Playlists**

A API oferece os seguintes métodos CRUD para cada tabela:

- **Usuário**: Cadastro, login e gerenciamento de dados do usuário.
- **Criador**: Cadastro, login e gerenciamento de criadores de conteúdo.
- **Conteúdos**: Adicionar, editar, deletar e visualizar conteúdos criados pelos criadores.
- **Playlists**: Criar, editar, deletar e associar conteúdos a playlists.

## App Criador - MAUI.NET (C#) - Desktop-Windows

O aplicativo Criador foi desenvolvido com .NET MAUI, oferecendo uma interface de desktop para os criadores gerenciarem seus conteúdos e playlists.

### Funcionalidades:

- **Login de Criador**: Acesso à conta do criador utilizando a API para autenticação.
- **Manipulação de Conteúdos**:
  - Adicionar novos conteúdos.
  - Deletar conteúdos criados pelo criador logado.
  - Visualizar os conteúdos criados pelo criador.
- **Manipulação de Playlists**:
  - Adicionar novas playlists.
  - Deletar playlists existentes.
  - Adicionar múltiplos conteúdos a uma playlist específica.
  - Visualizar as playlists criadas pelo criador logado.

## App Usuário - Android Studio (Java) - Mobile-Android

O aplicativo para usuários foi desenvolvido utilizando Android Studio em Java, permitindo que os usuários interajam com conteúdos e playlists.

### Funcionalidades:

- **Login de Usuário**: Acesso à conta do usuário utilizando a API para autenticação.
- **Página de Conteúdos**:
  - Exibe todos os conteúdos disponíveis.
  - Permite a visualização dos conteúdos.
  - Os conteúdos podem ser curtidos.
- **Página de Playlists**:
  - Exibe todas as playlists disponíveis.
  - Permite selecionar uma playlist específica para visualizar os conteúdos dentro dela.
- **Página de Minhas Playlists**:
  - Criar e apagar playlists próprias.
  - Exibe todas as playlists do usuário.
  - Permite selecionar uma playlist para visualizar os conteúdos dentro dela.
  - Permite adicionar e remover conteúdos das playlists.

## Tecnologias Utilizadas

- **Back-End**: ASP.NET, C#, SQLite Server
- **Front-End**: .NET MAUI, C#, XAML (para o app de Criador)
- **Mobile**: Android Studio, Java
- **Banco de Dados**: SQLite Server
- **Outras**: Git

## Como Rodar o Projeto

### 1. Rodando a API

1. Clone este repositório.
2. Abra o projeto da API no Visual Studio.
3. Configure o banco de dados e as conexões no arquivo de configuração `appsettings.json`.
4. Execute a API no Visual Studio.

### 2. Rodando o App Criador

1. Clone este repositório.
2. Abra o projeto no Visual Studio.
3. Execute o aplicativo para Windows (desktop).

### 3. Rodando o App Usuário

1. Clone este repositório.
2. Abra o projeto no Android Studio.
3. Configure as permissões de acesso à API no código.
4. Execute o aplicativo no emulador ou dispositivo Android.


Obrigado por conferir o projeto! Estou sempre aberto a novos desafios e oportunidades.
