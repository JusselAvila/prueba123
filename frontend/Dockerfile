# Etapa 1: Construcción (Build)
FROM node:22-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

# Etapa 2: Servidor Nginx (Final)
FROM nginx:alpine
# Copiamos la compilación al directorio raíz de Nginx

COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html

# Reemplazamos la configuración por defecto de Nginx con la tuya
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]