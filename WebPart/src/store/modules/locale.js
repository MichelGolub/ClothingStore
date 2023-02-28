import i18n, { selectedLocale } from "../../i18n.js";

export default {
    state: {
        locale: selectedLocale
    }, 
    getters: {
        getLocale: state => state.locale
    },
    mutations: {
        updateLocale(state, newLocale) {
            i18n.locale = newLocale;
            state.locale = newLocale;
            localStorage.setItem('locale', newLocale);
        }
    }
}