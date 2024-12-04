package com.example.appusuario.Service;

import com.example.appusuario.Entities.Conteudo;
import com.example.appusuario.Entities.Playlist;
import com.example.appusuario.Entities.PlaylistResponse;
import com.example.appusuario.Entities.Usuario;

import java.util.List;

import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.DELETE;
import retrofit2.http.GET;
import retrofit2.http.POST;
import retrofit2.http.Path;
import retrofit2.http.Query;

public interface ApiService {

    // Endpoint para criar um usuário
    @POST("Usuario")
    Call<Void> criarUsuario(@Body Usuario usuario);

    // Endpoint para obter todos os usuários
    @GET("Usuario")
    Call<List<Usuario>> getUsuarios();

    // Endpoint para obter todos os conteúdos disponíveis
    @GET("Conteudo")
    Call<List<Conteudo>> getAllConteudos();

    // Endpoint para curtir um conteúdo específico pelo ID
    @POST("Conteudo/{conteudoId}/curtir")
    Call<Void> curtirConteudo(@Path("conteudoId") int conteudoId);

    // Endpoint para obter todas as playlists
    @GET("Playlists")
    Call<List<Playlist>> getAllPlaylists();

    // Endpoint para obter as playlists de um usuário específico pelo ID
    @GET("Playlists/usuario/{usuarioId}")
    Call<List<Playlist>> getPlaylistsByUsuarioId(@Path("usuarioId") int usuarioId);

    // Endpoint para obter uma playlist específica pelo ID
    @GET("playlists/{id}")
    Call<PlaylistResponse> getPlaylistsById(@Path("id") int id);

    // Endpoint para obter uma playlist pelo nome
    @GET("Playlists/nome/{nome}")
    Call<Playlist> getPlaylistsByNome(@Path("nome") String nome);

    // Endpoint para criar uma nova playlist
    @POST("Playlists")
    Call<Void> criarPlaylist(@Body Playlist playlist);

    // Endpoint para excluir uma playlist específica pelo ID
    @DELETE("Playlists/{playlistId}")
    Call<Void> deletarPlaylist(@Path("playlistId") int playlistId);

    // Endpoint para adicionar conteúdos a uma playlist
    @POST("Playlists/{playlistId}/conteudos")
    Call<Void> adicionarConteudosNaPlaylist(
            @Path("playlistId") int playlistId,
            @Query("conteudosIds") List<Integer> conteudosIds);


    // Endpoint para remover conteúdos de uma playlist
    @DELETE("Playlists/{playlistId}/conteudos")
    Call<Void> removerConteudosDaPlaylist(
            @Path("playlistId") int playlistId,
            @Query("conteudosIds") List<Integer> conteudosIds);
}
