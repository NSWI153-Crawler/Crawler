<template>
  <h1 class="text-3xl font-bold text-center">{{ record.label }}</h1>
  <div class="w-1/2 min-w-[500px] mx-auto mb-4">  <!-- bar holding top buttons -->
    <div class="pl-4 float-left">                 <!-- view buttons container-->
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

  <div class="flex space-x-1 pr-4 justify-end"> <!-- mode buttons container -->
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
    <v-network-graph class="graph"
                     ref="graph"
                     :nodes="nodes"
                     :edges="edges"
                     :layouts="layouts"
                     :configs="configs"
                     :eventHandlers="eventHandlers"
    />
  </div>
  <div class="w-1/2 min-w-[500px] mx-auto mb-4 mt-4">   <!-- bar holding bottom buttons -->
    <div class="flex space-x-1 pr-4 justify-center">    <!-- layout buttons container -->
      <div class="p-1 select-none cursor-default">Layout:</div>
      <div class="toggle-container">
        <label
          :class="[
              layout_toggle === layout_lr
                ? 'bg-[#0ff] text-black'
                : 'disabled-state',
              'button-style',
            ]"
          @click="toggleLayout('LR')"
        >
          Left to Right
        </label>
        <label
          :class="[
              layout_toggle === layout_tb
                ? 'bg-[#0ff] text-black'
                : 'disabled-state',
              'button-style',
            ]"
          @click="toggleLayout('TB')"
        >
          Top to Bottom
        </label>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import * as vNG from "v-network-graph"
import { computed, onMounted, reactive, ref } from "vue"
import { useWebsiteRecordStore } from '../stores/records'
import { useRoute } from 'vue-router'
import dagre from "@dagrejs/dagre"

const route = useRoute()
const store = useWebsiteRecordStore()
let id = ""

onMounted(() => {
  id = Array.isArray(route.params.id) ? route.params.id[0] : route.params.id
  // store.fetchRecordById(id)
  layout("LR")
})
// const record = computed(() => store.getRecordById(id)) TODO
const record = { // crawled = matches regexp
  id: "1",
  url: "http://example.com",
  regexp: ".*",
  periodicity: 24,
  label: "example.com",
  isActive: true,
  tags: ["example", "com"],
  lastExecutionTime: "2021-09-01T00:00:00Z",
  lastExecutionStatus: "Success",
}


interface Node extends vNG.Node {
  name: string
  crawled?: boolean
  color?: string
  size?: number
}

interface Edge extends vNG.Edge {
  crawled?: boolean
  width?: number
  color?: string
}


const nodes: Record<string, Node> = {
  node1: { name: "example.com", crawled: true },
  node2: { name: "example.com/home", crawled: true },
  node3: { name: "example.com/info", crawled: true },
  node4: { name: "example.com/about", crawled: true },
  node5: { name: "example.com/contact", crawled: false },
}

const edges: Record<string, Edge> = {
  edge1: { source: "node1", target: "node2", crawled: true },
  edge2: { source: "node1", target: "node3", crawled: true },
  edge3: { source: "node1", target: "node4", crawled: true },
  edge4: { source: "node3", target: "node5", crawled: false },
  edge5: { source: "node3", target: "node4", crawled: true },
  edge6: { source: "node4", target: "node3", crawled: true },
}

const configs = reactive(
  vNG.defineConfigs<Node, Edge>({
    node: {
      normal: { color: (node) => (node.crawled ? "#000" : "#999"), },
      hover:  { color: (node) => (node.crawled ? "#000" : "#999"), },
      label:  { visible: node => !!node.name, },
      focusring: { visible: false, },
      selectable: true,
    },

    edge: {
      normal: { color: "#000", },
      hover: { color: "#000", },
      selectable: false,
      gap: 5,
      marker: {
        target: {
          type: "arrow",
          width: 4,
          height: 4,
        },
      },
    },
  })
)

const layouts: vNG.Layouts = reactive({
  nodes: {},
})

const nodeSize = 40
const graph = ref<vNG.VNetworkGraphInstance>()

function layout(direction: "TB" | "LR") {
  if (Object.keys(nodes).length <= 1 || Object.keys(edges).length == 0) {
    return
  }

  // convert graph
  // ref: https://github.com/dagrejs/dagre/wiki
  const g = new dagre.graphlib.Graph()
  // Set an object for the graph label
  g.setGraph({
    rankdir: direction,
    nodesep: nodeSize * 2,
    edgesep: nodeSize,
    ranksep: nodeSize * 2,
  })
  // Default to assigning a new object as a label for each new edge.
  g.setDefaultEdgeLabel(() => ({}))

  // Add nodes to the graph. The first argument is the node id. The second is
  // metadata about the node. In this case we're going to add labels to each of
  // our nodes.
  Object.entries(nodes).forEach(([nodeId, node]) => {
    g.setNode(nodeId, { label: node.name, width: nodeSize, height: nodeSize })
  })

  // Add edges to the graph.
  Object.values(edges).forEach(edge => {
    g.setEdge(edge.source, edge.target)
  })

  dagre.layout(g)

  g.nodes().forEach((nodeId: string) => {
    // update node position
    const x = g.node(nodeId).x
    const y = g.node(nodeId).y
    layouts.nodes[nodeId] = { x, y }
  })
}

function updateLayout(direction: "TB" | "LR") {
  // Animates the movement of an element.
  graph.value?.transitionWhile(() => {
    layout(direction)
  })
}

const eventHandlers: vNG.EventHandlers = {
  "node:dblclick": ({ node }) => {
    if (nodes[node].crawled) {
      console.log("open node detail: URL, Crawl Time, list of website record that crawled this node");
    }
    else {
      console.log("open node detail: URL");
    }
  },
}

const mode_static = "static"
const mode_live = "live"
const mode_toggle = ref(mode_static)
const view_domain = "domain"
const view_website = "website"
const view_toggle = ref(view_website)
const layout_lr = "lr"
const layout_tb = "tb"
const layout_toggle = ref(layout_lr)
function toggleMode() {}
function toggleView() {}
function toggleLayout(direction: "LR" | "TB"){
  layout(direction)
  layout_toggle.value = direction === "LR" ? layout_lr : layout_tb
}
</script>
