import { Action, Reducer } from 'redux'


export const data: Reducer<APP.TestState> = (state: APP.TestState, incomingAction: Action) => {

    return { index: 1111 }
};