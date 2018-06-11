import {Action, Reducer} from 'redux';

/**
 * Сохраняет список типов обращений в store
 * @param {APP.ExplorationsList} state
 * @param {Action} incomingAction
 * @returns {any}
 */
export const explorations: Reducer<APP.ExplorationsList> = (state: APP.ExplorationsList, incomingAction: Action)=>{
    const action = incomingAction as APP.GetExplorationAction;
    switch (action.type){
        case ('GET_EXPLORATION'):
            return {...state, ...action.payload}
    }
    return state || {items: []};
};