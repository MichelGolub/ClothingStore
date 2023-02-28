import { createRouter, createWebHistory } from 'vue-router'
import HomePage from './views/HomePage.vue'

const router = createRouter({
    "history": createWebHistory(),
    routes: [
        {
            path: '/',
            name: 'home',
            component: HomePage
        },
        {
            path: '/login',
            name: 'login',
            component: () => import('./views/Auth/LoginPage.vue')
        },
        {
            path: '/register',
            name: 'register',
            component: () => import('./views/Auth/RegistrationPage.vue')
        },
        {
            path: '/products',
            name: 'products',
            component: () => import('./views/Products/ProductsPage.vue')
        },
        {
            path: '/products/:id',
            name: 'product',
            component: () => import('./views/Products/ProductPage.vue')
        },
        {
            path: '/users',
            name: 'users',
            component: () => import('./views/Users/UsersPage.vue')
        },
        {
            path: '/users/:id',
            name: 'user',
            component: () => import('./views/Users/UserPage.vue')
        },
        {
            path: '/properties',
            name: 'properties',
            component: () => import('./views/ProductFilters/ProductFilters.vue')
        },
        {
            path: '/properties/:id',
            name: 'property',
            component: () => import('./views/ProductFilters/ProductFilter.vue')
        },
    ]
})

export default router;