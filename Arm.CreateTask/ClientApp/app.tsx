import './src/style/style.styl';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { AppContainer } from 'react-hot-loader';
import { Provider } from 'react-redux';
import {App} from './src/js/index'

import store from './src/js/store'

function renderApp() {
    ReactDOM.render(
        <AppContainer>
            <Provider store={ store }>
                <App/>
            </Provider>
        </AppContainer>,
        document.getElementById('react-app')
    );
}

renderApp();
