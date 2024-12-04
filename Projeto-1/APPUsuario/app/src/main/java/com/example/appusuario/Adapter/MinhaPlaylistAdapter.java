package com.example.appusuario.Adapter;

import android.content.Context;
import android.content.Intent;
import android.util.Patterns;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.example.appusuario.Entities.Playlist;
import com.example.appusuario.PlaylistContentsActivity;
import com.example.appusuario.R;
import com.example.appusuario.SelecConteudosActivity;

import java.io.Serializable;
import java.util.List;

public class MinhaPlaylistAdapter extends RecyclerView.Adapter<MinhaPlaylistAdapter.MinhaPlaylistViewHolder> {

    // Contexto da aplicação, necessário para inflar layouts e iniciar atividades
    private final Context context;

    // Lista de playlists a serem exibidas
    private final List<Playlist> playlists;

    // Link do primeiro conteúdo da playlist, usado para exibir a imagem
    private String firstContentLink;

    // Construtor para inicializar contexto e lista de playlists
    public MinhaPlaylistAdapter(Context context, List<Playlist> playlists) {
        this.context = context;
        this.playlists = playlists;
    }

    // Infla o layout do item e cria o ViewHolder
    @NonNull
    @Override
    public MinhaPlaylistViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(context).inflate(R.layout.item_minha_playlist, parent, false);
        return new MinhaPlaylistViewHolder(view);
    }

    // Define os dados de cada item do RecyclerView
    @Override
    public void onBindViewHolder(@NonNull MinhaPlaylistViewHolder holder, int position) {
        Playlist playlist = playlists.get(position);

        // Obtém o link do primeiro conteúdo da playlist
        firstContentLink = playlist.getFirstContentLink();

        // Define o título da playlist
        holder.textViewTitle.setText(playlist.getNome());

        // Verifica o tipo de link (imagem, YouTube ou outro) e exibe a mídia correspondente
        if (isImageLink(firstContentLink)) {
            Glide.with(context)
                    .load(firstContentLink) // URL da imagem
                    .into(holder.imageButtonIcon);
        } else if (isYouTubeLink(firstContentLink)) {
            String thumbnailUrl = getYouTubeThumbnail(firstContentLink);
            Glide.with(context)
                    .load(thumbnailUrl) // Thumbnail do YouTube
                    .into(holder.imageButtonIcon);
        } else {
            Glide.with(context)
                    .load(R.drawable.placeholder) // Placeholder genérico
                    .into(holder.imageButtonIcon);
        }

        // Configura clique no botão de imagem para abrir os conteúdos da playlist
        holder.imageButtonIcon.setOnClickListener(v -> {
            Intent intent = new Intent(context, PlaylistContentsActivity.class);
            intent.putExtra("playlist_conteudos", (Serializable) playlist.getConteudos());
            intent.putExtra("playlistNome", playlist.getNome());
            context.startActivity(intent);
        });

        // Configura clique no botão "Adicionar" para abrir a tela de seleção de conteúdos
        holder.btnAdicionar.setOnClickListener(v -> {
            Intent intent = new Intent(context, SelecConteudosActivity.class);
            intent.putExtra("playlistId", playlist.getId()); // Passa o ID da playlist
            context.startActivity(intent);
        });
    }

    // Retorna o número total de itens na lista
    @Override
    public int getItemCount() {
        return playlists.size();
    }

    // ViewHolder que mantém as referências dos elementos de interface de cada item
    public static class MinhaPlaylistViewHolder extends RecyclerView.ViewHolder {

        // Botão para exibir a imagem ou thumbnail
        ImageButton imageButtonIcon;

        // TextView para exibir o nome da playlist
        TextView textViewTitle;

        // Botão para adicionar conteúdos à playlist
        Button btnAdicionar;

        // Inicializa os elementos de interface do layout
        public MinhaPlaylistViewHolder(@NonNull View itemView) {
            super(itemView);
            imageButtonIcon = itemView.findViewById(R.id.imageButtonIcon);
            textViewTitle = itemView.findViewById(R.id.textViewTitle);
            btnAdicionar = itemView.findViewById(R.id.btnAdicionar);
        }
    }

    // Verifica se o link é uma URL de imagem válida
    private boolean isImageLink(String link) {
        return link != null && Patterns.WEB_URL.matcher(link).matches() && (
                link.endsWith(".jpg") || link.endsWith(".png") || link.endsWith(".jpeg") ||
                        link.matches(".*\\.(jpg|png|jpeg)\\?.*"));
    }

    // Verifica se o link é um vídeo do YouTube
    private boolean isYouTubeLink(String link) {
        return link != null && link.contains("youtube.com");
    }

    // Retorna a URL do thumbnail do YouTube com base na URL do vídeo
    private String getYouTubeThumbnail(String videoUrl) {
        String videoId = videoUrl.split("v=")[1].split("&")[0]; // Extrai o ID do vídeo
        return "https://img.youtube.com/vi/" + videoId + "/0.jpg";
    }
}
