package com.example.appusuario.Entities;

import java.util.List;

public class Playlist {

    // Atributos da classe
    private int id; // Identificador único da playlist
    private String nome; // Nome da playlist
    private int usuarioId; // Identificador do usuário que criou a playlist
    private List<Conteudo> conteudos; // Lista de conteúdos associados à playlist
    private String FirstContentLink; // Link do primeiro conteúdo da playlist (opcional)

    // Construtores

    // Construtor vazio (usado quando não há necessidade de inicializar atributos imediatamente)
    public Playlist() {}

    // Construtor para inicializar a playlist com ID, nome e lista de conteúdos
    public Playlist(int id, String nome, List<Conteudo> conteudos) {
        this.id = id;
        this.nome = nome;
        this.conteudos = conteudos;
    }

    // Construtor para inicializar a playlist com nome e ID do usuário
    public Playlist(String nome, int usuarioId) {
        this.nome = nome;
        this.usuarioId = usuarioId;
    }

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

    // Retorna o link do primeiro conteúdo da playlist
    public String getFirstContentLink() {
        return FirstContentLink;
    }

    // Define o link do primeiro conteúdo da playlist
    public void SetFirstContentLink(String setFirstContentLink) {
        this.FirstContentLink = setFirstContentLink;
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
