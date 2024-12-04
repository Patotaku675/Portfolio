package com.example.appusuario;

import android.os.Bundle;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;

import com.example.appusuario.Entities.Playlist;
import com.example.appusuario.Service.ApiService;
import com.example.appusuario.Service.RetrofitClient;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class CriarPlaylistActivity extends AppCompatActivity {

    // Variável para armazenar o ID do usuário
    private int _idUsuario;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.criar_playlist); // Define o layout da activity

        // Recupera o ID do usuário passado pela Intent
        _idUsuario = getIntent().getIntExtra("idUsuario", -1);

        // Inicializa os componentes de UI
        EditText edtNomePlaylist = findViewById(R.id.edtNomePlaylist);
        Button btnSalvar = findViewById(R.id.btnSalvar);

        // Configura o clique do botão para salvar a playlist
        btnSalvar.setOnClickListener(v -> {
            String nomePlaylist = edtNomePlaylist.getText().toString().trim();
            // Verifica se o nome da playlist foi preenchido e se o ID do usuário é válido
            if (!nomePlaylist.isEmpty() && _idUsuario != -1) {
                // Chama o método para criar a playlist
                criarPlaylist(nomePlaylist);
            } else {
                // Exibe mensagem caso o nome da playlist não tenha sido preenchido ou o ID do usuário seja inválido
                Toast.makeText(this, "Preencha o nome da playlist!", Toast.LENGTH_SHORT).show();
            }
        });
    }

    /**
     * Método que cria uma nova playlist através da API
     * @param nome Nome da playlist a ser criada
     */
    private void criarPlaylist(String nome) {
        // Obtém a instância do serviço da API
        ApiService apiService = RetrofitClient.getApiService();

        // Cria um objeto Playlist com o nome e o ID do usuário
        Playlist novaPlaylist = new Playlist(nome, _idUsuario);

        // Chama o endpoint da API para criar a playlist
        Call<Void> call = apiService.criarPlaylist(novaPlaylist);
        call.enqueue(new Callback<>() {
            @Override
            public void onResponse(@NonNull Call<Void> call, @NonNull Response<Void> response) {
                // Verifica se a resposta foi bem-sucedida
                if (response.isSuccessful()) {
                    // Exibe mensagem de sucesso e finaliza a activity
                    Toast.makeText(CriarPlaylistActivity.this, "Playlist criada com sucesso!", Toast.LENGTH_SHORT).show();
                    setResult(RESULT_OK); // Informa que a operação foi bem-sucedida
                    finish(); // Fecha a activity
                } else {
                    // Exibe mensagem de erro caso a criação da playlist falhe
                    Toast.makeText(CriarPlaylistActivity.this, "Erro ao criar playlist.", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(@NonNull Call<Void> call, @NonNull Throwable t) {
                // Exibe mensagem de falha na conexão com a API
                Toast.makeText(CriarPlaylistActivity.this, "Falha na conexão.", Toast.LENGTH_SHORT).show();
            }
        });
    }
}
