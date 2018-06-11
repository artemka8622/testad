import * as actions from '../actions';

/**
 * Вызывает редьюсер для action типа APP.SetTaskInfoAction чтобы в store сохранились
 * параметры для создания задачи в КСУ
 * @param {APP.Task} taskInfo параметры новой задачи
 * @returns {APP.AppThunkAction<APP.SetTaskInfoAction>}
 */
export function setTaskInfo(taskInfo: APP.Task): APP.AppThunkAction<APP.SetTaskInfoAction>{
    return (dispatch)=>{
        dispatch(<APP.SetTaskInfoAction>actions.setTaskInfo(taskInfo));
    }
}