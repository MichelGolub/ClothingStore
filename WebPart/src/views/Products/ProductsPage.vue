<template>
    <div class="container">


        <div class="row">
            <h2>{{ $t("products") }}</h2>
            <button class="button-primary" @click="creatingProduct = true">{{ $t("add") }}</button>
            <VForm
                @reset="creatingProduct = false"
                @submit="submitProduct"
                v-show="CreatingProduct">
                    <VField name="shopId" type="hidden" value="0" />
                    <label for="name">{{ $t("product_name") }}</label>
                    <VField name="name" type="text" :rules="productNameRule"/>
                    <ErrorMessage 
                    name="name"
                    class="error-message"
                    style="margin-left: 16px" 
                    />
                    
                    <button type="submit" class="u-pull-right" style="margin-left: 16px">{{ $t("ok") }}</button>
                    <button type="reset" class="u-pull-right">{{ $t("cancel") }}</button>
                </VForm>
        </div>
        

        <div class="row">
            <div class="eight columns">
                <table class="u-full-width">
                <thead>
                    <tr>
                        <th>№</th>
                        <th>Id</th>
                        <th>{{ $t("name") }}</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-show="!ProductsExist">{{ $t('no_data') }}</tr>
                    <tr v-show="ProductsLoading">{{ $t('loading') }}</tr>
                    <tr v-for="(product, index) in Products" :key="product.id">
                        <td>{{ index + 1 }}</td>
                        <td>{{ product.id }}</td>
                        <td>{{ product.name }}</td>
                        <td>
                            <a @click="$router.push(`/products/${product.id}`)" style="cursor: pointer">{{ $t("info") }}</a>
                            <button @click="deleteProduct(product.id)" style="margin-left: 16px">{{ $t("delete") }}</button>
                        </td>
                    </tr>
                </tbody>
                </table>
            </div>

            <div class="four columns">
                <filterItem 
                v-for="(property, index) in Properties"
                :key="property.id"
                :propertyId="property.id"
                :index="index"
                @filterChange="changeFilter"
                >
                </filterItem>
                <button @click="applyFilters" class="button-primary">{{ $t("apply") }}</button>
            </div>
        </div>

        <div class="row">
            <button class="three columns" :disabled="!hasPreviousPage" @click="changePage(-1)">{{ '<- ' + $t("previous") }}</button>
            <button class="three columns" :disabled="!hasNextPage" @click="changePage(1)">{{ $t("next") + ' ->' }}</button>
        </div>

    </div>
</template>

<script>
import { Field, Form, ErrorMessage } from 'vee-validate';
import * as yup from 'yup';

import filterItem from '../../components/FilterItem.vue';

export default({
    components: {
        VField: Field,
        VForm: Form,
        ErrorMessage,
        filterItem,
    },
    data() {
        return {
            creatingProduct: false,
            productNameRule: yup.string().required(this.$t("required")),
            filterParams: {
                nominalValuesId: [],
                numericPropertyRanges: [],
            },
            resFilterParams: {
                nominalValuesId: [],
                numericPropertyRanges: [],
            },
        }
    },
    computed: {
        Products() { return this.$store.getters.allProducts; },
        ProductStatus() { return this.$store.getters.getProductStatus; },
        CreatingProduct() { return this.creatingProduct; },

        getProductsParams() {
            let pageParams = Object.assign({}, this.pageParams);
            return Object.assign(pageParams, this.resFilterParams); 
        },

        ProductsPage() { return this.$store.getters.getProductPage; },
        pageParams() { 
            let page = this.ProductsPage;
            return {
                pageNumber: page.pageNumber,
                pageSize: page.pageSize,
            } 
        },
        hasNextPage() { 
            let page = this.ProductsPage;
            return page.pageNumber < page.totalPages
        },
        hasPreviousPage() {
            let page = this.ProductsPage;
            return page.pageNumber > 1;
        },

        Properties() { return this.$store.getters.getProperties },
        PropertiesPage() { return this.$store.getters.getPropertiesPage },

        ProductsLoading() {
            return this.$store.getters.getProductStatus === 'loading'
        },
        ProductsExist() {
            return this.ProductsLoading || this.Products.length > 0;
        }, 
    },
    methods: {
        changePage(step) {
            this.$store.commit('changePage', step);
            this.$store.dispatch('getProducts', this.getProductsParams);
        },
        deleteProduct(productId) {
            this.$store.dispatch('deleteProduct', { id: productId })
            .then(resp => {
                this.$store.dispatch('getProducts', this.getProductsParams)
            })
        },
        submitProduct(values, actions) {
            this.$store.dispatch('addProduct', values)
            .then(resp => {
                actions.resetForm();
                this.creatingProduct = false;
            })
            .catch(err => {
                console.log(err);
            });
            
        },
        changeFilter(values) {
            this.filterParams.nominalValuesId[values.index] = values.filterParam.nominalValueId;
            this.filterParams.numericPropertyRanges[values.index] = values.filterParam.numericPropertyRanges;
        },
        applyFilters() {
            let filters = this.filterParams;
            this.resFilterParams = {
                nominalValuesId: [],
                numericPropertyRanges: [],
            };
            let resFilterParams = this.resFilterParams;

            for(let i in filters.nominalValuesId) {

                let nominalValueId = filters.nominalValuesId[i];
                if(+nominalValueId !== 0) {
                    resFilterParams.nominalValuesId.push(nominalValueId)
                } else {
                    let numericPropertyRange = filters.numericPropertyRanges[i];
                    if(+numericPropertyRange.id !== 0) {
                        if(+numericPropertyRange.min !== 0 || +numericPropertyRange.max !== 0) {
                            resFilterParams.numericPropertyRanges.push(numericPropertyRange);
                        }
                    }
                }
            }

            this.$store.dispatch('getProducts', this.getProductsParams);
        }
    },
    mounted() {
        this.$store.dispatch('getProducts', this.getProductsParams);
        this.$store.dispatch('getProperties', this.PropertiesPage );
    }
})
</script>