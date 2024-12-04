package com.example.appusuario;

import android.annotation.SuppressLint;
import android.os.Bundle;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.appusuario.Adapter.SelecConteudoAdapter;
import com.example.appusuario.Entities.Conteudo;
import com.example.appusuario.Entities.PlaylistResponse;
import com.example.appusuario.Service.ApiService;
import com.example.appusuario.Service.RetrofitClient;

import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class SelecConteudosActivity extends AppCompatActivity {

    private SelecConteudoAdapter selecConteudoAdapter;  // Adaptador para exibir os conteúdos
    private List<Conteudo> conteudos;  // Lista de conteúdos para ser exibida no RecyclerView
    private Set<Integer> idsConteudosNaPlaylist;  // Conjunto para armazenar os IDs dos conteúdos que já estão na playlist

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.selec_conteudos);  // Define o layout da atividade

        // Inicializa o RecyclerView e configura seu layout
        RecyclerView recyclerView = findViewById(R.id.recyclerViewConteudos);
        recyclerView.setLayoutManager(new LinearLayoutManager(this));
        conteudos = new ArrayList<>();  // Inicializa a lista de conteúdos
        idsConteudosNaPlaylist = new HashSet<>();  // Inicializa o conjunto de IDs dos conteúdos

        // Inicializa o adaptador com a lista de conteúdos e o ID da playlist
        int playlistId = getIntent().getIntExtra("playlistId", -1);  // Recupera o ID da playlist passado pela Intent
        selecConteudoAdapter = new SelecConteudoAdapter(conteudos, this, playlistId);
        recyclerView.setAdapter(selecConteudoAdapter);  // Configura o adaptador no RecyclerView

        // Chama o método para carregar os conteúdos da playlist
        carregarConteudos(playlistId);
    }

    // Método para carregar os conteúdos da playlist a partir da API
    private void carregarConteudos(int playlistId) {
        ApiService apiService = RetrofitClient.getRetrofitInstance().create(ApiService.class);

        // Requisição para carregar os conteúdos da playlist específica
        apiService.getPlaylistsById(playlistId).enqueue(new Callback<>() {
            @Override
            public void onResponse(@NonNull Call<PlaylistResponse> call, @NonNull Response<PlaylistResponse> response) {
                if (response.isSuccessful() && response.body() != null) {
                    PlaylistResponse playlist = response.body();  // Obtém a resposta com a playlist
                    for (Conteudo conteudo : playlist.getConteudos()) {
                        // Adiciona os IDs dos conteúdos da playlist no conjunto
                        idsConteudosNaPlaylist.add(conteudo.getId());
                    }

                    // Após carregar os conteúdos da playlist, carrega todos os conteúdos gerais
                    carregarTodosConteudos();
                } else {
                    Toast.makeText(SelecConteudosActivity.this, "Erro ao carregar playlist.", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(@NonNull Call<PlaylistResponse> call, @NonNull Throwable t) {
                // Exibe mensagem de erro caso a requisição falhe
                Toast.makeText(SelecConteudosActivity.this, "Erro ao carregar playlist: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    // Método para carregar todos os conteúdos disponíveis a partir da API
    private void carregarTodosConteudos() {
        ApiService apiService = RetrofitClient.getRetrofitInstance().create(ApiService.class);

        // Requisição para carregar todos os conteúdos disponíveis
        apiService.getAllConteudos().enqueue(new Callback<>() {
            @SuppressLint("NotifyDataSetChanged")
            @Override
            public void onResponse(@NonNull Call<List<Conteudo>> call, @NonNull Response<List<Conteudo>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    conteudos.clear();  // Limpa a lista de conteúdos para adicionar os novos

                    // Itera sobre todos os conteúdos e verifica se já pertencem à playlist
                    for (Conteudo conteudo : response.body()) {
                        conteudo.setAdicionado(idsConteudosNaPlaylist.contains(conteudo.getId()));  // Marca se o conteúdo já foi adicionado à playlist
                        conteudos.add(conteudo);  // Adiciona o conteúdo à lista
                    }
                    selecConteudoAdapter.notifyDataSetChanged();  // Notifica o adaptador para atualizar a UI
                } else {
                    Toast.makeText(SelecConteudosActivity.this, "Nenhum conteúdo encontrado.", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(@NonNull Call<List<Conteudo>> call, @NonNull Throwable t) {
                // Exibe mensagem de erro caso a requisição falhe
                Toast.makeText(SelecConteudosActivity.this, "Erro ao carregar conteúdos: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }
}
