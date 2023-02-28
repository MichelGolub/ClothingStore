import { createStore } from 'vuex';

import auth from './modules/auth';
import product from './modules/product';
import property from './modules/property';
import user from './modules/user';
import locale from './modules/locale';

import axios from 'axios'

const store = createStore({
    actions: {},
    mutations: {},
    state: {},
    getters: {},
    modules: {
        auth,
        product,
        property,
        user,
        locale
    }
});

export default store;