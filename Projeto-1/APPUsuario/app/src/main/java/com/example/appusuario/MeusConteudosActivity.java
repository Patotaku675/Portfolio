package com.example.appusuario;

import android.annotation.SuppressLint;
import android.content.Intent;
import android.os.Bundle;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.appusuario.Entities.Conteudo;
import com.example.appusuario.Adapter.ConteudoAdapter;
import com.example.appusuario.Service.ApiService;
import com.example.appusuario.Service.RetrofitClient;

import java.util.ArrayList;
import java.util.List;
import java.util.Objects;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class MeusConteudosActivity extends AppCompatActivity {

    // Declaração dos atributos
    private ApiService apiService;
    private ConteudoAdapter conteudoAdapter;
    private List<Conteudo> conteudos;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.meus_conteudos);

        // Recuperar o nome do usuário e o ID do usuário passados pela Intent
        String nomeUsuario = getIntent().getStringExtra("nomeUsuario");
        int _idUsuario = getIntent().getIntExtra("idUsuario", -1 );

        // Inicialização dos botões
        Button btnPlaylist = findViewById(R.id.playlist);
        Button btnMinhasPlaylist = findViewById(R.id.minhaPlaylistVer);

        // Inicializar Retrofit e ApiService para fazer requisições à API
        apiService = RetrofitClient.getRetrofitInstance().create(ApiService.class);

        // Inicializar RecyclerView para exibir os conteúdos
        RecyclerView recyclerView = findViewById(R.id.recyclerViewConteudos);
        recyclerView.setLayoutManager(new LinearLayoutManager(this));
        conteudos = new ArrayList<>();

        // Inicializar o adaptador para o RecyclerView
        conteudoAdapter = new ConteudoAdapter(conteudos, this);
        recyclerView.setAdapter(conteudoAdapter);

        // Configuração do TextView para mostrar o nome do usuário
        TextView usuarioTextView = findViewById(R.id.meuUsuario);
        usuarioTextView.setText(Objects.requireNonNullElse(nomeUsuario, "Usuário não encontrado"));

        // Configuração do clique do botão para navegar até PlaylistsActivity
        btnPlaylist.setOnClickListener(v -> {
            Intent intent = new Intent(MeusConteudosActivity.this, PlaylistsActivity.class);
            intent.putExtra("nomeUsuario", nomeUsuario);
            startActivity(intent); // Inicia a nova atividade
        });

        // Configuração do clique do botão para navegar até MinhasPlaylistsActivity
        btnMinhasPlaylist.setOnClickListener(v -> {
            Intent intent = new Intent(MeusConteudosActivity.this, MinhasPlaylistsActivity.class);
            intent.putExtra("idUsuario", _idUsuario);
            startActivity(intent); // Inicia a nova atividade
        });

        // Chama o método para carregar os conteúdos
        carregarConteudos();
    }

    /**
     * Método responsável por carregar os conteúdos da API.
     * Utiliza Retrofit para fazer uma requisição assíncrona.
     */
    private void carregarConteudos() {
        // Chama o método getAllConteudos() da ApiService para obter todos os conteúdos
        apiService.getAllConteudos().enqueue(new Callback<List<Conteudo>>() {
            @SuppressLint("NotifyDataSetChanged")
            @Override
            public void onResponse(@NonNull Call<List<Conteudo>> call, @NonNull Response<List<Conteudo>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    // Caso a resposta seja bem-sucedida, atualiza a lista de conteúdos
                    List<Conteudo> conteudosList = response.body();
                    conteudos.clear(); // Limpa a lista antes de adicionar novos dados
                    conteudos.addAll(conteudosList); // Adiciona os conteúdos recebidos
                    conteudoAdapter.notifyDataSetChanged(); // Notifica o adaptador sobre as alterações
                } else {
                    // Exibe mensagem caso não encontre conteúdos
                    Toast.makeText(MeusConteudosActivity.this, "Nenhum conteúdo encontrado.", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(@NonNull Call<List<Conteudo>> call, @NonNull Throwable t) {
                // Exibe erro caso a requisição falhe
                Toast.makeText(MeusConteudosActivity.this, "Erro ao carregar conteúdos: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }
}
