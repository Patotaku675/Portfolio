package com.example.appusuario.Entities;

import java.util.List;

public class PlaylistResponse {

    // Atributos da classe
    private int id; // Identificador único da playlist
    private String nome; // Nome da playlist
    private int usuarioId; // Identificador do usuário que criou a playlist
    private String usuarioNome; // Nome do usuário que criou a playlist
    private List<Conteudo> conteudos; // Lista de conteúdos associados à playlist

    // Métodos Getters e Setters

    // Retorna o identificador da playlist
    public int getId() {
        return id;
    }

    // Define o identificador da playlist
    public void setId(int id) {
        this.id = id;
    }

    // Retorna o nome da playlist
    public String getNome() {
        return nome;
    }

    // Define o nome da playlist
    public void setNome(String nome) {
        this.nome = nome;
    }

    // Retorna o identificador do usuário associado à playlist
    public int getUsuarioId() {
        return usuarioId;
    }

    // Define o identificador do usuário associado à playlist
    public void setUsuarioId(int usuarioId) {
        this.usuarioId = usuarioId;
    }

    // Retorna o nome do usuário associado à playlist
    public String getUsuarioNome() {
        return usuarioNome;
    }

    // Define o nome do usuário associado à playlist
    public void setUsuarioNome(String usuarioNome) {
        this.usuarioNome = usuarioNome;
    }

    // Retorna a lista de conteúdos associados à playlist
    public List<Conteudo> getConteudos() {
        return conteudos;
    }

    // Define a lista de conteúdos associados à playlist
    public void setConteudos(List<Conteudo> conteudos) {
        this.conteudos = conteudos;
    }
}
