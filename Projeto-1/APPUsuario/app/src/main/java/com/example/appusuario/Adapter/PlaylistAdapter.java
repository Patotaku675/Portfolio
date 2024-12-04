package com.example.appusuario.Adapter;

import android.content.Context;
import android.content.Intent;
import android.util.Patterns;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageButton;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.example.appusuario.Entities.Playlist;
import com.example.appusuario.PlaylistContentsActivity;
import com.example.appusuario.R;

import java.io.Serializable;
import java.util.List;

public class PlaylistAdapter extends RecyclerView.Adapter<PlaylistAdapter.PlaylistViewHolder> {

    // Contexto necessário para inflar layouts e abrir novas atividades
    private final Context context;

    // Lista de playlists que serão exibidas no RecyclerView
    private final List<Playlist> playlists;

    // Armazena o link do primeiro conteúdo da playlist
    private String firstContentLink;

    // Construtor para inicializar o contexto e a lista de playlists
    public PlaylistAdapter(Context context, List<Playlist> playlists) {
        this.context = context;
        this.playlists = playlists;
    }

    // Infla o layout de cada item e retorna o ViewHolder
    @NonNull
    @Override
    public PlaylistViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(context).inflate(R.layout.item_playlist, parent, false);
        return new PlaylistViewHolder(view);
    }

    // Define os dados que serão exibidos em cada item do RecyclerView
    @Override
    public void onBindViewHolder(@NonNull PlaylistViewHolder holder, int position) {
        // Obtém a playlist na posição atual
        Playlist playlist = playlists.get(position);

        // Obtém o link do primeiro conteúdo da playlist
        firstContentLink = playlist.getFirstContentLink();

        // Define o título da playlist
        holder.textViewTitle.setText(playlist.getNome());

        // Verifica o tipo de link e carrega a mídia correspondente
        if (isImageLink(firstContentLink)) {
            // Caso seja um link de imagem, exibe a imagem
            Glide.with(context)
                    .load(firstContentLink)
                    .into(holder.imageButtonIcon);
        } else if (isYouTubeLink(firstContentLink)) {
            // Caso seja um link de YouTube, carrega o thumbnail
            String thumbnailUrl = getYouTubeThumbnail(firstContentLink);
            Glide.with(context)
                    .load(thumbnailUrl)
                    .into(holder.imageButtonIcon);
        } else {
            // Caso não seja imagem nem YouTube, exibe um placeholder
            Glide.with(context)
                    .load(R.drawable.placeholder)
                    .into(holder.imageButtonIcon);
        }

        // Configura o clique no botão de imagem para abrir os conteúdos da playlist
        holder.imageButtonIcon.setOnClickListener(v -> {
            Intent intent = new Intent(context, PlaylistContentsActivity.class);

            // Passa os conteúdos e o nome da playlist para a próxima atividade
            intent.putExtra("playlist_conteudos", (Serializable) playlist.getConteudos());
            intent.putExtra("playlistNome", playlist.getNome());
            context.startActivity(intent);
        });
    }

    // Retorna o número total de itens no RecyclerView
    @Override
    public int getItemCount() {
        return playlists.size();
    }

    // Classe interna que gerencia os elementos de interface de cada item
    public static class PlaylistViewHolder extends RecyclerView.ViewHolder {

        // Botão que exibe a imagem ou o thumbnail da playlist
        ImageButton imageButtonIcon;

        // Texto que exibe o nome da playlist
        TextView textViewTitle;

        // Construtor que inicializa os elementos do layout
        public PlaylistViewHolder(@NonNull View itemView) {
            super(itemView);
            imageButtonIcon = itemView.findViewById(R.id.imageButtonIcon);
            textViewTitle = itemView.findViewById(R.id.textViewTitle);
        }
    }

    // Verifica se o link é uma URL válida de imagem
    private boolean isImageLink(String link) {
        return link != null && Patterns.WEB_URL.matcher(link).matches() && (
                link.endsWith(".jpg") || link.endsWith(".png") || link.endsWith(".jpeg") ||
                        link.matches(".*\\.(jpg|png|jpeg)\\?.*"));
    }

    // Verifica se o link é um URL de YouTube
    private boolean isYouTubeLink(String link) {
        return link != null && link.contains("youtube.com");
    }

    // Retorna a URL do thumbnail de um vídeo do YouTube
    private String getYouTubeThumbnail(String videoUrl) {
        // Extrai o ID do vídeo a partir da URL
        String videoId = videoUrl.split("v=")[1].split("&")[0];
        return "https://img.youtube.com/vi/" + videoId + "/0.jpg";
    }
}
