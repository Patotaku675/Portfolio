from django.urls import path, include
from . import views

app_name = 'perfil'

urlpatterns = [
    path('', views.Criar.as_view(), name='criar'),
    path('login/', views.Login.as_view(), name='login'),
    path('logout/', views.Logout.as_view(), name='logout'),
]