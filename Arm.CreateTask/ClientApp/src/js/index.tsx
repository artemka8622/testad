import * as React from 'react'

import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';

import {setTaskInfo, requestTypeTasks, requestInformationSystems, requestExploration} from "./store/actionsCreators";

import Select from './components/Select'
import Input from './components/Input';

interface optionItem {
    value: string;
    label: string;
}

interface CompProps{
    onSetTaskInfo: typeof setTaskInfo
    onGetTaskType: typeof requestTypeTasks;
    onGetInformationSystems: typeof requestInformationSystems
    onGetExploration: typeof requestExploration
}


interface State{
    selectedOption: optionItem
}


class MainContainer extends React.Component<CompProps, State> {

    constructor(props: CompProps){
        super(props);
        this.state = {
            selectedOption: {
                value: '1',
                label: 'Опция1'
            }
        }
    }

    onSelectChange = (opt: optionItem) => {
        this.setState(()=>({
            selectedOption: { ...opt}
        }))
    };

    onCreateTaskButtonClick = () => {
        console.info('Create Task!');
        const initInfo: APP.Task = {type: 'A' , product: 'Cordis', exploration: 'Что-то', description: 'ololo!'};
        this.props.onSetTaskInfo(initInfo);
    };

    onSelectTaskTypeButtonClick = () => {
        this.props.onGetTaskType();
    };

    onSelectInfoSystemsButtonClick = () => {
        this.props.onGetInformationSystems('333');
    };

    onExplorationButtonClick = () =>{
        this.props.onGetExploration('111', '33');
    };

    public render() {

        //todo убрать после подсключения к серверу
        const options: optionItem[] = [
            { value: '1', label: 'Опция1'},
            { value: '2', label: 'Опция2'},
            { value: '3', label: 'Опция3'},
            { value: '4', label: 'Опция4'}
        ];

        return <div>
            Создание задачи
            <Select value={this.state.selectedOption}
                    options={options}
                    onChange={this.onSelectChange.bind(this)}/>

            <Input label='Тема:'
                   description='Опишите коротко суть задачи (от 2-х до 10-ти слов)'
                   handleChange={(value: string)=>{console.info(value)}}/>

            <button onClick={this.onSelectTaskTypeButtonClick}>Тип задачи</button>
            <button onClick={this.onSelectInfoSystemsButtonClick}>Програмное обеспечение</button>
            <button onClick={this.onExplorationButtonClick}>Тип обращения</button>
            <button onClick={this.onCreateTaskButtonClick}>Создать задачу</button>

        </div>
    }
}

/**
 * Подключем компонент react к store
 * @type {ComponentClass<any>}
 */
export const App = connect(
    (state: APP.ApplicationState)=>({}),
    (dispatch)=>bindActionCreators({
        onSetTaskInfo: setTaskInfo,
        onGetTaskType: requestTypeTasks,
        onGetInformationSystems: requestInformationSystems,
        onGetExploration: requestExploration}, dispatch))
(MainContainer);