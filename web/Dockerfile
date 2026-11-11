FROM mirror.gcr.io/node:24.11-slim AS builder

WORKDIR /app

COPY . ./

RUN --mount=type=secret,id=npm_token \
  export MMS_UI_LIBRARY_TOKEN="$(cat /run/secrets/npm_token)" && \
  corepack enable && \
  corepack prepare yarn@4.13.0 --activate && \
  yarn install --immutable

ENV NODE_OPTIONS="--max-old-space-size=8192"
ARG ENV=production

RUN if [ "$ENV" = "development" ]; then \
  yarn run build --mode development; \
  else \
  yarn run build; \
  fi

FROM mirror.gcr.io/nginx:1.12-alpine

COPY --from=builder /app/build /usr/share/nginx/html/
COPY nginx.conf /etc/nginx/nginx.conf
COPY mime.types /etc/nginx/mime.types

CMD ["nginx", "-g", "daemon off;"]

# FROM node:18-alpine as builder

# WORKDIR /app
# COPY package*.json ./
# COPY .npmrc ./
# RUN yarn install
# COPY . .
# RUN yarn build
# EXPOSE 3000
# CMD ["yarn", "serve"]
