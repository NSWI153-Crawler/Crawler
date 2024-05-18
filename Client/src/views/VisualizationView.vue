<template>
  <h1 class="text-3xl font-bold text-center">{{ record.label }}</h1>
  <div class="w-1/2 min-w-[500px] mx-auto mb-4">
    <div class="pl-4 float-left">
      <div class="toggle-container">
        <label
          :class="[
            view_toggle === view_domain
              ? 'bg-yellow-400 text-black'
              : 'disabled-state',
            'button-style',
          ]"
          @click="view_toggle = view_domain"
        >
          Domain View
        </label>
        <label
          :class="[
            view_toggle === view_website
              ? 'bg-blue-600 text-white'
              : 'disabled-state',
            'button-style',
          ]"
          @click="view_toggle = view_website"
        >
          Website View
        </label>
      </div>
    </div>

    <div class="flex space-x-1 pr-4 justify-end">
      <div class="p-1 select-none cursor-default">Mode:</div>
      <div class="toggle-container">
        <label
          :class="[
            mode_toggle === mode_static
              ? 'bg-orange-500 text-black'
              : 'disabled-state',
            'button-style',
          ]"
          @click="mode_toggle = mode_static"
        >
          Static
        </label>
        <label
          :class="[
            mode_toggle === mode_live
              ? 'bg-green-500 text-black'
              : 'disabled-state',
            'button-style',
          ]"
          @click="mode_toggle = mode_live"
        >
          Live
        </label>
      </div>
    </div>
  </div>

  <div class="w-1/2 min-w-[500px] border border-black rounded-lg overflow-hidden relative mx-auto h-[600px]">
    <v-network-graph class="z-10"
                     :nodes="nodes"
                     :edges="edges"
                     :layouts="layouts"
                     :configs="configs"
                     :eventHandlers="eventHandlers"
    >
      <!-- To use CSS within SVG, <defs> is used -->
      <defs>
        <!-- Cannot use <style> directly due to restrictions of Vue. -->
        <component is="style">
          <!-- prettier-ignore -->
          .marker {
          fill: {{ configs.edge.normal.color }};
          transition: fill 0.1s linear;
          pointer-events: none;
          }
          .marker.hovered { fill: {{ configs.edge.hover.color }}; }
          .marker.selected { fill: {{ configs.edge.selected.color }}; }
        </component>
      </defs>
      <template #edge-overlay="{ scale, center, position, hovered, selected }">
        <!-- Place the triangle at the center of the edge -->
        <path
          class="marker"
          :class="{ hovered, selected }"
          d="M-5 -5 L5 0 L-5 5 Z"
          :transform="makeTransform(center, position, scale)"
        />
      </template>
    </v-network-graph>
  </div>
</template>

<script setup lang="ts">
import * as vNG from "v-network-graph"
import { computed, onMounted, reactive, ref } from "vue"
import { useWebsiteRecordStore } from '../stores/records'
import { useRoute } from 'vue-router'

const route = useRoute()
const store = useWebsiteRecordStore()
let id = ""
onMounted(() => {
  id = Array.isArray(route.params.id) ? route.params.id[0] : route.params.id
})
// const record = computed(() => store.getRecordById(id)) TODO
const record = {
   label: "example.com",
}


const nodes = {
  node1: { name: "example.com", color: "white" },
  node2: { name: "example.com/home" },
  node3: { name: "example.com/info" },
  node4: { name: "example.com/about" },
  node5: { name: "example.com/contact" },
}

const edges = {
  edge1: { source: "node1", target: "node2" },
  edge2: { source: "node1", target: "node3" },
  edge3: { source: "node1", target: "node4" },
  edge4: { source: "node3", target: "node5" },
  edge5: { source: "node3", target: "node4" },
  edge6: { source: "node4", target: "node3" }
}

const layouts = {
  nodes: {
    node1: { x: 0, y: 100 },
    node2: { x: 100, y: 0 },
    node3: { x: 100, y: 200 },
    node4: { x: 100, y: 100 },
    node5: { x: 200, y: 200 },
  },
}

const configs = reactive(
  vNG.defineConfigs({
  node: {
    selectable: true,
    normal: { color: "#000" },
    hover: { color: "#000" },
  },
  edge: {
    selectable: false,
    normal : { color: "#000" },
    hover: { color: "#000" },
    selected: { color: "#000" },
    gap: 10,
  },
})
)

const eventHandlers: vNG.EventHandlers = {
  "node:dblclick": ({ node }) => {
    if (true){ //node.crawled) { TODO
      console.log("open node detail: URL, Crawl Time, list of website record that crawled this node");
    }
    else {
      console.log("open node detail: URL");
    }
  },
}

/**
 * Make `transform` value of the object placing on the edge.
 * @param center - center position
 * @param edgePos - edge source and target positions
 * @param scale - zooming scale
 */
function makeTransform(center: vNG.Point, edgePos: vNG.EdgePosition, scale: number) {
  const radian = Math.atan2(
    edgePos.target.y - edgePos.source.y,
    edgePos.target.x - edgePos.source.x
  )
  const degree = (radian * 180.0) / Math.PI

  return [
    `translate(${center.x} ${center.y})`,
    `scale(${scale}, ${scale})`,
    `rotate(${degree})`,
  ].join(" ")
}

const mode_static = "static"
const mode_live = "live"
const mode_toggle = ref(mode_static)
const view_domain = "domain"
const view_website = "website"
const view_toggle = ref(view_website)
</script>
