<template>
    <div class="container">
        <VForm @submit="onSubmit" :validation-schema="registerSchema">

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
                    <button class="button-primary" type="submit">{{ $t("sign_up") }}</button>
                    <button class="u-pull-right" type="reset">{{ $t("reset") }}</button>
                </div>
            </div>
            
        </VForm>
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
            registerSchema
        }
    },
    methods: {
        onSubmit(values, actions) {
            this.$store.dispatch('register', values)
            .then(resp => {
                if(resp.data.status === 0) {
                    this.$router.push('/login');
                } else {
                    actions.setFieldError (
                        'email',
                        this.$t("this_email_is_already_taken")
                    );
                }
            });
        }
    }
})
</script>
