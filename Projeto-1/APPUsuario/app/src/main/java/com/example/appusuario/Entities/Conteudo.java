package com.example.appusuario.Entities;

import java.io.Serializable;

public class Conteudo implements Serializable {
    // Atributos da classe
    private int id; // Identificador único do conteúdo
    private int curtidas; // Número de curtidas do conteúdo
    private String titulo; // Título do conteúdo
    private String link; // Link associado ao conteúdo (imagem, vídeo, etc.)
    private String criador; // Identificador do criador do conteúdo
    private String criadorNome; // Nome do criador do conteúdo
    private boolean isAdicionado; // Indica se o conteúdo está associado a uma playlist

    // Construtor para inicializar os atributos
    public Conteudo(int id, int curtidas, String titulo, String link, String criador, String criadorNome, boolean isAdicionado) {
        this.id = id;
        this.curtidas = curtidas;
        this.titulo = titulo;
        this.link = link;
        this.criador = criador;
        this.criadorNome = criadorNome;
        this.isAdicionado = isAdicionado;
    }

    // Métodos Getters e Setters

    // Retorna o identificador do conteúdo
    public int getId() {
        return id;
    }

    // Define o identificador do conteúdo
    public void setId(int id) {
        this.id = id;
    }

    // Retorna o número de curtidas do conteúdo
    public int getCurtidas() {
        return curtidas;
    }

    // Define o número de curtidas do conteúdo
    public void setCurtidas(int curtidas) {
        this.curtidas = curtidas;
    }

    // Retorna o título do conteúdo
    public String getTitulo() {
        return titulo;
    }

    // Define o título do conteúdo
    public void setTitulo(String titulo) {
        this.titulo = titulo;
    }

    // Retorna o link associado ao conteúdo
    public String getLink() {
        return link;
    }

    // Define o link associado ao conteúdo
    public void setLink(String link) {
        this.link = link;
    }

    // Retorna o identificador do criador do conteúdo
    public String getCriador() {
        return criador;
    }

    // Define o identificador do criador do conteúdo
    public void setCriador(String criador) {
        this.criador = criador;
    }

    // Retorna o nome do criador do conteúdo
    public String getCriadorNome() {
        return criadorNome;
    }

    // Define o nome do criador do conteúdo
    public void setCriadorNome(String criadorNome) {
        this.criadorNome = criadorNome;
    }

    // Verifica se o conteúdo está adicionado a uma playlist
    public boolean isAdicionado() {
        return isAdicionado;
    }

    // Define se o conteúdo está adicionado a uma playlist
    public void setAdicionado(boolean adicionado) {
        isAdicionado = adicionado;
    }
}
