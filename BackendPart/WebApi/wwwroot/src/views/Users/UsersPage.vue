<template>
    <div class="container">
        <div class="row">
            <h2>{{ $t("users") }}</h2>
            <button class="button-primary" @click="creatingUser = true">{{ $t("add") }}</button>
            
            <VForm
            @reset="creatingUser = false"
            @submit="submitUser"
            :validation-schema="registerSchema"
            v-show="CreatingUser">

            <div class="row">
                <div class="six columns">
                    <label for="firstName">{{ $t("first_name") }}</label>
                    <VField 
                        name="firstName" 
                        class="u-full-width"
                        type="text" 
                    />
                    <ErrorMessage name="firstName" class="error-message"/>
                </div>
            </div>

            <div class="row">
                <div class="six columns">
                    <label for="username">{{ $t("username") }}</label>
                    <VField 
                        name="username" 
                        class="u-full-width"
                        type="text" 
                    />
                    <ErrorMessage name="username" class="error-message"/>
                </div>
            </div>

            <div class="row">
                <div class="six columns">
                    <label for="email">{{ $t("email") }}</label>
                    <VField 
                        name="email" 
                        class="u-full-width"
                        type="email" 
                    />
                    <ErrorMessage name="email" class="error-message"/>
                </div>
            </div>

            <div class="row">
                <div class="six columns">
                    <label for="password">{{ $t("password") }}</label>
                    <VField 
                        name="password" 
                        class="u-full-width"
                        type="password" 
                    />
                    <ErrorMessage name="password" class="error-message"/>
                </div>
            </div>

            <div class="row">
                <div class="six columns">
                    <label for="passwordConfirm">{{ $t("password_confirm") }}</label>
                    <VField 
                        name="passwordConfirm" 
                        class="u-full-width"
                        type="password" 
                    />
                    <ErrorMessage name="passwordConfirm" class="error-message"/>
                </div>
            </div>
            
            <div class="row">
                <div class="six columns">
                    <button type="submit" class="u-pull-right" style="margin-left: 16px">{{ $t("ok") }}</button>
                    <button type="reset" class="u-pull-right">{{ $t("cancel") }}</button>
                </div>
            </div>
            
        </VForm>

        </div>
        

        <div class="row">
            <div class="eight columns">
                <table class="u-full-width">
                <thead>
                    <tr>
                        <th>№</th>
                        <th>{{ $t("name") }}</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-show="!UsersExist">{{ $t('no_data') }}</tr>
                    <tr v-show="UsersLoading">{{ $t('loading') }}</tr>
                    <tr v-for="(user, index) in Users" :key="user.id">
                        <td>{{ index + 1 }}</td>
                        <td>{{ user.firstName }}</td>
                        <td>
                            <!--
                            <a @click="$router.push(`/users/${user.id}`)" style="cursor: pointer">{{ $t("info") }}</a>
                            -->
                            <button @click="deleteUser(user.id)" style="margin-left: 16px">{{ $t("delete") }}</button>
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

export default ({
    components: {
        VField: Field,
        VForm: Form,
        ErrorMessage
    },
    data() {
        const registerSchema = yup.object({
            firstName: yup.string().required(this.$t("required")).label(this.$t('first_name')),
            username: yup.string().required(this.$t("required")).label(this.$t('username')),
            email: yup.string().required(this.$t("required")).email().label(this.$t('email')),
            password: yup.string().required(this.$t("required")).min(5).label(this.$t('password')),
            passwordConfirm: yup.string().required(this.$t("required"))
                .oneOf([yup.ref('password'), null], this.$t('passwords_must_match'))
                .label(this.$t('this')),
        })

        return {
            registerSchema,
            creatingUser: false,
        }
    },
    computed: {
        Users() { return this.$store.getters.allUsers; },
        UserStatus() { return this.$store.getters.getUserStatus; },
        CreatingUser() { return this.creatingUser; },

        /*
        getUsersParams() {
            let pageParams = Object.assign({}, this.pageParams);
            return Object.assign(pageParams, this.resFilterParams); 
        },
        
        UsersPage() { return this.$store.getters.getProductPage; },
        pageParams() { 
            let page = this.UsersPage;
            return {
                pageNumber: page.pageNumber,
                pageSize: page.pageSize,
            } 
        }, */
        hasNextPage() { 
            // let page = this.ProductsPage;
            // return page.pageNumber < page.totalPages
            return false;
        },
        hasPreviousPage() {
            // let page = this.ProductsPage;
            // return page.pageNumber > 1;
            return false;
        },

        Users() { return this.$store.getters.allUsers },
        // UsersPage() { return this.$store.getters.getUsersPage },

        UsersLoading() {
            return this.UserStatus === 'loading'
        },
        UsersExist() {
            return this.UsersLoading || this.Users.length > 0;
        }, 
    },
    methods: {
        changePage(step) {
            this.$store.commit('changePage', step);
            this.$store.dispatch('getUsers');
        },
        deleteUser(userId) {
            this.$store.dispatch('deleteUser', { id: userId })
            .then(resp => {
                this.$store.dispatch('getUsers')
            })
        },
        submitUser(values, actions) {
            this.$store.dispatch('addUser', values)
            .then(resp => {
                if(resp.data.status === 0) {
                    actions.resetForm();
                    this.creatingUser = false;
                } else {
                    actions.setFieldError (
                        'email',
                        this.$t("this_email_is_already_taken")
                    );
                }
            })
            .catch(err => {
                console.log(err);
            });
            
        },
    },
    mounted() {
        this.$store.dispatch('getUsers', this.getUsers)
        .catch(err => {
            this.$router.push('/login');
        });
    },
})
</script>
