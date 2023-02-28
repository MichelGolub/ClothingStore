import axios from 'axios'

export default {
    state: {
        status: '',
        token: localStorage.getItem('token') || '',
        user: '',
        roles: [],
        refreshToken: localStorage.getItem('refreshToken') || ''
    },
    getters: {
        isLoggedIn: state => !!state.token,
        authStatus: state => state.status,
        getRefreshToken: state => state.refreshToken
    },
    mutations: {
        authRequest(state) {
            state.status = 'loading'
        },
        authSuccess(state, payload) {
            state.status = 'success'
            state.user = payload.user
            state.roles = payload.roles
        },
        authError(state) {
            state.status = 'error'
        },
        registerSuccess(state) {
            state.status = 'success'
        },
        registerError(state) {
            state.status = 'error'
        },
        logout(state) {
            state.status = '',
            state.token = '',
            state.refreshToken = ''
        },
        setTokens(state, payload) {
            state.token = payload.token;
            state.refreshToken = payload.refreshToken;
        },
    },
    actions: {
        login({commit}, user) {
            return new Promise((resolve, reject) => {
                commit('authRequest')
                axios({
                    url: 'users/token', 
                    data: user,
                    method: 'POST'
                })
                .then(resp => {
                    const user = resp.data.userName
                    const roles = resp.data.roles
                    let payload = {
                        "user": user,
                        "roles": roles
                    }
                    commit('authSuccess', payload)

                    const token = 'Bearer ' + resp.data.token
                    const refreshToken = resp.data.refreshToken
                    let tokens = {
                        'token': token,
                        'refreshToken': refreshToken
                    }
                    commit('setTokens', tokens)

                    localStorage.setItem('token', token)
                    localStorage.setItem('refreshToken', refreshToken)
                    axios.defaults.headers.common['Authorization'] = token
                                
                    resolve(resp)
                })
                .catch(err => {
                    commit('authError')
                    localStorage.removeItem('token')
                    localStorage.removeItem('refreshToken')
                    reject(err)
                })
            })
        },
        register({commit}, registerModel) {
            return new Promise((resolve, reject) => {
                commit('authRequest')
                axios({
                    url: 'users/register', 
                    data: registerModel, 
                    method: 'POST'
                })
                .then(resp => {
                    commit('registerSuccess')
                    resolve(resp)
                })
                .catch(err => {
                    commit('registerError')
                    console.log(err.data)
                    reject(err)
                })
            })
        },
        logout({commit}) {
            return new Promise((resolve, reject) => {
                commit('logout')
                localStorage.removeItem('token')
                localStorage.removeItem('refreshToken')
                delete axios.defaults.headers.common['Authorization']
                resolve()
            })
        },
        refreshToken({ getters, commit }) {
            return new Promise((resolve, reject) => {
                commit('authRequest')
                axios({
                    url: 'users/refresh-token',
                    method: 'POST',
                    headers: {
                        refreshToken: getters.getRefreshToken
                    }
                })
                .then(resp => {
                    const token = 'Bearer ' + resp.data.token
                    const refreshToken = resp.data.refreshToken
                    let tokens = {
                        'token': token,
                        'refreshToken': refreshToken
                    }
                    commit('setTokens', tokens)

                    localStorage.setItem('token', token)
                    localStorage.setItem('refreshToken', refreshToken)
                    axios.defaults.headers.common['Authorization'] = token
                })
            })

        }
    }
}