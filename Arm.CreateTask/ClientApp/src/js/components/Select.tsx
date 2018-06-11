import * as React from 'react';
import '../../style/style.styl';

interface optionItem {
    value: string;
    label: string;
}

interface CompProps{
    value: optionItem
    options: optionItem[],
    onChange: any
}

interface CompState{
    highlightedValue: number | string;
    isFocused: boolean;
    isMenuOpened: boolean;
}


class Select extends React.Component<CompProps, CompState>{

    constructor(props: CompProps){
        super(props);
        this.state = {
            highlightedValue: -1,
            isFocused: false,
            isMenuOpened: false
        };
    }


    onSelectionFocus = () => {
        this.setState(()=>({
            isFocused: true
        }));
    };

    onSelectionBlur = () => {
        this.setState(()=>({
            isFocused: false
        }))
    };

    onSelectionClick = () => {
        this.setState(()=>({
            isFocused: true,
            isMenuOpened: true,
        }));
    };

    onOptionHighlight = (value: number| string) => {
        console.info('onOptionHighlight' + value);
        this.setState(()=>({
            highlightedValue: value
        }));
    };

    onOptionClick = (opt: optionItem) =>{
        this.setState(()=>({
            isFocused: true,
            isMenuOpened: false,
        }));

        console.info('Selected: ' + opt.label);
        this.props.onChange(opt);
    };

    /**
     * Находит опцию в списке доступных
     * @param {optionItem} opt
     * @returns {optionItem}
     */
    private findOpt = (opt: optionItem) => {
        //todo сделать поиск опций
        return opt
    };

    /**
     * Рендерит элемент selecta
     */
    private renderSelection(){
        const { value , options} = this.props;
        const selectedOption: optionItem = this.findOpt(value) || options[0];
        return (
            <span className={'Select-selection ' + (this.state.isFocused
                ? 'Select-selection--is-focused'
                : '')}
                  onFocus={this.onSelectionFocus}
                  onBlur={this.onSelectionBlur}
                  onClick={this.onSelectionClick}>
                {this.renderOptionChildren(selectedOption)}
            </span>
        )
    }


    /**
     * Рендерит список опций для селекта
     * @param {optionItem} option
     * @param {number} index
     * @returns {any}
     */
    private renderOption(option: optionItem, index: number){
        return (
            <li key={index + option.label}
                onMouseOver={this.onOptionHighlight.bind(this, option.value)}
                onClick={this.onOptionClick.bind(this, option)}
                className={'Select-option ' + (this.state.highlightedValue === option.value
                    ? 'Select-option--is-highlighted'
                    :'')}>{option.label}</li>
        )
    }

    /**
     * Рендерит строку в поле селекта
     * @param {optionItem} option
     * @returns {any}
     */
    private renderOptionChildren(option: optionItem){
        return (
            <span className='Select-optionText'>{option.label}</span>
        )
    }

    render(){
        return (
            <div className='Select'>
                {this.renderSelection()}
                <ul className={'Select-dropdown ' + (this.state.isMenuOpened
                    ? '': 'Select-dropdown--is-hidden')}>
                    {this.props.options.map(this.renderOption.bind(this))}
                </ul>
            </div>
        )
    }
}

export default Select;