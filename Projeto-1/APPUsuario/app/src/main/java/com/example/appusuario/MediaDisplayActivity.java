package com.example.appusuario;

import android.annotation.SuppressLint;
import android.net.Uri;
import android.os.Bundle;
import android.view.View;
import android.webkit.WebChromeClient;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.ImageView;
import android.widget.Toast;

import androidx.annotation.OptIn;
import androidx.appcompat.app.AppCompatActivity;
import androidx.media3.common.util.UnstableApi;

import com.bumptech.glide.Glide;

@OptIn(markerClass = UnstableApi.class)
public class MediaDisplayActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.media_display); // Define o layout da atividade

        // Recebe o link de mídia passado através do Intent
        String mediaLink = getIntent().getStringExtra("media_link");

        if (mediaLink != null) {
            // Verifica o tipo de mídia e exibe conforme o tipo (vídeo ou imagem)
            if (isYouTubeUrl(mediaLink)) {
                // Exibe o vídeo do YouTube
                showYouTubeVideo(mediaLink);
            } else if (isImageUrl(mediaLink)) {
                // Exibe a imagem
                showImage(mediaLink);
            } else {
                // Tipo de mídia não suportado
                Toast.makeText(this, "Tipo de mídia não suportado.", Toast.LENGTH_SHORT).show();
                showImage(mediaLink); // Exibe a imagem como fallback
            }
        } else {
            // Caso não seja fornecido nenhum link
            Toast.makeText(this, "Nenhum link fornecido.", Toast.LENGTH_SHORT).show();
            showImage(null); // Exibe imagem nula
        }
    }

    /**
     * Verifica se a URL fornecida é um link do YouTube.
     * @param url URL a ser verificada
     * @return true se for uma URL do YouTube, false caso contrário
     */
    private boolean isYouTubeUrl(String url) {
        return url.contains("youtube.com") || url.contains("youtu.be");
    }

    /**
     * Verifica se a URL fornecida é de uma imagem (jpg, png, jpeg).
     * @param url URL a ser verificada
     * @return true se for uma URL de imagem, false caso contrário
     */
    private boolean isImageUrl(String url) {
        return url != null && (url.contains(".jpg") || url.contains(".png") || url.contains(".jpeg"));
    }

    /**
     * Exibe um vídeo do YouTube na WebView.
     * @param url URL do vídeo do YouTube
     */
    @SuppressLint("SetJavaScriptEnabled")
    private void showYouTubeVideo(String url) {
        WebView webView = findViewById(R.id.web_view);

        // Configurações para a WebView
        WebSettings webSettings = webView.getSettings();
        webSettings.setJavaScriptEnabled(true);  // Habilitar JavaScript
        webSettings.setDomStorageEnabled(true);  // Habilitar armazenamento local
        webSettings.setMediaPlaybackRequiresUserGesture(false);  // Permitir autoplay de vídeos
        webSettings.setMixedContentMode(WebSettings.MIXED_CONTENT_ALWAYS_ALLOW);  // Permitir conteúdo misto (http/https)

        // Define um WebViewClient para tratar o carregamento da página
        webView.setWebViewClient(new WebViewClient() {
            @Override
            public void onPageStarted(WebView view, String url, android.graphics.Bitmap favicon) {
                super.onPageStarted(view, url, favicon);
            }

            @Override
            public void onPageFinished(WebView view, String url) {
                super.onPageFinished(view, url);
            }

            @Override
            public void onReceivedError(WebView view, android.webkit.WebResourceRequest request, android.webkit.WebResourceError error) {
                super.onReceivedError(view, request, error);
                Toast.makeText(MediaDisplayActivity.this, "Erro ao carregar a página.", Toast.LENGTH_SHORT).show();
            }
        });

        // Define um WebChromeClient para controle adicional do JavaScript
        webView.setWebChromeClient(new WebChromeClient() {
            @Override
            public void onProgressChanged(WebView view, int newProgress) {
                super.onProgressChanged(view, newProgress);
            }
        });

        // Extrai o ID do vídeo da URL e carrega o vídeo na WebView
        String videoId = extractVideoIdFromUrl(url);
        if (videoId != null) {
            String embedUrl = "https://www.youtube.com/embed/" + videoId + "?autoplay=1&rel=0";
            webView.loadUrl(embedUrl);
        } else {
            Toast.makeText(this, "Não foi possível processar a URL do vídeo.", Toast.LENGTH_SHORT).show();
        }
    }

    /**
     * Método para extrair o ID do vídeo da URL do YouTube.
     * @param url URL do YouTube
     * @return o ID do vídeo se encontrado, ou null se não for possível extrair
     */
    private String extractVideoIdFromUrl(String url) {
        try {
            Uri uri = Uri.parse(url);
            if (uri.getQueryParameter("v") != null) {
                return uri.getQueryParameter("v");
            } else if (url.contains("youtu.be/")) {
                return url.substring(url.lastIndexOf("/") + 1);
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
        return null;
    }

    /**
     * Exibe uma imagem usando a biblioteca Glide.
     * @param url URL da imagem
     */
    private void showImage(String url) {
        // Esconde a WebView
        WebView webView = findViewById(R.id.web_view);
        webView.setVisibility(View.GONE);

        // Exibe a ImageView
        ImageView imageView = findViewById(R.id.image_view);
        imageView.setVisibility(View.VISIBLE);  // Torna a ImageView visível

        // Carrega a imagem na ImageView com Glide
        Glide.with(this)
                .load(url)
                .placeholder(R.drawable.placeholder)  // Exibe placeholder enquanto a imagem está sendo carregada
                .error(R.drawable.placeholder)        // Exibe placeholder se ocorrer um erro ao carregar a imagem
                .into(imageView);
    }
}
