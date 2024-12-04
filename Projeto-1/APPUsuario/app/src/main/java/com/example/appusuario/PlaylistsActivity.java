package com.example.appusuario;

import android.os.Bundle;
import android.util.Log;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.appusuario.Adapter.PlaylistAdapter;
import com.example.appusuario.Entities.Playlist;
import com.example.appusuario.Service.ApiService;
import com.example.appusuario.Service.RetrofitClient;

import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class PlaylistsActivity extends AppCompatActivity {

    private RecyclerView recyclerViewPlaylists;  // RecyclerView para exibir as playlists

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.all_playlists);  // Define o layout da atividade para exibir todas as playlists

        // Inicializa o RecyclerView para exibir as playlists
        recyclerViewPlaylists = findViewById(R.id.recyclerViewPlaylists);
        recyclerViewPlaylists.setLayoutManager(new LinearLayoutManager(this));  // Configura o layout do RecyclerView

        // Chama o método para carregar as playlists da API
        loadPlaylistsFromApi();
    }

    // Método para carregar as playlists a partir da API
    private void loadPlaylistsFromApi() {
        ApiService apiService = RetrofitClient.getApiService();  // Obtém a instância do serviço API

        // Faz uma requisição assíncrona para obter todas as playlists
        apiService.getAllPlaylists().enqueue(new Callback<>() {
            @Override
            public void onResponse(@NonNull Call<List<Playlist>> call, @NonNull Response<List<Playlist>> response) {
                // Verifica se a resposta foi bem-sucedida e se a lista de playlists não é nula
                if (response.isSuccessful() && response.body() != null) {
                    List<Playlist> playlists = response.body();  // Obtém a lista de playlists

                    // Itera por cada playlist e obtém o link do primeiro conteúdo, se houver
                    for (Playlist playlist : playlists) {
                        if (playlist.getConteudos() != null && !playlist.getConteudos().isEmpty()) {
                            String firstContentLink = playlist.getConteudos().get(0).getLink();  // Pega o link do primeiro conteúdo
                            playlist.SetFirstContentLink(firstContentLink);  // Armazena o link do primeiro conteúdo na playlist
                        } else {
                            playlist.SetFirstContentLink(null);  // Caso não tenha conteúdos, seta o link como null
                        }
                    }

                    // Cria o adaptador para o RecyclerView com a lista de playlists e o configura
                    PlaylistAdapter adapter = new PlaylistAdapter(PlaylistsActivity.this, playlists);
                    recyclerViewPlaylists.setAdapter(adapter);  // Define o adaptador no RecyclerView
                } else {
                    // Exibe uma mensagem de erro se a requisição falhar ou se a resposta não for válida
                    Toast.makeText(PlaylistsActivity.this, "Erro ao carregar playlists", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(@NonNull Call<List<Playlist>> call, @NonNull Throwable t) {
                // Exibe uma mensagem de erro em caso de falha na requisição
                Toast.makeText(PlaylistsActivity.this, "Falha na conexão", Toast.LENGTH_SHORT).show();
                Log.e("PlaylistsActivity", "Erro na requisição da API", t);  // Registra o erro no log
            }
        });
    }
}
