import axios from 'axios'

export default {
    state: {
        types: {
            category: 1,
            numeric: 2        
        },
        properties: [],
        property: {
            id: 0,
            name: '',
            type: {
                id: 0,
                name: ''
            },
            typeId: 0,
            values: []
        },
        filters: [],
        status: '',
        page: {
            pageNumber: 1,
            pageSize: 50,
            totalPages: 1,
            totalRecords: 0,
        },
    },
    getters: {
        getPropertiesStatus: state => state.status,
        getProperties: state => state.properties,
        getProperty: state => state.property,
        getTypes: state => state.types,
        getPropertiesPage: state => state.page,
    },
    mutations: {
        propertiesRequest(state) {
            state.status = 'loading';
        },
        propertiesError(state, err) {
            state.status = err
        },
        propertyCreated(state) {
            state.status = 'success'
        },
        propertyAdded(state, property) {
            state.properties.push(property);
        },
        addPage(state) {
            state.page.totalPages += 1;
        },
        propertiesSuccess(state, payload) {
            state.status = 'success';
            state.properties = payload.data;
            state.page = {
                pageNumber: payload.pageNumber,
                pageSize: payload.pageSize,
                totalPages: payload.totalPages,
                totalRecords: payload.totalRecords
            };
        },
        propertySuccess(state, payload) {
            state.status = 'success';
            state.property = payload;
        },
        propertyDeleted(state, id) {
            for(let i in state.properties) {
                if(state.properties[i].id === id) {
                    state.properties.splice(i, 1);
                    state.page.totalRecords -= 1;
                    state.status = 'success';
                    return;
                }
            }
            state.status = 'not found';
        },
        propertyUpdated(state, payload) {
            let property = state.property;
            property.name = payload.name;
            property.typeId = payload.typeId;
            state.status = 'success';

            let properties = state.properties;
            for(let i in properties) {
                if(properties[i].id === property.id) {
                    properties[i].name = payload.name;
                    properties[i].typeId = payload.typeId;
                    break;
                }
            }
        },
        valueCreated(state, value) {
            state.property.values.push(value);
            state.status = 'success';
        },
        valueDeleted(state, id) {
            let values = state.property.values;
            for(let i in values) {
                if(values[i].id === id) {
                    values.splice(i, 1);
                    break;
                }
            }
            state.status = 'success';
        },
        valueUpdated(state, value) {
            let values = state.property.values;
            for(let i in values) {
                if(values[i].id === value.id) {
                    values.splice(i, 1, value);
                    break;
                }
            }
            state.status = 'success';
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
    },
    actions: {
        addProperty({commit, getters}, property) {
            return new Promise((resolve, reject) => {
                commit('propertiesRequest');
                axios({
                    url: 'properties',
                    method: 'POST',
                    data: property,
                })
                .then(resp => {
                    commit('propertyCreated')

                    let page = getters.getPropertiesPage;
                    let properties = getters.getProperties;
                    if(properties.length < page.pageSize) {
                        property = Object.assign({}, property);
                        property.id = resp.data;
                        commit('propertyAdded', property);
                    } else {
                        let isLastPage = page.pageNumber == page.totalPages;
                        if(isLastPage) {
                            commit('addPage');
                        }
                    }

                    resolve(resp);
                })
                .catch(err => {
                    commit('propertiesError');
                    reject(err);
                })
            });
        },
        getProperties({commit}, payload) {
            return new Promise((resolve, reject) => {
                commit('propertiesRequest');
                var qs = require('qs');
                axios({
                    method: 'GET',
                    url: 'properties',
                    params: payload,
                    paramsSerializer: params => {
                        return qs.stringify(
                            params,
                            { arrayFormat: 'repeat' }
                        );
                    }
                })
                .then(resp => {
                    commit('propertiesSuccess', resp.data);
                    resolve(resp);
                })
                .catch(err => {
                    commit('propertiesError', err);
                    reject(err);
                });
            });
        },
        getProperty({commit}, payload) {
            return new Promise((resolve, reject) => {
                commit('propertiesRequest')
                axios({
                    url: 'properties/' + payload.id,
                    method: 'GET',
                })
                .then(resp => {
                    commit('propertySuccess', resp.data);
                    resolve(resp);
                })
                .catch(err => {
                    commit('propertiesError', err);
                    reject(err);
                })
            });
        },
        deleteProperty({commit, state}, payload) {
            return new Promise((resolve, reject) => {
                commit('propertiesRequest')
                axios({
                    url: 'properties/' + payload.id,
                    method: 'DELETE',
                })
                .then(resp => {
                    commit('propertyDeleted', resp.data);
                    if(state.properties.length == 0) {
                        commit('deletePage');
                        commit('changePage', -1);
                    }
                    resolve(resp);
                })
                .catch(err => {
                    commit('propertiesError', err);
                    reject(err);
                });
            });
        },
        updateProperty({commit}, payload) {
            return new Promise((resolve, reject) => {
                commit('propertiesRequest');
                axios({
                    url: 'properties/' + payload.id,
                    method: 'PUT',
                    data: payload,
                })
                .then(resp => {
                    commit('propertyUpdated', payload)
                    resolve(resp);
                })
                .catch(err => {
                    commit('propertiesError', err);
                    reject(err);
                })
            });
        },
        createValue({commit}, payload) {
            return new Promise((resolve, reject) => {
                commit('propertiesRequest');
                axios({
                    url: 'values',
                    method: 'POST',
                    data: payload,
                })
                .then(resp => {
                    payload.id = resp.data;
                    commit('valueCreated', payload);
                    resolve(resp);
                })
                .catch(err => {
                    commit('propertiesError', err);
                    reject(err);
                })
            });
        },
        deleteValue({commit}, payload) {
            return new Promise((resolve, reject) => {
                commit('propertiesRequest')
                axios({
                    url: 'values/' + payload.id,
                    method: 'DELETE',
                })
                .then(resp => {
                    commit('valueDeleted', resp.data);
                    resolve(resp);
                })
                .catch(err => {
                    commit('propertiesError', err);
                    reject(err);
                });
            });
        },
        updateValue({commit}, payload) {
            return new Promise((resolve, reject) => {
                commit('propertiesRequest')
                axios({
                    url: 'values/' + payload.id,
                    method: 'PUT',
                    data: payload
                })
                .then(resp => {
                    commit('valueUpdated', payload);
                    resolve(resp);
                })
                .catch(err => {
                    commit('propertiesError', err);
                    reject(err);
                });
            });
        }
    },
}
