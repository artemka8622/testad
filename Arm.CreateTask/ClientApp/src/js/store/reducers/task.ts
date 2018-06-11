import {Action, Reducer} from 'redux';

/**
 * Сохраняет выбранные пользователем параметры для создания задачи в КСУ в store
 * @param {APP.Task} state
 * @param {Action} incomingAction
 * @returns {any}
 */
export const task: Reducer<APP.Task> = (state: APP.Task, incomingAction: Action)=>{
    const action = incomingAction as APP.SetTaskInfoAction;
    switch (action.type){
        case ('SET_TASK_INFO'):
            return {...state, ...action.payload}
    }
    return state || {type: '', product: '', exploration: '', description: ''};
};