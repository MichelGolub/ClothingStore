<template>
  <div>
    <nav class="nav-show">
      <div class="container">
        <ul>

          <li><router-link to="/">{{ $t("home") }}</router-link></li>
          <li><router-link to="/products">{{ $t("products") }}</router-link></li>
          <li><router-link to="/properties">{{ $t("filters") }}</router-link></li>
          <li><router-link to="/users">{{ $t("users") }}</router-link></li>

          <div class="u-pull-right" id="auth-buttons" v-if="!isLoggedIn">
            <li><router-link to="/login">{{ $t("log_in") }}</router-link></li>
            <li><router-link to="/register">{{ $t("sign_up") }}</router-link></li>
          </div>
          <div class="u-pull-right" id="logout-button" v-if="isLoggedIn">
            <li><a @click="logout" style="cursor: pointer">{{ $t("log_out") }}</a></li>
          </div>

          <li class="u-pull-right">
              <a href="#">Translation</a>
              <ul>
                <li @click="changeLang('ua')"><a href="#">Укр</a></li>
                <li @click="changeLang('en')"><a href="#">Eng</a></li>
              </ul>
          </li>

        </ul>
      </div>
    </nav>
    <router-view></router-view>
  </div>
</template>

<script>
import 'jquery';
import './assets/skeleton-css_nav-menu/gh.js'


export default {
  name: 'app',
  data () {
    return {
      message: 'Welcome to Vue.js'
    }
  },
  computed: {
    isLoggedIn () { return this.$store.getters.isLoggedIn },
  },
  methods: {
    logout() {
      this.$store.dispatch('logout')
          .then(() => {
            this.$router.push('/login')
          })
    },
    changeLang(newLang) {
      console.log(newLang)
      this.$store.commit("updateLocale", newLang);
      this.$router.go();
    }
  }
}


// todo скачать vue-loaders и импортировать стили
//@import '~vue-loaders/dist/vue-loaders.css';
</script>
 
<style>
@import '~skeleton-css/css/normalize.css';
@import '~skeleton-css/css/skeleton.css';
@import './assets/skeleton-css_nav-menu/gh.css';

.error-message {
  color: red;
}

button[disabled] {
    background:#ccc;
    border-color: #ccc;
    text-shadow:none;
}

.button-primary:disabled:hover {
    background:#ccc;
    border-color: #ccc;
    text-shadow:none;
}

</style>