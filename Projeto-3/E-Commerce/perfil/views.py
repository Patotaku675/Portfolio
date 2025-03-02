from django.shortcuts import render, get_object_or_404, redirect
from django.views.generic.list import ListView
from django.views import View
from django.http import HttpResponse
from django.contrib.auth.models import User
import copy
from django.contrib.auth import authenticate, login, logout
from django.contrib import messages

from .models import Perfil
from . import forms

class BasePerfil(View):
    template_name = 'perfil/criar.html'
    
    
    
    def setup(self, *args, **kwargs):
        super().setup(*args, **kwargs)
        
        self.carrinho = copy.deepcopy(self.request.session.get('carrinho, {}'))
        
        self.perfil = None
        
        if self.request.user.is_authenticated:
            self.perfil = Perfil.objects.filter(usuario=self.request.user) \
                .first()
            
            self.contexto = {
                'userform': forms.UserForm(
                    data=self.request.POST or None,
                    usuario=self.request.user,
                    instance=self.request.user,
                ),
                'perfilform': forms.PerfilForm(
                    data=self.request.POST or None,
                    instance=self.perfil,
                )
            }
        else:
            self.contexto = {
                'userform': forms.UserForm(
                    data=self.request.POST or None
                ),
                'perfilform': forms.PerfilForm(
                    data=self.request.POST or None
                )
            }
            
        self.userform = self.contexto['userform']
        self.perfilform = self.contexto['perfilform']
        
        if self.request.user.is_authenticated:
            self.template_name = 'perfil/atualizar.html'
        
        self.renderizar = render(
            self.request, self.template_name, self.contexto)
    
    def get(self, *args, **kwargs):
        return self.renderizar
    
class Criar(BasePerfil):
    def post(self, *args, **kwargs):
        if not self.userform.is_valid() or not self.perfilform.is_valid():
            messages.error(
                self.request,
                'Existem erros no formulário de cadastro.'
            )
            return self.renderizar
        
        username = self.userform.cleaned_data.get('username')
        password = self.userform.cleaned_data.get('password')
        email = self.userform.cleaned_data.get('email')
        first_name = self.userform.cleaned_data.get('first_name')
        last_name = self.userform.cleaned_data.get('last_name')
        
        if self.request.user.is_authenticated:
            usuario = get_object_or_404(User, username=self.request.user.username)# type: ignore
            usuario.username = username # type: ignore
            
            if password:
                usuario.set_password(password)
                
            usuario.email = email # type: ignore
            usuario.first_name = first_name # type: ignore
            usuario.last_name = last_name # type: ignore
            usuario.save()
            
            if not self.perfil:
                self.perfilform.cleaned_data['usuario'] = usuario
                perfil = Perfil(**self.perfilform.cleaned_data)
                perfil.save()
            else:
                perfil = self.perfilform.save(commit=False)
                perfil.usuario = usuario
                perfil.save()
                
                        
        else:
            usuario = self.userform.save(commit=False)
            usuario.set_password(password)
            usuario.save()
        
            perfil = self.perfilform.save(commit=False)
            perfil.usuario = usuario
            perfil.save()
        
        if password:
            autentica = authenticate(
                self.request,
                username=usuario,
                password=password
            )
            
            if autentica:
                login(self.request, user=usuario)
        
        self.request.session['carrinho'] = self.carrinho
        self.request.session.save()
        
        messages.success(
            self.request,
            'Seu cadastro foi criado ou atualizado com sucesso.'
        )
        
        messages.success(
            self.request,
            'Você fez login e pode concluir a sua compra.'
        )
        
        return redirect('produto:carrinho')
    
class Login(View):
    def post(self, *args, **kwargs):
        username = self.request.POST.get('username')
        password = self.request.POST.get('password')
        
        if not username or not password:
            messages.error(
                self.request,
                'Usuário ou senha inválidos.'
            )
            return redirect('perfil:criar')
        
        usuario = authenticate(
            self.request, username=username, password=password)
        
        if not usuario:
            messages.error(
                self.request,
                'Usuário ou senha inválidos.'
            )
            return redirect('perfil:criar')
        
        login(self.request, user=usuario)
        
        messages.success(
            self.request,
            'Você fez login no sistema e pode concluir sua compra.'
        )
        return redirect('produto:carrinho')
    
class Logout(View):
    def get(self, *args, **kwargs):
        carrinho = copy.deepcopy(self.request.session.get('carrinho'))
        logout(self.request)
        self.request.session['carrinho'] = carrinho
        self.request.session.save()
        return redirect('produto:lista')
    