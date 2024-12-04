package com.example.appusuario.Entities;

public class Usuario {

    // Atributos da classe
    private int id; // Identificador único do usuário
    private String nome; // Nome do usuário
    private String email; // Email do usuário

    // Construtor para inicializar o nome e o email do usuário
    public Usuario(String nome, String email) {
        this.nome = nome;
        this.email = email;
    }

    // Retorna o nome do usuário
    public String getNome() {
        return nome;
    }

    // Define o nome do usuário
    public void setNome(String nome) {
        this.nome = nome;
    }

    // Retorna o email do usuário
    public String getEmail() {
        return email;
    }

    // Define o email do usuário
    public void setEmail(String email) {
        this.email = email;
    }

    // Retorna o identificador do usuário
    public int getId() {
        return id;
    }

    // Define o identificador do usuário
    public void setId(int id) {
        this.id = id;
    }
}
