import axios from 'axios'

export default {
    state: {
        user: {},
        users: [],
        status: ''
    },
    getters: {
        getUserStatus: state => state.status,
        allUsers: state => state.users
    },
    mutations: {
        usersRequest(state) {
            state.status = 'loading'
        },
        userSuccess(state, user) {
            state.status = 'success'
            state.user = user
        },
        userUpdated(state) {
            state.status = 'success'
        },
        usersSuccess(state, users) {
            state.status = 'success'
            state.users = users
        },
        userError(state) {
            state.status = 'error'
        },
        userDeleted(state, id) {
            state.status = 'success'
            for(let i in state.users) {
                if(state.users[i].id === id) {
                    state.users.splice(i, 1)
                    break
                }
            }
        },
        userAdded(state, user) {
            state.status = 'success';
            state.users.push(user);
        }
    },
    actions: {
        addUser({commit}, user) {
            commit('usersRequest')
            return new Promise((resolve, reject) => {
                axios.post(`users/register`, user)
                .then(resp => {
                    if(resp.data.status === 0) {
                        commit('userAdded', user)
                    } 
                    resolve(resp)
                })
                .catch(err => {
                    commit('userError')
                    reject(err)
                })
            })
        },
        deleteUser({commit}, id) {
            return new Promise((resolve, reject) => {
                commit('usersRequest')
                axios.delete(`users/${id}`)
                .then(resp => {
                    commit('userDeleted', id)
                    //this._vm.$toast.success('User deleted')
                    resolve(resp)
                })
                .catch(err => {
                    commit('userError', err)
                    reject(err)
                })
            })
        },
        getUsers({commit}) {
            return new Promise((resolve, reject) => {
                commit('usersRequest')
                axios.get(`users`)
                .then(resp => {
                    let users = resp.data;
                    commit('usersSuccess', users)
                    resolve(resp)
                })
                .catch(err => {
                    commit('userError', err)
                    reject(err)
                })
            })
        }
    },
}