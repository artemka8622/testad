import {Action, Reducer} from 'redux';

/**
 * Сохраняет список из справочника информационных систем в store
 * @param {APP.InfoSysProductsList} state
 * @param {Action} incomingAction
 * @returns {any}
 */
export const infoSystems: Reducer<APP.InfoSysProductsList> = (state: APP.InfoSysProductsList, incomingAction: Action)=>{
    const action = incomingAction as APP.GetInformationSystemsAction;
    switch (action.type){
        case ('GET_INFO_SYSTEMS'):
            return {...state, ...action.payload}
    }
    return state || {items: []};
};