# Calculadora

Este projeto consiste em uma calculadora interativa desenvolvida com **PySide6**. Ele oferece suporte a operações aritméticas, funcionalidade de teclado e um design intuitivo com botões para interação.

---

## Funcionalidades

### **Operações Suportadas**
- **Aritméticas:** Soma (`+`), Subtração (`-`), Multiplicação (`*`), Divisão (`/`), Potenciação (`P` ou `^`).
- **Adicionais:**  
  - **Inverter sinal:** Muda o número atual de positivo para negativo ou vice-versa. (`N`)  
  - **Apagar dígito:** Remove o último dígito inserido.  
  - **Limpar tudo:** Reseta a calculadora para o estado inicial.  

### **Interface do Usuário**
- **Display:**  
  - O display principal mostra o número atual ou o resultado de uma operação.  
  - Um **display superior** exibe a operação sendo realizada, como `20 +` ou `20 + 30 = 50`.  

- **Botões Incluídos:**  
  - Números: `0-9` e `.` (ponto decimal).  
  - Operadores: `+`, `-`, `*`, `/`, `P` (potenciação).  
  - Controle:  
    - **C:** Limpa tudo.  
    - **Backspace:** Apaga o último dígito.  
    - **N:** Inverte o sinal do número atual.  
    - **Enter ou `=`:** Mostra o resultado da operação.  

### **Funcionalidade do Teclado**
- Digite os números e operadores diretamente pelo teclado.  
- Utilize os atalhos:  
  - `C` para limpar tudo.  
  - `Backspace` para apagar o último dígito.  
  - `N` para inverter o sinal.  
  - `P` ou `^` para potenciação.  
  - `Enter` para exibir o resultado.  

---

## Como Rodar o Projeto

1. **Clone o Repositório**
   ```bash
   git clone https://github.com/seu-usuario/calculadora-pyside6.git
   cd calculadora-pyside6
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
5. Inicialize o servidor:
   ```
   python main.py
   ```
