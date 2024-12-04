package com.example.appusuario;

import android.content.Intent;
import android.os.Bundle;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.app.AppCompatDelegate;

import com.example.appusuario.Entities.Usuario;
import com.example.appusuario.Service.ApiService;
import com.example.appusuario.Service.RetrofitClient;

import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class MainActivity extends AppCompatActivity {

    // Definição dos componentes de entrada de dados
    private EditText nomeEntry;
    private EditText emailEntry;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.main_page); // Define o layout da atividade

        // Força o tema claro
        AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_NO);

        // Vincula os elementos do layout às variáveis
        nomeEntry = findViewById(R.id.nomeEntry);
        emailEntry = findViewById(R.id.emailEntry);
        Button cadastroButton = findViewById(R.id.cadastro);
        Button loginButton = findViewById(R.id.login);  // Botão de login

        // Configura a ação para o botão de cadastro
        cadastroButton.setOnClickListener(v -> {
            String nome = nomeEntry.getText().toString().trim();
            String email = emailEntry.getText().toString().trim();

            // Verifica se todos os campos estão preenchidos
            if (nome.isEmpty() || email.isEmpty()) {
                Toast.makeText(MainActivity.this, "Preencha todos os campos!", Toast.LENGTH_SHORT).show();
            } else {
                cadastrarUsuario(nome, email); // Chama o método de cadastro
            }
        });

        // Configura a ação para o botão de login
        loginButton.setOnClickListener(v -> {
            String nome = nomeEntry.getText().toString().trim();
            String email = emailEntry.getText().toString().trim();

            // Verifica se todos os campos estão preenchidos
            if (nome.isEmpty() || email.isEmpty()) {
                Toast.makeText(MainActivity.this, "Preencha todos os campos!", Toast.LENGTH_SHORT).show();
            } else {
                realizarLogin(nome, email); // Chama o método de login
            }
        });
    }

    /**
     * Método para cadastrar um novo usuário
     * @param nome Nome do usuário
     * @param email Email do usuário
     */
    private void cadastrarUsuario(String nome, String email) {
        // Obtém o serviço da API
        ApiService apiService = RetrofitClient.getRetrofitInstance().create(ApiService.class);

        // Cria um objeto Usuario com os dados informados
        Usuario usuario = new Usuario(nome, email);

        // Envia a solicitação POST para cadastrar o usuário
        Call<Void> call = apiService.criarUsuario(usuario);

        call.enqueue(new Callback<>() {
            @Override
            public void onResponse(@NonNull Call<Void> call, @NonNull Response<Void> response) {
                if (response.isSuccessful()) {
                    // Exibe mensagem de sucesso se o cadastro for bem-sucedido
                    Toast.makeText(MainActivity.this, "Usuário cadastrado com sucesso!", Toast.LENGTH_SHORT).show();
                } else {
                    // Exibe mensagem de erro caso o cadastro falhe
                    Toast.makeText(MainActivity.this, "Erro no cadastro: " + response.code(), Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(@NonNull Call<Void> call, @NonNull Throwable t) {
                // Exibe mensagem de erro caso a conexão falhe
                Toast.makeText(MainActivity.this, "Erro na conexão: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    /**
     * Método para realizar o login do usuário
     * @param nome Nome do usuário
     * @param email Email do usuário
     */
    private void realizarLogin(String nome, String email) {
        // Obtém o serviço da API
        ApiService apiService = RetrofitClient.getRetrofitInstance().create(ApiService.class);

        // Realiza uma requisição GET para obter a lista de usuários cadastrados
        Call<List<Usuario>> call = apiService.getUsuarios();

        call.enqueue(new Callback<>() {
            @Override
            public void onResponse(@NonNull Call<List<Usuario>> call, @NonNull Response<List<Usuario>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    List<Usuario> usuarios = response.body();
                    boolean usuarioEncontrado = false;

                    // Verifica se algum usuário na lista corresponde ao nome e e-mail fornecidos
                    for (Usuario usuario : usuarios) {
                        if (usuario.getNome().equals(nome) && usuario.getEmail().equals(email)) {
                            usuarioEncontrado = true;

                            // Salva os dados do usuário no SharedPreferences
                            getSharedPreferences("userPrefs", MODE_PRIVATE)
                                    .edit()
                                    .putInt("id", usuario.getId())
                                    .putString("nome", usuario.getNome())
                                    .putString("email", usuario.getEmail())
                                    .apply();

                            // Navega para a tela de conteúdos do usuário
                            try {
                                Intent intent = new Intent(MainActivity.this, MeusConteudosActivity.class);
                                intent.putExtra("nomeUsuario", usuario.getNome()); // Envia nome do usuário
                                intent.putExtra("idUsuario", usuario.getId()); // Envia ID do usuário
                                startActivity(intent);
                            } catch (Exception e) {
                                // Exibe erro caso a navegação falhe
                                Toast.makeText(MainActivity.this, "Erro ao navegar: " + e.getMessage(), Toast.LENGTH_SHORT).show();
                                e.printStackTrace();  // Imprime o erro no log para depuração
                            }

                            break; // Interrompe o loop após encontrar o usuário
                        }
                    }

                    // Caso o usuário não tenha sido encontrado
                    if (!usuarioEncontrado) {
                        Toast.makeText(MainActivity.this, "Usuário ou senha inválidos!", Toast.LENGTH_SHORT).show();
                    }
                } else {
                    // Exibe mensagem de erro se falhar ao acessar os usuários
                    Toast.makeText(MainActivity.this, "Erro ao acessar os usuários: " + response.code(), Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(@NonNull Call<List<Usuario>> call, @NonNull Throwable t) {
                // Exibe mensagem de erro caso a requisição falhe
                Toast.makeText(MainActivity.this, "Erro na conexão: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }
}
