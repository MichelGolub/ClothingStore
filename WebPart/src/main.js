import { createApp } from 'vue';
import App from './App.vue';

import router from './router';
import store from './store';

import axios from 'axios';
import VueAxios from 'vue-axios';
axios.defaults.baseURL = 'https://localhost:44385/api/';
const token = localStorage.getItem('token');
if (token) {
    axios.defaults.headers.common['Authorization'] = token
  }

import { library } from '@fortawesome/fontawesome-svg-core';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import {
    faHome,
    faUser,
    faUserPlus,
    faSignInAlt,
    faSignOutAlt
} from '@fortawesome/free-solid-svg-icons';
library.add(faHome, faUser, faUserPlus, faSignInAlt, faSignOutAlt)

import i18n from './i18n';


const app = createApp(App);
app.config.performance = true;

app.use(router);
app.use(store);
app.use(VueAxios, axios);
app.use(i18n);

app.component('font-awesome-icon', FontAwesomeIcon)

app.mount('#app');