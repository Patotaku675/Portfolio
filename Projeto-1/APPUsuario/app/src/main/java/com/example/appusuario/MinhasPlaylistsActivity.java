package com.example.appusuario;

import android.content.Intent;
import android.os.Bundle;
import android.widget.Button;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.appusuario.Entities.Playlist;
import com.example.appusuario.Adapter.MinhaPlaylistAdapter; // Alterado para usar MinhaPlaylistAdapter
import com.example.appusuario.Service.ApiService;
import com.example.appusuario.Service.RetrofitClient;

import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class MinhasPlaylistsActivity extends AppCompatActivity {

    // Atributos para armazenar o ID do usuário, RecyclerView, Adapter e lista de playlists
    private int _idUsuario;
    private RecyclerView recyclerViewPlaylists;
    private MinhaPlaylistAdapter playlistAdapter; // Adapter para exibir as playlists
    private List<Playlist> playlists; // Lista que vai armazenar as playlists do usuário

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.minhas_playlists); // Define o layout da atividade

        // Obter o ID do usuário da Intent que iniciou esta atividade
        _idUsuario = getIntent().getIntExtra("idUsuario", -1);

        // Configuração do RecyclerView para exibir as playlists
        recyclerViewPlaylists = findViewById(R.id.recyclerViewPlaylists);
        recyclerViewPlaylists.setLayoutManager(new LinearLayoutManager(this));

        // Configuração do botão de criar playlist
        Button btnCriarPlaylist = findViewById(R.id.criarPlaylist);
        btnCriarPlaylist.setOnClickListener(v -> {
            // Ao clicar no botão, abre a atividade para criar uma nova playlist
            Intent intent = new Intent(MinhasPlaylistsActivity.this, CriarPlaylistActivity.class);
            intent.putExtra("idUsuario", _idUsuario); // Passa o ID do usuário para a nova atividade
            startActivity(intent);
        });

        // Configuração do botão de apagar playlist
        Button btnApagarPlaylist = findViewById(R.id.apagarPlaylist);
        btnApagarPlaylist.setOnClickListener(v -> {
            // Ao clicar no botão, abre a atividade para apagar uma playlist
            Intent intent = new Intent(MinhasPlaylistsActivity.this, ApagarPlaylistActivity.class);
            intent.putExtra("idUsuario", _idUsuario); // Passa o ID do usuário para a nova atividade
            startActivity(intent);
        });

        // Verificar se o ID do usuário é válido e carregar as playlists
        if (_idUsuario != -1) {
            obterPlaylistsDoUsuario(_idUsuario); // Chama o método para obter as playlists
        } else {
            // Exibe mensagem de erro se o ID do usuário for inválido
            Toast.makeText(this, "ID de usuário inválido!", Toast.LENGTH_SHORT).show();
        }
    }

    @Override
    protected void onResume() {
        super.onResume();
        // Recarrega as playlists toda vez que a atividade for visível
        if (_idUsuario != -1) {
            obterPlaylistsDoUsuario(_idUsuario); // Chama o método para obter as playlists
        } else {
            Toast.makeText(this, "ID de usuário inválido!", Toast.LENGTH_SHORT).show();
        }
    }

    /**
     * Método que obtém as playlists do usuário através da API.
     * Utiliza Retrofit para fazer uma requisição assíncrona.
     */
    private void obterPlaylistsDoUsuario(int usuarioId) {
        // Instancia o serviço da API
        ApiService apiService = RetrofitClient.getApiService();
        // Cria uma chamada para obter as playlists do usuário
        Call<List<Playlist>> call = apiService.getPlaylistsByUsuarioId(usuarioId);

        // Executa a chamada assíncrona
        call.enqueue(new Callback<List<Playlist>>() {
            @Override
            public void onResponse(@NonNull Call<List<Playlist>> call, @NonNull Response<List<Playlist>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    // Se a resposta for bem-sucedida, armazena as playlists
                    playlists = response.body();

                    // Itera pelas playlists e adiciona o link do primeiro conteúdo, se houver
                    for (Playlist playlist : playlists) {
                        if (playlist.getConteudos() != null && !playlist.getConteudos().isEmpty()) {
                            String firstContentLink = playlist.getConteudos().get(0).getLink();
                            playlist.SetFirstContentLink(firstContentLink);  // Define o link do primeiro conteúdo
                        } else {
                            playlist.SetFirstContentLink(null);  // Caso não tenha conteúdos, define como null
                        }
                    }

                    // Configura o adaptador com a lista de playlists
                    playlistAdapter = new MinhaPlaylistAdapter(MinhasPlaylistsActivity.this, playlists);
                    recyclerViewPlaylists.setAdapter(playlistAdapter); // Define o adaptador para o RecyclerView
                } else {
                    // Caso a resposta seja negativa, exibe uma mensagem de erro
                    Toast.makeText(MinhasPlaylistsActivity.this, "Erro ao carregar playlists", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(@NonNull Call<List<Playlist>> call, @NonNull Throwable t) {
                // Exibe mensagem de erro caso haja falha na requisição
                Toast.makeText(MinhasPlaylistsActivity.this, "Falha na conexão", Toast.LENGTH_SHORT).show();
            }
        });
    }
}
