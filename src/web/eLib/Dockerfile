FROM node:18-alpine

WORKDIR /app

COPY package*.json ./
RUN npm install
RUN npm install -g serve

COPY . ./

ENV NODE_PATH=src

RUN npm run build

EXPOSE 80

CMD ["serve", "-s", "dist", "-l", "80"]