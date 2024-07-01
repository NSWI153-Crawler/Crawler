import './assets/main.css'
import App from './App.vue'
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import VNetworkGraph from 'v-network-graph'
import 'v-network-graph/lib/style.css'

const app = createApp(App)
const pinia = createPinia()

app.use(VNetworkGraph)
app.use(pinia)
app.mount('#app')
