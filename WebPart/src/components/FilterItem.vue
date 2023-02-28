<template>
    <div class="row">
        <label class="ten columns" for="name">{{ Property.name }}</label>

        <div 
        v-if="Type.name === 'numeric'">

            <a style="cursor: pointer" @click="setZeros">Reset</a>

            <input 
            @change="onFilterChange" 
            class="five columns"
            type="number" 
            v-model="filterParam.numericPropertyRanges.min">

            <div class="one column">-</div>

            <input
            @change="onFilterChange" 
            class="five columns"
            type="number" 
            v-model="filterParam.numericPropertyRanges.max">

        </div>

        <select 
        v-if="Type.name !== 'numeric'"
        class="u-full-width"
        name="value"
        v-model="filterParam.nominalValueId"
        @change="onFilterChange" >
            <option value='0'>All</option>
            <option 
            v-for="(value) in Property.values" 
            :value="value.id"
            :key="value.id"
            >
                {{ value.name }}
            </option>
        </select>
    </div>
</template>

<script>

export default {
    props: {
        propertyId: {
            type: Number,
            required: true
        },
        index: {
            type: Number,
            required: true
        }
    },
    
    data() {
        return {
            property: {
                id: 0,
                name: '',
                type: {
                    id: 0,
                    name: '',
                },
                values: [],
            },
            filterParam: {
                nominalValueId: 0,
                numericPropertyRanges: {
                    id: 0,
                    min: 0,
                    max: 0
                }
            }
        }
    },
    computed: {
        Property() { return this.property; },
        Type() { return this.Property.type; },
    },
    methods: {
        setZeros() {
            this.filterParam.numericPropertyRanges.min = 0;
            this.filterParam.numericPropertyRanges.max = 0;

            this.onFilterChange();
        },
        onFilterChange() {
            let ranges = this.filterParam.numericPropertyRanges;
            
            if(ranges.min > ranges.max) {
                ranges.max = ranges.min;
            }

            let param = Object.assign({}, this.filterParam);
            this.$emit('filterChange', { index: this.index, filterParam: param });
        }
    },
    mounted() {
        this.$store.dispatch('getProperty', { id: this.propertyId })
        .then(() => {
            this.property = this.$store.getters.getProperty;
            this.filterParam.numericPropertyRanges.id = this.property.id;
        });
    },
}
</script>
