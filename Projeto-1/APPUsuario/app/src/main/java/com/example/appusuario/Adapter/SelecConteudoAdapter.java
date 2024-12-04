package com.example.appusuario.Adapter;

import android.content.Context;
import android.util.Patterns;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.OptIn;
import androidx.media3.common.util.Log;
import androidx.media3.common.util.UnstableApi;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.example.appusuario.Entities.Conteudo;
import com.example.appusuario.R;
import com.example.appusuario.Service.ApiService;
import com.example.appusuario.Service.RetrofitClient;

import java.util.ArrayList;
import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class SelecConteudoAdapter extends RecyclerView.Adapter<SelecConteudoAdapter.ConteudoViewHolder> {

    // Lista de conteúdos que serão exibidos
    private final List<Conteudo> conteudos;

    // Contexto usado para carregar recursos e mostrar mensagens
    private final Context context;

    // ID da playlist para adicionar ou remover conteúdos
    private final int playlistId;

    // Serviço da API para realizar chamadas de rede
    private final ApiService apiService;

    // Construtor que inicializa os atributos necessários
    public SelecConteudoAdapter(List<Conteudo> conteudos, Context context, int playlistId) {
        this.conteudos = conteudos;
        this.context = context;
        this.playlistId = playlistId;
        this.apiService = RetrofitClient.getRetrofitInstance().create(ApiService.class);
    }

    // Cria a visualização de cada item da lista
    @NonNull
    @Override
    public ConteudoViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_selec_conteudos, parent, false);
        return new ConteudoViewHolder(view);
    }

    // Configura os dados e interações de cada item exibido
    @Override
    public void onBindViewHolder(@NonNull ConteudoViewHolder holder, int position) {
        // Obtém o conteúdo na posição atual
        Conteudo conteudo = conteudos.get(position);

        // Configura os textos e o botão de ação
        holder.txtTitulo.setText(conteudo.getTitulo());
        holder.txtCriador.setText(conteudo.getCriadorNome());
        holder.btnAdicionar.setText(conteudo.isAdicionado() ? "-" : "+");

        // Obtém o link associado ao conteúdo
        String link = conteudo.getLink();

        // Configura a imagem do conteúdo ou um placeholder
        if (isImageLink(link)) {
            Glide.with(context)
                    .load(link)
                    .into(holder.imgBtnImagem);
        } else if (isYouTubeLink(link)) {
            Glide.with(context)
                    .load(getYouTubeThumbnail(link))
                    .into(holder.imgBtnImagem);
        } else {
            Glide.with(context)
                    .load(R.drawable.placeholder)
                    .into(holder.imgBtnImagem);
        }

        // Configura o clique no botão de adicionar/remover
        holder.btnAdicionar.setOnClickListener(v -> {
            if (conteudo.isAdicionado()) {
                removerConteudoDaPlaylist(conteudo, holder);
            } else {
                adicionarConteudoAPlaylist(conteudo, holder);
            }
        });
    }

    // Adiciona um conteúdo à playlist
    private void adicionarConteudoAPlaylist(Conteudo conteudo, ConteudoViewHolder holder) {
        List<Integer> idsConteudos = new ArrayList<>();
        idsConteudos.add(conteudo.getId());

        // Faz a chamada para adicionar o conteúdo
        apiService.adicionarConteudosNaPlaylist(playlistId, idsConteudos).enqueue(new Callback<Void>() {
            @Override
            public void onResponse(@NonNull Call<Void> call, @NonNull Response<Void> response) {
                if (response.isSuccessful()) {
                    // Atualiza o estado do conteúdo para adicionado
                    conteudo.setAdicionado(true);
                    updateButtonState(holder, conteudo);
                } else {
                    Toast.makeText(context, "Erro ao adicionar conteúdo.", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(@NonNull Call<Void> call, @NonNull Throwable t) {
                Toast.makeText(context, "Erro de conexão: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    // Remove um conteúdo da playlist
    @OptIn(markerClass = UnstableApi.class)
    private void removerConteudoDaPlaylist(Conteudo conteudo, ConteudoViewHolder holder) {
        List<Integer> idsConteudos = new ArrayList<>();
        idsConteudos.add(conteudo.getId());

        Log.d("SelecConteudoAdapter", "Removendo conteúdo da playlist...");

        // Faz a chamada para remover o conteúdo
        apiService.removerConteudosDaPlaylist(playlistId, idsConteudos)
                .enqueue(new Callback<Void>() {
                    @Override
                    public void onResponse(Call<Void> call, Response<Void> response) {
                        if (response.isSuccessful()) {
                            // Atualiza o estado do conteúdo para não adicionado
                            conteudo.setAdicionado(false);
                            updateButtonState(holder, conteudo);
                        } else {
                            Toast.makeText(context, "Erro ao remover conteúdo.", Toast.LENGTH_SHORT).show();
                        }
                    }

                    @Override
                    public void onFailure(Call<Void> call, Throwable t) {
                        Toast.makeText(context, "Erro de conexão: " + t.getMessage(), Toast.LENGTH_SHORT).show();
                    }
                });
    }

    // Atualiza o estado do botão de adicionar/remover
    private void updateButtonState(ConteudoViewHolder holder, Conteudo conteudo) {
        int position = holder.getAdapterPosition();
        if (position != RecyclerView.NO_POSITION) {
            notifyItemChanged(position);
        }
    }

    // Retorna o número total de itens na lista
    @Override
    public int getItemCount() {
        return conteudos.size();
    }

    // Verifica se um link é de imagem
    private boolean isImageLink(String link) {
        return link != null && Patterns.WEB_URL.matcher(link).matches() && (
                link.endsWith(".jpg") || link.endsWith(".png") || link.endsWith(".jpeg") ||
                        link.matches(".*\\.(jpg|png|jpeg)\\?.*"));
    }

    // Verifica se um link é do YouTube
    private boolean isYouTubeLink(String link) {
        return link != null && link.contains("youtube.com");
    }

    // Obtém o thumbnail de um vídeo do YouTube
    private String getYouTubeThumbnail(String videoUrl) {
        String videoId = videoUrl.split("v=")[1].split("&")[0];
        return "https://img.youtube.com/vi/" + videoId + "/0.jpg";
    }

    // Classe interna que define a estrutura de cada item na lista
    public static class ConteudoViewHolder extends RecyclerView.ViewHolder {
        private final TextView txtTitulo; // Título do conteúdo
        private final TextView txtCriador; // Nome do criador do conteúdo
        private final Button btnAdicionar; // Botão para adicionar/remover o conteúdo
        private final ImageButton imgBtnImagem; // Botão de imagem do conteúdo

        public ConteudoViewHolder(@NonNull View itemView) {
            super(itemView);
            txtTitulo = itemView.findViewById(R.id.txtTitulo);
            txtCriador = itemView.findViewById(R.id.txtCriador);
            btnAdicionar = itemView.findViewById(R.id.btnAdicionar);
            imgBtnImagem = itemView.findViewById(R.id.btnImagem);
        }
    }
}
