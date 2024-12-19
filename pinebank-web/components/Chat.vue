<template>
  <v-container fluid class="fill-height hackathon-bg">
    <v-row align="center" justify="center">
      <v-col cols="12" sm="8" md="6">
        <v-card class="elevation-12">
          <v-toolbar dark class="pine-header">
            <v-toolbar-title class="golden-text">Pine+ Bot</v-toolbar-title>
          </v-toolbar>

          <v-card-text class="chat-container">
            <div ref="messagesContainer" class="messages-wrapper">
              <v-slide-y-transition group>
                <div v-for="(msg, index) in messages" :key="index" 
                     :class="['message-bubble', msg.role === 'user' ? 'user-message' : 'bot-message']">
                  <strong>{{ capitalize(msg.role) }}:</strong> {{ msg.content }}
                </div>
              </v-slide-y-transition>
            </div>
          </v-card-text>

          <v-card-actions class="pa-4">
            <v-text-field
              v-model="userMessage"
              label="Type your message"
              variant="outlined"
              append-inner-icon="mdi-send"
              @click:append-inner="sendMessage"
              @keyup.enter="sendMessage"
              :loading="isLoading"
              class="pine-input"
            ></v-text-field>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

interface Message {
  role: string
  content: string
}

const userMessage = ref('')
const messages = ref<Message[]>([])
const isLoading = ref(false)
const token = ref('')
const messagesContainer = ref<HTMLDivElement | null>(null)

const config = useRuntimeConfig()
const userId = 'test-user'

const scrollToBottom = () => {
  if (messagesContainer.value) {
    setTimeout(() => {
      messagesContainer.value?.scrollTo({
        top: messagesContainer.value.scrollHeight,
        behavior: 'smooth'
      })
    }, 100)
  }
}

const sendMessage = async () => {
  if (!userMessage.value.trim() || isLoading.value) return

  const messageToSend = userMessage.value // Store the message
  userMessage.value = '' // Clear input immediately
  isLoading.value = true
  
  messages.value.push({ role: 'user', content: messageToSend })
  scrollToBottom()

  try {
    const response: { message: string } = await $fetch(`${config.public.apiBase}/Chat`, {
      method: 'POST',
      body: { message: messageToSend, userId },
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token.value}`
      }
    })

    messages.value.push({ role: 'assistant', content: response.message })
    scrollToBottom()

  } catch (error) {
    console.error('Error sending message:', error)
    messages.value.push({ 
      role: 'assistant', 
      content: 'Desculpe, ocorreu um erro. Por favor, tente novamente mais tarde.' 
    })
  } finally {
    isLoading.value = false
  }
}

const authenticate = async () => {
  try {
    const response: { token: string } = await $fetch(`${config.public.apiBase}/Auth/login`, {
      method: 'POST',
      body: { username: 'admin', password: 'password' },
      headers: {
        'Content-Type': 'application/json'
      }
    })
    token.value = response.token
    console.log('Authentication successful')
  } catch (error) {
    console.error('Authentication failed:', error)
  }
}

const capitalize = (str: string) => str.charAt(0).toUpperCase() + str.slice(1)

onMounted(() => {
  authenticate()
})
</script>

<style scoped>
.hackathon-bg {
  background-color: #4A0404;
  background-image: linear-gradient(45deg, #4A0404 0%, #2b0202 100%);
}

.pine-header {
  background-color: #4A0404 !important;
  border-bottom: 2px solid #B8860B;
}

.golden-text {
  color: #B8860B;
  font-family: Arial, sans-serif;
  font-weight: bold;
  letter-spacing: 1px;
}

.chat-container {
  height: 60vh;
  padding: 1rem;
  background-color: rgba(255, 255, 255, 0.95);
}

.messages-wrapper {
  height: 100%;
  overflow-y: auto;
  scroll-behavior: smooth;
  padding: 0.5rem;
}

.message-bubble {
  margin: 8px;
  padding: 12px;
  border-radius: 8px;
  max-width: 80%;
  word-wrap: break-word;
  box-shadow: 0 1px 2px rgba(0,0,0,0.1);
}

.user-message {
  background-color: #fdf5e6;
  margin-left: auto;
  border-left: 3px solid #B8860B;
}

.bot-message {
  background-color: #f8f8f8;
  margin-right: auto;
  border-left: 3px solid #4A0404;
}

.user-message strong, .bot-message strong {
  color: #4A0404;
}

:deep(.pine-input .v-field) {
  border-color: #B8860B !important;
}

:deep(.pine-input .v-field:hover), :deep(.pine-input .v-field:focus-within) {
  border-color: #4A0404 !important;
}

:deep(.pine-input .v-field__append-inner) {
  color: #B8860B;
}

:deep(.pine-input .v-field__append-inner:hover) {
  color: #4A0404;
}
</style>