FROM node:25-alpine AS builder

WORKDIR /app

COPY package.json package-lock.json* ./

RUN npm ci --legacy-peer-deps

COPY . .

ARG NEXT_PUBLIC_API_BASE_URL
ARG NEXT_PUBLIC_YMAPS_JS_API_KEY
ARG NEXT_PUBLIC_YMAPS_GEOCODER_API_KEY

ENV NEXT_PUBLIC_API_BASE_URL=$NEXT_PUBLIC_API_BASE_URL
ENV NEXT_PUBLIC_YMAPS_JS_API_KEY=$NEXT_PUBLIC_YMAPS_JS_API_KEY
ENV NEXT_PUBLIC_YMAPS_GEOCODER_API_KEY=$NEXT_PUBLIC_YMAPS_GEOCODER_API_KEY

RUN npm run build

FROM node:25-alpine AS runner

WORKDIR /app

ENV NODE_ENV=production

COPY --from=builder /app/public ./public
COPY --from=builder /app/.next/standalone ./
COPY --from=builder /app/.next/static ./.next/static

EXPOSE 3000

ENV PORT=3000
ENV HOSTNAME="0.0.0.0"

CMD ["node", "server.js"]