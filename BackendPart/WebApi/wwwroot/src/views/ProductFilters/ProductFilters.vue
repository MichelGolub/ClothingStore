<template>
    <div class="container">

        <h2 class="row">{{ $t("filters") }}</h2>

        <div class="row">
            <div class="u-full-width">
                <button class="button-primary" @click="creatingProperty = true">{{ $t("add") }}</button>
                <VForm
                @reset="creatingProperty = false"
                @submit="submitProperty"
                v-show="creatingProperty">
                    <VField name="shopId" type="hidden" value="0" />
                    <label for="name">{{ $t("property_name") }}</label>
                    <VField name="name" type="text" :rules="propertyNameRule"/>
                    <ErrorMessage 
                    name="name"
                    class="error-message"
                    style="margin-left: 16px" 
                    />

                    <label for="name">{{ $t("property_type") }}</label>
                    <VField name="typeId" as="select">
                        <option disabled value="">{{ $t("choose_one") }}</option>
                        <option 
                        v-for="(value, key) in PropertyTypes"
                        :key="key"
                        :value="value"
                        >
                            {{ key }}
                        </option>
                    </VField>
                    
                    <button type="submit" class="u-pull-right" style="margin-left: 16px">{{ $t("ok") }}</button>
                    <button type="reset" class="u-pull-right">{{ $t("cancel") }}</button>
                </VForm>
            </div>
        </div>

        <div class="row">
            <div class="u-full-width">
                <table class="u-full-width">
                <thead>
                    <tr>
                        <th>№</th>
                        <th>{{ $t("name") }}</th>
                        <th>{{ $t("type") }}</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-show="!PropertiesExist">{{ $t('no_data') }}</tr>
                    <tr v-show="PropertiesLoading">{{ $t('loading') }}</tr>
                    <tr v-for="(property, index) in Properties" :key="property.id">
                        <td>{{ index + 1 }}</td>
                        <td>{{ property.name }}</td>
                        <td>{{ getPropertyTypeById(property.typeId) }}</td>
                        <td>
                            <a @click="$router.push(`/properties/${property.id}`)" style="cursor: pointer">{{ $t("info") }}</a>
                            <button @click="deleteFilter(property.id)" style="margin-left: 16px">{{ $t("delete") }}</button>
                        </td>
                    </tr>
                </tbody>
                </table>
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


export default({
    components: {
        VField: Field,
        VForm: Form,
        ErrorMessage
    },
    data() {
        return {
            creatingProperty: false,
            propertyNameRule: yup.string().required(this.$t("required")),
        }
    },
    computed: {
        Properties() { return this.$store.getters.getProperties; },
        PropertyStatus() { return this.$store.getters.getPropertiesStatus; },
        PropertyTypes() { return this.$store.getters.getTypes; },
        CreatingProperty() { return this.creatingProperty; },
        PropertiesPage() { return this.$store.getters.getPropertiesPage },
        pageParams() { 
            let page = this.PropertiesPage;
            return {
                pageNumber: page.pageNumber,
                pageSize: page.pageSize,
            } 
        },
        hasNextPage() { 
            let page = this.PropertiesPage;
            return page.pageNumber < page.totalPages
        },
        hasPreviousPage() {
            let page = this.PropertiesPage;
            return page.pageNumber > 1;
        },
        PropertiesLoading() {
            return this.$store.getters.getPropertiesStatus === 'loading'
        },
        PropertiesExist() {
            return this.PropertiesLoading || this.Properties.length > 0;
        } 
    },
    methods: {
        changePage(step) {
            console.log('changing page...');
            //this.$store.commit('changePropertiesPage', step);
            //this.$store.dispatch('getProperties', this.PropertiesPage);
        },
        deleteProperty(propertyId) {
            this.$store.dispatch('deleteProperty', { id: propertyId })
            .then(resp => {
                this.$store.dispatch('getProperties', this.PropertiesPage)
            })
        },
        submitProperty(values, actions) {
            this.$store.dispatch('addProperty', values)
            .then(resp => {
                actions.resetForm();
                this.creatingProperty = false;
            })
            .catch(err => {
                console.log(err);
            });
        },
        getPropertyTypeById(typeId) {
            let types = this.$store.getters.getTypes;
            for(let type in types) {
                if(types[type] == typeId) {
                    return type;
                }
            }
            return '';
        },
        deleteFilter(propertyId) {
            this.$store.dispatch('deleteProperty', { id: propertyId })
            .then(resp => {
                this.$store.dispatch('getProperties', this.getProductsParams)
            })
        }
    },
    mounted() {
        this.$store.dispatch('getProperties', this.PropertiesPage );
    }
})
</script>
