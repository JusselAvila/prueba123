using EventosAPI.Data;
using EventosAPI.Models;
using EventosAPI.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using System.Runtime.InteropServices;

namespace EventosAPI.Gestion;

public class AuthGestion
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public AuthGestion(AppDbContext context) => _context = context;


    private async Task<bool> CorreoYaRegistrado(string Correo)
    {
        return await _context.Usuarios.AnyAsync(u => u.Correo == Correo);
    }

    public async Task<(bool Exito, string Mensaje)> RegistrarUsuario(RegistroDTO r)
    {
        if (await CorreoYaRegistrado(r.Correo)) return (false, "El correo electrónico ya está registrado.");

        try 
        {
            var nuevoUsuario = new Usuario
            {
                Nombre = r.Nombre,
                Correo = r.Correo,
                Rol = "Usuario"
            };

            nuevoUsuario.ContraseñaHash = _passwordHasher.HashPassword(nuevoUsuario, r.Password);  

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            return (true, "Usuario registrado exitosamente.");
        }
        catch (Exception ex)
        {
            return (false, "Error al registrar el usuario: " + ex.Message);
        }
    }
    public async Task<(Usuario? Usuario, string Mensaje)> ValidarLogin(LoginDTO r)
    {
        var ExisteUsuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == r.Correo);

        if (ExisteUsuario == null) return (null, "El correo no está registrado.");

        var resultado = _passwordHasher.VerifyHashedPassword(ExisteUsuario, ExisteUsuario.ContraseñaHash, r.Password);
        
        if (resultado != PasswordVerificationResult.Success) return (null, "La contraseña es incorrecta.");

        return (ExisteUsuario, "Login exitoso.");
    }
}