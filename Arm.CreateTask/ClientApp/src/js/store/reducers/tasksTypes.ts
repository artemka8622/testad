import {Action, Reducer} from 'redux';

/**
 * Сохраняет список из справочника типов задач в store
 * @param {APP.TaskTypeList} state
 * @param {Action} incomingAction
 * @returns {any}
 */
export const tasksTypes: Reducer<APP.TaskTypeList> = (state: APP.TaskTypeList, incomingAction: Action)=>{
    const action = incomingAction as APP.GetTypeTasksAction;
    switch (action.type){
        case ('GET_TYPE_TASKS'):
            return {...state, ...action.payload}
    }
    return state || {items: []};
};