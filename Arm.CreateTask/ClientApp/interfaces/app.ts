declare namespace APP {
    export interface ApplicationState {
        [key: string]: any;
        task: Task;
    }

    export interface TestState {
        [key: string]: any
    }

    export interface Task {
        type: string;
        product: string;
        exploration: string;
        description: string;
    }

    export interface TaskType {
        code: string;
        name: string;
    }

    export interface TaskTypeList {
        items: TaskType[];
    }

    export interface InfoSysProduct{
        code: string;
        name: string;
    }

    export interface InfoSysProductsList{
        items: InfoSysProduct[];
    }

    export interface Exploration {
        id: number;
        name: string;
    }

    export interface ExplorationsList{
        items: Exploration[];
    }

    // interface actions
    export interface AppThunkAction<TAction> {
        (dispatch: (action: TAction) => void, getState: () => APP.ApplicationState): void;
    }

    export interface SetTaskInfoAction {
        type: 'SET_TASK_INFO', payload: Task
    }

    export interface GetTypeTasksAction {
        type: 'GET_TYPE_TASKS', payload: TaskTypeList
    }

    export interface GetInformationSystemsAction {
        type: 'GET_INFO_SYSTEMS', payload: InfoSysProductsList
    }

    export interface GetExplorationAction {
        type: 'GET_EXPLORATION', payload: ExplorationsList
    }
}

