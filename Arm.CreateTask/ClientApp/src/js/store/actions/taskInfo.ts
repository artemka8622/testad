/**
 * Создает action для SET_TASK_INFO, чтобы сохранить в store параметры для
 * создания новой задачи в КСУ
 * @param {APP.Task} task парамтры для задачи
 * @returns {{type: string; payload: APP.Task}}
 */
export function setTaskInfo(task: APP.Task) {
    return {
        type: 'SET_TASK_INFO',
        payload: task
    }
}