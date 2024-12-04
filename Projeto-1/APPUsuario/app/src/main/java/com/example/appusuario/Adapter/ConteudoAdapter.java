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
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.example.appusuario.Entities.Conteudo;
import com.example.appusuario.MediaDisplayActivity;
import com.example.appusuario.R;
import com.example.appusuario.Service.ApiService;
import com.example.appusuario.Service.RetrofitClient;

import java.util.List;

import retrofit2.Call;
import retrofit2.Response;

public class ConteudoAdapter extends RecyclerView.Adapter<ConteudoAdapter.ConteudoViewHolder> {

    // Lista de conteúdos a serem exibidos no RecyclerView
    private final List<Conteudo> conteudos;

    // Contexto da aplicação, necessário para inflar layouts e iniciar atividades
    private final Context context;

    // Construtor que inicializa os dados do adaptador
    public ConteudoAdapter(List<Conteudo> conteudos, Context context) {
        this.conteudos = conteudos;
        this.context = context;
    }

    // Infla o layout do item e cria o ViewHolder
    @NonNull
    @Override
    public ConteudoViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(context).inflate(R.layout.item_conteudos, parent, false);
        return new ConteudoViewHolder(view);
    }

    // Define os dados de cada item do RecyclerView
    @Override
    public void onBindViewHolder(@NonNull ConteudoViewHolder holder, int position) {
        Conteudo conteudo = conteudos.get(position);

        // Define os textos com as informações do conteúdo
        holder.tvTitulo.setText(conteudo.getTitulo());
        holder.tvCriador.setText("Criador: " + conteudo.getCriadorNome());
        holder.tvCurtidas.setText("Likes: " + conteudo.getCurtidas());

        String link = conteudo.getLink();

        // Verifica o tipo de link (imagem, YouTube ou outro) e exibe a mídia correspondente
        if (isImageLink(link)) {
            // Exibe diretamente a imagem no botão
            Glide.with(context)
                    .load(link)
                    .into(holder.imgBtnImagem);
        } else if (isYouTubeLink(link)) {
            // Exibe o thumbnail do vídeo do YouTube
            String thumbnailUrl = getYouTubeThumbnail(link);
            Glide.with(context)
                    .load(thumbnailUrl)
                    .into(holder.imgBtnImagem);
        } else {
            // Exibe um placeholder para outros tipos de links
            Glide.with(context)
                    .load(R.drawable.placeholder)
                    .into(holder.imgBtnImagem);
        }

        // Configura o clique no botão de imagem para abrir a atividade de exibição da mídia
        holder.imgBtnImagem.setOnClickListener(v -> {
            String _link = conteudo.getLink();
            Intent intent = new Intent(context, MediaDisplayActivity.class);
            intent.putExtra("media_link", _link);
            context.startActivity(intent);
        });

        // Configura o clique no botão '+' para curtir o conteúdo
        holder.btnAdicionar.setOnClickListener(v -> {
            int conteudoId = conteudo.getId();

            // Obter a instância da ApiService
            ApiService apiService = RetrofitClient.getApiService();

            // Envia a requisição para curtir o conteúdo
            apiService.curtirConteudo(conteudoId).enqueue(new retrofit2.Callback<Void>() {
                @Override
                public void onResponse(Call<Void> call, Response<Void> response) {
                    if (response.isSuccessful()) {
                        // Atualiza a contagem de curtidas e exibe um feedback
                        Toast.makeText(context, "Conteúdo curtido com sucesso!", Toast.LENGTH_SHORT).show();
                        conteudo.setCurtidas(conteudo.getCurtidas() + 1);

                        // Atualiza o item na lista de forma eficiente
                        int updatedPosition = holder.getAdapterPosition();
                        if (updatedPosition != RecyclerView.NO_POSITION) {
                            notifyItemChanged(updatedPosition);
                        }
                    } else {
                        // Exibe mensagem de erro caso a requisição falhe
                        Toast.makeText(context, "Erro ao curtir conteúdo", Toast.LENGTH_SHORT).show();
                    }
                }

                @Override
                public void onFailure(Call<Void> call, Throwable t) {
                    // Exibe mensagem de falha na conexão
                    Toast.makeText(context, "Falha na conexão", Toast.LENGTH_SHORT).show();
                }
            });
        });
    }

    // Retorna o número total de itens na lista
    @Override
    public int getItemCount() {
        return conteudos.size();
    }

    // ViewHolder que mantém as referências dos elementos de interface de cada item
    public static class ConteudoViewHolder extends RecyclerView.ViewHolder {

        TextView tvTitulo, tvCriador, tvCurtidas; // TextViews para exibir informações do conteúdo
        ImageButton imgBtnImagem; // Botão para exibir a imagem ou thumbnail
        Button btnAdicionar; // Botão para curtir o conteúdo

        public ConteudoViewHolder(@NonNull View itemView) {
            super(itemView);

            // Inicializa os elementos da interface
            tvTitulo = itemView.findViewById(R.id.txtTitulo);
            tvCriador = itemView.findViewById(R.id.txtCriador);
            tvCurtidas = itemView.findViewById(R.id.txtCurtidas);
            imgBtnImagem = itemView.findViewById(R.id.btnImagem);
            btnAdicionar = itemView.findViewById(R.id.btnAdicionar);
        }
    }

    // Verifica se o link é uma URL de imagem válida
    private boolean isImageLink(String link) {
        return link != null && Patterns.WEB_URL.matcher(link).matches() && (
                link.endsWith(".jpg") || link.endsWith(".png") || link.endsWith(".jpeg") ||
                        link.matches(".*\\.(jpg|png|jpeg)\\?.*"));
    }

    // Verifica se o link é de um vídeo do YouTube
    private boolean isYouTubeLink(String link) {
        return link != null && link.contains("youtube.com");
    }

    // Gera a URL do thumbnail de um vídeo do YouTube com base na URL do vídeo
    private String getYouTubeThumbnail(String videoUrl) {
        String videoId = videoUrl.split("v=")[1].split("&")[0];
        return "https://img.youtube.com/vi/" + videoId + "/0.jpg";
    }
}
