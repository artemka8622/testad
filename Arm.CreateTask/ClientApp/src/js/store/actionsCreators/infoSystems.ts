import * as actions from '../actions';
import axios from 'axios';

/**
 * Запрашивает с сервера получение справочника информационных систем по коду типа задачи
 * @param {string} code значение кода типа задачи
 * @returns {APP.AppThunkAction<APP.GetInformationSystemsAction>}
 */
export function requestInformationSystems(code: string): APP.AppThunkAction<APP.GetInformationSystemsAction>{
    return (dispatch)=>{

        // todo запрос на сервер
        axios.get(`api/GetInformationSystems?code=${code}`)
            .then((response) => {
                console.log(response);
            })
            .catch( (error) => {
                console.log(error);
            });

        //Заглушка. Вместо нее должны быть данные с сервера todo убрать, когда будет соединение
        const infoSisList: APP.InfoSysProductsList = {
            items:[
                {code: '11', name: 'ИС 1'},
                {code: '22', name: 'ИС 2'},
                {code: '33', name: 'ИС 3'},
                {code: '44', name: 'ИС 4'},
            ]};

        //Получаем список типов задач с сервера
        dispatch(<APP.GetInformationSystemsAction>actions.getInformationSystems(infoSisList));
    }
}