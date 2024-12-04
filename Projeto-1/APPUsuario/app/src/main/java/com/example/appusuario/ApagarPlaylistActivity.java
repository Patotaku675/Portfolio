package com.example.appusuario;

import android.os.Bundle;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;

import com.example.appusuario.Entities.Playlist;
import com.example.appusuario.Service.ApiService;
import com.example.appusuario.Service.RetrofitClient;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ApagarPlaylistActivity extends AppCompatActivity {

    // Variável para armazenar o ID do usuário
    private int _idUsuario;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.apagar_playlist); // Define o layout da activity

        // Recupera o ID do usuário passado pela Intent
        _idUsuario = getIntent().getIntExtra("idUsuario", -1);

        // Inicializa os componentes de UI
        EditText edtNomePlaylist = findViewById(R.id.edtNomePlaylist);
        Button btnApagar = findViewById(R.id.btnApagar);

        // Configura o clique do botão para apagar a playlist
        btnApagar.setOnClickListener(v -> {
            String nomePlaylist = edtNomePlaylist.getText().toString().trim();
            if (!nomePlaylist.isEmpty()) {
                // Verifica e apaga a playlist se o nome for válido
                verificarEApagarPlaylist(nomePlaylist);
            } else {
                // Exibe mensagem caso o nome da playlist não tenha sido preenchido
                Toast.makeText(this, "Preencha o nome da playlist!", Toast.LENGTH_SHORT).show();
            }
        });
    }

    /**
     * Método que verifica a existência da playlist e a autorização do usuário para apagá-la
     * @param nomePlaylist Nome da playlist a ser apagada
     */
    private void verificarEApagarPlaylist(String nomePlaylist) {
        // Obtém a instância do serviço da API
        ApiService apiService = RetrofitClient.getApiService();

        // Chama o endpoint para buscar a playlist pelo nome
        Call<Playlist> call = apiService.getPlaylistsByNome(nomePlaylist);
        call.enqueue(new Callback<>() {
            @Override
            public void onResponse(@NonNull Call<Playlist> call, @NonNull Response<Playlist> response) {
                // Se a resposta for bem-sucedida e a playlist for encontrada
                if (response.isSuccessful() && response.body() != null) {
                    Playlist playlistEncontrada = response.body();

                    // Verifica se o ID do usuário corresponde ao dono da playlist
                    if (playlistEncontrada.getUsuarioId() == _idUsuario) {
                        // Exibe a confirmação para apagar a playlist
                        exibirConfirmacao(playlistEncontrada);
                    } else {
                        // Se o usuário não for o dono, exibe mensagem de erro
                        Toast.makeText(ApagarPlaylistActivity.this, "Você não tem permissão para apagar esta playlist.", Toast.LENGTH_SHORT).show();
                    }
                } else {
                    // Se a playlist não for encontrada
                    Toast.makeText(ApagarPlaylistActivity.this, "Playlist não encontrada.", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(@NonNull Call<Playlist> call, @NonNull Throwable t) {
                // Exibe mensagem de falha na conexão
                Toast.makeText(ApagarPlaylistActivity.this, "Falha na conexão.", Toast.LENGTH_SHORT).show();
            }
        });
    }

    /**
     * Exibe um diálogo de confirmação para o usuário antes de apagar a playlist
     * @param playlist A playlist que será apagada
     */
    private void exibirConfirmacao(Playlist playlist) {
        // Cria e exibe uma caixa de diálogo de confirmação
        new AlertDialog.Builder(this)
                .setTitle("Apagar Playlist")
                .setMessage("Deseja apagar a playlist: " + playlist.getNome() + "?")
                .setPositiveButton("Sim", (dialog, which) -> deletarPlaylist(playlist.getId()))  // Ao clicar em "Sim", chama o método para deletar
                .setNegativeButton("Não", null)  // Se clicar em "Não", não faz nada
                .show();
    }

    /**
     * Método que realiza a exclusão da playlist
     * @param playlistId ID da playlist a ser apagada
     */
    private void deletarPlaylist(int playlistId) {
        // Obtém a instância do serviço da API
        ApiService apiService = RetrofitClient.getApiService();

        // Chama o endpoint para deletar a playlist
        Call<Void> call = apiService.deletarPlaylist(playlistId);
        call.enqueue(new Callback<>() {
            @Override
            public void onResponse(@NonNull Call<Void> call, @NonNull Response<Void> response) {
                // Se a resposta for bem-sucedida
                if (response.isSuccessful()) {
                    // Exibe mensagem de sucesso e fecha a activity
                    Toast.makeText(ApagarPlaylistActivity.this, "Playlist apagada com sucesso!", Toast.LENGTH_SHORT).show();
                    finish();
                } else {
                    // Se ocorrer um erro ao apagar a playlist
                    Toast.makeText(ApagarPlaylistActivity.this, "Erro ao apagar playlist.", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(@NonNull Call<Void> call, @NonNull Throwable t) {
                // Exibe mensagem de falha na conexão
                Toast.makeText(ApagarPlaylistActivity.this, "Falha na conexão.", Toast.LENGTH_SHORT).show();
            }
        });
    }
}
