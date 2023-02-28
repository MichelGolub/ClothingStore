<template>
    <div class="row">
        <span  v-show="!updatingValue">{{ Value.name }}</span>

        <button 
        v-show="!updatingValue" 
        style="margin-left: 16px" 
        class="u-pull-right"
        @click="updatingValue = true">
            Edit
        </button>

        <button 
        v-show="!updatingValue" 
        class="u-pull-right" 
        @click="deleteValue">
            Delete
        </button>

        <VForm
        @reset="updatingValue = false"
        @submit="updateValue"
        :initial-values="Value"
        :validation-schema="valueValidationSchema"
        v-show="updatingValue"
        >
            <VField name='id' type='hidden' />
            <VField name='propertyId' type='hidden' />
            <VField name='name' type='text' />
            <ErrorMessage name='name' />
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
        Value: {
            type: Object,
            default: function() {
                return {
                    id: 0,
                    propertyId: 0,
                    name: '',
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
        const valueValidationSchema = {
            name: yup.string().required(),
        }

        return {
            updatingValue: false,
            valueValidationSchema,
        }
    },
    methods: {
        updateValue(values, actions) {
            this.$store.dispatch('updateValue', values)
            .then(resp => {
                this.updatingValue = false;
            });
        },
        deleteValue() {
            this.$store.dispatch('deleteValue', { id: this.Value.id });
        },
    }
}
</script>
