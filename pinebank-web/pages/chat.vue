<!-- PineBank\pinebank-web\pages\chat.vue -->
<template>
  <div class="chat-page">
    <Chat />
  </div>
</template>

<script setup lang="ts">
import Chat from '~/components/Chat.vue'
</script>

<style scoped>
.chat-page {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background-color: #f5f5f5;
}
</style>
