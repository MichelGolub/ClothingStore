<template>
    <div class="row">
        <label for="propertyName">{{ Property.name }}</label>
        <span v-show="!updatingMark">{{ Mark.value.name }}</span>

        <button 
        v-show="!updatingMark" 
        style="margin-left: 16px" 
        class="u-pull-right"
        @click="editMark">
            Edit
        </button>

        <button 
        v-show="!updatingMark" 
        class="u-pull-right" 
        @click="deleteMark">
            Delete
        </button>

        <VForm
        @reset="cancelEditing"
        @submit="updateMark"
        :initial-values="Mark"
        v-show="updatingNumericMark"
        >
            <VField name='id' type='hidden' />
            <VField name='productId' type='hidden' />
            <VField name='valueId' type='hidden' />
            <VField name='value.name' :rules="numericValueRule" />
            <ErrorMessage name='value.name' />

            <button type="submit" class="u-pull-right" style="margin-left: 16px">Ok</button>
            <button type="reset" class="u-pull-right">Cancel</button>

        </VForm>

        <VForm
        @reset="cancelEditing"
        @submit="updateMark"
        :initial-values="Mark"
        v-show="updatingCategoryMark"
        >
            <VField name='id' type='hidden' />
            <VField name='productId' type='hidden' />
            <VField name="valueId" as="select" :rules='selectionRule'>
                    <option disabled value="">Choose one</option>
                    <option 
                    v-for="(value) in Property.values"
                    :key="value.id"
                    :value="value.id"
                    >
                        {{ value.name }}
                    </option>
                </VField>

            <button type="submit" class="u-pull-right" style="margin-left: 16px">Ok</button>
            <button type="reset" class="u-pull-right">Cancel</button>

        </VForm>


    </div>
</template>

<script>
import { Field, Form, ErrorMessage } from 'vee-validate';
import * as yup from 'yup';

export default {
    name: '',
    props: {
        Mark: {
            type: Object,
            default: function() {
                return {
                    id: 0,
                    productId: 0,
                    valueId: 0,
                    value: {
                        id: 0,
                        name: '',
                        propertyId: 0,
                    }
                }
            }
        }
    },
    components: {
        VField: Field,
        VForm: Form,
        ErrorMessage,
    },
    data() {

        const selectionRule = yup.number().required();
        const numericValueRule = yup
            .number()
            .transform((_value, originalValue) => Number(originalValue.replace(/,/g, '')))
            .required();

        return {
            updatingMark: false,
            updatingNumericMark: false,
            updatingCategoryMark: false,
            selectionRule,
            numericValueRule,
            property: {},
        }
    },
    computed: {
        Property() { return this.property },
        isNumeric() { return this.Property.type.name === "numeric" }
    },
    methods: {
        editMark() {
            if(this.isNumeric) {
                this.updatingNumericMark = true;
            } else {
                this.updatingCategoryMark = true;
            }
            this.updatingMark = true;
        },
        updateMark(values, actions) {
            if(this.isNumeric) {
                let newValue = values.value;
                this.$store.dispatch('updateValue', newValue)
                .then(() => {
                    this.$store.dispatch('getProduct', { id: this.Mark.productId });
                });
            } else {
                let newMark = {
                    id: values.id,
                    valueId: values.valueId,
                    productId: values.productId,
                }
                this.$store.dispatch('updateMark', newMark)
                .then(() => {
                    this.$store.dispatch('getProduct', { id: this.Mark.productId });
                });
            }
            
            this.cancelEditing();
        },
        cancelEditing() {
            this.updatingMark = false;
            this.updatingNumericMark = false;
            this.updatingCategoryMark = false;
        },
        deleteMark() {
            if(this.isNumeric) {
                this.$store.dispatch('deleteValue', { id: this.Mark.valueId })
                .then(() => {
                    this.$store.dispatch('getProduct', { id: this.Mark.productId });
                })
            } else {
                this.$store.dispatch('deleteMark', { id: this.Mark.id })
            };
        },
    },
    mounted() {
        this.$store.dispatch('getProperty', { id: this.Mark.value.propertyId })
        .then(() => {
            this.property = this.$store.getters.getProperty; 
        });
    },
}
</script>
