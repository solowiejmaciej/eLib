FROM node:18-alpine

# Ustawiamy właściwy kontekst
WORKDIR /app

COPY package*.json ./
RUN npm install
RUN npm install -g serve

# Kopiujemy pliki z zachowaniem struktury
COPY . ./

# Ustawiamy NODE_PATH
ENV NODE_PATH=src

RUN npm run build

EXPOSE 80

CMD ["serve", "-s", "dist", "-l", "80"]