FROM nginx:latest
RUN rm -rf /usr/share/nginx/html/*
COPY Satochat.Client/bin/Release/app.publish /usr/share/nginx/html
RUN chmod -R 644 /usr/share/nginx/html
EXPOSE 80/tcp
ENTRYPOINT ["nginx", "-g", "daemon off;"]
