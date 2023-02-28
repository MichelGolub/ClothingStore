<template>
    <div class="container">
        <VForm @submit="onSubmit" :validation-schema="loginSchema">
            
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
                        type="password"
                        class="u-full-width"  
                    />
                    <ErrorMessage name="password" class="error-message"/>
                </div>
            </div>
            
            <button class="button-primary" type="submit">{{ $t("sign_in") }}</button>
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
        const loginSchema = yup.object({
            email: yup.string().required(this.$t("required")).email().label(this.$t("email")),
            password: yup.string().required(this.$t("required")).min(5).label(this.$t("password")),
        });

        return {
            loginSchema,
        };
    },
    methods: {
        onSubmit(values, actions) {
            this.$store.dispatch('login', values)
            .then(resp => {
                if(resp.data.isAuthenticated) {
                    this.$router.push('/');
                } else {
                    actions.setFieldError(
                        'password',
                        this.$t("incorrect_password_or_email")
                    );
                }
            });
        }
    },
});
</script>
