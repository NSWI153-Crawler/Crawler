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
          @click="toggleView('domain')"
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
          @click="toggleView('website')"
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
}

interface Edge extends vNG.Edge {
  color?: string
}


const nodes: Record<string, Node> = reactive({
  node1: { name: "example.com", crawled: true },
  node2: { name: "http://example.com/home", crawled: true },
  node3: { name: "https://exam.com/info", crawled: true },
  node4: { name: "https://www.exampl.com/about", crawled: true },
  node5: { name: "example.com/contact", crawled: false },
})

const edges: Record<string, Edge> = reactive({
  edge1: { source: "node1", target: "node2", },
  edge2: { source: "node1", target: "node3", },
  edge3: { source: "node1", target: "node4", },
  edge4: { source: "node3", target: "node5", },
  edge5: { source: "node3", target: "node4", },
  edge6: { source: "node4", target: "node3", },
})

const configs = reactive(
  vNG.defineConfigs<Node, Edge>({
    node: {
      normal: { color: (node) => (node.crawled ? "#000" : "#999"), },
      hover:  { color: (node) => (node.crawled ? "#000" : "#999"), },
      label:  { visible: node => !!node.name,
                color: (node) => (node.crawled ? "#000" : "#666"),
      },
      focusring: { visible: false, },
      selectable: true,
    },

    edge: {
      normal: { color: "#999", },
      hover:  { color: "#999", },
      selectable: false,
      gap: 5,
      marker: {
        target: {
          type: "arrow",
          width:  4,
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

function extractDomain(url: string): string | null {
  try {
    const matches = url.match(/^(https?:\/\/)?(www.)?([^/?#]+)(?:[/?#]|$)/i);
    const domain = matches && matches[3];
    return domain;
  } catch (error) {
    console.error('Invalid URL:', error);
    return null;
  }
}

interface IdSetDictionary {
  [key: number]: Set<any>;
}
type NodePair = [string, boolean];
type EdgePair = [string, string];

const websiteEdgesBackup: EdgePair[] = []
const websiteNodesBackup: NodePair[] = []

function changeView(toggleTo: 'domain' | 'website'){

  let nextNodeIndex = 1;
  let nextEdgeIndex = 1;

  if (view_toggle.value === view_domain && toggleTo === 'website') {
    /* show website view */
    for (const [nodeID, _] of Object.entries(nodes))
      delete nodes[nodeID]
    for (const [edgeID, _] of Object.entries(edges))
      delete edges[edgeID]
    for (const node of websiteNodesBackup) {
      const nodeName= `node${nextNodeIndex}`
      nodes[nodeName] = { name: node[0], crawled: node[1] }
      nextNodeIndex++
    }
    for (const edge of websiteEdgesBackup) {
      const edgeName = `edge${nextEdgeIndex}`
      edges[edgeName] = { source: edge[0], target: edge[1]}
      nextEdgeIndex++
    }
  }
  else if (view_toggle.value === view_website && toggleTo === 'domain') {
    /* show domain view */
    const domainEdges: IdSetDictionary = {} // domainSource: Set<domainTarget>
    const domainNodes = new Set<string>()
    const domainNodeIdMap = new Map<string, number>()

    websiteNodesBackup.length = 0
    for (const [_, node] of Object.entries(nodes)) {
      websiteNodesBackup.push([node.name, node.crawled === true])
      const domain = extractDomain(node.name)
      if (domain)
        domainNodes.add(domain)
    }
    for (const domain of domainNodes) {
      domainNodeIdMap.set(domain, nextNodeIndex)
      nextNodeIndex++
    }

    websiteEdgesBackup.length = 0
    for (const [edgeID, edge] of Object.entries(edges)) {
      websiteEdgesBackup.push([edge.source, edge.target])
      const sourceDomain = domainNodeIdMap.get(extractDomain(nodes[edge.source].name) ?? "")
      const targetDomain = domainNodeIdMap.get(extractDomain(nodes[edge.target].name) ?? "")
      if (sourceDomain !== undefined && targetDomain !== undefined && sourceDomain !== targetDomain) {
        if (domainEdges[sourceDomain]) domainEdges[sourceDomain].add(targetDomain)
        else domainEdges[sourceDomain] = new Set([targetDomain])
      }
      delete edges[edgeID]
    }
    for (const [nodeID, _] of Object.entries(nodes)) {
      delete nodes[nodeID]
    }
    for (const domain of domainNodes) {
      const nodeName= `node${domainNodeIdMap.get(domain)}`
      nodes[nodeName] = { name: domain, crawled: true }
    }
    for (const [source, targets] of Object.entries(domainEdges)) {
      for (const target of targets) {
        const edgeName = `edge${nextEdgeIndex}`
        const sourceName = `node${source}`
        const targetName = `node${target}`
        edges[edgeName] = { source: sourceName, target: targetName}
        nextEdgeIndex++
      }
    }
  }
  layout(layout_toggle.value === layout_lr ? "LR" : "TB")
}

const mode_static = "static"
const mode_live = "live"
const mode_toggle = ref(mode_static)
function toggleMode() {}

const view_domain = "domain"
const view_website = "website"
const view_toggle = ref(view_website)
function toggleView(viewType: 'domain' | 'website') {
  changeView(viewType)
  if (viewType === 'domain') {
    view_toggle.value = view_domain
  }
  else {
    view_toggle.value = view_website
  }
}

const layout_lr = "lr"
const layout_tb = "tb"
const layout_toggle = ref(layout_lr)
function toggleLayout(direction: "LR" | "TB"){
  updateLayout(direction)
  layout_toggle.value = direction === "LR" ? layout_lr : layout_tb
}
</script>
