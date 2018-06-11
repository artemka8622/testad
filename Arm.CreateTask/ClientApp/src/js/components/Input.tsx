import * as React from 'react'
import '../../style/style.styl';

interface CompProps{
    label: string;
    description?: string
    handleChange: any;
}

interface CompState{
    isValid: boolean;
}

class Input extends React.Component<CompProps, CompState>{

    constructor(props: CompProps){
        super(props);
        this.state = {
            isValid: true
        };
    }

    onInputChange = (event: any) =>{
        this.validateInput(event.target.value);
        this.props.handleChange(event.target.value);
    };

    validateInput = (value: string) => {
        const wordsCount: number = value.split(/\s[А-Яа-яA-Za-z]+/).length;
        console.info(wordsCount);
        if (wordsCount < 2) {
            this.setState(()=>({
                isValid: false
            }));
        }

        if (!this.state.isValid && wordsCount >=2) {
            this.setState(()=>({
                isValid: true
            }));
        }
    };

    render(){
        const {label, description} = this.props;
        //todo вынести в отдельную переменнцю текст ошибки
        const errorElement = !this.state.isValid && <p className='Input-descriptionError'>Текст ошибки</p>;
        return(
            <div className='Input '>
                <label className={'Input-label ' + (this.state.isValid ? '':'Input-label--error')}
                       htmlFor='description'
                >{label}</label>
                <input className={'Input-content ' + (this.state.isValid ? '': 'Input-content--error')}
                       type='text'
                       name='description'
                       onChange={this.onInputChange} />
                <span className={'Input-description ' + (this.state.isValid ? '': 'Input-description--error')}>{description}</span>
                {errorElement}
            </div>
        )
    }
}

export default Input;