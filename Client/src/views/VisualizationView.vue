<template>
  <div class="w-1/2 min-w-[500px] rounded-lg overflow-hidden relative mx-auto mb-4">
    <div class="flex space-x-1 pl-4 justify-end float-left">
      <div class="flex border border-black rounded-lg overflow-hidden">
        <label
          :class="[
            view_toggle === view_domain
              ? 'highlighted-static bg-yellow-400 text-black'
              : 'not-highlighted bg-gray-700 text-gray-400 border-gray-600',
            'px-4 py-1 select-none cursor-pointer transition-colors duration-300',
          ]"
          @click="view_toggle = view_domain"
        >
          Domain View
        </label>
        <label
          :class="[
            view_toggle === view_website
              ? 'highlighted-live bg-blue-600 text-white'
              : 'not-highlighted bg-gray-700 text-gray-400 border-gray-600',
            'px-4 py-1 select-none cursor-pointer transition-colors duration-300',
          ]"
          @click="view_toggle = view_website"
        >
          Website View
        </label>
      </div>
    </div>

    <div class="flex space-x-1 pr-4 justify-end">
      <div class="p-1 select-none cursor-default">Mode:</div>
      <div class="flex border border-black rounded-lg overflow-hidden">
        <label
          :class="[
            mode_toggle === mode_static
              ? 'highlighted-static bg-orange-500 text-black'
              : 'not-highlighted bg-gray-700 text-gray-400 border-gray-600',
            'px-4 py-1 select-none cursor-pointer transition-colors duration-300',
          ]"
          @click="mode_toggle = mode_static"
        >
          Static
        </label>
        <label
          :class="[
            mode_toggle === mode_live
              ? 'highlighted-live bg-green-500 text-black'
              : 'not-highlighted bg-gray-700 text-gray-400 border-gray-600',
            'px-4 py-1 select-none cursor-pointer transition-colors duration-300',
          ]"
          @click="mode_toggle = mode_live"
        >
          Live
        </label>
      </div>
    </div>
  </div>

  <div class="w-1/2 min-w-[500px] border border-black rounded-lg overflow-hidden relative mx-auto h-[60vh]">
    <v-network-graph class="z-10"
                     :nodes="nodes"
                     :edges="edges"
                     :layouts="layouts"
                     :configs="configs"
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
import { reactive, ref } from "vue"

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
