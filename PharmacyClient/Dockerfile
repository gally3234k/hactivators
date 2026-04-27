FROM node:20 AS build

WORKDIR /app

COPY package.json package-lock.json ./
RUN npm install

COPY . .
RUN npm run build --configuration PharmacyClient

# Stage 2: Serve with simple server
FROM node:20

WORKDIR /app

RUN npm install -g serve

COPY --from=build /app/dist/PharmacyClient/browser ./dist

EXPOSE 80

CMD ["serve", "-s", "dist", "-l", "80"]