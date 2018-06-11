import * as actions from '../actions';
import axios from 'axios'

/**
 * Запрашивает с сервера получение справочника типов задач
 * @returns {APP.AppThunkAction<APP.GetTypeTasksAction>}
 */
export function requestTypeTasks(): APP.AppThunkAction<APP.GetTypeTasksAction>{
    return (dispatch)=>{

        // todo запрос на сервер
        axios.get('api/GetTypeTasks')
            .then((response) => {
                console.log(response);
            })
            .catch( (error) => {
                console.log(error);
            });

        //Заглушка. Вместо нее должны быть данные с сервера todo убрать, когда будет соединение
        const tasksTypeList: APP.TaskTypeList = {
            items:[
                {code: '111', name: 'Тип задачи 1'},
                {code: '222', name: 'Тип задачи 2'},
                {code: '333', name: 'Тип задачи 3'},
                {code: '444', name: 'Тип задачи 4'},
                {code: '555', name: 'Тип задачи 5'},
                {code: '666', name: 'Тип задачи 6'},
        ]};

        //Получаем список типов задач с сервера
        dispatch(<APP.GetTypeTasksAction>actions.getTypeTasks(tasksTypeList));
    }
}