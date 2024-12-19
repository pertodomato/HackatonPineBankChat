<!-- PineBank\pinebank-web\pages\index.vue -->
<template>
  <div>
    <Chat />
  </div>
</template>

<script setup>
import { onMounted } from 'vue'
import Chat from '~/components/Chat.vue'
</script>

<style scoped>
div {
  height: 100vh;
  width: 100vw;
}
</style>