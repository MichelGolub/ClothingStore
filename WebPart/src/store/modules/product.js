import axios from 'axios'

export default {
    state: {
        products: [],
        product: {
            id: 0,
            shopId: 0,
            marks: [],
        },
        page: {
            pageNumber: 1,
            pageSize: 15,
            totalPages: 1,
            totalRecords: 0,
        },
        status: '',
    },
    getters: {
        allProducts: state => state.products,
        getProduct: state => state.product,
        getProductPage: state => state.page,
        getProductStatus: state => state.status,
    },
    mutations: {
        productsRequest(state) {
            state.status = 'loading';
        },
        productsSuccess(state, payload) {
            state.status = 'success';
            state.products = payload.data;
            state.page = {
                pageNumber: payload.pageNumber,
                pageSize: payload.pageSize,
                totalPages: payload.totalPages,
                totalRecords: payload.totalRecords
            };
        },
        productSuccess(state, product) {
            state.status = 'success';
            state.product = product;
        },
        productsError(state, error) {
            state.status = error;
        },
        productCreated(state) {
            state.status = 'success';
            state.page.totalRecords += 1;
        },
        productAdded(state, product) {
            state.products.push(product);
        },
        productUpdated(state, payload) {
            let product = state.product;
            product.name = payload.name;
            state.status = 'success';

            let products = state.products;
            for(let i in products) {
                if(products[i].id === product.id) {
                    products[i].name = payload.name;
                    break;
                }
            }
        },
        productDeleted(state, id) {
            for(let i in state.products) {
                if(state.products[i].id === id) {
                    state.products.splice(i, 1);
                    state.page.totalRecords -= 1;
                    state.status = 'success';
                    return;
                }
            }
            state.status = 'not found';
        },
        markCreated(state) {
            state.status = 'success'
        },
        markUpdated(state, payload) {
            state.status = 'success';
            let marks = state.product.marks;
            for(let i in marks) {
                if(marks[i].id === payload.id) {
                    marks[i].valueId = payload.valueId;
                    break;
                }
            }
        },
        markDeleted(state, payload) {
            state.status = 'success';
            let marks = state.product.marks;
            for(let i in marks) {
                if(marks[i].id === payload.id) {
                    marks.splice(i, 1);
                    break;
                }
            }
        },
        changePage(state, pageStep) {
            let page = state.page;
            let newPageNumber = page.pageNumber + pageStep;
            if(pageStep > 0) {
                if(newPageNumber <= page.totalPages) {
                    state.page.pageNumber = newPageNumber;
                }
            } else {
                if(newPageNumber >= 1) {
                    state.page.pageNumber = newPageNumber;
                }
            }
        },
        addPage(state) {
            state.page.totalPages += 1;
        },
        deletePage(state) {
            state.page.totalPages -= 1;
        }
    },
    actions: {
        getProducts({commit}, payload) {
            return new Promise((resolve, reject) => {
                commit('productsRequest');
                var qs = require('qs');
                axios({
                    method: 'GET',
                    url: 'products',
                    params: payload,
                    paramsSerializer: params => {
                        return qs.stringify(
                            params,
                            { 
                                encodeValuesOnly: true,
                                allowDots: true
                            }
                        );
                    }
                })
                .then(resp => {
                    commit('productsSuccess', resp.data);
                    resolve(resp);
                })
                .catch(err => {
                    commit('productsError', err);
                    reject(err);
                });
            });
        },
        getProduct({commit}, payload) {
            return new Promise((resolve, reject) => {
                commit('productsRequest');
                axios({
                    method: 'GET',
                    url: 'products/' + payload.id,
                })
                .then(resp => {
                    commit('productSuccess', resp.data);
                    resolve(resp);
                })
                .catch(err => {
                    commit('productsError', err);
                    reject(err);
                });
            });
        },
        addProduct({commit, getters}, payload) {
            return new Promise((resolve, reject) => {
                commit('productsRequest');
                axios({
                    url: 'products',
                    method: 'POST',
                    data: payload
                })
                .then(resp => {
                    commit('productCreated');

                    let page = getters.getProductPage;
                    let products = getters.allProducts;
                    if(products.length < page.pageSize) {
                        let product = Object.assign({}, payload);
                        product.id = resp.data;
                        commit('productAdded', product);
                    } else {
                        let isLastPage = page.pageNumber == page.totalPages;
                        if(isLastPage) {
                            commit('addPage');
                        }
                    }

                    resolve(resp);
                })
                .catch(err => {
                    commit('productsError', err);
                    reject(err);
                });
            });
        },
        updateProduct({commit}, payload) {
            return new Promise((resolve, reject) => {
                commit('productsRequest');
                axios({
                    url: 'products/' + payload.id,
                    method: 'PUT',
                    data: payload,
                })
                .then(resp => {
                    commit('productUpdated', payload)
                    resolve(resp);
                })
                .catch(err => {
                    commit('productsError', err);
                    reject(err);
                })
            });
        },
        deleteProduct({commit, state}, payload) {
            return new Promise((resolve, reject) => {
                commit('productsRequest')
                axios({
                    url: 'products/' + payload.id,
                    method: 'DELETE',
                })
                .then(resp => {
                    commit('productDeleted', resp.data);
                    if(state.products.length == 0) {
                        commit('deletePage');
                        commit('changePage', -1);
                    }
                    console.log(resp.data);
                    resolve(resp);
                })
                .catch(err => {
                    commit('productsError', err);
                    reject(err);
                });
            });
        },
        createMark({commit}, payload) {
            return new Promise((resolve, reject) => {
                commit('productsRequest');
                axios({
                    url: 'marks',
                    method: 'POST',
                    data: payload,
                })
                .then(resp => {
                    commit('markCreated');
                    resolve(resp);
                })
                .catch(err => {
                    commit('productsError', err);
                    reject(err);
                })
            });
        },
        updateMark({commit}, payload) {
            return new Promise((resolve, reject) => {
                commit('productsRequest');
                axios({
                    url: 'marks/' + payload.id,
                    method: 'PUT',
                    data: payload,
                })
                .then(resp => {
                    commit('markUpdated', payload);
                    resolve(resp);
                })
                .catch(err => {
                    commit('productsError', err);
                    reject(err);
                })
            });
        },
        deleteMark({commit}, payload) {
            return new Promise((resolve, reject) => {
                commit('productsRequest');
                axios({
                    url: 'marks/' + payload.id,
                    method: 'DELETE',
                })
                .then(resp => {
                    commit('markDeleted', payload);
                    resolve(resp);
                })
                .catch(err => {
                    commit('productsError', err);
                    reject(err);
                })
            });
        }
    }
}