<template>
    <div class="container">
        
        <div class="row">
            <label for="propertyName">{{ $t("name") }}</label>
            <span  v-show="!updatingProperty">{{ Property.name }}</span>
            <button v-show="!updatingProperty" class="u-pull-right" @click="updatingProperty = true">{{ $t("edit") }}</button>
            <VForm
            @reset="updatingProperty = false"
            @submit="updateProperty"
            :initial-values="Property"
            :validation-schema="propertyValidationSchema"
            v-show="updatingProperty"
            >
                <VField name='id' type='hidden' />
                <VField name='shopId' type='hidden' />
                <VField name='typeId' type='hidden' />
                <VField name='name' type='text' />
                <ErrorMessage name='name' />
                <button type="submit" class="u-pull-right" style="margin-left: 16px">{{ $t("ok") }}</button>
                <button type="reset" class="u-pull-right">{{ $t("cancel") }}</button>
            </VForm>
        </div>

        <div v-show="Type.name != 'numeric'" class="row">
            <label for="values">{{ $t("values") }}</label>
            <div v-show="!Loading && (Property.values.length == 0)">{{ $t("no_data") }}</div>
            <valueForm v-for="value in Property.values" :Value="value" :key="value.id"></valueForm>

            <button class="button-primary" @click="creatingValue = true">{{ $t("add") }}</button>
            <VForm
            @reset="creatingValue = false"
            @submit="createValue"
            :validation-schema="valueValidationSchema"
            v-show="creatingValue"
            >
                <VField name='propertyId' type='hidden'/>
                <label for="name">{{ $t("value") }}</label>
                <VField name='name' type='text' />
                <ErrorMessage name='name' />
                <button type="submit" class="u-pull-right" style="margin-left: 16px">{{ $t("ok") }}</button>
                <button type="reset" class="u-pull-right">{{ $t("cancel") }}</button>
            </VForm>
        </div>

        <div class="row">
            <button @click="$router.go(-1)">{{ $t("back") }}</button>
        </div>

    </div>
</template>

<script>
import { Field, Form, ErrorMessage } from 'vee-validate';
import * as yup from 'yup';

import ValueForm from '../../components/ValueForm.vue'


export default({
    name: 'property',
    components: {
        VField: Field,
        VForm: Form,
        ErrorMessage,
        ValueForm
    },
    data() {

        const propertyValidationSchema = {
            name: yup.string().required(this.$t("required")),
        };

        const valueValidationSchema = {
            name: yup.string().required(this.$t("required")),
        };

        return {
            id: this.$route.params.id,
            updatingProperty: false,
            creatingValue: false,
            propertyValidationSchema,
            valueValidationSchema,
        };
    },
    computed: {
        Property() { return this.$store.getters.getProperty; },
        Type() { return this.Property.type; },
        Values() { return this.Property.values; },
        Loading() { return this.$store.getters.getPropertiesStatus === 'loading' }
    },
    methods: {
        updateProperty(values) {
            let payload = {
                id: values.id,
                name: values.name,
                shopId: values.shopId,
                typeId: values.typeId,
            };
            this.$store.dispatch('updateProperty', payload)
            .then(resp => {
                this.updatingProperty = false;
            })
        },
        createValue(values, actions) {
            values.propertyId = this.id;
            this.$store.dispatch('createValue', values)
            .then(resp => {
                actions.resetForm();
                this.creatingValue = false;
            })
        }
    },
    mounted() {
        this.$store.dispatch('getProperty', { id: this.id });
    }
})
</script>
