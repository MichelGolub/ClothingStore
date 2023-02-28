<template>
    <div class="container">
        
        <div class="row">
            <label for="productName">{{ $t("name") }}</label>
            <span  v-show="!updatingProduct">{{ Product.name }}</span>
            <button v-show="!updatingProduct" class="u-pull-right" @click="updatingProduct = true">{{ $t("edit") }}</button>
            <VForm
            @reset="updatingProduct = false"
            @submit="updateProduct"
            :initial-values="Product"
            :validation-schema="productValidationSchema"
            v-show="updatingProduct"
            >
                <VField name='id' type='hidden' />
                <VField name='shopId' type='hidden' />
                <VField name='name' type='text' />
                <ErrorMessage name='name' />
                <button type="submit" class="u-pull-right" style="margin-left: 16px">{{ $t("ok") }}</button>
                <button type="reset" class="u-pull-right">{{ $t("cancel") }}</button>
            </VForm>
        </div>

        <div class="row">
            <div v-show="!MarksExist">{{ $t("no_data") }}</div>
            <div v-show="ProductLoading">{{ $t("loading") }}</div>
            <markForm v-for="mark in Product.marks" :Mark="mark" :key="mark.id"></markForm>

            <button 
            :disabled="!properties.length" 
            class="button-primary" 
            @click="selectingProperty = true">
                {{ $t("add") }}
            </button>
            
            <VForm
            @reset="selectingProperty = false"
            @submit="selectProperty"
            v-show="selectingProperty"
            > 
                <VField name="propertyId" as="select" :rules='selectionRule'>
                    <option disabled value="">{{ $t("choose_one") }}</option>
                    <option 
                    v-for="(property) in Properties"
                    :key="property.id"
                    :value="property.id"
                    >
                        {{ property.name }}
                    </option>
                </VField>

                <button type="submit" class="u-pull-right" style="margin-left: 16px">{{ $t("ok") }}</button>
                <button type="reset" class="u-pull-right">{{ $t("cancel") }}</button>
            </VForm>

            <VForm
            @reset="creatingMark = false"
            @submit="createMark"
            v-show="creatingMark"
            >
                <label for="propertyName">{{ Property.name }}</label>

                <VField name="valueId" as="select" :rules='selectionRule'>
                    <option disabled value="">{{ $t("choose_one") }}</option>
                    <option 
                    v-for="(value) in Property.values"
                    :key="value.id"
                    :value="value.id"
                    >
                        {{ value.name }}
                    </option>
                </VField>

                <button type="submit" class="u-pull-right" style="margin-left: 16px">{{ $t("ok") }}</button>
                <button type="reset" class="u-pull-right">{{ $t("cancel") }}</button>

            </VForm>

            <VForm
            @reset="creatingNumericMark = false"
            @submit="createMark"
            v-show="creatingNumericMark"
            >
                <label for="propertyName">{{ Property.name }}</label>

                <VField name='name' :rules='numericValueRule' />
                <ErrorMessage name='name'/>

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

import markForm from '../../components/MarkForm.vue'

export default({
    name: 'product',
    components: {
        VField: Field,
        VForm: Form,
        ErrorMessage,
        markForm,
    },
    data() {

        const productValidationSchema = {
            name: yup.string().required(this.$t("required")),
        };

        const selectionRule = yup.number().required(this.$t("required"));
        const numericValueRule = yup.number().required(this.$t("required"));

        return {
            id: this.$route.params.id,
            updatingProduct: false,
            creatingMark: false,
            creatingNumericMark: false,
            selectingProperty: false,
            productValidationSchema,
            selectionRule,
            numericValueRule,
            properties: [],
        };
    },
    computed: {
        Product() { return this.$store.getters.getProduct; },
        Properties() { return this.properties; },
        Property() { return this.$store.getters.getProperty; },
        ProductLoading() { return this.$store.getters.getProductStatus === 'loading' },
        MarksExist() { return !(this.ProductLoading) && this.Product.marks.length > 0 }
    },
    methods: {
        updateProduct(values) {
            let payload = {
                id: values.id,
                name: values.name,
                shopId: values.shopId,
            };
            this.$store.dispatch('updateProduct', payload)
            .then(resp => {
                this.updatingProduct = false;
            })
        },
        selectProperty(values) {
            this.$store.dispatch('getProperty', { id: values.propertyId })
            .then(() => {
                this.selectingProperty = false;
                if(this.Property.type.name === 'numeric') {
                    this.creatingNumericMark = true;
                } else {
                    this.creatingMark = true;
                }
                
            })
        },
        createMark(values, actions) {
            let productId = this.Product.id;
            let mark = {
                productId
            };

            if(this.Property.type.name === 'numeric') {
                let propertyId = this.Property.id;
                let value = {
                    name: values.name,
                    propertyId
                }

                mark = Object.assign(mark, { value });
            } else {
                mark = Object.assign(mark, { valueId: values.valueId });
            }            

            this.$store.dispatch('createMark', mark)
            .then(resp => {
                actions.resetForm();
                this.creatingMark = false;
                this.$store.dispatch('getProduct', { id: this.id })
                .then(() => {
                    let properties = this.properties;
                    for(let i in properties) {
                        if(properties[i].id === this.Property.id) {
                            properties.splice(i, 1);
                            this.creatingNumericMark = false;
                            break;
                        }
                    }
                });
            }) 
        },
    },
    mounted() {
        this.$store.dispatch('getProduct', { id: this.id })
        .then(() => {
            this.$store.dispatch('getProperties')
            .then(() => {
                let properties = this.$store.getters.getProperties;
                let marks = this.Product.marks;
                let isUsed = false;
                for(let i in properties) {
                    for(let j in marks) {
                        if(properties[i].id === marks[j].value.propertyId) {
                            isUsed = true;
                            break;
                        }
                    }
                    if(!isUsed) {
                        this.properties.push(properties[i]);
                    }
                    isUsed = false;
                }
            })
        })
    }
})
</script>