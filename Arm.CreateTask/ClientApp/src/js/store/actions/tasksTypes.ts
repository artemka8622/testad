/**
 * Создает action для GET_TYPE_TASKS, чтобы сохранить в store список значений задач
 * @param {APP.TaskTypeList} typeTasks список значений задач
 * @returns {{type: string; payload: APP.TaskTypeList}}
 */
export function getTypeTasks(typeTasks: APP.TaskTypeList) {
    return {
        type: 'GET_TYPE_TASKS',
        payload: typeTasks
    }
}