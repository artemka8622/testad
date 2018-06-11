import * as actions from '../actions';
import axios from 'axios';

/**
 * Запрашивает с сервера список типов обращений
 * @param {string} information_system_code код ИС
 * @param {string} task_code код типа задач
 * @returns {APP.AppThunkAction<APP.GetExplorationAction>}
 */
export function requestExploration(information_system_code: string, task_code: string): APP.AppThunkAction<APP.GetExplorationAction>{
    return (dispatch)=>{

        // todo запрос на сервер
        axios.get(`api/GetExploration?information_system_code=${information_system_code}&task_code=${task_code}`)
            .then((response) => {
                console.log(response);
            })
            .catch( (error) => {
                console.log(error);
            });

        //Заглушка. Вместо нее должны быть данные с сервера todo убрать, когда будет соединение
        const expList: APP.ExplorationsList = {
            items:[
                {id: 1, name: 'Тип обращения 1'},
                {id: 2, name: 'Тип обращения 2'},
                {id: 3, name: 'Тип обращения 3'},
                {id: 4, name: 'Тип обращения 4'},
                {id: 5, name: 'Тип обращения 5'},
            ]};

        //Получаем список типов задач с сервера
        dispatch(<APP.GetExplorationAction>actions.getExploration(expList));
    }
}