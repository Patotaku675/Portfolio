package com.example.appusuario;

import android.os.Bundle;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.appusuario.Adapter.ConteudoAdapter;
import com.example.appusuario.Entities.Conteudo;

import java.util.List;
import java.util.Objects;

public class PlaylistContentsActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.conteudos_playlist);  // Define o layout da atividade para exibir os conteúdos da playlist

        // Inicializa o RecyclerView para exibir os conteúdos da playlist
        RecyclerView recyclerViewConteudos = findViewById(R.id.recyclerViewConteudos);
        recyclerViewConteudos.setLayoutManager(new LinearLayoutManager(this));  // Configura o RecyclerView com layout de lista linear

        // Recupera a lista de conteúdos passada pela Intent
        List<Conteudo> conteudos = (List<Conteudo>) getIntent().getSerializableExtra("playlist_conteudos");

        // Recupera o nome da playlist passado pela Intent
        String _playlistnome = getIntent().getStringExtra("playlistNome");

        // Configura o TextView para exibir o nome da playlist
        TextView usuarioTextView = findViewById(R.id.nomePlaylist);
        usuarioTextView.setText(Objects.requireNonNullElse(_playlistnome, "Playlist")); // Define o nome da playlist ou "Playlist" caso o nome seja nulo

        // Verifica se a lista de conteúdos não é nula
        if (conteudos != null) {
            // Se houver conteúdos, configura o adaptador para exibir os conteúdos no RecyclerView
            ConteudoAdapter adapter = new ConteudoAdapter(conteudos, this);
            recyclerViewConteudos.setAdapter(adapter);  // Define o adaptador para o RecyclerView
        } else {
            // Se não houver conteúdos, exibe uma mensagem informando ao usuário
            Toast.makeText(this, "Nenhum conteúdo encontrado.", Toast.LENGTH_SHORT).show();
        }
    }

}
